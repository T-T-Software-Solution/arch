﻿using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using TTSS.Core.Data;
using TTSS.Core.Models;

namespace TTSS.Core.Facades;

/// <summary>
/// Facade for request operations.
/// </summary>
public static class RequestFacade
{
    #region Fields

    /// <summary>
    /// Ascending order keyword.
    /// </summary>
    public const string Ascending = "asc";

    /// <summary>
    /// Descending order keyword.
    /// </summary>
    public const string Descending = "desc";

    private const char Whitespaces = ' ';
    private const char SortSplitter = ':';
    private static readonly string[] OrderBys = [Ascending, Descending];
    private static readonly MethodInfo ToLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
    private static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains", [typeof(string)])!;

    #endregion

    #region Methods

    /// <summary>
    /// Convert paging request to expressions.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="target">Target paging request</param>
    /// <returns>Sorting and filter expressions</returns>
    public static (Action<IPagingRepository<TEntity>> sort, Expression<Func<TEntity, bool>> filter) ToExpressions<TEntity>(this IPagingRequest target)
        where TEntity : class
        => (target.ToSortingExpression<TEntity>(), target.ToFilterExpression<TEntity>());

    /// <summary>
    /// Convert paging request to sorting expression.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="target">Target paging request</param>
    /// <returns>Sorting expression</returns>
    public static Action<IPagingRepository<TEntity>> ToSortingExpression<TEntity>(this IPagingRequest target)
        where TEntity : class
    {
        if (target is null || target.Sort is null || false == target.Sort.Any())
        {
            return _ => { };
        }

        var properties = typeof(TEntity).GetProperties();
        var sorts = (target.Sort ?? [])
            .Select(it => it
                .ToLowerInvariant()
                .Replace(Whitespaces.ToString(), string.Empty)
                .Split(SortSplitter, StringSplitOptions.RemoveEmptyEntries))
            .Where(it => properties.Any(p => string.Equals(p.Name, it.First(), StringComparison.OrdinalIgnoreCase)))
            .Select(it => new
            {
                Field = it.First(),
                IsAscending = (OrderBys.Contains(it.Last()) ? it.Last() : OrderBys.First()) == OrderBys.First()
            }).ToArray();

        return pagingRepository =>
        {
            foreach (var item in sorts)
            {
                Func<Expression<Func<TEntity, object>>, IPagingRepository<TEntity>> func = item.IsAscending
                    ? pagingRepository.OrderBy
                    : pagingRepository.OrderByDescending;
                func(GetSelectorExpression(item.Field));
            }
        };

        static Expression<Func<TEntity, object>> GetSelectorExpression(string propertyName)
        {
            var propertyInfo = typeof(TEntity)
                .GetProperties()
                .FirstOrDefault(it => string.Equals(it.Name, propertyName, StringComparison.OrdinalIgnoreCase));
            ArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));
            var parameter = Expression.Parameter(typeof(TEntity));
            var property = Expression.Property(parameter, propertyInfo.Name);
            var convert = Expression.Convert(property, typeof(object));
            return Expression.Lambda<Func<TEntity, object>>(convert, parameter);
        }
    }

    /// <summary>
    /// Convert paging request to filter expression.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="target">Target paging request</param>
    /// <returns>Filter expression</returns>
    public static Expression<Func<TEntity, bool>> ToFilterExpression<TEntity>(this IPagingRequest target)
    {
        if (target is null || target.Filter is null || target.Filter.Count == 0)
        {
            return _ => true;
        }

        var parameter = Expression.Parameter(typeof(TEntity));
        Expression? expression = null;
        var qry = from property in typeof(TEntity).GetProperties()
                  from filter in target.Filter
                  where false == string.IsNullOrWhiteSpace(filter.Key)
                      && false == string.IsNullOrWhiteSpace(filter.Value)
                      && string.Equals(property.Name, filter.Key, StringComparison.OrdinalIgnoreCase)
                  select new
                  {
                      filter.Value,
                      Property = property,
                  };
        foreach (var filter in qry)
        {
            var targetProperty = Expression.Property(parameter, filter.Property);
            var rawValue = ConvertFilterValue(filter.Value, filter.Property.PropertyType);
            if (rawValue is null)
            {
                continue;
            }

            var targetValue = Expression.Constant(rawValue);

            Expression comparison;
            if (filter.Property.PropertyType != typeof(string))
            {
                comparison = Expression.Equal(targetProperty, targetValue);
            }
            else
            {
                var propertyToLower = Expression.Call(targetProperty, ToLowerMethod);
                var constantToLower = Expression.Call(targetValue, ToLowerMethod);
                comparison = Expression.Call(propertyToLower, ContainsMethod, constantToLower);
            }

            expression = expression is null
                ? comparison
                : Expression.AndAlso(expression, comparison);
        }

        expression ??= Expression.Constant(true);
        return Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
    }

    /// <summary>
    /// Combine two expressions with AND operator.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="expr1">First expression</param>
    /// <param name="expr2">Second expression</param>
    /// <returns>Filter expression</returns>
    public static Expression<Func<TEntity, bool>> And<TEntity>(this Expression<Func<TEntity, bool>> expr1, Expression<Func<TEntity, bool>> expr2)
        where TEntity : class
    {
        CombineExpressions(expr1, expr2, out var parameter, out var left, out var right);
        return Expression.Lambda<Func<TEntity, bool>>(Expression.AndAlso(left, right), parameter);
    }

    /// <summary>
    /// Combine two expressions with OR operator.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="expr1">First expression</param>
    /// <param name="expr2">Second expression</param>
    /// <returns>Filter expression</returns>
    public static Expression<Func<TEntity, bool>> Or<TEntity>(this Expression<Func<TEntity, bool>> expr1, Expression<Func<TEntity, bool>> expr2)
    {
        CombineExpressions(expr1, expr2, out var parameter, out var left, out var right);
        return Expression.Lambda<Func<TEntity, bool>>(Expression.OrElse(left, right), parameter);
    }

    private static void CombineExpressions<TEntity>(Expression<Func<TEntity, bool>> expr1, Expression<Func<TEntity, bool>> expr2, out ParameterExpression parameter, out Expression left, out Expression right)
    {
        parameter = Expression.Parameter(typeof(TEntity));
        left = new ReplaceExpressionVisitor(expr1.Parameters.First(), parameter).Visit(expr1.Body)!;
        right = new ReplaceExpressionVisitor(expr2.Parameters.First(), parameter).Visit(expr2.Body)!;
    }

    private static object? ConvertFilterValue(string value, Type targetType)
    {
        if (targetType == typeof(string))
        {
            return value;
        }

        var converter = TypeDescriptor.GetConverter(targetType);
        if (converter != null && converter.IsValid(value))
        {
            return converter.ConvertFromInvariantString(value);
        }

        return null;
    }

    #endregion

    private class ReplaceExpressionVisitor(Expression oldValue, Expression newValue) : ExpressionVisitor
    {
        public override Expression? Visit(Expression? node)
        {
            if (node == oldValue)
            {
                return newValue;
            }
            return base.Visit(node);
        }
    }
}
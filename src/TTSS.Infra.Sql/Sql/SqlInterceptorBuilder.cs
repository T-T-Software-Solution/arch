﻿using Microsoft.EntityFrameworkCore.Diagnostics;
using TTSS.Infra.Data.Sql.Interceptors;

namespace TTSS.Infra.Data.Sql;

/// <summary>
/// SQL interceptor builder.
/// </summary>
public sealed class SqlInterceptorBuilder
{
    #region Fields

    private readonly List<Type> _interceptors = [];

    #endregion

    #region Properties

    /// <summary>
    /// Gets the default instance of <see cref="SqlInterceptorBuilder"/>.
    /// </summary>
    public static SqlInterceptorBuilder Default => new();

    /// <summary>
    /// Gets the registered interceptor types.
    /// </summary>
    public IEnumerable<Type> InterceptorTypes => _interceptors.Distinct();

    #endregion

    #region Constructors

    /// <summary>
    /// Create a new instance of <see cref="SqlInterceptorBuilder"/>.
    /// </summary>
    /// <param name="excludeDefaultInterceptors">True, to exclude default interceptors; otherwise, false</param>
    public SqlInterceptorBuilder(bool excludeDefaultInterceptors = false)
    {
        if (excludeDefaultInterceptors)
        {
            return;
        }

        Register<SqlMaskingInterceptor>();
        Register<SqlActivityLogInterceptor>();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Register an interceptor.
    /// </summary>
    /// <typeparam name="TInterceptor">Interceptor type</typeparam>
    /// <returns>The <see cref="SqlInterceptorBuilder"/> instance</returns>
    public SqlInterceptorBuilder Register<TInterceptor>() where TInterceptor : class, IInterceptor
    {
        var type = typeof(TInterceptor);
        if (false == _interceptors.Contains(type))
        {
            _interceptors.Add(type);
        }
        return this;
    }

    #endregion
}
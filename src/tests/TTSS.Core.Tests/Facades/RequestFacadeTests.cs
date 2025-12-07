using FluentAssertions;
using TTSS.Core.Data;
using TTSS.Core.Facades.Models;

namespace TTSS.Core.Facades;

public class RequestFacadeTests
{
    private readonly Student[] _students =
    [
        new() { Age = 25, Id = "S1", Name = "John", IsMale = true, GPA = 123.4, Type = StudentType.Regular },
        new() { Age = 30, Id = "S2", Name = "Mary", IsMale = true, GPA = 002.0, Type = StudentType.Regular },
        new() { Age = 27, Id = "S3", Name = "Jane", IsMale = false, GPA = 3.01, Type = StudentType.Exchange },
        new() { Age = -9, Id = "S4", Name = "Dojo", IsMale = false, GPA = -1.3, Type = StudentType.Exchange },
    ];

    [Theory]
    [InlineData("Name", "John", 0)]
    [InlineData("name", "John", 0)]
    [InlineData("Name", "john", 0)]
    [InlineData("nAMe", "jOhN", 0)]
    [InlineData("name", "jo", 0, 3)]
    [InlineData("name", "J", 0, 2, 3)]
    [InlineData("name", "NEVER_MATCH")]
    [InlineData("UNKNOWN", "John", 0, 1, 2, 3)]
    [InlineData("", "Key is empty", 0, 1, 2, 3)]
    [InlineData(" ", "Key is white space", 0, 1, 2, 3)]
    [InlineData("Value is empty", "", 0, 1, 2, 3)]
    [InlineData("Value is null", null, 0, 1, 2, 3)]
    [InlineData("Value is white space", " ", 0, 1, 2, 3)]
    public void SingleFilter_Text_ThenFilteredResultMustWorkAsNormally(string key, string value, params int[] expectedIndexs)
        => ValidateSingleFilter(key, value, expectedIndexs);

    [Theory]
    [InlineData("Age", "25", 0)]
    [InlineData("age", "25", 0)]
    [InlineData("age", "2")]
    [InlineData("age", "9999")]
    [InlineData("age", "-9", 3)]
    [InlineData("age", "", 0, 1, 2, 3)]
    [InlineData("age", " ", 0, 1, 2, 3)]
    [InlineData("age", null, 0, 1, 2, 3)]
    [InlineData("age", "INVALID", 0, 1, 2, 3)]
    public void SingleFilter_Integer_ThenFilteredResultMustWorkAsNormally(string key, string value, params int[] expectedIndexs)
       => ValidateSingleFilter(key, value, expectedIndexs);

    [Theory]
    [InlineData("GPA", "123.4", 0)]
    [InlineData("GPA", "2", 1)]
    [InlineData("GPA", "3.01", 2)]
    [InlineData("GPA", "-1.3", 3)]
    [InlineData("GPA", "", 0, 1, 2, 3)]
    [InlineData("GPA", " ", 0, 1, 2, 3)]
    [InlineData("GPA", null, 0, 1, 2, 3)]
    [InlineData("GPA", "INVALID", 0, 1, 2, 3)]

    public void SingleFilter_Double_ThenFilteredResultMustWorkAsNormally(string key, string value, params int[] expectedIndexs)
       => ValidateSingleFilter(key, value, expectedIndexs);

    [Theory]
    [InlineData("IsMale", "TRUE", 0, 1)]
    [InlineData("IsMale", "True", 0, 1)]
    [InlineData("IsMale", "true", 0, 1)]
    [InlineData("IsMale", "FALSE", 2, 3)]
    [InlineData("IsMale", "False", 2, 3)]
    [InlineData("IsMale", "false", 2, 3)]
    [InlineData("isMale", "TRUE", 0, 1)]
    [InlineData("ismale", "TRUE", 0, 1)]
    [InlineData("iSMalE", "TruE", 0, 1)]
    [InlineData("IsMale", "", 0, 1, 2, 3)]
    [InlineData("IsMale", " ", 0, 1, 2, 3)]
    [InlineData("IsMale", null, 0, 1, 2, 3)]
    [InlineData("IsMale", "INVALID", 0, 1, 2, 3)]
    public void SingleFilter_Boolean_ThenFilteredResultMustWorkAsNormally(string key, string value, params int[] expectedIndexs)
       => ValidateSingleFilter(key, value, expectedIndexs);

    [Theory]
    [InlineData("Type", "Regular", 0, 1)]
    [InlineData("Type", "Exchange", 2, 3)]
    [InlineData("Type", "1", 0, 1, 2, 3)]
    [InlineData("Type", "2", 0, 1, 2, 3)]
    [InlineData("Type", "0", 0, 1, 2, 3)]
    [InlineData("Type", "-999", 0, 1, 2, 3)]
    [InlineData("tYPe", "rEGuLAR", 0, 1, 2, 3)]
    [InlineData("Type", "", 0, 1, 2, 3)]
    [InlineData("Type", null, 0, 1, 2, 3)]
    [InlineData("Type", " ", 0, 1, 2, 3)]
    [InlineData("Type", "INVALID", 0, 1, 2, 3)]
    public void SingleFilter_Enum_ThenFilteredResultMustWorkAsNormally(string key, string value, params int[] expectedIndexs)
       => ValidateSingleFilter(key, value, expectedIndexs);

    private void ValidateSingleFilter(string key, string value, params int[] expectedIndexs)
    {
        Dictionary<string, string> filter = new() { { key, value } };
        List<Student> expected = [];
        foreach (var item in expectedIndexs)
        {
            expected.Add(_students[item]);
        }
        ValidateFilterResult(filter, [.. expected]);
    }

    [Theory]
#pragma warning disable xUnit1045 // Avoid using TheoryData type arguments that might not be serializable
    [ClassData(typeof(MultipleFilterScenarios))]
#pragma warning restore xUnit1045 // Avoid using TheoryData type arguments that might not be serializable
    public void MultipleFilter_ThenFilteredResultMustWorkAsNormally(Dictionary<string, string> filter, int[] expectedIndexs)
    {
        List<Student> expected = [];
        foreach (var item in expectedIndexs)
        {
            expected.Add(_students[item]);
        }
        ValidateFilterResult(filter, [.. expected]);
    }
    class MultipleFilterScenarios : TheoryData<Dictionary<string, string>, int[]>
    {
        public MultipleFilterScenarios()
        {
            Add(new() { { "id", "s" }, { "name", "J" } }, [0, 2, 3]);
            Add(new() { { "id", "S1" }, { "name", "J" } }, [0]);
            Add(new() { { "title", "mr" }, { "firstName", "John" } }, [0, 1, 2, 3]);
            Add(new() { { "age", "25" }, { "name", "J" } }, [0]);
            Add(new() { { "name", "j" }, { "ismale", "true" }, { "type", "regular" } }, [0]);
        }
    }

    void ValidateFilterResult(Dictionary<string, string> filters, params Student[] expected)
    {
        var filter = new StudentPagingRequest { Filter = filters }.ToFilterExpression<Student>();
        var actual = _students.Where(filter.Compile()).ToArray();
        actual.Should().HaveCount(expected.Length);
        actual.Should().BeEquivalentTo(expected);
    }

    #region Sorting Tests

    [Theory]
    [InlineData("name:asc", new[] { 3, 2, 0, 1 })] // Dojo, Jane, John, Mary
    [InlineData("name:desc", new[] { 1, 0, 2, 3 })] // Mary, John, Jane, Dojo
    [InlineData("age:asc", new[] { 3, 0, 2, 1 })] // -9, 25, 27, 30
    [InlineData("age:desc", new[] { 1, 2, 0, 3 })] // 30, 27, 25, -9
    [InlineData("NAME:ASC", new[] { 3, 2, 0, 1 })] // Case insensitive
    [InlineData("Name:Asc", new[] { 3, 2, 0, 1 })] // Mixed case
    [InlineData("name", new[] { 3, 2, 0, 1 })] // Default to ascending
    public void SingleSort_ThenSortedResultMustWorkCorrectly(string sort, int[] expectedOrder)
        => ValidateSortResult([sort], expectedOrder);

    [Theory]
    [InlineData(new[] { "ismale:asc", "name:asc" }, new[] { 3, 2, 0, 1 })] // false first (Dojo, Jane), then true (John, Mary)
    [InlineData(new[] { "ismale:desc", "name:asc" }, new[] { 0, 1, 3, 2 })] // true first (John, Mary), then false (Dojo, Jane)
    [InlineData(new[] { "ismale:asc", "name:desc" }, new[] { 2, 3, 1, 0 })] // false first (Jane, Dojo), then true (Mary, John)
    [InlineData(new[] { "ismale:asc", "age:asc" }, new[] { 3, 2, 0, 1 })] // false (Dojo:-9, Jane:27), true (John:25, Mary:30)
    [InlineData(new[] { "type:asc", "gpa:asc" }, new[] { 1, 0, 3, 2 })] // Regular (Mary:2, John:123.4), Exchange (Dojo:-1.3, Jane:3.01)
    [InlineData(new[] { "type:desc", "gpa:desc" }, new[] { 2, 3, 0, 1 })] // Exchange (Jane:3.01, Dojo:-1.3), Regular (John:123.4, Mary:2)
    public void MultipleSort_TwoFields_ThenSortedResultMustWorkCorrectly(string[] sorts, int[] expectedOrder)
        => ValidateSortResult(sorts, expectedOrder);

    [Theory]
    [InlineData(new[] { "type:asc", "ismale:asc", "name:asc" }, new[] { 0, 1, 3, 2 })] // Regular+Male (John, Mary), Exchange+Female (Dojo, Jane)
    [InlineData(new[] { "type:asc", "ismale:desc", "name:asc" }, new[] { 0, 1, 3, 2 })] // Regular (John:M, Mary:M), Exchange (Dojo:F, Jane:F)
    public void MultipleSort_ThreeFields_ThenSortedResultMustWorkCorrectly(string[] sorts, int[] expectedOrder)
        => ValidateSortResult(sorts, expectedOrder);

    [Fact]
    public void MultipleSort_WithInvalidField_ShouldIgnoreInvalidField()
    {
        // Arrange: "invalidfield" should be ignored, only "name:asc" should apply
        var sorts = new[] { "invalidfield:asc", "name:asc" };

        // Act & Assert
        ValidateSortResult(sorts, [3, 2, 0, 1]); // Sorted by name only: Dojo, Jane, John, Mary
    }

    [Fact]
    public void MultipleSort_WithAllInvalidFields_ShouldReturnOriginalOrder()
    {
        // Arrange
        var sorts = new[] { "invalid1:asc", "invalid2:desc" };

        // Act & Assert
        ValidateSortResult(sorts, [0, 1, 2, 3]); // Original order preserved
    }

    [Fact]
    public void MultipleSort_EmptySortList_ShouldReturnOriginalOrder()
    {
        // Arrange
        var sorts = Array.Empty<string>();

        // Act & Assert
        ValidateSortResult(sorts, [0, 1, 2, 3]); // Original order preserved
    }

    [Fact]
    public void MultipleSort_NullSortList_ShouldReturnOriginalOrder()
    {
        // Arrange & Act
        var request = new StudentPagingRequest { Sort = null };
        var sortAction = request.ToSortingExpression<Student>();
        var mockRepo = new MockPagingRepository<Student>(_students);
        sortAction(mockRepo);

        // Assert
        mockRepo.OrderByCalls.Should().BeEmpty();
        mockRepo.ThenByCalls.Should().BeEmpty();
    }

    [Theory]
    [InlineData(new[] { "name : asc" }, new[] { 3, 2, 0, 1 })] // With spaces
    [InlineData(new[] { " name:asc " }, new[] { 3, 2, 0, 1 })] // Leading/trailing spaces in value
    public void MultipleSort_WithWhitespace_ShouldHandleCorrectly(string[] sorts, int[] expectedOrder)
        => ValidateSortResult(sorts, expectedOrder);

    [Fact]
    public void MultipleSort_ShouldUseOrderByForFirstField()
    {
        // Arrange
        var request = new StudentPagingRequest { Sort = ["name:asc"] };
        var sortAction = request.ToSortingExpression<Student>();
        var mockRepo = new MockPagingRepository<Student>(_students);

        // Act
        sortAction(mockRepo);

        // Assert
        mockRepo.OrderByCalls.Should().HaveCount(1);
        mockRepo.ThenByCalls.Should().BeEmpty();
    }

    [Fact]
    public void MultipleSort_ShouldUseThenByForSubsequentFields()
    {
        // Arrange
        var request = new StudentPagingRequest { Sort = ["ismale:asc", "name:asc", "age:desc"] };
        var sortAction = request.ToSortingExpression<Student>();
        var mockRepo = new MockPagingRepository<Student>(_students);

        // Act
        sortAction(mockRepo);

        // Assert: First field uses OrderBy, subsequent fields use ThenBy
        mockRepo.OrderByCalls.Should().HaveCount(1);
        mockRepo.ThenByCalls.Should().HaveCount(2);
    }

    [Fact]
    public void MultipleSort_ShouldUseThenByDescendingForSubsequentDescFields()
    {
        // Arrange
        var request = new StudentPagingRequest { Sort = ["ismale:asc", "name:desc"] };
        var sortAction = request.ToSortingExpression<Student>();
        var mockRepo = new MockPagingRepository<Student>(_students);

        // Act
        sortAction(mockRepo);

        // Assert: First field uses OrderBy, second uses ThenByDescending
        mockRepo.OrderByCalls.Should().HaveCount(1);
        mockRepo.ThenByCalls.Should().ContainSingle().Which.Should().Be("ThenByDescending");
    }

    private void ValidateSortResult(string[] sorts, int[] expectedOrder)
    {
        // Arrange
        var request = new StudentPagingRequest { Sort = sorts };
        var sortAction = request.ToSortingExpression<Student>();
        var mockRepo = new MockPagingRepository<Student>(_students);

        // Act
        sortAction(mockRepo);
        var actual = mockRepo.GetSortedData().ToArray();

        // Assert
        var expected = expectedOrder.Select(i => _students[i]).ToArray();
        actual.Should().ContainInOrder(expected);
    }

    #endregion

    #region Mock Paging Repository

    private class MockPagingRepository<TEntity> : IPagingRepository<TEntity>
    {
        private IEnumerable<TEntity> _entities;
        public List<string> OrderByCalls { get; } = [];
        public List<string> ThenByCalls { get; } = [];

        public MockPagingRepository(IEnumerable<TEntity> entities)
        {
            _entities = entities;
        }

        public IEnumerable<TEntity> GetSortedData() => _entities;

        public PagingSet<TEntity> GetPage(int pageNo) => throw new NotImplementedException();
        public Task<IEnumerable<TEntity>> GetDataAsync(int pageNo) => throw new NotImplementedException();

        public IPagingRepository<TEntity> OrderBy(System.Linq.Expressions.Expression<Func<TEntity, object>> keySelector)
        {
            OrderByCalls.Add("OrderBy");
            _entities = _entities.OrderBy(keySelector.Compile());
            return this;
        }

        public IPagingRepository<TEntity> OrderByDescending(System.Linq.Expressions.Expression<Func<TEntity, object>> keySelector)
        {
            OrderByCalls.Add("OrderByDescending");
            _entities = _entities.OrderByDescending(keySelector.Compile());
            return this;
        }

        public IPagingRepository<TEntity> ThenBy(System.Linq.Expressions.Expression<Func<TEntity, object>> keySelector)
        {
            ThenByCalls.Add("ThenBy");
            _entities = _entities is IOrderedEnumerable<TEntity> ordered
                ? ordered.ThenBy(keySelector.Compile())
                : _entities.OrderBy(keySelector.Compile());
            return this;
        }

        public IPagingRepository<TEntity> ThenByDescending(System.Linq.Expressions.Expression<Func<TEntity, object>> keySelector)
        {
            ThenByCalls.Add("ThenByDescending");
            _entities = _entities is IOrderedEnumerable<TEntity> ordered
                ? ordered.ThenByDescending(keySelector.Compile())
                : _entities.OrderByDescending(keySelector.Compile());
            return this;
        }
    }

    #endregion
}
using FluentAssertions;
using TTSS.Core.Facades.Models;

namespace TTSS.Core.Facades;

public class RequestFacadeTests
{
    private readonly Student[] _students =
    [
        new (){ Age = 25, Id = "S1", Name = "John", IsMale = true, GPA = 123.4, Type = StudentType.Regular },
        new (){ Age = 30, Id = "S2", Name = "Mary", IsMale = true, GPA = 002.0, Type = StudentType.Regular },
        new (){ Age = 27, Id = "S3", Name = "Jane", IsMale = false, GPA = 3.01, Type = StudentType.Exchange },
        new (){ Age = -9, Id = "S4", Name = "Dojo", IsMale = false, GPA = -1.3, Type = StudentType.Exchange },
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
}
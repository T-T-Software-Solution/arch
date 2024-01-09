using FluentAssertions;

namespace TTSS.Core.Services;

public class DateTimeServiceTests
{
    private readonly DateTimeService _sut;

    public DateTimeServiceTests()
        => _sut = new DateTimeService();

    [Fact]
    public void GetUtcNow_ShouldBe_UtcFormat()
        => _sut.UtcNow.Kind.Should().Be(DateTimeKind.Utc);

    [Fact]
    public void GetNumberDateTimeString()
    {
        const string expected = "202212150708090000000";
        _sut.ToNumericDateTime(new DateTime(2022, 12, 15, 7, 8, 9, DateTimeKind.Utc)).Should().Be(expected);
    }

    [Fact]
    public void ParseNumbericDateTime_ToUtc_MustWorkCorrectly()
    {
        var numericTime = "202212150708090000000";
        var expectedUtc = new DateTime(2022, 12, 15, 7, 8, 9, DateTimeKind.Utc);
        _sut.ParseNumericToUtcDateTime(numericTime).Should().Be(expectedUtc);
    }

    [Fact]
    public void ParseNumbericDateTime_ToEst_MustWorkCorrectly()
    {
        var numericTime = "202212150708090000000";
        var utcTime = new DateTime(2022, 12, 15, 7, 8, 9, DateTimeKind.Utc);
        var easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        var expectedEst = TimeZoneInfo.ConvertTimeFromUtc(utcTime, easternZone);
        _sut.ParseNumericToEstDateTime(numericTime).Should().Be(expectedEst);
    }

    [Fact]
    public void ParseNumericDateTime_MustWorkCorrectlyWith_Utc_And_Est()
    {
        var numericTime = "202212150708090000000";

        var est = _sut.ParseNumericToEstDateTime(numericTime);
        est.Should().Be(new DateTime(2022, 12, 15, 2, 8, 9, DateTimeKind.Utc));

        var utc = _sut.ParseNumericToUtcDateTime(numericTime);
        utc.Should().Be(new DateTime(2022, 12, 15, 7, 8, 9, DateTimeKind.Utc));
    }

    [Fact]
    public void Convert_UtcToEst_MustWorkCorrectly()
    {
        var utcTime = new DateTime(2022, 12, 15, 2, 8, 9, DateTimeKind.Utc);
        var easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        var expectedEst = TimeZoneInfo.ConvertTimeFromUtc(utcTime, easternZone);
        _sut.ToEstTime(utcTime).Should().Be(expectedEst);
    }

    [Fact]
    public void Convert_EstToUtc_MustWorkCorrectly()
    {
        var expectedUtcTime = new DateTime(2022, 12, 15, 2, 8, 9, DateTimeKind.Utc);
        var easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        var estTime = TimeZoneInfo.ConvertTimeFromUtc(expectedUtcTime, easternZone);
        _sut.ToUtcTime(estTime).Should().Be(expectedUtcTime);
    }
}
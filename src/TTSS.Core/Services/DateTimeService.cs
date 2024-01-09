using System.Globalization;

namespace TTSS.Core.Services;

internal sealed class DateTimeService : IDateTimeService
{
    #region Fields

    private const string FormatString = "yyyyMMddHHmmssfffffff";
    private static readonly IFormatProvider CultureUsFormat = new CultureInfo(IDateTimeService.DefaultCultureName);
    private static readonly TimeZoneInfo EstZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

    #endregion

    #region Properties

    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime EstNow => ToEstTime(UtcNow);

    #endregion

    #region Methods

    public string ToNumericDateTime(DateTime dateTime, string cultureName = IDateTimeService.DefaultCultureName)
        => dateTime.ToString(FormatString, CreateFormatProvider(cultureName));

    public DateTime ParseNumericToUtcDateTime(string numericDateTime, string cultureName = IDateTimeService.DefaultCultureName)
        => DateTime.ParseExact(numericDateTime, FormatString, CreateFormatProvider(cultureName), DateTimeStyles.AdjustToUniversal);

    public DateTime ParseNumericToEstDateTime(string numericDateTime, string cultureName = IDateTimeService.DefaultCultureName)
        => ToEstTime(DateTime.ParseExact(numericDateTime, FormatString, CreateFormatProvider(cultureName), DateTimeStyles.None));

    public DateTime ToEstTime(DateTime utcDateTime)
        => TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, EstZone);

    public DateTime ToUtcTime(DateTime estDateTime)
        => TimeZoneInfo.ConvertTimeToUtc(estDateTime, EstZone);

    public double ToUnixTime(DateTime dateTime)
        => (dateTime - DateTimeOffset.UnixEpoch).TotalSeconds;

    private static IFormatProvider CreateFormatProvider(string cultureName)
        => (string.IsNullOrWhiteSpace(cultureName) || cultureName is IDateTimeService.DefaultCultureName)
            ? CultureUsFormat
            : new CultureInfo(cultureName);

    #endregion
}
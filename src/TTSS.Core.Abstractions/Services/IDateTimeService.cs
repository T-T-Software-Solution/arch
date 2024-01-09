namespace TTSS.Core.Services;

/// <summary>
/// Date and time service.
/// </summary>
public interface IDateTimeService
{
    /// <summary>
    /// US culture name.
    /// </summary>
    protected const string DefaultCultureName = "en-US";

    /// <summary>
    /// Current UTC date and time.
    /// </summary>
    DateTime UtcNow { get; }

    /// <summary>
    /// Current EST date and time.
    /// </summary>
    DateTime EstNow { get; }

    /// <summary>
    /// Create date and time in numeric format.
    /// </summary>
    /// <param name="dateTime">From date and time</param>
    /// <param name="cultureName">Culture name</param>
    /// <returns>Date and time in numeric format</returns>
    string ToNumericDateTime(DateTime dateTime, string cultureName = DefaultCultureName);

    /// <summary>
    /// Convert numeric date time string to UTC DateTime.
    /// </summary>
    /// <param name="numericDateTime">Numeric date time string</param>
    /// <param name="cultureName">Culture name</param>
    /// <returns>DateTime</returns>
    DateTime ParseNumericToUtcDateTime(string numericDateTime, string cultureName = DefaultCultureName);

    /// <summary>
    /// Convert numeric date time string to EST DateTime.
    /// </summary>
    /// <param name="numericDateTime">Numeric date time string</param>
    /// <param name="cultureName">Culture name</param>
    /// <returns>DateTime</returns>
    DateTime ParseNumericToEstDateTime(string numericDateTime, string cultureName = IDateTimeService.DefaultCultureName);

    /// <summary>
    /// Convert to EST.
    /// </summary>
    /// <param name="utcDateTime">From UTC date and time</param>
    /// <returns>Date and time in EST</returns>
    DateTime ToEstTime(DateTime utcDateTime);

    /// <summary>
    /// Convert to UTC.
    /// </summary>
    /// <param name="estDateTime">From EST date and time</param>
    /// <returns>Date and time in UTC</returns>
    DateTime ToUtcTime(DateTime estDateTime);

    /// <summary>
    /// Convert to Unix time.
    /// </summary>
    /// <param name="dateTime">From date and time</param>
    /// <returns>Unix time</returns>
    double ToUnixTime(DateTime dateTime);
}
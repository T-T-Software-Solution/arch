namespace TTSS.Core.Configurations;

/// <summary>
/// Contract for configuration options validator.
/// </summary>
public interface IOptionsValidator : IValidator
{
    /// <summary>
    /// Section name in the configuration file.
    /// </summary>
    static abstract string SectionName { get; }
}
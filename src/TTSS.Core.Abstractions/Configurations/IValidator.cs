namespace TTSS.Core.Configurations;

/// <summary>
/// Contract for validator.
/// </summary>
public interface IValidator
{
    /// <summary>
    /// Validate the options.
    /// </summary>
    /// <returns>True if the options are valid</returns>
    bool Validate();
}
namespace TTSS.Core.Data;


/// <summary>
/// Contract for validatable entity.
/// </summary>
public interface IValidatableEntity
{
    /// <summary>
    /// Check if the entity is in a valid state.
    /// </summary>
    /// <returns>True if the entity is in a valid state.</returns>
    bool IsValidState();
}
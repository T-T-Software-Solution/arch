using System.ComponentModel.DataAnnotations;

namespace TTSS.Core.Annotations;

/// <summary>
/// Description attribute to indicate the operation type and description of a class.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class OperationDescriptionAttribute(OperationType type, string module, string? description = default) : Attribute
{
    #region Properties

    /// <summary>
    /// Operation type of the class.
    /// </summary>
    [Required]
    public OperationType Operation { get; } = type;

    /// <summary>
    /// Module name of the class.
    /// </summary>
    [Required]
    public string ModuleName { get; } = module;

    /// <summary>
    /// Description of the class.
    /// </summary>
    public string? Description { get; } = description;

    #endregion
}
namespace TTSS.Core.Messaging;

/// <summary>
/// Contract for a publication message.
/// </summary>
public interface IPublication : IPublish, MediatR.INotification;
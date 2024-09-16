using TTSS.Core.Messaging;

namespace Sample09.Basic.ConsoleApp.PublishToMultipleHandlers.Messages;

public sealed record NotifyWeatherChanged : IPublication
{
    public int FromValue { get; set; }
    public int NewValue { get; set; }
}
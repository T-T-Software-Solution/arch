using Sample09.Basic.ConsoleApp.PublishToMultipleHandlers.Messages;
using TTSS.Core.Messaging.Handlers;

namespace Sample09.Basic.ConsoleApp.PublishToMultipleHandlers.Handlers;

file sealed class WeatherReportHandler : PublicationHandler<NotifyWeatherChanged>
{
    public override void Handle(NotifyWeatherChanged publication)
        => Console.WriteLine($"Weather changed from {publication.FromValue} to {publication.NewValue}");
}
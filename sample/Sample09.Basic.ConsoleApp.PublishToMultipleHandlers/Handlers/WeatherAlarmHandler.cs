using Sample09.Basic.ConsoleApp.PublishToMultipleHandlers.Messages;
using TTSS.Core.Messaging.Handlers;

namespace Sample09.Basic.ConsoleApp.PublishToMultipleHandlers.Handlers;

file sealed class WeatherAlarmHandler : PublicationHandler<NotifyWeatherChanged>
{
    public override void Handle(NotifyWeatherChanged publication)
    {
        var message = publication.NewValue switch
        {
            < 0 => "It's freezing outside!",
            > 40 => "THE WORLD IS BURNINGGGGGG !!!!",
            _ => "Weather is normal."
        };
        Console.WriteLine(message);
    }
}
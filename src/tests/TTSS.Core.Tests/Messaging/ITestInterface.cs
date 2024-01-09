namespace TTSS.Core.Messaging;

public interface ITestInterface
{
    void Execute(object data);
    Task ExecuteAsync(object data, CancellationToken cancellationToken = default);
}
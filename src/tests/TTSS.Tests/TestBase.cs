using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using TTSS.Core.Services;
using YamlDotNet.Serialization;

namespace TTSS.Tests;

/// <summary>
/// Base class for all tests.
/// </summary>
public abstract class TestBase
{
    #region Properties

    /// <summary>
    /// Object creation services.
    /// </summary>
    protected Fixture Fixture { get; }

    /// <summary>
    /// Current UTC time.
    /// </summary>
    protected DateTime CurrentTime { get; set; }

    /// <summary>
    /// Yaml serializer.
    /// </summary>
    protected ISerializer Serializer { get; }

    /// <summary>
    /// Yaml deserializer.
    /// </summary>
    protected IDeserializer Deserializer { get; }

    /// <summary>
    /// Date time service.
    /// </summary>
    protected IDateTimeService DateTimeService => DateTimeSerivceMock.Object;

    /// <summary>
    /// Current EST time.
    /// </summary>
    protected DateTime CurrentEstTime => DateTimeService.ToEstTime(CurrentTime);

    /// <summary>
    /// Mock object of <see cref="IDateTimeService"/>.
    /// </summary>
    protected Mock<IDateTimeService> DateTimeSerivceMock { get; }

    /// <summary>
    /// Mock object of <see cref="ILogger"/>.
    /// </summary>
    protected Mock<ILogger> LoggerMock { get; }

    /// <summary>
    /// Mock object of <see cref="ILoggerFactory"/>.
    /// </summary>
    protected Mock<ILoggerFactory> LoggerFactoryMock { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TestBase"/> class.
    /// </summary>
    public TestBase()
    {
        Fixture = new Fixture();
        Fixture.Customize(new AutoMoqCustomization());

        Serializer = new SerializerBuilder().Build();
        Deserializer = new DeserializerBuilder().Build();

        LoggerMock = Fixture.Freeze<Mock<ILogger>>();
        LoggerFactoryMock = Fixture.Freeze<Mock<ILoggerFactory>>();

        var dateTimeSvc = new DateTimeService();
        CurrentTime = dateTimeSvc.UtcNow;
        DateTimeSerivceMock = Fixture.Freeze<Mock<IDateTimeService>>();
        setupLoggers();
        setupDateTimeService();

        void setupLoggers()
        {
            LoggerMock
                .Setup(it => it.IsEnabled(It.IsAny<LogLevel>()))
                .Returns(true);
            Fixture.Register(() => LoggerMock.Object);

            LoggerFactoryMock
                .Setup(it => it.CreateLogger(It.IsAny<string>()))
                .Returns(LoggerMock.Object);
            Fixture.Register(() => LoggerFactoryMock.Object);
        }
        void setupDateTimeService()
        {
            DateTimeSerivceMock
                .Setup(it => it.EstNow)
                .Returns(() => dateTimeSvc.ToEstTime(CurrentTime));
            DateTimeSerivceMock
                .Setup(it => it.UtcNow)
                .Returns(() => CurrentTime);
            DateTimeSerivceMock
                .Setup(it => it.ToNumericDateTime(It.IsAny<DateTime>(), It.IsAny<string>()))
                .Returns(dateTimeSvc.ToNumericDateTime);
            DateTimeSerivceMock
                .Setup(it => it.ParseNumericToUtcDateTime(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(dateTimeSvc.ParseNumericToUtcDateTime);
            DateTimeSerivceMock
                .Setup(it => it.ParseNumericToEstDateTime(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(dateTimeSvc.ParseNumericToEstDateTime);
            DateTimeSerivceMock
                .Setup(it => it.ToEstTime(It.IsAny<DateTime>()))
                .Returns(dateTimeSvc.ToEstTime);
            DateTimeSerivceMock
                .Setup(it => it.ToUtcTime(It.IsAny<DateTime>()))
                .Returns(dateTimeSvc.ToUtcTime);
            DateTimeSerivceMock
                .Setup(it => it.ToUnixTime(It.IsAny<DateTime>()))
                .Returns(dateTimeSvc.ToUnixTime);
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Register and setup service as a mock object.
    /// </summary>
    /// <typeparam name="TService">The service type</typeparam>
    /// <param name="setup">Setup mock object</param>
    /// <returns>The mock object</returns>
    protected virtual Mock<TService> SetupMock<TService>(Action<Mock<TService>>? setup = null) where TService : class
    {
        var mock = Fixture.Freeze<Mock<TService>>();
        Fixture.Register(() => mock.Object);
        setup?.Invoke(mock);
        return mock;
    }

    /// <summary>
    /// Verify loggging.
    /// </summary>
    /// <param name="expression">Verification expression</param>
    /// <param name="expectedTime">Expected times</param>
    protected void VerifyLog(Expression<Action<ILogger>> expression, Times expectedTime)
        => LoggerMock.VerifyLog(expression, expectedTime);

    #endregion
}
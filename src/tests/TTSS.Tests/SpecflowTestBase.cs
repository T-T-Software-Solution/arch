using Moq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace TTSS.Tests;

/// <summary>
/// Base class for all specflow tests.
/// </summary>
public abstract class SpecflowTestBase : TestBase
{
    #region Fields

    private readonly ISpecFlowOutputHelper _testOutput;

    #endregion

    #region Properties

    /// <summary>
    /// Scenario context.
    /// </summary>
    protected ScenarioContext Context { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SpecflowTestBase"/> class.
    /// </summary>
    /// <param name="context">The scenario context</param>
    /// <param name="testOutput">The output helper</param>
    /// <exception cref="ArgumentNullException">The testOutput is required</exception>
    public SpecflowTestBase(ScenarioContext context, ISpecFlowOutputHelper testOutput)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        _testOutput = testOutput ?? throw new ArgumentNullException(nameof(testOutput));
    }

    #endregion

    /// <summary>
    /// Write a line to the test output.
    /// </summary>
    /// <param name="message">The message</param>
    protected void WriteLine(string message)
        => _testOutput.WriteLine(message);

    /// <summary>
    /// Write a line to the test output.
    /// </summary>
    /// <param name="format">The format</param>
    /// <param name="args">Parameters</param>
    protected void WriteLine(string format, params object[] args)
        => _testOutput.WriteLine(format, args);

    /// <summary>
    /// Register and setup service as a mock object.
    /// </summary>
    /// <typeparam name="TService">The service type</typeparam>
    /// <param name="setup">Setup mock object</param>
    /// <returns>The mock object</returns>
    protected override Mock<TService> SetupMock<TService>(Action<Mock<TService>>? setup = null)
    {
        var mock = base.SetupMock(setup);
        Context.Set(mock.Object);
        return mock;
    }
}
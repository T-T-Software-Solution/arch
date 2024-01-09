using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using TTSS.Core.Loggings;
using TTSS.Tests;

namespace TTSS.Infra.Infra.Loggings;

public abstract class CommonTestCases : IoCTestBase
{
    public abstract string ExpectedLogCategoryName { get; }

    [Fact]
    public void Resolve_IActivityFactory_ShouldBeResolved()
    {
        var sut = ServiceProvider.GetRequiredService<IActivityFactory>();
        Assert.NotNull(sut);
    }

    [Fact]
    public void Create_ActivityFromGeneric_ShouldBeCreatedWithBasicInformation()
    {
        var sut = ServiceProvider.GetRequiredService<IActivityFactory>();
        var actual = sut.CreateActivity<CommonTestCases>();
        var expectedCallerName = nameof(Create_ActivityFromGeneric_ShouldBeCreatedWithBasicInformation);
        var expectedLogCategory = nameof(CommonTestCases);
        VerifyActivityCreation(actual, expectedCallerName, expectedLogCategory);
    }

    [Fact]
    public void Create_ActivityFromGeneric_WithCustomCallerName_ShouldBeCreatedWithSpecificCallerName()
    {
        var sut = ServiceProvider.GetRequiredService<IActivityFactory>();
        var callerName = Fixture.Create<string>();
        var actual = sut.CreateActivity<CommonTestCases>(callerName);
        var expectedCallerName = callerName;
        var expectedLogCategory = nameof(CommonTestCases);
        VerifyActivityCreation(actual, expectedCallerName, expectedLogCategory);
    }

    [Fact]
    public void Create_ActivityFromCustom_ShouldBeCreatedWithBasicInformation()
    {
        var sut = ServiceProvider.GetRequiredService<IActivityFactory>();
        var actual = sut.CreateActivity(this);
        var expectedCallerName = nameof(Create_ActivityFromCustom_ShouldBeCreatedWithBasicInformation);
        VerifyActivityCreation(actual, expectedCallerName, ExpectedLogCategoryName);
    }

    [Fact]
    public void Create_ActivityFromCustom_WithCustomCallerName_ShouldBeCreatedWithSpecificCallerName()
    {
        var sut = ServiceProvider.GetRequiredService<IActivityFactory>();
        var callerName = Fixture.Create<string>();
        var actual = sut.CreateActivity(this, callerName);
        var expectedCallerName = callerName;
        VerifyActivityCreation(actual, expectedCallerName, ExpectedLogCategoryName);
    }

    [Fact]
    public void Create_ActivityFromCustom_WithCustomCategoryNameAndCallerName_ShouldBeCreatedWithSpecificCategoryNameAndCallerName()
    {
        var sut = ServiceProvider.GetRequiredService<IActivityFactory>();
        var categoryName = Fixture.Create<string>();
        var callerName = Fixture.Create<string>();
        var actual = sut.CreateActivity(categoryName, callerName, System.Diagnostics.ActivityKind.Internal);
        var expectedCallerName = callerName;
        var expectedLogCategory = categoryName;
        VerifyActivityCreation(actual, expectedCallerName, expectedLogCategory);
    }

    #region Trace
    [Fact]
    public void Create_LogTrace_ShouldBeLogged()
    {
        var message = Fixture.Create<string>();
        VerifyLog(it =>
            it.LogTrace(message),
            it => it.LogTrace(
                It.Is<string>(actual => actual == message)));
    }
    [Fact]
    public void Create_LogTrace_WithParameters_ShouldBeLogged()
    {
        var message = Fixture.Create<string>();
        var parameters = Fixture.Create<IEnumerable<string>>();
        VerifyLog(it =>
            it.LogTrace(message, parameters),
            it => it.LogTrace(
                It.Is<string>(actual => actual == message),
                It.Is<IEnumerable<string>>(actual => actual == parameters)));
    }
    [Fact]
    public void Create_LogTrace_WithException_ShouldBeLogged()
    {
        var message = Fixture.Create<string>();
        var exception = Fixture.Create<Exception>();
        var parameters = Fixture.Create<IEnumerable<string>>();
        VerifyLog(it =>
            it.LogTrace(exception, message, parameters),
            it => it.LogTrace(
                It.Is<Exception>(actual => actual == exception),
                It.Is<string>(actual => actual == message),
                It.Is<IEnumerable<string>>(actual => actual == parameters)));
    }
    #endregion

    #region Debug
    [Fact]
    public void Create_LogDebug_ShouldBeLogged()
    {
        var message = Fixture.Create<string>();
        VerifyLog(it =>
            it.LogDebug(message),
            it => it.LogDebug(
                It.Is<string>(actual => actual == message)));
    }
    [Fact]
    public void Create_LogDebug_WithParameters_ShouldBeLogged()
    {
        var message = Fixture.Create<string>();
        var parameters = Fixture.Create<IEnumerable<string>>();
        VerifyLog(it =>
            it.LogDebug(message, parameters),
            it => it.LogDebug(
                It.Is<string>(actual => actual == message),
                It.Is<IEnumerable<string>>(actual => actual == parameters)));
    }
    [Fact]
    public void Create_LogDebug_WithException_ShouldBeLogged()
    {
        var message = Fixture.Create<string>();
        var exception = Fixture.Create<Exception>();
        var parameters = Fixture.Create<IEnumerable<string>>();
        VerifyLog(it =>
            it.LogDebug(exception, message, parameters),
            it => it.LogDebug(
                It.Is<Exception>(actual => actual == exception),
                It.Is<string>(actual => actual == message),
                It.Is<IEnumerable<string>>(actual => actual == parameters)));
    }
    #endregion

    #region Information
    [Fact]
    public void Create_LogInformation_ShouldBeLogged()
    {
        var message = Fixture.Create<string>();
        VerifyLog(it =>
            it.LogInformation(message),
            it => it.LogInformation(
                It.Is<string>(actual => actual == message)));
    }
    [Fact]
    public void Create_LogInformation_WithParameters_ShouldBeLogged()
    {
        var message = Fixture.Create<string>();
        var parameters = Fixture.Create<IEnumerable<string>>();
        VerifyLog(it =>
            it.LogInformation(message, parameters),
            it => it.LogInformation(
                It.Is<string>(actual => actual == message),
                It.Is<IEnumerable<string>>(actual => actual == parameters)));
    }
    [Fact]
    public void Create_LogInformation_WithException_ShouldBeLogged()
    {
        var message = Fixture.Create<string>();
        var exception = Fixture.Create<Exception>();
        var parameters = Fixture.Create<IEnumerable<string>>();
        VerifyLog(it =>
            it.LogInformation(exception, message, parameters),
            it => it.LogInformation(
                It.Is<Exception>(actual => actual == exception),
                It.Is<string>(actual => actual == message),
                It.Is<IEnumerable<string>>(actual => actual == parameters)));
    }
    #endregion

    #region Warning
    [Fact]
    public void Create_LogWarning_ShouldBeLogged()
    {
        var message = Fixture.Create<string>();
        VerifyLog(it =>
            it.LogWarning(message),
            it => it.LogWarning(
                It.Is<string>(actual => actual == message)));
    }
    [Fact]
    public void Create_LogWarning_WithParameters_ShouldBeLogged()
    {
        var message = Fixture.Create<string>();
        var parameters = Fixture.Create<IEnumerable<string>>();
        VerifyLog(it =>
            it.LogWarning(message, parameters),
            it => it.LogWarning(
                It.Is<string>(actual => actual == message),
                It.Is<IEnumerable<string>>(actual => actual == parameters)));
    }
    [Fact]
    public void Create_LogWarning_WithException_ShouldBeLogged()
    {
        var message = Fixture.Create<string>();
        var exception = Fixture.Create<Exception>();
        var parameters = Fixture.Create<IEnumerable<string>>();
        VerifyLog(it =>
            it.LogWarning(exception, message, parameters),
            it => it.LogWarning(
                It.Is<Exception>(actual => actual == exception),
                It.Is<string>(actual => actual == message),
                It.Is<IEnumerable<string>>(actual => actual == parameters)));
    }
    #endregion

    #region Error
    [Fact]
    public void Create_LogError_ShouldBeLogged()
    {
        var message = Fixture.Create<string>();
        VerifyLog(it =>
            it.LogError(message),
            it => it.LogError(
                It.Is<string>(actual => actual == message)));
    }
    [Fact]
    public void Create_LogError_WithParameters_ShouldBeLogged()
    {
        var message = Fixture.Create<string>();
        var parameters = Fixture.Create<IEnumerable<string>>();
        VerifyLog(it =>
            it.LogError(message, parameters),
            it => it.LogError(
                It.Is<string>(actual => actual == message),
                It.Is<IEnumerable<string>>(actual => actual == parameters)));
    }
    [Fact]
    public void Create_LogError_WithException_ShouldBeLogged()
    {
        var message = Fixture.Create<string>();
        var exception = Fixture.Create<Exception>();
        var parameters = Fixture.Create<IEnumerable<string>>();
        VerifyLog(it =>
            it.LogError(exception, message, parameters),
            it => it.LogError(
                It.Is<Exception>(actual => actual == exception),
                It.Is<string>(actual => actual == message),
                It.Is<IEnumerable<string>>(actual => actual == parameters)));
    }
    #endregion

    #region Critical
    [Fact]
    public void Create_LogCritical_ShouldBeLogged()
    {
        var message = Fixture.Create<string>();
        VerifyLog(it =>
            it.LogCritical(message),
            it => it.LogCritical(
                It.Is<string>(actual => actual == message)));
    }
    [Fact]
    public void Create_LogCritical_WithParameters_ShouldBeLogged()
    {
        var message = Fixture.Create<string>();
        var parameters = Fixture.Create<IEnumerable<string>>();
        VerifyLog(it =>
            it.LogCritical(message, parameters),
            it => it.LogCritical(
                It.Is<string>(actual => actual == message),
                It.Is<IEnumerable<string>>(actual => actual == parameters)));
    }
    [Fact]
    public void Create_LogCritical_WithException_ShouldBeLogged()
    {
        var message = Fixture.Create<string>();
        var exception = Fixture.Create<Exception>();
        var parameters = Fixture.Create<IEnumerable<string>>();
        VerifyLog(it =>
            it.LogCritical(exception, message, parameters),
            it => it.LogCritical(
                It.Is<Exception>(actual => actual == exception),
                It.Is<string>(actual => actual == message),
                It.Is<IEnumerable<string>>(actual => actual == parameters)));
    }
    #endregion

    private void VerifyLog(Action<IActivity> action, Expression<Action<ILogger>> expression)
    {
        var sut = ServiceProvider.GetRequiredService<IActivityFactory>();
        var actual = sut.CreateActivity<CommonTestCases>();
        action(actual);
        VerifyLog(expression, Times.Exactly(1));
    }

    private static void VerifyActivityCreation(IActivity actual, string expectedCallerName, string expectedLogCategory)
    {
        actual.Should().NotBeNull();
        actual.Should().BeOfType<ActivityLogger>();
        var activity = actual as ActivityLogger;
        activity.CallerName.Should().Be(expectedCallerName);
        activity.LogCategory.Should().Be(expectedLogCategory);
    }
}
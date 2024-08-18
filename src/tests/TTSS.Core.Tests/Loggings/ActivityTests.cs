using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics;
using TTSS.Tests;
using Xunit.Abstractions;

namespace TTSS.Core.Loggings;

public class ActivityTests : TestBase
{
    private readonly ActivityListener _listener;

    private ActivityFactory Sut => Fixture.Create<ActivityFactory>();

    public ActivityTests(ITestOutputHelper testOutput)
    {
        _listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStopped = activity => testOutput.WriteLine($"{activity.ParentId}:{activity.Id} - Stop"),
            ActivityStarted = activity => testOutput.WriteLine($"{activity.ParentId}:{activity.Id} - Start"),
        };
        ActivitySource.AddActivityListener(_listener);
        Fixture.Register(() => new ActivitySource(nameof(ActivityTests)));
    }

    #region Create by object

    [Fact]
    public void CreateActivityByObject()
    {
        using var activity = (ActivityLogger)Sut.CreateActivity(this);
        VerifyPlainActivityObject(activity, nameof(ActivityTests), nameof(CreateActivityByObject), ActivityKind.Internal);
    }

    [Theory]
    [InlineData(ActivityKind.Internal)]
    [InlineData(ActivityKind.Server)]
    [InlineData(ActivityKind.Client)]
    [InlineData(ActivityKind.Producer)]
    [InlineData(ActivityKind.Consumer)]
    public void CreateActivityByObjectWithCustomAttributes(ActivityKind kind)
    {
        var callerName = Guid.NewGuid().ToString();
        using var activity = (ActivityLogger)Sut.CreateActivity(this, callerName, kind);
        VerifyPlainActivityObject(activity, nameof(ActivityTests), callerName, kind);
    }

    #endregion

    #region Create by Generic

    [Fact]
    public void CreateActivityByGeneric()
    {
        using var activity = (ActivityLogger)Sut.CreateActivity<ActivityTests>();
        VerifyPlainActivityObject(activity, nameof(ActivityTests), nameof(CreateActivityByGeneric), ActivityKind.Internal);
    }

    [Theory]
    [InlineData(ActivityKind.Internal)]
    [InlineData(ActivityKind.Server)]
    [InlineData(ActivityKind.Client)]
    [InlineData(ActivityKind.Producer)]
    [InlineData(ActivityKind.Consumer)]
    public void CreateActivityByGenericWithCustomAttributes(ActivityKind kind)
    {
        var callerName = Guid.NewGuid().ToString();
        using var activity = (ActivityLogger)Sut.CreateActivity<ActivityTests>(callerName, kind);
        VerifyPlainActivityObject(activity, nameof(ActivityTests), callerName, kind);
    }

    #endregion

    #region Create by primitive

    [Fact]
    public void CreateActivityByPrimitive()
    {
        var logCategoryName = nameof(ActivityTests);
        var callerName = nameof(CreateActivityByPrimitive);
        using var activity = (ActivityLogger)Sut.CreateActivity(logCategoryName, callerName, ActivityKind.Internal);
        VerifyPlainActivityObject(activity, nameof(ActivityTests), nameof(CreateActivityByPrimitive), ActivityKind.Internal);
    }

    [Theory]
    [InlineData(ActivityKind.Internal)]
    [InlineData(ActivityKind.Server)]
    [InlineData(ActivityKind.Client)]
    [InlineData(ActivityKind.Producer)]
    [InlineData(ActivityKind.Consumer)]
    public void CreateActivityByPrimitiveWithCustomAttributes(ActivityKind kind)
    {
        var callerName = Guid.NewGuid().ToString();
        var logCategoryName = nameof(ActivityTests);
        using var activity = (ActivityLogger)Sut.CreateActivity(logCategoryName, callerName, kind);
        VerifyPlainActivityObject(activity, nameof(ActivityTests), callerName, kind);
    }

    #endregion

    private void VerifyPlainActivityObject(IActivity actual, string expectedLogCategory, string expectedCallerName, ActivityKind expectedKind)
    {
        var activity = (ActivityLogger)actual;
        activity.Should().NotBeNull();
        activity.Activity.Source.Name.Should().Be(nameof(ActivityTests));
        activity.Activity.Kind.Should().Be(expectedKind);
        activity.Activity.Events.Should().BeEmpty();
        activity.Activity.Baggage.Should().BeEmpty();
        activity.Activity.TagObjects.Should().BeEmpty();
        activity.LogCategory.Should().Be(expectedLogCategory);
        activity.CallerName.Should().Be(expectedCallerName);
    }

    [Theory]
    [InlineData("TagName", "Text")]
    [InlineData("TagName", "Text with whitespace")]
    [InlineData("TagName", "Text with special character !@#$%^&*()_-+")]
    [InlineData("TagName", " ")]
    [InlineData("TagName", "")]
    [InlineData("TagName", null)]
    [InlineData("Tag whit whitespace", "Text")]
    [InlineData("Tag with special character !@#$%^&*()_-+", "Text")]
    [InlineData(" ", "Text")]
    [InlineData("", "Text")]
    [InlineData(null, "Text")]
    public void AddTagWithPlainText(string name, string value)
    {
        using var activity = (ActivityLogger)Sut.CreateActivity<ActivityTests>();
        activity.AddTag(name, value);
        activity.Activity.TagObjects.Should().HaveCount(1);
        activity.Activity.TagObjects.Should().AllBeEquivalentTo(new KeyValuePair<string, object>(name, value));
    }

    [Theory]
    [InlineData("TagName", 100)]
    [InlineData("TagName", 9999999999)]
    [InlineData("TagName", 100.999999)]
    [InlineData("TagName", 0)]
    [InlineData("TagName", -100)]
    [InlineData("TagName", -9999999999)]
    [InlineData("TagName", -100.999999)]
    public void AddTagWithNumber(string name, double value)
    {
        using var activity = (ActivityLogger)Sut.CreateActivity<ActivityTests>();
        activity.AddTag(name, value);
        activity.Activity.TagObjects.Should().HaveCount(1);
        activity.Activity.TagObjects.Should().AllBeEquivalentTo(new KeyValuePair<string, object>(name, value));
    }

    [Fact]
    public void AddTagWithObject()
    {
        using var activity = (ActivityLogger)Sut.CreateActivity<ActivityTests>();
        const string name = "TagName";
        var value = new { Id = 1, Name = Guid.NewGuid() };
        activity.AddTag(name, value);
        activity.Activity.TagObjects.Should().HaveCount(1);
        activity.Activity.TagObjects.Should().AllBeEquivalentTo(new KeyValuePair<string, object>(name, value));
    }

    [Fact]
    public void DuplicatedAddTags()
    {
        using var activity = (ActivityLogger)Sut.CreateActivity<ActivityTests>();
        const string name = "TagName";
        activity.AddTag(name, "Hello World");
        activity.AddTag(name, 9999);
        activity.AddTag(name, new { Id = 1, Name = 1111 });
        activity.AddTag(name, new { Id = 2, Name = 2222 });
        activity.Activity.TagObjects.Should().HaveCount(4);
        activity.Activity.TagObjects.Should().BeEquivalentTo(new[]
        {
                new KeyValuePair<string, object>(name, "Hello World"),
                new KeyValuePair<string, object>(name, 9999),
                new KeyValuePair<string, object>(name, new { Id = 1, Name = 1111 }),
                new KeyValuePair<string, object>(name, new { Id = 2, Name = 2222 }),
            });
    }

    [Theory]
    [InlineData("TagName", "Text")]
    [InlineData("TagName", "Text with whitespace")]
    [InlineData("TagName", "Text with special character !@#$%^&*()_-+")]
    [InlineData("TagName", " ")]
    [InlineData("TagName", "")]
    [InlineData("Tag whit whitespace", "Text")]
    [InlineData("Tag with special character !@#$%^&*()_-+", "Text")]
    [InlineData(" ", "Text")]
    [InlineData("", "Text")]
    [InlineData(null, "Text")]
    public void SetTagWithPlainText(string name, string value)
    {
        using var activity = (ActivityLogger)Sut.CreateActivity<ActivityTests>();
        activity.SetTag(name, value);
        activity.Activity.TagObjects.Should().HaveCount(1);
        activity.Activity.TagObjects.Should().AllBeEquivalentTo(new KeyValuePair<string, object>(name, value));
    }

    [Fact]
    public void SetTagWithNullValue_MustNotAddTag()
    {
        using var activity = (ActivityLogger)Sut.CreateActivity<ActivityTests>();
        activity.SetTag("TagName", null);
        activity.Activity.TagObjects.Should().BeEmpty();
    }

    [Theory]
    [InlineData("TagName", 100)]
    [InlineData("TagName", 9999999999)]
    [InlineData("TagName", 100.999999)]
    [InlineData("TagName", 0)]
    [InlineData("TagName", -100)]
    [InlineData("TagName", -9999999999)]
    [InlineData("TagName", -100.999999)]
    public void SetTagWithNumber(string name, double value)
    {
        using var activity = (ActivityLogger)Sut.CreateActivity<ActivityTests>();
        activity.SetTag(name, value);
        activity.Activity.TagObjects.Should().HaveCount(1);
        activity.Activity.TagObjects.Should().AllBeEquivalentTo(new KeyValuePair<string, object>(name, value));
    }

    [Fact]
    public void SetTagWithObject()
    {
        using var activity = (ActivityLogger)Sut.CreateActivity<ActivityTests>();
        var name = "TagName";
        var value = new { Id = 1, Name = Guid.NewGuid() };
        activity.SetTag(name, value);
        activity.Activity.TagObjects.Should().HaveCount(1);
        activity.Activity.TagObjects.Should().AllBeEquivalentTo(new KeyValuePair<string, object>(name, value));
    }

    [Fact]
    public void DuplicatedSetTags()
    {
        using var activity = (ActivityLogger)Sut.CreateActivity<ActivityTests>();
        const string name = "TagName";
        activity.SetTag(name, "Hello World");
        activity.SetTag(name, 9999);
        activity.SetTag(name, new { Id = 1, Name = 1111 });
        activity.SetTag(name, new { Id = 2, Name = 2222 });
        activity.Activity.TagObjects.Should().HaveCount(1);
        activity.Activity.TagObjects.Should().BeEquivalentTo(new[] { new KeyValuePair<string, object>(name, new { Id = 2, Name = 2222 }) });
    }

    [Theory]
    [InlineData("TagName", "Text")]
    [InlineData("TagName", "Text with whitespace")]
    [InlineData("TagName", "Text with special character !@#$%^&*()_-+")]
    [InlineData("TagName", " ")]
    [InlineData("TagName", "")]
    [InlineData("TagName", null)]
    [InlineData("Tag whit whitespace", "Text")]
    [InlineData("Tag with special character !@#$%^&*()_-+", "Text")]
    [InlineData(" ", "Text")]
    [InlineData("", "Text")]
    [InlineData(null, "Text")]
    public void AddBaggage(string name, string value)
    {
        using var activity = (ActivityLogger)Sut.CreateActivity<ActivityTests>();
        activity.AddBaggage(name, value);
        activity.Activity.Baggage.Should().HaveCount(1);
        activity.Activity.Baggage.Should().AllBeEquivalentTo(new KeyValuePair<string, string>(name, value));
    }

    [Fact]
    public void DuplicatedBaggages()
    {
        using var activity = (ActivityLogger)Sut.CreateActivity<ActivityTests>();
        const string name = "TagName";
        activity.AddBaggage(name, "1111");
        activity.AddBaggage(name, "2222");
        activity.AddBaggage(name, "3333");
        activity.AddBaggage(name, "4444");
        activity.Activity.Baggage.Should().HaveCount(4);
        activity.Activity.Baggage.Should().BeEquivalentTo(new[]
        {
                new KeyValuePair<string, string>(name, "1111"),
                new KeyValuePair<string, string>(name, "2222"),
                new KeyValuePair<string, string>(name, "3333"),
                new KeyValuePair<string, string>(name, "4444"),
            });
    }

    [Fact]
    public void AddEvent()
    {
        using var activity = (ActivityLogger)Sut.CreateActivity<ActivityTests>();
        activity.AddEvent(new());
        activity.Activity.Events.Should().HaveCount(1);
        activity.Activity.Events.Should().AllBeEquivalentTo(new ActivityEvent());
    }

    [Theory]
    [InlineData("EventName")]
    [InlineData("EventName with whitespace")]
    [InlineData("EventName with special character !@#$%^&*()_-+")]
    [InlineData(" ")]
    [InlineData("")]
    [InlineData(null)]
    public void AddEventName(string name)
    {
        using var activity = (ActivityLogger)Sut.CreateActivity<ActivityTests>();
        var eventObj = new ActivityEvent(name);
        activity.AddEvent(eventObj);
        activity.Activity.Events.Should().HaveCount(1);
        activity.Activity.Events.Should().AllBeEquivalentTo(eventObj);
    }

    [Fact]
    public void DuplicatedAddEventName()
    {
        using var activity = (ActivityLogger)Sut.CreateActivity<ActivityTests>();
        var eventObj = new ActivityEvent(Guid.NewGuid().ToString());
        activity.AddEvent(eventObj);
        activity.AddEvent(eventObj);
        activity.AddEvent(eventObj);
        activity.AddEvent(eventObj);
        activity.Activity.Events.Should().HaveCount(4);
        activity.Activity.Events.Should().BeEquivalentTo(new[]
        {
                eventObj,
                eventObj,
                eventObj,
                eventObj,
            });
    }

    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    [InlineData(LogLevel.None)]
    public void WriteLog(LogLevel logLevel)
    {
        using var activity = Sut.CreateActivity<ActivityTests>();
        activity.Log(logLevel, null, "Hello World");
        LoggerMock.Verify(it => it.IsEnabled(logLevel), Times.Exactly(1));
    }

#pragma warning disable xUnit1013 // Public method should be marked as test
    public void Dispose()
        => _listener?.Dispose();
#pragma warning restore xUnit1013 // Public method should be marked as test
}
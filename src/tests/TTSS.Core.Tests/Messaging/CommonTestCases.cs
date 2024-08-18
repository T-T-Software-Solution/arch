using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Messaging.Pipelines;
using TTSS.Core.Messaging.Subscribers;
using TTSS.Tests;

namespace TTSS.Core.Messaging;

public abstract class CommonTestCases : IoCTestBase
{
    private Mock<ITestInterface> Mock => Fixture.Freeze<Mock<ITestInterface>>();

    [Fact]
    public void Resolve_MessaingHubFromIoC_ShouldBeResolved()
    {
        var hub = ServiceProvider.GetRequiredService<IMessagingHub>();
        hub.Should().NotBeNull();
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<AsyncTwoWayInternal>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Send_OneWayWithNull_TheSystemMustNotThrowAnException()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        OneWay request = null;
        await sut.SendAsync(request);
        Mock.Verify(it => it.Execute(It.IsAny<OneWay>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Send_TwoWayWithNull_TheSystemMustNotThrowAnException()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        TwoWay request = null;
        var actual = await sut.SendAsync(request);
        actual.Should().BeNull();
        Mock.Verify(it => it.Execute(It.IsAny<TwoWay>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #region One-way requests

    [Fact]
    public async Task Send_OneWayMessage_PairedHandlerWillHandleIt()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<OneWay>();
        await sut.SendAsync(request);
        request.Name.Should().Be(nameof(OneWayHandler));
        Mock.Verify(it => it.Execute(It.Is<OneWay>(act => act == request)), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Send_OneWayInternalMessage_PairedHandlerWillHandleIt()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<OneWayInternal>();
        await sut.SendAsync(request);
        request.Name.Should().Be(nameof(OneWayInternalHandler));
        Mock.Verify(it => it.Execute(It.Is<OneWayInternal>(act => act == request)), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Send_OneWayWithoutHandlerMessage_AnExceptionWillBeThrown()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var name = Guid.NewGuid().ToString();
        var request = Fixture
            .Build<OneWayWithoutHandler>()
            .With(it => it.Name, name)
            .Create();
        var act = async () => await sut.SendAsync(request);
        await act.Should().ThrowAsync<InvalidOperationException>();
        request.Name.Should().Be(name);
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Send_OneWayErrorMessage_AnExceptionWillBeThrown()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<OneWayError>();
        var act = async () => await sut.SendAsync(request);
        await act.Should().ThrowAsync<InvalidOperationException>();
        request.Name.Should().Be(nameof(OneWayErrorHandler));
        Mock.Verify(it => it.Execute(It.IsAny<OneWayError>()), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Two-way requests

    [Fact]
    public async Task Send_TwoWayMessage_PairedHandlerWillHandleIt()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<TwoWay>();
        var actual = await sut.SendAsync(request);
        actual.Should().BeEquivalentTo(new TwoWayResponse(99));
        request.Name.Should().Be(nameof(TwoWayHandler));
        Mock.Verify(it => it.Execute(It.Is<TwoWay>(act => act == request)), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Send_TwoWayInternalMessage_PairedHandlerWillHandleIt()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<TwoWayInternal>();
        var actual = await sut.SendAsync(request);
        actual.Should().BeEquivalentTo(new TwoWayResponse(99));
        request.Name.Should().Be(nameof(TwoWayInternalHandler));
        Mock.Verify(it => it.Execute(It.Is<TwoWayInternal>(act => act == request)), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Send_TwoWayWithoutHandlerMessage_AnExceptionWillBeThrown()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<TwoWayWithoutHandler>();
        var act = async () => await sut.SendAsync(request);
        await act.Should().ThrowAsync<InvalidOperationException>();
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Send_TwoWayErrorMessage_AnExceptionWillBeThrown()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<TwoWayError>();
        var act = async () => await sut.SendAsync(request);
        await act.Should().ThrowAsync<InvalidOperationException>();
        request.Name.Should().Be(nameof(TwoWayErrorHandler));
        Mock.Verify(it => it.Execute(It.IsAny<TwoWayError>()), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region One-way asynchroneous requests

    [Fact]
    public async Task Send_AsyncOneWayMessage_PairedHandlerWillHandleIt()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<AsyncOneWay>();
        await sut.SendAsync(request);
        request.Name.Should().Be(nameof(AsyncOneWayHandler));
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.Is<AsyncOneWay>(act => act == request), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Send_AsyncOneWayInternalMessage_PairedHandlerWillHandleIt()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<AsyncOneWayInternal>();
        await sut.SendAsync(request);
        request.Name.Should().Be(nameof(AsyncOneWayInternalHandler));
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.Is<AsyncOneWayInternal>(act => act == request), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Send_AsyncOneWayWithoutHandlerMessage_AnExceptionWillBeThrown()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var name = Guid.NewGuid().ToString();
        var request = Fixture
            .Build<AsyncOneWayWithoutHandler>()
            .With(it => it.Name, name)
            .Create();
        var act = async () => await sut.SendAsync(request);
        await act.Should().ThrowAsync<InvalidOperationException>();
        request.Name.Should().Be(name);
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Send_AsyncOneWayErrorMessage_AnExceptionWillBeThrown()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<AsyncOneWayError>();
        var act = async () => await sut.SendAsync(request);
        await act.Should().ThrowAsync<InvalidOperationException>();
        request.Name.Should().Be(nameof(AsyncOneWayErrorHandler));
        Mock.Verify(it => it.Execute(It.IsAny<AsyncOneWayError>()), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Two-way asynchroneous requests

    [Fact]
    public async Task Send_AsyncTwoWayMessage_PairedHandlerWillHandleIt()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<AsyncTwoWay>();
        var actual = await sut.SendAsync(request);
        actual.Should().BeEquivalentTo(new AsyncTwoWayResponse(999));
        request.Name.Should().Be(nameof(AsyncTwoWayHandler));
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.Is<AsyncTwoWay>(act => act == request), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Send_AsyncTwoWayInternalMessage_PairedHandlerWillHandleIt()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<AsyncTwoWayInternal>();
        var actual = await sut.SendAsync(request);
        actual.Should().BeEquivalentTo(new AsyncTwoWayResponse(999));
        request.Name.Should().Be(nameof(AsyncTwoWayInternalHandler));
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.Is<AsyncTwoWayInternal>(act => act == request), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Send_AsyncTwoWayWithoutHandlerMessage_AnExceptionWillBeThrown()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<AsyncTwoWayWithoutHandler>();
        var act = async () => await sut.SendAsync(request);
        await act.Should().ThrowAsync<InvalidOperationException>();
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Send_AsyncTwoWayErrorMessage_AnExceptionWillBeThrown()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<AsyncTwoWayError>();
        var act = async () => await sut.SendAsync(request);
        await act.Should().ThrowAsync<InvalidOperationException>();
        request.Name.Should().Be(nameof(AsyncTwoWayErrorHandler));
        Mock.Verify(it => it.Execute(It.IsAny<AsyncTwoWayError>()), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region One-way chain calls

    [Fact]
    public async Task Send_OneWayCallTwoWayMessage_PairedHandlersWillHandleIt()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<OneWayCallTwoWay>();
        await sut.SendAsync(request);
        request.Name.Should().Be(nameof(OneWayCallTwoWayHandler));
        request.ValueFromTwoWay.Should().Be(99);
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Exactly(2));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Send_OneWayCallAsyncTwoWayMessage_PairedHandlersWillHandleIt()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<OneWayCallAsyncTwoWay>();
        await sut.SendAsync(request);
        request.Name.Should().Be(nameof(OneWayCallAsyncTwoWayHandler));
        request.ValueFromTwoWay.Should().Be(999);
        Mock.Verify(it => it.Execute(It.IsAny<OneWayCallAsyncTwoWay>()), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<AsyncTwoWay>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Send_AsyncOneWayCallTwoWayMessage_PairedHandlersWillHandleIt()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<AsyncOneWayCallTwoWay>();
        await sut.SendAsync(request);
        request.Name.Should().Be(nameof(AsyncOneWayCallTwoWayHandler));
        request.ValueFromTwoWay.Should().Be(99);
        Mock.Verify(it => it.Execute(It.IsAny<TwoWay>()), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<AsyncOneWayCallTwoWay>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Send_AsyncOneWayCallAsyncTwoWayMessage_PairedHandlersWillHandleIt()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<AsyncOneWayCallAsyncTwoWay>();
        await sut.SendAsync(request);
        request.Name.Should().Be(nameof(AsyncOneWayCallAsyncTwoWayHandler));
        request.ValueFromTwoWay.Should().Be(999);
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    #endregion

    #region Two-way chain calls

    [Fact]
    public async Task Send_TwoWayCallOneWayMessage_PairedHandlerWillHandleIt()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<TwoWayCallOneWay>();
        var actual = await sut.SendAsync(request);
        actual.Should().BeEquivalentTo(new TwoWayCallOneWayResponse(99) { NameFromOneWay = nameof(OneWayHandler) });
        request.Name.Should().Be(nameof(TwoWayCallOneWayHandler));
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Exactly(2));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Send_TwoWayCallAsyncOneWayMessage_PairedHandlerWillHandleIt()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<TwoWayCallAsyncOneWay>();
        var actual = await sut.SendAsync(request);
        actual.Should().BeEquivalentTo(new TwoWayCallOneWayResponse(99) { NameFromOneWay = nameof(AsyncOneWayHandler) });
        request.Name.Should().Be(nameof(TwoWayCallAsyncOneWayHandler));
        Mock.Verify(it => it.Execute(It.IsAny<TwoWayCallAsyncOneWay>()), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<AsyncOneWay>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Send_AsyncTwoWayCallOneWayMessage_PairedHandlerWillHandleIt()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<AsyncTwoWayCallOneWay>();
        var actual = await sut.SendAsync(request);
        actual.Should().BeEquivalentTo(new AsyncTwoWayCallOneWayResponse(999) { NameFromOneWay = nameof(OneWayHandler) });
        request.Name.Should().Be(nameof(AsyncTwoWayCallOneWayHandler));
        Mock.Verify(it => it.Execute(It.IsAny<OneWay>()), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<AsyncTwoWayCallOneWay>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Send_AsyncTwoWayCallAsyncOneWayMessage_PairedHandlerWillHandleIt()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture.Create<AsyncTwoWayCallAsyncOneWay>();
        var actual = await sut.SendAsync(request);
        actual.Should().BeEquivalentTo(new AsyncTwoWayCallAsyncOneWayResponse(999) { NameFromOneWay = nameof(AsyncOneWayHandler) });
        request.Name.Should().Be(nameof(AsyncTwoWayCallAsyncOneWayHandler));
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    #endregion

    #region Publish synchronous

    [Fact]
    public async Task Publish_NoneSubscriber_ShouldBeNotifiedWithoutException()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var noti = new NoneSubscriber();
        await sut.PublishAsync(noti);
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Publish_SingleSubscriber_ShouldBeNotified()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var noti = new SingleSubscriber();
        await sut.PublishAsync(noti);
        noti.HandlerNames.Should().BeEquivalentTo([nameof(SingleSubscriberHandler)]);
        Mock.Verify(it => it.Execute(It.Is<SingleSubscriber>(act => act == noti)), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Publish_MultiSubscriber_ShouldBeNotified()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var noti = new MultiSubscriber();
        await sut.PublishAsync(noti);
        var handlers = new[]
        {
                nameof(MultiSubscriberHandler1),
                nameof(MultiSubscriberHandler2),
                nameof(MultiSubscriberHandler3),
            };
        noti.HandlerNames.Should().BeEquivalentTo(handlers);
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Exactly(handlers.Length));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Publish_SingleSubscriberInternal_ShouldBeNotified()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var noti = new SingleSubscriberInternal();
        await sut.PublishAsync(noti);
        noti.HandlerNames.Should().BeEquivalentTo([nameof(SingleSubscriberInternalHandler)]);
        Mock.Verify(it => it.Execute(It.Is<SingleSubscriberInternal>(act => act == noti)), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Publish_ChainPublishNotification_ShouldBeNotified()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var noti = new ChainPublishNotification();
        await sut.PublishAsync(noti);
        var handlers = new[]
        {
            nameof(ChainPublishNotificationHandler),
            nameof(MultiSubscriberHandler1),
            nameof(MultiSubscriberHandler2),
            nameof(MultiSubscriberHandler3),
        };
        noti.HandlerNames.Should().BeEquivalentTo(handlers);
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Exactly(handlers.Length));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Publish_SingleSubscriberCallAsyncSingleSubscriber_ShouldBeNotified()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var noti = new SingleSubscriberCallAsyncSingleSubscriber();
        await sut.PublishAsync(noti);
        var handlers = new[]
        {
            nameof(SingleSubscriberCallAsyncSingleSubscriberHandler),
            nameof(AsyncSingleSubscriberHandler),
        };
        noti.HandlerNames.Should().BeEquivalentTo(handlers);
        Mock.Verify(it => it.Execute(It.Is<SingleSubscriberCallAsyncSingleSubscriber>(act => act == noti)), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    #endregion

    #region Publish asynchronous

    [Fact]
    public async Task Publish_AsyncNoneSubscriber_ShouldBeNotifiedWithoutException()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var noti = new AsyncNoneSubscriber();
        await sut.PublishAsync(noti);
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Publish_AsyncSingleSubscriber_ShouldBeNotified()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var noti = new AsyncSingleSubscriber();
        await sut.PublishAsync(noti);
        noti.HandlerNames.Should().BeEquivalentTo([nameof(AsyncSingleSubscriberHandler)]);
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.Is<AsyncSingleSubscriber>(act => act == noti), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Publish_AsyncMultiSubscriber_ShouldBeNotified()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var noti = new AsyncMultiSubscriber();
        await sut.PublishAsync(noti);
        var handlers = new[]
        {
            nameof(AsyncMultiSubscriberHandler1),
            nameof(AsyncMultiSubscriberHandler2),
            nameof(AsyncMultiSubscriberHandler3),
        };
        noti.HandlerNames.Should().BeEquivalentTo(handlers);
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Exactly(handlers.Length));
    }

    [Fact]
    public async Task Publish_AsyncSingleSubscriberInternal_ShouldBeNotified()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var noti = new AsyncSingleSubscriberInternal();
        await sut.PublishAsync(noti);
        noti.HandlerNames.Should().BeEquivalentTo([nameof(AsyncSingleSubscriberInternalHandler)]);
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.Is<AsyncSingleSubscriberInternal>(act => act == noti), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Publish_AsyncChainPublishNotification_ShouldBeNotified()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var noti = new AsyncChainPublishNotification();
        await sut.PublishAsync(noti);
        var handlers = new[]
        {
                nameof(AsyncChainPublishNotificationHandler),
                nameof(AsyncMultiSubscriberHandler1),
                nameof(AsyncMultiSubscriberHandler2),
                nameof(AsyncMultiSubscriberHandler3),
            };
        noti.HandlerNames.Should().BeEquivalentTo(handlers);
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Exactly(handlers.Length));
    }

    [Fact]
    public async Task Publish_AsyncSingleSubscriberCallAsyncSingleSubscriber_ShouldBeNotified()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var noti = new AsyncSingleSubscriberCallAsyncSingleSubscriber();
        await sut.PublishAsync(noti);
        var handlers = new[]
        {
            nameof(AsyncSingleSubscriberCallAsyncSingleSubscriberHandler),
            nameof(AsyncSingleSubscriberHandler),
        };
        noti.HandlerNames.Should().BeEquivalentTo(handlers);
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    #endregion

    #region Cross handlers from Notification to Request

    [Fact]
    public async Task Publish_SingleSubscriberCallOneWayRequest_ShouldBeNotifiedAndHandled()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var noti = new SingleSubscriberCallOneWayRequest();
        await sut.PublishAsync(noti);
        var handlers = new[]
        {
            nameof(SingleSubscriberCallOneWayRequestHandler),
            nameof(OneWayHandler),
        };
        noti.HandlerNames.Should().BeEquivalentTo(handlers);
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Exactly(2));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Publish_SingleSubscriberCallAsyncOneWayRequest_ShouldBeNotifiedAndHandled()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var noti = new SingleSubscriberCallAsyncOneWayRequest();
        await sut.PublishAsync(noti);
        var handlers = new[]
        {
            nameof(SingleSubscriberCallAsyncOneWayRequestHandler),
            nameof(AsyncOneWayHandler),
        };
        noti.HandlerNames.Should().BeEquivalentTo(handlers);
        Mock.Verify(it => it.Execute(It.IsAny<SingleSubscriberCallAsyncOneWayRequest>()), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<AsyncOneWay>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Publish_AsyncSingleSubscriberCallOneWayRequest_ShouldBeNotifiedAndHandled()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var noti = new AsyncSingleSubscriberCallOneWayRequest();
        await sut.PublishAsync(noti);
        var handlers = new[]
        {
            nameof(AsyncSingleSubscriberCallOneWayRequestHandler),
            nameof(OneWayHandler),
        };
        noti.HandlerNames.Should().BeEquivalentTo(handlers);
        Mock.Verify(it => it.Execute(It.IsAny<OneWay>()), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<AsyncSingleSubscriberCallOneWayRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Publish_AsyncSingleSubscriberCallAsyncOneWayRequest_ShouldBeNotifiedAndHandled()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var noti = new AsyncSingleSubscriberCallAsyncOneWayRequest();
        await sut.PublishAsync(noti);
        var handlers = new[]
        {
            nameof(AsyncSingleSubscriberCallAsyncOneWayRequestHandler),
            nameof(AsyncOneWayHandler),
        };
        noti.HandlerNames.Should().BeEquivalentTo(handlers);
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    #endregion

    #region Cross handlers from Request to Notification

    [Fact]
    public async Task Send_OneWayCallSingleSubscriber_ShouldBeNotifiedAndHandled()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture
            .Build<OneWayCallSingleSubscriber>()
            .With(it => it.NotificationHandlerNames, [])
            .Create();
        await sut.SendAsync(request);
        request.Name.Should().Be(nameof(OneWayCallSingleSubscriberHandler));
        var handlers = new[]
        {
            nameof(SingleSubscriberHandler),
        };
        request.NotificationHandlerNames.Should().BeEquivalentTo(handlers);
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Exactly(2));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Send_OneWayCallAsyncSingleSubscriber_ShouldBeNotifiedAndHandled()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture
            .Build<OneWayCallAsyncSingleSubscriber>()
            .With(it => it.NotificationHandlerNames, [])
            .Create();
        await sut.SendAsync(request);
        request.Name.Should().Be(nameof(OneWayCallAsyncSingleSubscriberHandler));
        var handlers = new[]
        {
            nameof(AsyncSingleSubscriberHandler),
        };
        request.NotificationHandlerNames.Should().BeEquivalentTo(handlers);
        Mock.Verify(it => it.Execute(It.IsAny<OneWayCallAsyncSingleSubscriber>()), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<AsyncSingleSubscriber>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Send_AsyncOneWayCallSingleSubscriber_ShouldBeNotifiedAndHandled()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture
            .Build<AsyncOneWayCallSingleSubscriber>()
            .With(it => it.NotificationHandlerNames, [])
            .Create();
        await sut.SendAsync(request);
        request.Name.Should().Be(nameof(AsyncOneWayCallSingleSubscriberHandler));
        var handlers = new[]
        {
            nameof(SingleSubscriberHandler),
        };
        request.NotificationHandlerNames.Should().BeEquivalentTo(handlers);
        Mock.Verify(it => it.Execute(It.IsAny<SingleSubscriber>()), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<AsyncOneWayCallSingleSubscriber>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Send_AsyncOneWayCallAsyncSingleSubscriber_ShouldBeNotifiedAndHandled()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture
            .Build<AsyncOneWayCallAsyncSingleSubscriber>()
            .With(it => it.NotificationHandlerNames, [])
            .Create();
        await sut.SendAsync(request);
        request.Name.Should().Be(nameof(AsyncOneWayCallAsyncSingleSubscriberHandler));
        var handlers = new[]
        {
            nameof(AsyncSingleSubscriberHandler),
        };
        request.NotificationHandlerNames.Should().BeEquivalentTo(handlers);
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    #endregion

    #region Pipeline synchronous

    [Fact]
    public async Task Send_IncrementRequest_ThePipelinesMustBeTriggered()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture
            .Build<IncrementRequest>()
            .With(it => it.Number, 100)
            .Create();
        var actual = await sut.SendAsync(request);
        request.Number.Should().Be(101);
        actual.HandlerName.Should().Be(nameof(IncrementHandler));
        Mock.Verify(it => it.Execute(It.IsAny<IncrementRequest>()), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Send_MoreThan10Request_WithNumberMoreThan10_ThePipelinesMustBeTriggered()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture
            .Build<MoreThan10Request>()
            .With(it => it.Number, 20)
            .Create();
        var actual = await sut.SendAsync(request);
        request.Number.Should().Be(20);
        actual.HandlerName.Should().Be(nameof(MoreThan10Handler));
        Mock.Verify(it => it.Execute(It.IsAny<MoreThan10Request>()), Times.Exactly(1));
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Send_MoreThan10Request_WithNumberLowerThan10_ThePipelinesMustBeTriggered()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture
            .Build<MoreThan10Request>()
            .With(it => it.Number, 5)
            .Create();
        var actual = await sut.SendAsync(request);
        request.Number.Should().Be(5);
        actual.Should().BeNull();
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Pipeline asynchronous

    [Fact]
    public async Task Send_AsyncIncrementRequest_ThePipelinesMustBeTriggered()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture
            .Build<AsyncIncrementRequest>()
            .With(it => it.Number, 100)
            .Create();
        var actual = await sut.SendAsync(request);
        request.Number.Should().Be(101);
        actual.HandlerName.Should().Be(nameof(AsyncIncrementHandler));
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<AsyncIncrementRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Send_AsyncMoreThan10Request_WithNumberMoreThan10_ThePipelinesMustBeTriggered()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture
            .Build<AsyncMoreThan10Request>()
            .With(it => it.Number, 20)
            .Create();
        var actual = await sut.SendAsync(request);
        request.Number.Should().Be(20);
        actual.HandlerName.Should().Be(nameof(AsyncMoreThan10Handler));
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<AsyncMoreThan10Request>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Send_AsyncMoreThan10Request_WithNumberLowerThan10_ThePipelinesMustBeTriggered()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var request = Fixture
            .Build<AsyncMoreThan10Request>()
            .With(it => it.Number, 5)
            .Create();
        var actual = await sut.SendAsync(request);
        request.Number.Should().Be(5);
        actual.Should().BeNull();
        Mock.Verify(it => it.Execute(It.IsAny<It.IsAnyType>()), Times.Never);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Context

    [Fact]
    public async Task SendAMessageWithContextThruMultipleHandlers_ThenTheContextMustBeTheSameObjectAcrossThoseHandlers()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();
        var actual = await sut.SendAsync(Fixture.Create<FirstRequest>());
        VerifyExecutionWithContext(actual, 3, 1);
    }

    [Fact]
    public async Task SendAMessageWithContextThruMultipleHandlers_MultipleTimes_ThenTheContextMustBeTheSameObjectAcrossThoseHandlers()
    {
        var sut = ServiceProvider.GetRequiredService<IMessagingHub>();

        var act1 = await sut.SendAsync(Fixture.Create<FirstRequest>());
        VerifyExecutionWithContext(act1, 3, 1);

        var act2 = await sut.SendAsync(Fixture.Create<FirstRequest>());
        VerifyExecutionWithContext(act2, 6, 2);

        var act3 = await sut.SendAsync(Fixture.Create<FirstRequest>());
        VerifyExecutionWithContext(act3, 9, 3);
    }

    private void VerifyExecutionWithContext(FirstResponse actual, int expectedSummary, int expectedCalledHandlers)
    {
        actual.Should().NotBeNull();
        actual.Message.Should().Be(nameof(FirstHandler));
        actual.Data.Summary.Should().Be(expectedSummary);
        actual.Data.FirstHandlerCanReceive.Should().BeTrue();
        actual.Data.SecondHandlerCanReceive.Should().BeTrue();
        actual.Data.ThirdHandlerCanReceive.Should().BeTrue();
        actual.Data.MessageFromThirdHandler.Should().Be(nameof(ThirdHandler));
        actual.Data.FirstHandlerReceivedCorrelationId
            .Should()
            .Be(actual.Data.SecondHandlerReceivedCorrelationId)
            .And
            .Be(actual.Data.ThirdHandlerReceivedCorrelationId);
        Mock.Verify(it => it.ExecuteAsync(It.IsAny<FirstRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(expectedCalledHandlers));
        Mock.Verify(it => it.Execute(It.IsAny<SecondRequest>()), Times.Exactly(expectedCalledHandlers));
        Mock.Verify(it => it.Execute(It.IsAny<ThirdRequest>()), Times.Exactly(expectedCalledHandlers));
    }

    #endregion
}
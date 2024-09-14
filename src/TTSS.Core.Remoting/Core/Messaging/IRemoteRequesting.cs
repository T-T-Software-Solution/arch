﻿using TTSS.Core.Models;

namespace TTSS.Core.Messaging;

/// <summary>
/// Contract for a remote request message.
/// </summary>
public interface IRemoteRequesting : IRemoteRequest;

/// <summary>
/// Contract for a remote request message with a response.
/// </summary>
/// <typeparam name="TResponse">Response type</typeparam>
public interface IRemoteRequesting<out TResponse> : IRemoteRequest<TResponse> where TResponse : class;
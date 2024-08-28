﻿using TTSS.Core.Data;

namespace TTSS.Core.IdentityServer;

/// <summary>
/// Contract for identity database context.
/// </summary>
public interface IIdentityDbContext : IDbWarmup;
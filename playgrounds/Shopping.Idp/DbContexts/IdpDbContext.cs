using Microsoft.EntityFrameworkCore;
using TTSS.Core.IdentityServer;

namespace Shopping.Idp.DbContexts;

public class IdpDbContext(DbContextOptions options) : IdentityDbContextBase(options);
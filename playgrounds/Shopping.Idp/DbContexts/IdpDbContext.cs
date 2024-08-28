using Microsoft.EntityFrameworkCore;
using TTSS.Core.Web.IdentityServer;

namespace Shopping.Idp.DbContexts;

public class IdpDbContext(DbContextOptions options) : IdentityDbContextBase(options);
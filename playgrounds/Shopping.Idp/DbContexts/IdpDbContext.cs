using Microsoft.EntityFrameworkCore;
using TTSS.Core.Web.Identity.Server.DbContexts;

namespace Shopping.Idp.DbContexts;

public class IdpDbContext(DbContextOptions<IdpDbContext> options) : IdentityDbContextBase(options);
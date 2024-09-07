using Microsoft.EntityFrameworkCore;
using TTSS.Core.Web.Identity;

namespace Shopping.Idp.DbContexts;

public class IdpDbContext(DbContextOptions<IdpDbContext> options) : IdentityDbContextBase(options);
//public class IdpDbContextX(DbContextOptions<IdpDbContextX> opt) : IdentityDbContext<IdentityHubUser>(opt), IDbWarmup
//{
//    protected override void OnModelCreating(ModelBuilder builder)
//    {
//        base.OnModelCreating(builder);
//        builder.UseOpenIddict();
//    }

//    async Task IDbWarmup.WarmupAsync()
//    {
//        await Database.EnsureCreatedAsync();
//        await Database.ExecuteSqlRawAsync("SELECT 1"); // TODO: TBD alternative query
//    }
//}

//public class IdentityHubUser : IdentityUser
//{

//}
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using TTSS.Core.Web.Identity.Server.Configurations;
using TTSS.Core.Web.Identity.Server.Controllers;

namespace Shopping.Idp.Controllers;

public class AuthorizationController(IOptions<IdentityServerOptions> options, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    : AuthorizationControllerBase<IdentityUser>(options, userManager, signInManager)
{
    protected override void SetExtraUserInfo(ClaimsPrincipal claims, IDictionary<string, object?> userInfo)
    {
    }
}
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Client.AspNetCore;
using System.Security.Claims;
using TTSS.Core.Web.Identity.Client.Controllers;

namespace Shopping.WebApi.Controllers;

public class AuthenticationController : AuthenticationControllerBase
{
    protected override ActionResult? CreateLoginResult(ClaimsIdentity identity, AuthenticationProperties properties)
    {
        // NOT DO THIS IN PRODUCTION. THIS IS JUST FOR DEMO PURPOSES.
        var token = properties
            .GetTokens()
            .FirstOrDefault(it => it.Name == OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken)
            ?.Value ?? "NONE";

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("========== ACCESS TOKEN ==========");
        Console.WriteLine(token);
        Console.WriteLine("==================================");
        Console.ForegroundColor = ConsoleColor.White;

        return null;
    }
}
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Tokens;

public sealed record CreateToken : IRequesting<Response>
{
    public required string UserId { get; init; }
    public string FullName { get; init; }
}

internal sealed class CreateTokenHandler : RequestHandler<CreateToken, Response>
{
    // DON'T DO THIS IN PRODUCTION
    public const string Issuer = "demo";
    public const string Audience = "demo";
    private static readonly SecurityKey SigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("bf967e82cd8fe8ceb6596a395a9561ed3a14311a04c48bde41ac4039f3c7a1e1"));
    private static readonly JwtSecurityTokenHandler JwtTokenHandler = new JwtSecurityTokenHandler();
    public static readonly SigningCredentials SigningCreds = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256);

    public override Response Handle(CreateToken request)
    {
        var token = JwtTokenHandler.CreateJwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            signingCredentials: SigningCreds,
            expires: DateTime.UtcNow.AddDays(1),
            subject: new([
                new (ClaimTypes.Name, request.UserId),
                new (ClaimTypes.GivenName, request.FullName ?? "Undefined"),
            ]));
        return new() { Message = JwtTokenHandler.WriteToken(token) };
    }
}
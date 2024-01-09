using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TTSS.Tests;

namespace TTSS.Core.Securities;

public class JsonWebTokenDescriberTests : TestBase
{
    private const string Secret = "Default-Secret-Key-1157EE6FDB4141048CEB6C5CBA8ED6AA";
    private const string Issuer = "Default-Issuer-Name";
    private const string Audience = "Default-Audience-Name";

    [Fact]
    public void GenerateToken_MustBeUseable()
        => GenerateToken().Should().NotBeNullOrWhiteSpace();

    [Fact]
    public void Describe_PlainToken_MustBeDescribable()
    {
        var token = GenerateToken();
        VerifyTokenDescriptor(token, false, DateTime.MinValue, DateTime.MinValue);
    }

    [Fact]
    public void Describe_PlainTokenWithNbf_MustBeDescribableWithoutNbf()
    {
        var nbf = CurrentTime.ToUniversalTime();
        var token = GenerateToken(notBefore: nbf);
        VerifyTokenDescriptor(token, false, DateTime.MinValue, DateTime.MinValue);
    }

    [Fact]
    public void Describe_PlainTokenWithExpires_MustBeDescribable()
    {
        var expires = CurrentTime.ToUniversalTime().Date.AddHours(1);
        var token = GenerateToken(expires: expires);
        var expectedClaims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Exp, DateTimeService.ToUnixTime(expires).ToString()),
        };
        VerifyTokenDescriptor(token, false, DateTime.MinValue, expires, expectedClaims);
    }

    [Fact]
    public void Describe_PlainTokenWithNbfAndExpires_MustBeDescribableWithBothTimes()
    {
        var nbf = CurrentTime.ToUniversalTime().Date;
        var expires = nbf.AddHours(1);
        CurrentTime = nbf.AddSeconds(1);
        var token = GenerateToken(notBefore: nbf, expires: expires);
        var expectedClaims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Nbf, DateTimeService.ToUnixTime(nbf).ToString()),
            new Claim(JwtRegisteredClaimNames.Exp, DateTimeService.ToUnixTime(expires).ToString()),
        };
        VerifyTokenDescriptor(token, true, nbf, expires, expectedClaims);
    }

    [Fact]
    public void Describe_TokenWithSimpleClaims_MustBeDescribable()
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "sakul"),
            new Claim(JwtRegisteredClaimNames.Jti, "123456789"),
        };
        var token = GenerateToken(claims: claims);
        VerifyTokenDescriptor(token, false, DateTime.MinValue, DateTime.MinValue, claims);
    }

    [Fact]
    public void Describe_TokenWithSimpleClaimsAndNbf_MustBeDescribableWithoutNbf()
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "sakul"),
            new Claim(JwtRegisteredClaimNames.Jti, "123456789"),
        };
        var nbf = CurrentTime.ToUniversalTime();
        var token = GenerateToken(notBefore: nbf, claims: claims);
        VerifyTokenDescriptor(token, false, DateTime.MinValue, DateTime.MinValue, claims);
    }

    [Fact]
    public void Describe_TokenWithSimpleClaimsAndNbfAndExpires_MustBeDescribableWithBothTimes()
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "sakul"),
            new Claim(JwtRegisteredClaimNames.Jti, "123456789"),
        };
        var nbf = CurrentTime.ToUniversalTime().Date;
        var expires = nbf.AddHours(1);
        CurrentTime = nbf.AddSeconds(1);
        var token = GenerateToken(notBefore: nbf, expires: expires, claims: claims);
        var expectedClaims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "sakul"),
            new Claim(JwtRegisteredClaimNames.Jti, "123456789"),
            new Claim(JwtRegisteredClaimNames.Nbf, DateTimeService.ToUnixTime(nbf).ToString()),
            new Claim(JwtRegisteredClaimNames.Exp, DateTimeService.ToUnixTime(expires).ToString()),
        };
        VerifyTokenDescriptor(token, true, nbf, expires, expectedClaims);
    }

    [Fact]
    public void Describe_TokenWithCustomClaims_TheCustomClaimsMustBeDescribable()
    {
        var claims = new[]
        {
            new Claim("CustomClaim-1", "TheValue-1"),
            new Claim("CustomClaim-2", "TheValue-2"),
            new Claim("CustomClaim-3", "TheValue-3"),
        };
        var token = GenerateToken(claims: claims);
        VerifyTokenDescriptor(token, false, DateTime.MinValue, DateTime.MinValue, claims);
    }

    private void VerifyTokenDescriptor(string token, bool isTokenValid, DateTime expectedNbf, DateTime expectedExp, params Claim[] defaultClaims)
    {
        var describer = new JsonWebTokenDescriber(DateTimeService);
        var descriptor = describer.Describe(token);
        descriptor.Should().NotBeNull();
        descriptor.AccessToken.Should().Be(token);
        descriptor.Claims.Should().NotBeEmpty();
        descriptor.IsValid.Should().Be(isTokenValid);
        descriptor.ValidFrom.Should().Be(expectedNbf);
        descriptor.ValidTo.Should().Be(expectedExp);
        var expectedClaims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Iss, Issuer),
            new Claim(JwtRegisteredClaimNames.Aud, Audience),
        };
        foreach (var item in defaultClaims)
        {
            expectedClaims.Add(new Claim(item.Type, item.Value));
        }
        descriptor.Claims
            .Select(it => (it.Type, it.Value))
            .Should()
            .BeEquivalentTo(expectedClaims.Select(it => (it.Type, it.Value)));
    }

    private string GenerateToken(
        string secret = Secret,
        string issuer = Issuer,
        string audience = Audience,
        DateTime? notBefore = null,
        DateTime? expires = null,
        params Claim[] claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer, audience, claims, notBefore, expires, credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
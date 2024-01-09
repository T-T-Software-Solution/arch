using System.IdentityModel.Tokens.Jwt;
using TTSS.Core.Services;

namespace TTSS.Core.Securities;

/// <summary>
/// Handles the JWT token.
/// </summary>
public class JsonWebTokenDescriber : ITokenDescriber
{
    #region Fields

    private readonly IDateTimeService? _dateTimeService;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonWebTokenDescriber"/> class.
    /// </summary>
    /// <param name="dateTimeService">Date time service</param>
    /// <exception cref="ArgumentNullException">The date time service is required</exception>
    public JsonWebTokenDescriber(IDateTimeService dateTimeService)
        => _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonWebTokenDescriber"/> class.
    /// </summary>
    public JsonWebTokenDescriber()
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Converts the specified token to a <see cref="ITokenDescriptor"/> object.
    /// </summary>
    /// <param name="token">The token</param>
    /// <returns>The token descriptor</returns>
    public ITokenDescriptor? Describe(string? token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;
        var handler = new JwtSecurityTokenHandler();
        if (false == handler.CanReadToken(token)) return null;

        var jwtToken = handler.ReadJwtToken(token);
        var descriptor = new JsonWebTokenDescriptor
        {
            AccessToken = token,
            Claims = jwtToken.Claims,
            ValidFrom = jwtToken.ValidFrom,
            ValidTo = jwtToken.ValidTo,
            CurrentTime = _dateTimeService == null
                ? null
                : () => _dateTimeService.UtcNow
        };
        return Decorate(descriptor, jwtToken);
    }

    /// <summary>
    /// Post-processes the token descriptor.
    /// </summary>
    /// <param name="tokenDescriptor">The token descriptor</param>
    /// <param name="jwtSecurityToken">The JWT security token</param>
    /// <returns>The token descriptor</returns>
    public virtual ITokenDescriptor Decorate(ITokenDescriptor tokenDescriptor, JwtSecurityToken jwtSecurityToken)
        => tokenDescriptor;

    #endregion
}
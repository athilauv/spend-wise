namespace SpendWise.Application.Features.Authentication;
public sealed class JwtTokenOptions
{
    public const string SectionName = "Jwt";
    public int AccessTokenExpirationMinutes { get; set; } = 15;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}

namespace PropCore.Application.Abstractions.Authentication;

public interface ITokenService
{
    (string AccessToken, DateTime ExpiresAt) GenerateAccessToken(Guid userId, Guid? organizationId, IEnumerable<string> permissions);
    (string RefreshToken, DateTime ExpiresAt) GenerateRefreshToken();
}
namespace Fossa.API.FunctionalTests;

public interface IAccessTokenContext
{
  void ClearAccessToken();

  void SetAccessToken(string accessToken);
}

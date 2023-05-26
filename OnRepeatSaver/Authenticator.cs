using Newtonsoft.Json;
using OnRepeatSaver.Configuration;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using static SpotifyAPI.Web.Scopes;

namespace OnRepeatSaver;

public class Authenticator
{
    public event Func<Task> UserLoggedIn;
    
    public Authenticator(string clientId)
    {
        _clientId = clientId;
    }

    private readonly string _clientId;

    public bool HasUserLoggedIn()
    {
        return File.Exists(ConfigurationPaths.Credentials);
    }

    public async Task Login()
    {
        var server = new EmbedIOAuthServer(new Uri("http://localhost:5000/callback"), 5000);

        var (verifier, challenge) = PKCEUtil.GenerateCodes();

        await server.Start();
        server.AuthorizationCodeReceived += async (_, response) =>
        {
            await server.Stop();
            var token = await new OAuthClient().RequestToken(
                new PKCETokenRequest(_clientId, response.Code, server.BaseUri, verifier)
            );

            await File.WriteAllTextAsync(ConfigurationPaths.Credentials, JsonConvert.SerializeObject(token));
            await UserLoggedIn();
        };

        var request = new LoginRequest(server.BaseUri, _clientId, LoginRequest.ResponseType.Code)
        {
            CodeChallenge = challenge,
            CodeChallengeMethod = "S256",
            Scope = new List<string>
            {
                UserReadEmail, UserReadPrivate, PlaylistReadPrivate, PlaylistReadCollaborative,
                PlaylistModifyPrivate, PlaylistModifyPublic, PlaylistReadPrivate
            }
        };

        var uri = request.ToUri();
        BrowserUtil.Open(uri);
    }

    public async Task<SpotifyClient> GetSpotifyClient()
    {
        var json = await File.ReadAllTextAsync(ConfigurationPaths.Credentials);
        var token = JsonConvert.DeserializeObject<PKCETokenResponse>(json);

        var authenticator = new PKCEAuthenticator(_clientId, token!);
        authenticator.TokenRefreshed += (_, pkceTokenResponse) =>
            File.WriteAllText(ConfigurationPaths.Credentials, JsonConvert.SerializeObject(pkceTokenResponse));

        var config = SpotifyClientConfig.CreateDefault()
            .WithAuthenticator(authenticator);

        return new SpotifyClient(config);
    }
}
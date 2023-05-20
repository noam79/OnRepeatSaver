using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        private static  EmbedIOAuthServer _server;

        public static async Task Main()
        {
            // Make sure "http://localhost:5000/callback" is in your spotify application as redirect uri!t
            _server = new EmbedIOAuthServer(new Uri("http://localhost:5000/callback"), 5000);
            await _server.Start();

            _server.AuthorizationCodeReceived += OnAuthorizationCodeReceived;
            _server.ErrorReceived += OnErrorReceived;

            var request = new LoginRequest(_server.BaseUri, "379169d105884b85a15d862a9d4a2728", LoginRequest.ResponseType.Code)
            {
                Scope = new List<string> { Scopes.PlaylistReadPrivate, Scopes.PlaylistModifyPrivate, Scopes.PlaylistModifyPublic }
            };
            BrowserUtil.Open(request.ToUri());
            await Task.Delay(new TimeSpan(1, 0, 0));
        }

        private static async Task OnAuthorizationCodeReceived(object sender, AuthorizationCodeResponse response)
        {
            await _server.Stop();
            Console.WriteLine("hi");
            var config = SpotifyClientConfig.CreateDefault();
            var tokenResponse = await new OAuthClient(config).RequestToken(
              new AuthorizationCodeRefreshRequest(
                "379169d105884b85a15d862a9d4a2728", "3a530d6d7e404c6badf9de8aeed28938", "AQDbs14wxfCUqAqrtT4z-jSEm6XybNkzN0VvtvWp7ucRgoFN4gnbGeXoG3wQLA9DtpPDrkgwlG0eakaL1jP-ex33xzfthMF_vsZVbqv-C0tXMD3ku7FMRbb0WdRtFGgB3f8")
            );

            var spotify = new SpotifyClient(tokenResponse.AccessToken);
            var writer = new StreamWriter("token.txt");
            writer.AutoFlush = true;
            writer.WriteLine(tokenResponse.RefreshToken);
            writer.Flush();
            Console.WriteLine(tokenResponse.RefreshToken);
            // do calls with Spotify and save token?
        }

        private static async Task OnErrorReceived(object sender, string error, string state)
        {
            Console.WriteLine($"Aborting authorization, error received: {error}");
            await _server.Stop();
        }
    }
}

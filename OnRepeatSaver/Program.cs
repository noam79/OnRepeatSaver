using OnRepeatSaver;

var clientId = await File.ReadAllTextAsync(ConfigurationPaths.ClientId);
var authenticator = new Authenticator(clientId);

if (!authenticator.HasUserLoggedIn())
{
    authenticator.UserLoggedIn += CopySongs;
    await authenticator.Login();
}
else
{
    await CopySongs();
}

async Task CopySongs()
{
    var client = await authenticator.GetSpotifyClient();
    await SongCopier.CopySongsAsync(client, "37i9dQZF1Epkozm1fivAi8", "6Ndi0GvLeraJLN3mhbmzxO");
}

Console.ReadLine();
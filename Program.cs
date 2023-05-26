using OnRepeatSaver;
using OnRepeatSaver.Configuration;

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
    await SongCopier.CopySongsAsync(client, args[0], args[1]);
}

Console.ReadLine();
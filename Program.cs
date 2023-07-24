using MultiLogger;
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
    
    try
    {
        var copiedSongsCount = await SongCopier.CopySongsAsync(client, args[0], args[1]);
        Logger.WriteLine($"Copied {copiedSongsCount} songs");
    }
    catch (IndexOutOfRangeException)
    {
        Logger.WriteLine("No source/target playlist ids given as command line arguments");
    }
}

Console.ReadLine();
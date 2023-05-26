using SpotifyAPI.Web;

namespace OnRepeatSaver;

public class SongCopier
{
    public static async Task CopySongsAsync(SpotifyClient client, string sourcePlaylistId, string targetPlaylistId)
    {
        var sourcePlaylistSongsUris = (await client.PaginateAll(await client.Playlists.GetItems(sourcePlaylistId)))
            .Select(playlistTrack => ((FullTrack)playlistTrack.Track).Uri);

        var targetPlaylistSongsUris = (await client.PaginateAll(await client.Playlists.GetItems(targetPlaylistId)))
            .Select(playlistTrack => ((FullTrack)playlistTrack.Track).Uri)
            .ToList();

        var trackUrisToAdd = sourcePlaylistSongsUris
            .Where(trackUri => !targetPlaylistSongsUris.Contains(trackUri))
            .ToList();

        if (trackUrisToAdd.Count is 0)
        {
            return;
        }

        var addPlaylistItemsRequest = new PlaylistAddItemsRequest(trackUrisToAdd);

        await client.Playlists.AddItems(
            targetPlaylistId,
            addPlaylistItemsRequest);

        Console.WriteLine($"Added {trackUrisToAdd.Count} Songs To Playlist");
    }
}
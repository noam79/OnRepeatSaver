using SpotifyAPI.Web;

namespace OnRepeatSaver;

public static class SongCopier
{
    public static async Task<int> CopySongsAsync(SpotifyClient client, string sourcePlaylistId, string targetPlaylistId)
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
            return 0;
        }

        var addPlaylistItemsRequest = new PlaylistAddItemsRequest(trackUrisToAdd);

        await client.Playlists.AddItems(
            targetPlaylistId,
            addPlaylistItemsRequest);

        return trackUrisToAdd.Count;
    }
}
using SpotifyAPI.Web;

namespace OnRepeatSaver;

public class SongCopier
{
    public static async Task CopySongsAsync(SpotifyClient client, string sourcePlaylistId, string targetPlaylistId)
    {
        // TODO: only add distinct songs to avoid duplications.
        
        var sourcePlaylistSongs = await client.PaginateAll(await client.Playlists.GetItems(sourcePlaylistId));
        var targetPlaylistSongs = await client.PaginateAll(await client.Playlists.GetItems(targetPlaylistId));

        var songsToAdd = targetPlaylistSongs.Except(sourcePlaylistSongs);

        var addPlaylistItemsRequest = new PlaylistAddItemsRequest(
            songsToAdd.Select(playlistTrack => ((FullTrack)playlistTrack.Track).Uri
            ).ToList());

        await client.Playlists.AddItems(
            targetPlaylistId,
            addPlaylistItemsRequest);
    }
}
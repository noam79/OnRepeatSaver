using SpotifyAPI.Web;

namespace OnRepeatSaver;

public static class SongCopier
{
    public static async Task<IEnumerable<FullTrack>> CopySongsAsync(SpotifyClient client, string sourcePlaylistId, string targetPlaylistId)
    {
        var sourcePlaylistTracks = (await client.PaginateAll(await client.Playlists.GetItems(sourcePlaylistId)))
            .Select(x => (FullTrack)x.Track);

        var targetPlaylistTracks = (await client.PaginateAll(await client.Playlists.GetItems(targetPlaylistId)))
            .Select(x => (FullTrack)x.Track)
            .ToList();

        var tracksToAdd = GetTracksToAdd(sourcePlaylistTracks, targetPlaylistTracks).ToList();

        if (tracksToAdd.Count is 0)
        {
            return Enumerable.Empty<FullTrack>();
        }

        var addPlaylistItemsRequest = new PlaylistAddItemsRequest(tracksToAdd.Select(track => track.Uri).ToList());

        await client.Playlists.AddItems(targetPlaylistId, addPlaylistItemsRequest);
        return tracksToAdd;
    }

    private static IEnumerable<FullTrack> GetTracksToAdd(IEnumerable<FullTrack> sourcePlaylistTracks, List<FullTrack> targetPlaylistTracks)
    {
        return sourcePlaylistTracks.Where(sourceTrack =>
            !targetPlaylistTracks.Exists(targetTrack => targetTrack.Uri == sourceTrack.Uri));
    }
}
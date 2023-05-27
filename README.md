# OnRepeatSaver
OnRepeatSaver C# application that saves all songs that you have played on repeat on Spotify to a playlist of your choice. <br/>
Using OnRepeatSaver, you can accumulate the songs that pass through the On Repeat playlist to a dedicated "All TIme Favorites" playlist.

# How To Run
* Acquire the source and target playlist ids (For example, by browsing spotify's web interface)
* Clone the repo
* ```dotnet run . [source playlist id] [target playlist id]```

# Run Periodically
Create a scheduled task/cron job and run OnRepeatSaver each day. Now your "All TIme Favorites" playlist will accumelate songs from your favorite Spotify-generated daily playlist.

using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Control;
using DynamicIsland.Core.Interfaces;
using DynamicIsland.Core.Models;

namespace DynamicIsland.Platform.Windows.Media;

public class MediaSessionService : IMediaSessionService
{
    private GlobalSystemMediaTransportControlsSessionManager? _sessionManager;
    private GlobalSystemMediaTransportControlsSession? _currentSession;
    private MediaSessionSnapshot? _lastSnapshot;
    
    public event EventHandler<MediaSessionSnapshot?>? SessionChanged;

    public async Task InitializeAsync()
    {
        try
        {
            _sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            if (_sessionManager != null)
            {
                _sessionManager.CurrentSessionChanged += OnCurrentSessionChanged;
                UpdateCurrentSession();
            }
        }
        catch
        {
            // Ignore if OS does not support
        }
    }

    private void OnCurrentSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender, CurrentSessionChangedEventArgs args)
    {
        UpdateCurrentSession();
    }

    private void UpdateCurrentSession()
    {
        if (_sessionManager == null) return;
        
        var newSession = _sessionManager.GetCurrentSession();
        
        if (_currentSession != null)
        {
            _currentSession.MediaPropertiesChanged -= OnMediaPropertiesChanged;
            _currentSession.PlaybackInfoChanged -= OnPlaybackInfoChanged;
        }

        _currentSession = newSession;

        if (_currentSession != null)
        {
            _currentSession.MediaPropertiesChanged += OnMediaPropertiesChanged;
            _currentSession.PlaybackInfoChanged += OnPlaybackInfoChanged;
        }

        NotifySessionChangedAsync().FireAndForgetSafeAsync();
    }

    private void OnMediaPropertiesChanged(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args)
    {
        NotifySessionChangedAsync().FireAndForgetSafeAsync();
    }

    private void OnPlaybackInfoChanged(GlobalSystemMediaTransportControlsSession sender, PlaybackInfoChangedEventArgs args)
    {
        NotifySessionChangedAsync().FireAndForgetSafeAsync();
    }

    private async Task NotifySessionChangedAsync()
    {
        var snapshot = await CreateSnapshotAsync(_currentSession);
        SessionChanged?.Invoke(this, snapshot);
    }

    public MediaSessionSnapshot? GetCurrentSession()
    {
        return _lastSnapshot;
    }

    private async Task<MediaSessionSnapshot?> CreateSnapshotAsync(GlobalSystemMediaTransportControlsSession? session)
    {
        if (session == null)
        {
            _lastSnapshot = null;
            return null;
        }

        try
        {
            var properties = await session.TryGetMediaPropertiesAsync();
            var playback = session.GetPlaybackInfo();

            var snapshot = new MediaSessionSnapshot
            {
                SessionId = session.SourceAppUserModelId,
                SourceAppId = session.SourceAppUserModelId,
                Title = string.IsNullOrEmpty(properties?.Title) ? "Unknown" : properties.Title,
                Artist = string.IsNullOrEmpty(properties?.Artist) ? "Unknown" : properties.Artist,
                IsPlaying = playback?.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing,
                CanPlay = playback?.Controls.IsPlayEnabled ?? false,
                CanPause = playback?.Controls.IsPauseEnabled ?? false,
                CanNext = playback?.Controls.IsNextEnabled ?? false,
                CanPrevious = playback?.Controls.IsPreviousEnabled ?? false,
                LastUpdated = DateTimeOffset.Now
            };

            if (properties?.Thumbnail != null)
            {
                using var stream = await properties.Thumbnail.OpenReadAsync();
                using var dr = new global::Windows.Storage.Streams.DataReader(stream.GetInputStreamAt(0));
                var bytes = new byte[stream.Size];
                await dr.LoadAsync((uint)stream.Size);
                dr.ReadBytes(bytes);
                snapshot.ArtworkBytes = bytes;
            }
            _lastSnapshot = snapshot;
            return snapshot;
        }
        catch
        {
            return null;
        }
    }

    public async Task PlayAsync()
    {
        if (_currentSession != null) await _currentSession.TryPlayAsync();
    }

    public async Task PauseAsync()
    {
        if (_currentSession != null) await _currentSession.TryPauseAsync();
    }

    public async Task NextAsync()
    {
        if (_currentSession != null) await _currentSession.TrySkipNextAsync();
    }

    public async Task PreviousAsync()
    {
        if (_currentSession != null) await _currentSession.TrySkipPreviousAsync();
    }
}

public static class TaskExtensions
{
    public static async void FireAndForgetSafeAsync(this Task task)
    {
        try
        {
            await task;
        }
        catch
        {
            // Ignore errors in fire-and-forget
        }
    }
}

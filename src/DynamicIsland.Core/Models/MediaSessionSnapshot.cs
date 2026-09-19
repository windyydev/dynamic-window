using System;

namespace DynamicIsland.Core.Models;

public class MediaSessionSnapshot
{
    public string SessionId { get; set; } = string.Empty;
    public string SourceAppId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public byte[]? ArtworkBytes { get; set; }
    public bool IsPlaying { get; set; }
    public bool CanPlay { get; set; }
    public bool CanPause { get; set; }
    public bool CanNext { get; set; }
    public bool CanPrevious { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
}

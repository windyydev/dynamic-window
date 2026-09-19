using System;
using System.Threading.Tasks;
using DynamicIsland.Core.Models;

namespace DynamicIsland.Core.Interfaces;

public interface IMediaSessionService
{
    Task InitializeAsync();
    MediaSessionSnapshot? GetCurrentSession();
    Task PlayAsync();
    Task PauseAsync();
    Task NextAsync();
    Task PreviousAsync();
    event EventHandler<MediaSessionSnapshot?>? SessionChanged;
}

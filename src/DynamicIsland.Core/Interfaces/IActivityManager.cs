using DynamicIsland.Core.Models;
using System;

namespace DynamicIsland.Core.Interfaces;

public interface IActivityManager
{
    ActivityState CurrentState { get; }
    MediaSessionSnapshot? CurrentMedia { get; }
    event EventHandler? StateChanged;
}

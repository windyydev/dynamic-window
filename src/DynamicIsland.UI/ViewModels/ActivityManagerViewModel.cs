using System;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicIsland.Core.Interfaces;
using DynamicIsland.Core.Models;

namespace DynamicIsland.UI.ViewModels;

public partial class ActivityManagerViewModel : ObservableObject, IActivityManager
{
    private readonly IMediaSessionService _mediaSessionService;
    private readonly Dispatcher _dispatcher;

    [ObservableProperty]
    private ActivityState _currentState = ActivityState.Idle;

    [ObservableProperty]
    private MediaSessionSnapshot? _currentMedia;

    public event EventHandler? StateChanged;

    public ActivityManagerViewModel(IMediaSessionService mediaSessionService)
    {
        _mediaSessionService = mediaSessionService;
        _dispatcher = Dispatcher.CurrentDispatcher;

        _mediaSessionService.SessionChanged += OnMediaSessionChanged;
        _ = _mediaSessionService.InitializeAsync();
    }

    private void OnMediaSessionChanged(object? sender, MediaSessionSnapshot? snapshot)
    {
        _dispatcher.InvokeAsync(() =>
        {
            CurrentMedia = snapshot;
            if (CurrentState != ActivityState.Expanded)
            {
                if (snapshot != null && snapshot.IsPlaying)
                {
                    CurrentState = ActivityState.Peek;
                }
                else if (snapshot == null || !snapshot.IsPlaying)
                {
                    CurrentState = ActivityState.Idle;
                }
            }
        });
    }

    partial void OnCurrentStateChanged(ActivityState value)
    {
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void HoverEnter()
    {
        if (CurrentState == ActivityState.Idle && CurrentMedia != null)
        {
            CurrentState = ActivityState.Peek;
        }
    }

    [RelayCommand]
    private void HoverLeave()
    {
        if (CurrentState == ActivityState.Peek && (CurrentMedia == null || !CurrentMedia.IsPlaying))
        {
            CurrentState = ActivityState.Idle;
        }
    }

    [RelayCommand]
    private void Click()
    {
        if (CurrentState == ActivityState.Peek)
        {
            CurrentState = ActivityState.Expanded;
        }
        else if (CurrentState == ActivityState.Expanded)
        {
            CurrentState = ActivityState.Peek;
            if (CurrentMedia == null || !CurrentMedia.IsPlaying)
            {
                CurrentState = ActivityState.Idle;
            }
        }
        else if (CurrentState == ActivityState.Idle && CurrentMedia != null)
        {
            CurrentState = ActivityState.Expanded;
        }
    }

    [RelayCommand]
    private async void TogglePlayPause()
    {
        if (CurrentMedia != null)
        {
            if (CurrentMedia.IsPlaying)
            {
                await _mediaSessionService.PauseAsync();
            }
            else
            {
                await _mediaSessionService.PlayAsync();
            }
        }
    }

    [RelayCommand]
    private async void Next()
    {
        await _mediaSessionService.NextAsync();
    }

    [RelayCommand]
    private async void Previous()
    {
        await _mediaSessionService.PreviousAsync();
    }
}

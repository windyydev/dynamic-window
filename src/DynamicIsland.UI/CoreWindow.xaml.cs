using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using DynamicIsland.Core.Interfaces;
using DynamicIsland.Core.Models;
using DynamicIsland.UI.ViewModels;

namespace DynamicIsland.UI;

public partial class CoreWindow : Window
{
    private readonly IOverlayWindowService _overlayWindowService;
    private readonly ActivityManagerViewModel _viewModel;

    public CoreWindow(IOverlayWindowService overlayWindowService, ActivityManagerViewModel viewModel)
    {
        InitializeComponent();
        _overlayWindowService = overlayWindowService;
        _viewModel = viewModel;
        DataContext = _viewModel;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        
        var hwnd = new WindowInteropHelper(this).Handle;
        _overlayWindowService.Initialize(hwnd);

        this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
        this.Top = 0;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ActivityManagerViewModel.CurrentState))
        {
            // Phải đợi DataBinding và Style Trigger cập nhật xong Visibility="Visible" rồi mới đo kích thước
            Dispatcher.InvokeAsync(() => AnimateToState(_viewModel.CurrentState), System.Windows.Threading.DispatcherPriority.Loaded);
        }
        else if (e.PropertyName == nameof(ActivityManagerViewModel.CurrentMedia))
        {
            if (_viewModel.CurrentState == ActivityState.Peek)
            {
                Dispatcher.InvokeAsync(() => AnimateToState(_viewModel.CurrentState), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }
    }

    private void AnimateToState(ActivityState state)
    {
        double targetWidth = 16;
        double targetHeight = 16;

        if (state == ActivityState.Peek)
        {
            // Ép buộc Measure để lấy kích thước thật
            var oldVis = MediaPeekContent.Visibility;
            MediaPeekContent.Visibility = Visibility.Visible;
            MediaPeekContent.Measure(new Size(double.PositiveInfinity, 32));
            double desiredWidth = MediaPeekContent.DesiredSize.Width + 16; // Add padding
            MediaPeekContent.Visibility = oldVis;

            targetWidth = Math.Min(Math.Max(16, desiredWidth), 350);
            targetHeight = 32;
        }
        else if (state == ActivityState.Expanded)
        {
            targetWidth = 350;
            targetHeight = 160;
        }

        var sb = new Storyboard();
        var ease = new ExponentialEase { EasingMode = EasingMode.EaseInOut, Exponent = 4 };

        var widthAnim = new DoubleAnimation
        {
            To = targetWidth,
            Duration = TimeSpan.FromMilliseconds(300),
            EasingFunction = ease
        };
        Storyboard.SetTarget(widthAnim, CoreBorder);
        Storyboard.SetTargetProperty(widthAnim, new PropertyPath(FrameworkElement.WidthProperty));

        var heightAnim = new DoubleAnimation
        {
            To = targetHeight,
            Duration = TimeSpan.FromMilliseconds(300),
            EasingFunction = ease
        };
        Storyboard.SetTarget(heightAnim, CoreBorder);
        Storyboard.SetTargetProperty(heightAnim, new PropertyPath(FrameworkElement.HeightProperty));

        sb.Children.Add(widthAnim);
        sb.Children.Add(heightAnim);
        sb.Begin();
    }

    private void CoreBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        _viewModel.HoverEnterCommand.Execute(null);
    }

    private void CoreBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        _viewModel.HoverLeaveCommand.Execute(null);
    }

    private void CoreBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _viewModel.ClickCommand.Execute(null);
    }
}

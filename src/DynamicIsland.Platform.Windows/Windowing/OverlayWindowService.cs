using System;
using DynamicIsland.Core.Interfaces;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Foundation;

namespace DynamicIsland.Platform.Windows.Windowing;

public class OverlayWindowService : IOverlayWindowService
{
    private HWND _hwnd;

    public void Initialize(nint windowHandle)
    {
        _hwnd = new HWND(windowHandle);

        var exStyle = (WINDOW_EX_STYLE)PInvoke.GetWindowLong(_hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        
        exStyle |= WINDOW_EX_STYLE.WS_EX_TOOLWINDOW;
        exStyle |= WINDOW_EX_STYLE.WS_EX_NOACTIVATE;
        
        PInvoke.SetWindowLong(_hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, (int)exStyle);
    }

    public void SetInteractive(bool isInteractive)
    {
        var exStyle = (WINDOW_EX_STYLE)PInvoke.GetWindowLong(_hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        
        if (isInteractive)
        {
            exStyle &= ~WINDOW_EX_STYLE.WS_EX_TRANSPARENT;
        }
        else
        {
            exStyle |= WINDOW_EX_STYLE.WS_EX_TRANSPARENT;
        }

        PInvoke.SetWindowLong(_hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, (int)exStyle);
    }
}

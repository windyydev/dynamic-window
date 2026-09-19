using System;

namespace DynamicIsland.Core.Interfaces;

public interface IOverlayWindowService
{
    void Initialize(nint windowHandle);
    void SetInteractive(bool isInteractive);
}

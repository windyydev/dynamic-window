# Dynamic Island for Windows

A minimalist, context-aware desktop interaction layer for Windows, inspired by the Dynamic Island concept. Built entirely with **C# 13, .NET 10, and WPF**.

## 🚀 Features (Currently Implemented)

- **Native Windows Overlay:** A transparent, topmost window that anchors to the top center of your screen without stealing focus (`WS_EX_TOOLWINDOW`, `WS_EX_NOACTIVATE`).
- **Media Integration (GSMTC):** Connects directly to Windows native media controls to intercept playback from Spotify, YouTube (Chrome/Edge), VLC, etc.
- **Dynamic Width & Animations:** The island automatically morphs from an idle dot to a pill shape, smoothly adapting its width based on the song title and artist.
- **Interactive UI (Hover & Click):**
  - **Hover:** Reveals the currently playing track.
  - **Click:** Expands the island into a full media player showing album artwork and playback controls.

## 🛠 Tech Stack

- **Framework:** .NET 10
- **UI:** Windows Presentation Foundation (WPF)
- **Architecture:** MVVM using `CommunityToolkit.Mvvm`
- **Dependency Injection:** `Microsoft.Extensions.Hosting`
- **Native Interop:** `Microsoft.Windows.CsWin32`

## 🏗 Project Structure

- `DynamicIsland.Core`: Core models, interfaces, and state (e.g., `MediaSessionSnapshot`).
- `DynamicIsland.Infrastructure`: Common utilities, logging, storage.
- `DynamicIsland.Platform.Windows`: Windows-specific API implementations (GSMTC, Windowing).
- `DynamicIsland.UI`: The WPF Views, Animations, and ViewModels.
- `DynamicIsland.App`: The entry point and dependency injection setup.

## 🏃 How to Run

1. Make sure you have the [.NET 10 SDK](https://dotnet.microsoft.com/) installed.
2. Clone this repository.
3. Run the application from the source folder:
   ```bash
   dotnet run --project src/DynamicIsland.App/DynamicIsland.App.csproj
   ```
4. Play some music on YouTube or Spotify, and watch the magic happen!

## 📝 Roadmap

- Adaptive Taskbar (Bloom Launcher)
- Notification & Download Integrations
- System Tray Support & Multi-monitor Positioning
- Artwork Caching & Performance Optimization

using System;
using AllSaintsFrights.UserInterface.Windows;
using Dalamud.Interface.Windowing;

namespace AllSaintsFrights.UserInterface
{
    internal sealed class WindowManager : IDisposable
    {
        private readonly JumpscareOverlayWindow jumpscareOverlayWindow = new();
        private readonly ConfigurationWindow configurationWindow = new();
        private bool disposedValue;

        /// <summary>
        ///     The windowing system.
        /// </summary>
        private readonly WindowSystem windowingSystem;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowManager" /> class.
        /// </summary>
        public WindowManager()
        {
            this.windowingSystem = new WindowSystem(Plugin.PluginInterface.Manifest.InternalName);
            this.windowingSystem.AddWindow(this.jumpscareOverlayWindow);
            this.windowingSystem.AddWindow(this.configurationWindow);
            Plugin.PluginInterface.UiBuilder.OpenConfigUi += this.configurationWindow.Toggle;
            Plugin.PluginInterface.UiBuilder.Draw += this.windowingSystem.Draw;
        }

        /// <summary>
        ///     Disposes of the window manager.
        /// </summary>
        public void Dispose()
        {
            if (this.disposedValue)
            {
                ObjectDisposedException.ThrowIf(this.disposedValue, nameof(this.windowingSystem));
                return;
            }
            Plugin.PluginInterface.UiBuilder.OpenConfigUi -= this.configurationWindow.Toggle;
            Plugin.PluginInterface.UiBuilder.Draw -= this.windowingSystem.Draw;
            this.windowingSystem.RemoveAllWindows();
            this.jumpscareOverlayWindow.Dispose();
            this.disposedValue = true;
        }
    }
}

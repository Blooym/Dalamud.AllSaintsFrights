using AllSaintsFrights.Configuration;
using AllSaintsFrights.UserInterface;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace AllSaintsFrights
{
    internal sealed class Plugin : IDalamudPlugin
    {
        // Safety: valid from plugin start.
#pragma warning disable CS8618
        [PluginService] public static IDalamudPluginInterface PluginInterface { get; private set; }
        [PluginService] public static IClientState ClientState { get; private set; }
        [PluginService] public static ITextureProvider TextureProvider { get; private set; }
        [PluginService] public static IPluginLog Log { get; private set; }
        public static PluginConfiguration Configuration { get; private set; }
#pragma warning restore CS8618

        private readonly WindowManager windowManager;

        public Plugin()
        {
            Configuration = PluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();
            this.windowManager = new();
        }

        public void Dispose() => this.windowManager.Dispose();
    }
}

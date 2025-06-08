using System;
using System.Collections.Generic;
using AllSaintsFrights.Jumpscares;
using Dalamud.Configuration;

namespace AllSaintsFrights.Configuration
{
    internal sealed class PluginConfiguration : IPluginConfiguration
    {
        public int Version { get; set; }
        public TimeSpan JumpscareMinimumInterval = TimeSpan.FromHours(1);
        public TimeSpan JumpscareMaximumInterval = TimeSpan.FromHours(12);
        public HashSet<JumpscarePack> EnabledJumpscarePacks = [.. Enum.GetValues<JumpscarePack>()];

        public void Save() => Plugin.PluginInterface.SavePluginConfig(this);
    }
}

using System;
using System.Numerics;
using AllSaintsFrights.Jumpscares;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace AllSaintsFrights.UserInterface.Windows
{
    internal sealed class ConfigurationWindow : Window
    {
        public static event Action? IntervalChanged;
        public static event Action? PlayTestJumpscare;

        public ConfigurationWindow() : base("All Saints' Frights Configuration")
        {
            this.Size = ImGuiHelpers.ScaledVector2(450, 280);
            this.SizeCondition = ImGuiCond.FirstUseEver;
            this.Flags = ImGuiWindowFlags.NoResize;
            this.SizeConstraints = new()
            {
                MinimumSize = ImGuiHelpers.ScaledVector2(450, 280),
                MaximumSize = ImGuiHelpers.ScaledVector2(450, 280)
            };
        }

        public override void Draw()
        {
            // Jumpscare Interval Settings
            var minInterval = (int)Plugin.Configuration.JumpscareMinimumInterval.TotalMinutes;
            var maxInterval = (int)Plugin.Configuration.JumpscareMaximumInterval.TotalMinutes;
            ImGui.TextDisabled("Jumpscare Intervals");
            ImGui.Separator();
            ImGui.TextWrapped("Control the minimum and maximum intervals between jumpscares. A value will be randomly chosen between these two values each time a jumpscare plays.");
            ImGui.Spacing();
            ImGui.Text("Minimum Interval (minutes):");
            if (ImGui.InputInt("##MinimumInterval", ref minInterval, 1, 5, ImGuiInputTextFlags.EnterReturnsTrue))
            {
                if (minInterval > 0 && minInterval <= maxInterval)
                {
                    Plugin.Configuration.JumpscareMinimumInterval = TimeSpan.FromMinutes(minInterval);
                    Plugin.Configuration.Save();
                    IntervalChanged?.Invoke();
                }
            }
            ImGui.Text("Maximum Interval (minutes):");
            if (ImGui.InputInt("##MaximumInterval", ref maxInterval, 1, 5, ImGuiInputTextFlags.EnterReturnsTrue))
            {
                if (maxInterval >= minInterval && maxInterval > 0)
                {
                    Plugin.Configuration.JumpscareMaximumInterval = TimeSpan.FromMinutes(maxInterval);
                    Plugin.Configuration.Save();
                    IntervalChanged?.Invoke();
                }
            }
            ImGui.Dummy(new Vector2(0, 10));

            // Jumpscare Options
            ImGui.TextDisabled("Jumpscare Options");
            ImGui.Separator();
            using (var combo = ImRaii.Combo("Enabled Jumpscare Packs", $"{Plugin.Configuration.EnabledJumpscarePacks.Count} Jumpscare Packs"))
            {
                if (combo)
                {
                    foreach (var jumpscareCategory in Enum.GetValues<JumpscarePack>())
                    {
                        var isEnabled = Plugin.Configuration.EnabledJumpscarePacks.Contains(jumpscareCategory);
                        using var disabled = ImRaii.Disabled(Plugin.Configuration.EnabledJumpscarePacks.Count == 1 && isEnabled);
                        if (ImGui.Checkbox(jumpscareCategory.ToString(), ref isEnabled))
                        {
                            if (isEnabled)
                            {
                                Plugin.Configuration.EnabledJumpscarePacks.Add(jumpscareCategory);
                            }
                            else
                            {
                                Plugin.Configuration.EnabledJumpscarePacks.Remove(jumpscareCategory);
                            }
                            Plugin.Configuration.Save();
                        }
                    }
                }
            }
            ImGui.Dummy(new Vector2(0, 10));

            // Testing Options
            ImGui.TextDisabled("Testing Options");
            ImGui.Separator();
            if (ImGui.Button("Play Test Jumpscare"))
            {
                PlayTestJumpscare?.Invoke();
            }
        }
    }
}

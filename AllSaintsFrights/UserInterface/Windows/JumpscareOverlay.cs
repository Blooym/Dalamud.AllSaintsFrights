using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using AllSaintsFrights.Jumpscares;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace AllSaintsFrights.UserInterface.Windows
{
    internal sealed class JumpscareOverlayWindow : Window, IDisposable
    {
        private bool playJumpscare;
        private Jumpscare currentJumpscare;
        private bool testJumpscarePending;
        private readonly Timer jumpscareTimer;
        private readonly JumpscareHandler jumpscareHandler;

        public JumpscareOverlayWindow() : base("Jumpscare Overlay")
        {
            // Set the window to cover the entire screen
            // and to act as an overlay.
            this.Size = ImGui.GetIO().DisplaySize;
            this.Position = new System.Numerics.Vector2(0, 0);
            this.SizeCondition = ImGuiCond.Always;
            this.Flags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
            this.DisableFadeInFadeOut = true;
            this.DisableWindowSounds = true;
            this.IsOpen = true;

            // Initialize the jumpscare handler.
            this.jumpscareHandler = new JumpscareHandler(assetsBasePath: Path.Combine(Plugin.PluginInterface.AssemblyLocation.DirectoryName!, "jumpscares"));
            this.currentJumpscare = this.jumpscareHandler.GetRandomJumpscare();

            // Schedule the first jumpscare.
            this.jumpscareTimer = new Timer();
            this.jumpscareTimer.Elapsed += this.OnJumpscareTimerElapsed;
            this.jumpscareTimer.AutoReset = false;
            this.ScheduleJumpscare();

            // Subscribe to the interval changed event from the configuration window.
            ConfigurationWindow.IntervalChanged += this.OnIntervalChanged;
            ConfigurationWindow.PlayTestJumpscare += this.OnPlayTestJumpscare;
        }

        public void Dispose()
        {
            this.jumpscareHandler.Dispose();
            this.jumpscareTimer.Elapsed -= this.OnJumpscareTimerElapsed;
            ConfigurationWindow.IntervalChanged -= this.OnIntervalChanged;
            ConfigurationWindow.PlayTestJumpscare -= this.OnPlayTestJumpscare;
            this.jumpscareTimer.Stop();
            this.jumpscareTimer.Dispose();
        }

        public void ScheduleJumpscare()
        {
            Plugin.Log.Debug("Scheduling next jumpscare.");
            this.jumpscareTimer.Stop();
            this.jumpscareTimer.Interval = Random.Shared.Next(
                (int)Plugin.Configuration.JumpscareMinimumInterval.TotalMilliseconds,
                (int)Plugin.Configuration.JumpscareMaximumInterval.TotalMilliseconds);
            this.jumpscareTimer.Start();
            Plugin.Log.Debug($"Scheduled next jumpscare to play at {DateTime.Now.AddMilliseconds(this.jumpscareTimer.Interval)}");
        }

        public void OnIntervalChanged()
        {
            Plugin.Log.Debug("Jumpscare interval changed, rescheduling jumpscare.");
            this.ScheduleJumpscare();
        }

        public void OnPlayTestJumpscare()
        {
            if (this.testJumpscarePending)
            {
                return;
            }
            this.testJumpscarePending = true;
            Plugin.Log.Debug("Playing test jumpscare.");
            Task.Run(() =>
            {
                // Add a random delay because it's funny if the user
                // gets caught off guard sometimes.
                Task.Delay(Random.Shared.Next(0, 5000)).Wait();
                this.currentJumpscare = this.jumpscareHandler.GetRandomJumpscare();
                this.playJumpscare = true;
                this.testJumpscarePending = false;
            });
        }

        public void OnJumpscareTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            Plugin.Log.Debug("Jumpscare timer elapsed, playing jumpscare.");
            this.currentJumpscare = this.jumpscareHandler.GetRandomJumpscare();
            this.playJumpscare = true;
            this.ScheduleJumpscare();
        }

        public override void Draw()
        {
            this.currentJumpscare.Gif.Draw(ImGui.GetContentRegionAvail());
            if (this.playJumpscare)
            {
                this.playJumpscare = false;
                this.currentJumpscare.Gif.Play(false);
                this.currentJumpscare.Sound.Play();
            }
        }
    }
}

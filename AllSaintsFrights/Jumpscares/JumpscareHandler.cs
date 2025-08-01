using System;
using System.Collections.Generic;
using System.IO;

namespace AllSaintsFrights.Jumpscares
{
    internal sealed class JumpscareHandler(string assetsBasePath) : IDisposable
    {
        private bool disposedValue;

        private readonly List<Jumpscare> jumpscares =
        [
            new Jumpscare(gifPath: Path.Combine(assetsBasePath, "fnaf2", "witheredfoxy", "jumpscare.gif"), soundPath: Path.Combine(assetsBasePath, "fnaf2", "jumpscare_sfx.wav"), pack: JumpscarePack.FNAF),
        ];

        public Jumpscare GetRandomJumpscare()
        {
            ObjectDisposedException.ThrowIf(this.disposedValue, nameof(JumpscareHandler));
            if (this.jumpscares.Count == 0)
            {
                throw new InvalidOperationException("No jumpscares loaded");
            }
            var enabledJumpscares = this.jumpscares.FindAll(j => Plugin.Configuration.EnabledJumpscarePacks.Contains(j.Pack));
            if (enabledJumpscares.Count == 0)
            {
                throw new InvalidOperationException("No enabled jumpscares available");
            }
            return enabledJumpscares[Random.Shared.Next(enabledJumpscares.Count)];
        }

        public void Dispose()
        {
            ObjectDisposedException.ThrowIf(this.disposedValue, nameof(JumpscareHandler));
            this.disposedValue = true;
            foreach (var jumpscare in this.jumpscares)
            {
                jumpscare.Dispose();
            }
        }
    }
}

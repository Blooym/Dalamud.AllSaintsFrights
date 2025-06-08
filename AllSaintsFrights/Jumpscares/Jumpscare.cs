using System;
using System.Media;
using AllSaintsFrights.UserInterface.Components;

namespace AllSaintsFrights.Jumpscares
{
    internal enum JumpscarePack
    {
        FNAF2,
    }

    internal sealed class Jumpscare : IDisposable
    {
        public ImGuiGif Gif { get; init; }
        public SoundPlayer Sound { get; init; }
        public JumpscarePack Pack { get; init; }

        public Jumpscare(string gifPath, string soundPath, JumpscarePack pack)
        {
            this.Gif = ImGuiGif.CreateAsync(gifPath).GetAwaiter().GetResult();
            this.Sound = new SoundPlayer(soundPath);
            this.Pack = pack;
            this.Sound.LoadAsync();
        }

        public void Dispose()
        {
            this.Gif.Dispose();
            this.Sound.Dispose();
        }
    }
}

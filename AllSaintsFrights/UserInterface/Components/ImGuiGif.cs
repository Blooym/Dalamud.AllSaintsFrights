using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

// Thanks https://github.com/Infiziert90/ChatTwo/blob/3951c49e1aa9941afbefe855298b663e68bff3e4/ChatTwo/EmoteCache.cs#L217

namespace AllSaintsFrights.UserInterface.Components
{
    internal sealed class ImGuiGif : IDisposable
    {
        private readonly List<(IDalamudTextureWrap Texture, float Delay)> frames = [];
        private float frameTimer;
        private int currentFrame;
        private ulong globalFrameCount;
        private bool shouldPlay;
        private bool isLooping;

        /// <summary>
        ///     Creates an ImGuiGif instance from a GIF file.
        /// </summary>
        /// <param name="filePath">The path to the GIF file.</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">If the GIF file does not exist or is empty.</exception>
        /// <exception cref="InvalidDataException">If the GIF file does not contain any frames.</exception>
        public static async Task<ImGuiGif> CreateAsync(string filePath)
        {
            var gif = new ImGuiGif();

            var imageBytes = File.ReadAllBytes(filePath);
            if (imageBytes.Length == 0)
                throw new FileNotFoundException("GIF file not found or is empty.", filePath);

            using var memoryStream = new MemoryStream(imageBytes);
            using var image = Image.Load<Rgba32>(memoryStream);
            if (image.Frames.Count == 0)
                throw new InvalidDataException("GIF file does not contain any frames.");

            foreach (var frame in image.Frames)
            {
                var delay = frame.Metadata.GetGifMetadata().FrameDelay / 100f;

                if (delay < 0.02f)
                    delay = 0.1f;

                var buffer = new byte[4 * frame.Width * frame.Height];
                frame.CopyPixelDataTo(buffer);
                var tex = await Plugin.TextureProvider.CreateFromRawAsync(RawImageSpecification.Rgba32(frame.Width, frame.Height), buffer);
                gif.frames.Add((tex, delay));
            }

            return gif;
        }

        public void Draw(Vector2 size)
        {
            if (!this.shouldPlay)
                return;

            if (this.frames.Count == 0)
                return;

            if (this.currentFrame >= this.frames.Count)
            {
                if (!this.isLooping)
                {
                    this.shouldPlay = false;
                    return;
                }

                this.currentFrame = 0;
                this.frameTimer = -1f;
            }

            var (texture, delay) = this.frames[this.currentFrame];
            if (this.frameTimer <= 0.0f)
                this.frameTimer = delay;

            ImGui.Image(texture.Handle, size);

            if (this.globalFrameCount == Plugin.PluginInterface.UiBuilder.FrameCount)
                return;

            this.globalFrameCount = Plugin.PluginInterface.UiBuilder.FrameCount;

            this.frameTimer -= ImGui.GetIO().DeltaTime;
            if (this.frameTimer <= 0f)
                this.currentFrame++;
        }

        public void Play(bool loop)
        {
            this.shouldPlay = true;
            this.isLooping = loop;
            this.currentFrame = 0;
            this.frameTimer = -1f;
            this.globalFrameCount = 0;
        }

        public void Stop()
        {
            this.shouldPlay = false;
            this.isLooping = false;
            this.currentFrame = 0;
            this.frameTimer = -1f;
            this.globalFrameCount = 0;
        }

        public void Dispose()
        {
            this.frames.ForEach(frame => frame.Texture.Dispose());
            this.frames.Clear();
            this.shouldPlay = false;
        }
    }
}

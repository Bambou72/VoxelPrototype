/**
 * Main file for client
 * Copyright Florian Pfeiffer
 * Author Florian Pfeiffer
 **/
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using StbImageSharp;
using System.Runtime.InteropServices;
using VoxelPrototype.client;
using VoxelPrototype.client.Utils;
namespace VoxelPrototypeClient
{
    public static class Program
    {
        private static void Main()
        {
#if !DEBUG
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
            var handle = ConsoleUtils.GetConsoleWindow();
                // Hide
                ConsoleUtils.ShowWindow(handle, ConsoleUtils.SW_HIDE);
            }
#endif
            ImageResult image;
            using (Stream stream = File.OpenRead("icon.png"))
            {
                image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            }
            Image[] iconImages = { new Image(image.Width, image.Height, image.Data) };

            Config conf = new Config();
            var Client = new Client(
                new GameWindowSettings()
                {
                    UpdateFrequency = 60,
                }, new NativeWindowSettings()
                {
                    Flags = ContextFlags.ForwardCompatible,
                    API = ContextAPI.OpenGL,
                    DepthBits = 32,
                    Title = "Voxel Prototype",
                    ClientSize = new Vector2i((int)(long)conf.GetProperty("width"), (int)(long)conf.GetProperty("height")),
                    WindowState = conf.GetProperty("mode") == "fullscreen" ? WindowState.Fullscreen : WindowState.Normal,
                    Icon = new(iconImages),
                    Vsync = VSyncMode.Off
                });
            Client.Run();
        }
    }
}
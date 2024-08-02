/**
 * Main file for client
 * Copyright Florian Pfeiffer
 * Author Florian Pfeiffer
 **/
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using System.CommandLine;
using System.Runtime.InteropServices;
using VoxelPrototype.client;
using VoxelPrototype.client.utils.StbImageSharp;
namespace client
{
    public static class ConsoleUtils
    {
        [DllImport("kernel32.dll")]
        public static extern nint GetConsoleWindow();
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(nint hWnd, int nCmdShow);
        public const int SW_HIDE = 0;
    }
    public static class Program
    {
        static string[] ResourcePacksPaths;
        private static void Main(string[] args)
        {
#if !DEBUG
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var handle = ConsoleUtils.GetConsoleWindow();
                // Hide
                ConsoleUtils.ShowWindow(handle, ConsoleUtils.SW_HIDE);
            }
#endif
            var resourcepackOption = new Option<string[]?>(
            name: "--resourcepacks-paths",
            description: "Add paths to the game to search resourcepacks"); 
            var rootCommand = new RootCommand();
            rootCommand.AddOption(resourcepackOption);
            rootCommand.SetHandler((packs) =>
            {
                ResourcePacksPaths = packs;
            },resourcepackOption);
            rootCommand.Invoke(args);
            ImageResult image;
            using (Stream stream = File.OpenRead("icon.png"))
            {
                image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            }
            Image[] iconImages = { new Image(image.Width, image.Height, image.Data) };
            //Config conf = new Config();
            NativeWindowSettings Settings = new NativeWindowSettings()
            {
                ClientSize = new(900,600) ,//new((int)(long)conf.GetProperty("width"), (int)(long)conf.GetProperty("height")),
                Title = "Voxel Prototype",
                //WindowState = conf.GetProperty("mode") == "fullscreen" ? WindowState.Fullscreen : WindowState.Normal,
                Icon = new(iconImages),
                Vsync = VSyncMode.On,
                APIVersion = new(4, 6),
            };
            using (var Client = new Client(ResourcePacksPaths, GameWindowSettings.Default, Settings))
            {
                Client.Run();
            }
            NLog.LogManager.Shutdown();
        }
    }
}
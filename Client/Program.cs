/**
 * Main file for client
 * Copyright Florian Pfeiffer
 * Author Florian Pfeiffer
 **/
using CommandLine;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using System.Runtime.InteropServices;
using VoxelPrototype.client;
using VoxelPrototype.client.utils.StbImageSharp;
namespace client
{
    public class Options
    {
        [Option("resourcepacks-paths", Required = false, Default = null, HelpText = "Add folder to the to look-up for resourcepacks.")]
        public IEnumerable<string> RessourcesPaths { get; set; }
    }
    public static class Program
    {
        [DllImport("kernel32.dll")]
        public static extern nint GetConsoleWindow();
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(nint hWnd, int nCmdShow);
        private static void Main(string[] args)
        {
#if DEBUG
            Options options = new Options();
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => options = opts);
#endif
#if !DEBUG
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var handle = GetConsoleWindow();
                // Hide
                ShowWindow(handle, 0);
            }
#endif
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
            var Client = new Client(
#if DEBUG 
                options.RessourcesPaths.ToArray()
#else
            null
#endif
, GameWindowSettings.Default, Settings);
            Client.Run();
            Client.Dispose();
            NLog.LogManager.Shutdown();
        }
    }
}
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
    public static class ConsoleUtils
    {
        [DllImport("kernel32.dll")]
        public static extern nint GetConsoleWindow();
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(nint hWnd, int nCmdShow);
        public const int SW_HIDE = 0;
    }
    public class Options
    {
        [Option("resources-paths", Required = false, Default = null, HelpText = "Add folder to the to look-up for resourcespacks.")]
        public IEnumerable<string> RessourcesPaths { get; set; }
    }
    public static class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            Options options = new Options();
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => options = opts)
                .WithNotParsed((errs) => HandleParseError(errs));
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
            using (var Client = new Client(options.RessourcesPaths.ToArray(), GameWindowSettings.Default, Settings))
            {
                Client.Run();
            }
            NLog.LogManager.Shutdown();
        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            // Handle errors here
            Logger.Error(errs);
        }
    }
}
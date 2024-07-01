/**
 * Main file for client
 * Copyright Florian Pfeiffer
 * Author Florian Pfeiffer
 **/
using CommandLine;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using StbImageSharp;
using System.Collections;
using System.Runtime.InteropServices;
using VoxelPrototype.client;
using VoxelPrototype.client.Utils;
namespace DesktopClient
{

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
            Config conf = new Config();
            NativeWindowSettings Settings = new NativeWindowSettings()
            {
                ClientSize = new((int)(long)conf.GetProperty("width"), (int)(long)conf.GetProperty("height")),
                Title = "Voxel Prototype",
                WindowState =  conf.GetProperty("mode") == "fullscreen" ? OpenTK.Windowing.Common.WindowState.Fullscreen : OpenTK.Windowing.Common.WindowState.Normal,
                Icon = new( iconImages),
            };
            var Client = new Client( options.RessourcesPaths.ToArray(),GameWindowSettings.Default,Settings);
            Client.Run();
            NLog.LogManager.Shutdown();
        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            // Handle errors here
            Logger.Error(errs);
        }
    }
}
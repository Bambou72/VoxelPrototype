/**
 * Main file for client
 * Copyright Florian Pfeiffer
 * Author Florian Pfeiffer
 **/
using CommandLine;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;
using System.Collections;
using System.Runtime.InteropServices;
using VoxelPrototype.client;
using VoxelPrototype.client.Utils;
namespace Client
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
            ThreadPool.SetMaxThreads(0, 0);
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
            Image[] iconImages;
            unsafe
            {
                fixed (byte* bytePointer = image.Data)
                iconImages = new Image[]{ new Image(image.Width, image.Height,bytePointer ) };

            }
            Config conf = new Config();
            var ClientInter = new ClientInterface(
                new ClientWindow((int)(long)conf.GetProperty("width"),
                (int)(long)conf.GetProperty("height"),
                "Voxel Prototype",
                Fullscreen : conf.GetProperty("mode") == "fullscreen"
                ));
            ClientInter.window.SetIcon( iconImages);

            var Client = new VoxelPrototype.client.Client(ClientInter, options.RessourcesPaths.ToArray());
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
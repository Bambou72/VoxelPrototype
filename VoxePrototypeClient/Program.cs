/**
 * Main file for client
 * Copyright Florian Pfeiffer
 * Author Florian Pfeiffer
 **/
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Runtime.InteropServices;
using VoxelPrototype.client;
namespace VoxelPrototypeClient
{
    public static class Program
    {
        private static void Main()
        {
#if !DEBUG
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
            var handle = Utils.GetConsoleWindow();
            // Hide
            Utils.ShowWindow(handle, Utils.SW_HIDE);
            }
#endif
            var Client = new Client();
            Client.Run();
        }
    }
}
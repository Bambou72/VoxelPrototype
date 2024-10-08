﻿using VoxelPrototype.client.Resources.temp;
using VoxelPrototype.game;

namespace Server
{
    internal class StandaloneServer
    {
        internal WorldSettings InitialSettings;
        internal ResourcePackManager RessourcePackManager;
        internal static string Title = @"
         __      __           _ _____           _        _                       _____                          
         \ \    / /          | |  __ \         | |      | |                     / ____|                         
          \ \  / /____  _____| | |__) | __ ___ | |_ ___ | |_ _   _ _ __   ___  | (___   ___ _ ____   _____ _ __ 
           \ \/ / _ \ \/ / _ \ |  ___/ '__/ _ \| __/ _ \| __| | | | '_ \ / _ \  \___ \ / _ \ '__\ \ / / _ \ '__|
            \  / (_) >  <  __/ | |   | | | (_) | || (_) | |_| |_| | |_) |  __/  ____) |  __/ |   \ V /  __/ |   
             \/ \___/_/\_\___|_|_|   |_|  \___/ \__\___/ \__|\__, | .__/ \___| |_____/ \___|_|    \_/ \___|_|   
                                                              __/ | |                                           
                                                             |___/|_|                                           
        ";
        public StandaloneServer() : base()
        {
            //Ressources
            RessourcePackManager = new();
            //InitialSettings = Settings;
            //this.Path = Path;
        }
        public void Run()
        {
            Console.WriteLine(Title);

            //World = new(Settings, Path);
            //ServerNetwork.StartServer(ServerPort);
            //Logger.Info("The server has finished initializing, it is now ready at: " + ServerNetwork.server.LocalPort);
            //Logger.Info("Server engine version: " + Version.EngineVersion);
            //Logger.Info("Server api version: " + Version.APIVersion);
            /*while (Running)
            {
                if (TickCounter < 20)
                {
                    Stopwatch.Start();
                    ServerNetwork.Update();
                    World.Tick();
                    TickCounter++;
                    Stopwatch.Stop();
                }
                else
                {
                    TickCounter = 0;
                }
                double elapsed = Stopwatch.ElapsedMilliseconds;
                if (elapsed < 50)
                {

                    Thread.Sleep(50 - (int)elapsed);
                }
                else
                {
                    Logger.Warn("Tick can't run at 20tps, current time for one tick :" + elapsed);
                }
                Stopwatch.Reset();
            }*/
            //World.Dispose();
            // ServerNetwork.StopServer();


        }
    }
}

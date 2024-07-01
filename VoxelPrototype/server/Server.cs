using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using VoxelPrototype.client;
using VoxelPrototype.common.Network.server;
using VoxelPrototype.common.Utils;
using VoxelPrototype.common.World;

namespace VoxelPrototype.server
{
    public class Server : IRunnable
    {
        //Singleton
        public static Server TheServer;
        //Param
        internal static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        internal string ServerName;
        internal int ServerPort = 23482;
        //Data
        internal volatile bool Running = false;
        internal World.World World;
        //TIMING
        private readonly Stopwatch _watchUpdate = new Stopwatch();
        private int _slowUpdates = 0;
        protected bool IsRunningSlowly;
        public double UpdateFrequency = 20;
        public int Counter;
        public float DeltaSum;
        public int TPS;

        public Server(string path,WorldSettings Settings = null)
        {
            if (TheServer == null)
            {
                TheServer = this;
            }else
            {
                throw new Exception("Two servers can't run in the same process");
            }
            World = new(null,path,Settings);
        }
        public World.World GetWorld()
        {
            return World;
        }
        public void Stop()
        {
            Running = false;

        }
        public bool IsRunning()
        {
            return Running;
        }

        public int GetTPS()
        {
            return TPS;
        }
        public  void Run()
        {
          
            ServerNetwork.StartServer(ServerPort);
            Logger.Info("The server has finished initializing, it is now ready at: " + ServerNetwork.server.LocalPort);
            Logger.Info("Server engine version: " + EngineVersion.Version);
            _watchUpdate.Start();
            while (Running )
            {
                ServerNetwork.Update();
                if (DeltaSum >= 1)
                {
                    DeltaSum = 0;
                    TPS =Counter;
                    Counter = 0;
                }

                double updatePeriod = 1 / UpdateFrequency;
                double elapsed = _watchUpdate.Elapsed.TotalSeconds;
                if (elapsed > updatePeriod)
                {
                    _watchUpdate.Restart();
                    Counter++;
                    DeltaSum += (float)elapsed;

                    World.Tick((float)elapsed);
                    const int MaxSlowUpdates = 80;
                    const int SlowUpdatesThreshold = 45;

                    double time = _watchUpdate.Elapsed.TotalSeconds;
                    if (updatePeriod < time)
                    {
                        _slowUpdates++;
                        if (_slowUpdates > MaxSlowUpdates)
                        {
                            _slowUpdates = MaxSlowUpdates;
                        }
                    }
                    else
                    {
                        _slowUpdates--;
                        if (_slowUpdates < 0)
                        {
                            _slowUpdates = 0;
                        }
                    }
                    IsRunningSlowly = _slowUpdates > SlowUpdatesThreshold;
                }
                // The time we have left to the next update.
                double timeToNextUpdate = updatePeriod - _watchUpdate.Elapsed.TotalSeconds;

                if (timeToNextUpdate > 0)
                {
                    AccurateSleep(timeToNextUpdate, 1);
                }
            }
            World.Dispose();
            ServerNetwork.StopServer();
            TheServer = null;
            Logger.Info("Server closed");
        }
        public static void AccurateSleep(double seconds, int expectedSchedulerPeriod)
        {
            // FIXME: Make this a parameter?
            const double TOLERANCE = 0.02;

            long t0 = Stopwatch.GetTimestamp();
            long target = t0 + (long)(seconds * Stopwatch.Frequency);

            double ms = (seconds * 1000) - (expectedSchedulerPeriod * TOLERANCE);
            int ticks = (int)(ms / expectedSchedulerPeriod);
            if (ticks > 0)
            {
                Thread.Sleep(ticks * expectedSchedulerPeriod);
            }

            while (Stopwatch.GetTimestamp() < target)
            {
                Thread.Yield();
            }
        }
    }
}

using System.Diagnostics;
using VoxelPrototype.game;
using VoxelPrototype.network;

namespace VoxelPrototype.server
{
    public class Server 
    {
        //Singleton
        public static Server TheServer;
        //Param
        public static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Server");
        public string ServerName;
        public int ServerPort = 23482;
        //Data
        public volatile bool Running = false;
        public game.world.World World;
        public ServerNetworkManager NetworkManager;
        //TIMING
        public readonly Stopwatch _watchUpdate = new Stopwatch();
        public int _slowUpdates = 0;
        public bool IsRunningSlowly;
        public double UpdateFrequency = 20;
        public int Counter;
        public float DeltaSum;
        public int TPS;

        public Server(string path,WorldSettings Settings = null)
        {
            NetworkManager = new();
            if (TheServer == null)
            {
                TheServer = this;
            }else
            {
                throw new Exception("Two servers can't run in the same process");
            }
            World = new(path,Settings);
        }
        public game.world.World GetWorld()
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
            try
            {
                NetworkManager.StartServer(ServerPort);
                Logger.Info("The server has finished initializing, it is now ready at: " + NetworkManager.NetManager.LocalPort);
                Logger.Info("Server engine version: " + EngineVersion.Version);
                _watchUpdate.Start();
                while (Running)
                {
                    NetworkManager.Update();
                    if (DeltaSum >= 1)
                    {
                        DeltaSum = 0;
                        TPS = Counter;
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
                NetworkManager.Stop();
                TheServer = null;
                Logger.Info("Server closed");
            }catch(Exception ex)
            {
                Logger.Fatal(ex);
            }
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

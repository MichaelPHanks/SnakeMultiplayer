using System;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class ServerMain
    {
        public static int Main(string[] args)
        {
            if (readArgs(args, out ushort port))
            {
                if (MessageQueueServer.instance.initialize(port))
                {
                    Console.WriteLine("Server waiting for incomming connections on port {0}", port);
                    startServer();
                    MessageQueueServer.instance.shutdown();
                }
                else
                {
                    Console.WriteLine("Failure to initialize the MessageQueueServer");
                }
            }
            else
            {
                Console.WriteLine("Invalid port parameter.");
                Console.WriteLine("Example program usage: server --port 3000");
            }

            return 0;
        }


        /// <summary>
        /// Verify the command line port parameter is correct and if so, return the specified port
        /// </summary>
        private static bool readArgs(string[] args, out ushort port)
        {
            Predicate<string> PortParam = (arg => arg == "-p" || arg == "--port" || arg == "-port");

            port = 0;
            bool valid = true;
            if (args.Length != 2)
            {
                valid = false;
            }
            else if (!PortParam(args[0].ToLower()))
            {
                valid = false;
            }
            else
            {
                if (!ushort.TryParse(args[1], out port))
                {
                    valid = false;
                }
            }

            return valid;
        }

        private static void startServer()
        {
            TimeSpan SIMULATION_UPDATE_RATE_MS = TimeSpan.FromMilliseconds(33);

            GameModel model = new GameModel();
            bool running = model.initialize();
            TimeSpan totalTime = new TimeSpan(0);
            DateTime previousTime = DateTime.Now;
            while (running)
            {
                // work out the elapsed time
                DateTime currentTime = DateTime.Now;
                TimeSpan elapsedTime = currentTime - previousTime;
                previousTime = currentTime;

                // If we are running faster than the simulation update rate, then go to sleep
                // for a bit so we don't burn up the CPU unnecessarily.
                TimeSpan sleepTime = SIMULATION_UPDATE_RATE_MS - elapsedTime;
                if (sleepTime > TimeSpan.Zero)
                {
                    //Console.WriteLine("Sleep: {0}", sleepTime.TotalMilliseconds);
                    System.Threading.Thread.Sleep(sleepTime);
                }
                previousTime += sleepTime;

                Console.WriteLine(elapsedTime);
                Console.WriteLine(sleepTime);

                // Now, after having slept for a bit, now compute the elapsed time and perform
                // the game model update.
                elapsedTime += (sleepTime > TimeSpan.Zero ? sleepTime : TimeSpan.Zero);
                
               Console.WriteLine(elapsedTime);
                
                //totalTime += elapsedTime;
                elapsedTime = TimeSpan.FromMilliseconds(33);

                model.update(elapsedTime);
            }

            /*DateTime previousTime = DateTime.Now;
            while (running)
            {
                // Work out the elapsed time since the last frame
                DateTime currentTime = DateTime.Now;
                TimeSpan frameTime = currentTime - previousTime;
                previousTime = currentTime;

                // Perform the game model update
                model.update(frameTime);

                // Calculate the remaining time to sleep
                TimeSpan sleepTime = SIMULATION_UPDATE_RATE_MS - frameTime;

                // If we still have time to sleep, then sleep
                if (sleepTime > TimeSpan.Zero)
                {
                    System.Threading.Thread.Sleep(sleepTime);
                }
                *//*else
                {
                    // If the frame took longer than the desired update rate,
                    // consider adjusting the simulation update rate or optimizing the game logic.
                    Console.WriteLine("Frame took longer than desired update rate.");
                }*//*
            }*/

            model.shutdown();
        }
    }
}
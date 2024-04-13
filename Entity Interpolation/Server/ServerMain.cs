﻿using System;
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
            TimeSpan SIMULATION_UPDATE_RATE_MS = TimeSpan.FromMilliseconds(16);

            GameModel model = new GameModel();
            bool running = model.initialize();

            DateTime previousTime = DateTime.Now;
            while (running)
            {
                // Busy wait until we hit the simulation update rate
                // The busy wait isn't a great approach, but it is what we
                // are going to do right now.
                TimeSpan elapsedTime = DateTime.Now - previousTime;
                while (elapsedTime < SIMULATION_UPDATE_RATE_MS)
                {
                    elapsedTime = DateTime.Now - previousTime;
                }
                previousTime = DateTime.Now;

                model.update(elapsedTime);
            }

            model.shutdown();
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

        
    }
}
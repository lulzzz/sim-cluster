﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;

namespace SimMach
{
    class Program {

        const string LoadBalancer = "lb.us-west";
        


        static void Main(string[] args) {



            var t = new Topology {
                {"lb.eu-west:nginx", RunNginx},
                {"lb.eu-west:telegraf", RunTelegraf}
            };


            var machines = t.GroupBy(p => p.Key.Split(':')[0]);
            // printing
            foreach (var m in machines) {
                Console.WriteLine($"{m.Key}");

                foreach (var svc in m) {
                    Console.WriteLine($"  {svc.Key.Split(':')[1]}");
                }
            }
            
            var runtime = new Runtime(t);
            
            runtime.Start();


            Console.ReadLine();

            Console.WriteLine("Shutting down...");
            runtime.ShutDown(1000).Wait();
            Console.WriteLine("Done. Starting");
            runtime.Start();
            Console.WriteLine("Booted");
            

            Console.ReadLine();

            // at the time of the configuration, simulation doesn't exist, yet!
        }

        static async Task RunTelegraf(Sim env) {
            env.Debug("Starting");
            try {
                while (!env.Token.IsCancellationRequested) {
                    env.Debug("Running");
                    await Task.Delay(1000);
                }
            } catch (TaskCanceledException) { }

            env.Debug("Shutting down");
        }

        static async Task RunNginx(Sim env) {
            env.Debug("Starting");
            try {
                while (!env.Token.IsCancellationRequested) {
                    env.Debug("Running");
                    await Task.Delay(5000, env.Token);
                }
            } catch (TaskCanceledException) { }

            env.Debug("Shutting down");
        }
    }

    class Topology : Dictionary<string,Func<Sim,Task>> { }


    
    
}

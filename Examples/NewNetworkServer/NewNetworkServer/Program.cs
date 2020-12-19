using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using BrainBlo.NewNetwork;
using Google.Protobuf.Collections;
using mt = BBMessage.Types.MessageType;
using PlayerInfo = BBMessage.Types.PlayerInfo;
using vec3 = BBMessage.Types.Vector3;

namespace NewNetworkServer
{
    class Program
    {
        static MapField<IPEndPoint, Player> players = new MapField<IPEndPoint, Player>();
        static readonly object lockObj = new object();
        static void Main(string[] args)
        {
            ServerNetHandle snh = new ServerNetHandle(25000);
            LogManager lm = new LogManager();
            snh.SetLog(lm);
            snh.Use(MessageCallback);
            Task.Run(async () =>
            {
                while (snh.IsRunning)
                {
                    AddSeconds(snh);
                    await Task.Delay(1000);
                }
                Console.WriteLine("Stopped add seconds");
            });
            Console.WriteLine("Server was used");
            while (true)
            {
                string cmd = Console.ReadLine().ToLower();
                if(cmd == "players")
                {
                    lock (lockObj)
                    {
                        int counter = 0;
                        Console.WriteLine("Players online: " + players.Count);
                        foreach(var player in players)
                        {
                            var info = player.Value.playerInfo;
                            ++counter;
                            Console.WriteLine("{0}. {1} | {2} | X: {3} | Y: {4}", counter, info.Name, player.Key.ToString(), info.Position.X, info.Position.Y);
                        }
                    }
                }else if(cmd == "exit")
                {
                    break;
                }else if(cmd == "seconds")
                {
                    lock (lockObj)
                    {
                        foreach(var player in players)
                        {
                            Console.WriteLine("{0} time: {1}", player.Value.playerInfo.Name, player.Value.playerTimeout.GetSeconds());
                        }
                    }
                }
                else
                {
                    Console.WriteLine("That command doesn't exist");
                }
            }
        }
        private static void AddSeconds(ServerNetHandle snh)
        {
            if (players.Count == 0) return;

            foreach (var player in players.ToArray())
            {
                Console.WriteLine("Seconds {0} Count: {1}", player.Value.playerInfo.Name, players.Count());
                var info = player.Value.playerInfo;
                var timeout = player.Value.playerTimeout;
                timeout.AddSeconds(1);
                if (timeout.GetSeconds() == 10)
                {
                    Console.WriteLine(info.Name + " timed out");
                }
                else if (timeout.GetSeconds() == 20)
                {
                    Console.WriteLine(info.Name + " kicked");
                    players.Remove(player.Key);
                }
            }
            
        }



        private static void MessageCallback(NetHandle caller, Message message)
        {
            if (message == null) return;
            BBMessage bbmessage = BBMessage.Parser.ParseFrom(message.messageBuffer, 0, message.messageSize);

            switch (bbmessage.MessageType)
            {
                case mt.Connect:
                    lock (lockObj)
                    {
                        foreach (var player in players)
                        {
                            if(player.Value.playerInfo.Name == bbmessage.PlayerInfo.Name) goto default;
                        }
                        players.Add(message.point, new Player { playerInfo = bbmessage.PlayerInfo, playerTimeout = new PlayerTimeout() });
                    }
                    caller.Send(message);
                    Console.WriteLine("Player {0} was connected", bbmessage.PlayerInfo.Name);
                    break;
                case mt.Text:
                    Console.WriteLine(bbmessage.MessageText);
                    break;
                case mt.PlayerData:
                    lock (lockObj)
                    {
                        foreach (var player in players)
                        {
                            var info = player.Value.playerInfo;
                            if (Equals(player.Key, message.point))
                            {
                                info.Position = bbmessage.PlayerInfo.Position;
                            }
                        }
                    }
                    break;
                default:

                    break;
            }
            lock (lockObj)
            {
                foreach (var player in players)
                {
                    if(Equals(player.Key, message.point))
                    {
                        player.Value.playerTimeout.ClearSeconds();
                    }
                }
            }
        }
        public struct Player
        {
            public PlayerTimeout playerTimeout;
            public PlayerInfo playerInfo;
        }
        public class PlayerTimeout
        {
            private int seconds;
            public PlayerTimeout()
            {
                seconds = 0;
            }
            public void AddSeconds(int value)
            {
                seconds += value;
            }
            public void ClearSeconds()
            {
                seconds = 0;
            }
            public int GetSeconds()
            {
                return seconds;
            }

        }
        public class LogManager : ILog
        {
            public void LogCallback(LogData logData)
            {
            }
        }
    }
}

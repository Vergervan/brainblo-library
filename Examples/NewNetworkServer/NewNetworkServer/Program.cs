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
                            Console.WriteLine("{0}. {1} | X: {2} | Y: {3} ", counter, info.Name, info.Position.X, info.Position.Y);
                        }
                    }
                }else if(cmd == "exit")
                {
                    break;
                }
            }
        }
        private static void AddSeconds(ServerNetHandle snh)
        {
            lock (lockObj)
            {
                foreach(var player in players)
                {
                    var info = player.Value.playerInfo;
                    var timeout = player.Value.playerTimeout;
                    timeout.Seconds += 1;
                    if(timeout.Seconds == 10)
                    {
                        Console.WriteLine(info.Name + " timed out");
                    }else if(timeout.Seconds == 20)
                    {
                        Console.WriteLine(info.Name + " kicked");
                        players.Remove(player.Key);
                    }
                }
            }
        }



        private static async void MessageCallback(NetHandle caller, Message message)
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
                            var info = player.Value.playerInfo;
                            if (info.Name == bbmessage.PlayerInfo.Name) goto default;
                        }
                        players.Add(message.point, new Player { playerInfo = bbmessage.PlayerInfo, playerTimeout = new PlayerTimeout() });
                    }
                    await Task.Delay(3000);
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
                            if (player.Key == message.point)
                            {
                                info.Position = bbmessage.PlayerInfo.Position;
                            }
                        }
                    }
                    break;
                default:

                    break;
            }
            var curPlayer = players.Where((s) => s.Key == message.point)?.FirstOrDefault().Value;
            if (curPlayer != null) curPlayer.Value.playerTimeout.Seconds = 0;
        }
        public struct Player
        {
            public PlayerTimeout playerTimeout;
            public PlayerInfo playerInfo;
        }
        public class PlayerTimeout
        {
            private int seconds;
            public int Seconds
            {
                get { return seconds; }
                set
                {
                    seconds = value;
                }
            }
            public PlayerTimeout()
            {
                Seconds = 0;
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

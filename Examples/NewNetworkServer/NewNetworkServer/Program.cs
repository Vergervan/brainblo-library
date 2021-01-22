using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using BrainBlo.NewNetwork;
using BrainBlo.Projects;
using Google.Protobuf.Collections;
using Google.Protobuf;
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

            Timer timer = new Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

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

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (players.Count == 0) return;
            foreach(var player in players.ToArray())
            {
                var info = player.Value.playerInfo;
                var timeout = player.Value.playerTimeout;
                //Console.WriteLine("{0} | Seconds: {1} | Online: {2}", player.Value.playerInfo.Name, timeout.GetSeconds(), players.Count());
                timeout.AddSeconds(1);
                if (timeout.GetSeconds() == 10)
                {
                    Console.WriteLine(info.Name + " timed out");
                }
                else if (timeout.GetSeconds() == 15)
                {
                    Console.WriteLine(info.Name + " kicked");
                    players.Remove(player.Key);
                }
            }
        }
        private static void MessageCallback(NetHandle caller, DatagramPacket datagram)
        {
            if (datagram == null) return;
            BBPacket packet = new BBPacket(datagram);
            BBMessage bbmessage = BBMessage.Parser.ParseFrom(packet.Bytes, 0, packet.Bytes.Length);

            switch (bbmessage.MessageType)
            {
                case mt.Connect:
                    lock (lockObj)
                    {
                        foreach (var player in players)
                        {
                            if(player.Value.playerInfo.Name == bbmessage.PlayerInfo.Name) goto default;
                        }
                        players.Add(datagram.point, new Player { playerInfo = bbmessage.PlayerInfo, playerTimeout = new PlayerTimeout() });
                    }
                    caller.Send(datagram);
                    Console.WriteLine("Player {0} was connected", bbmessage.PlayerInfo.Name);
                    SendPlayersInfo(caller);
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
                            if (Equals(player.Key, datagram.point))
                            {
                                info.MergeFrom(bbmessage.PlayerInfo);
                            }
                        }
                    }
                    SendPlayersInfo(caller);
                    break;
                default:
                   
                    break;
            }
            lock (lockObj)
            {
                foreach (var player in players)
                {
                    if(Equals(player.Key, datagram.point))
                    {
                        player.Value.playerTimeout.ClearSeconds();
                    }
                }
            }
        }

        private static void SendPlayersInfo(NetHandle caller)
        {
            if (players.Count < 2) return; 
            List<PlayerInfo> playerInfos = new List<PlayerInfo>();
            foreach (var player in players.ToArray())
            {
                playerInfos.Add(player.Value.playerInfo);
            }
            BBMessage message = new BBMessage();
            message.MessageType = mt.PlayerData;
            message.PlayerInfos.AddRange(playerInfos);
            DatagramPacket packet = BBPacket.BuildOnRun(0, 0, message.ToByteArray());
            foreach(var player in players.ToArray())
            {
                packet.Send(caller, player.Key);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
        static MapField<IPEndPoint, PlayerInfo> players = new MapField<IPEndPoint, PlayerInfo>();
        static readonly object lockObj = new object();
        static void Main(string[] args)
        {
            ServerNetHandle snh = new ServerNetHandle(25000);
            LogManager lm = new LogManager();
            snh.SetLog(lm);
            snh.Use(MessageCallback);
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
                            var info = player.Value;
                            ++counter;
                            Console.WriteLine("{0}: {1} - {2}:{3}", counter, info.Name, info.Position.X, info.Position.Y);
                        }
                    }
                }else if(cmd == "exit")
                {
                    break;
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
                            if (player.Value.Name == bbmessage.PlayerInfo.Name) goto default;
                        }
                        players.Add(message.point, bbmessage.PlayerInfo);
                    }
                    await Task.Delay(1000);
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
                            if (player.Value.Name == bbmessage.PlayerInfo.Name)
                            {
                                player.Value.Position = bbmessage.PlayerInfo.Position;
                            }
                        }
                    }
                    break;
                default:

                    break;
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

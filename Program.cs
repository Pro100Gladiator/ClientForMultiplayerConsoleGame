using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        //static Player player = new Player(1, 1, '@', ConsoleColor.Yellow, 0);
        static Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        static Random random = new Random();
        static MemoryStream ms = new MemoryStream(new byte[256], 0, 256, true, true);
        static BinaryWriter writer = new BinaryWriter(ms);
        static BinaryReader reader = new BinaryReader(ms);
        static List<Player> players = new List<Player>();

        static Player player;
        static void Main(string[] args)
        {
            Console.Title = "Client";
            Console.CursorVisible = false;

            Console.WriteLine("Connection to server...");
            socket.Connect("127.0.0.1", 2048);
            Console.WriteLine("Connected to server.");
            Thread.Sleep(1000);
            Console.Clear();

            Console.WriteLine("Type your sprite for the game:");
            char spr = Convert.ToChar(Console.ReadLine());
            Console.Clear();

            Console.WriteLine("Choose your color for this game (from 0 up to 14):");
            for (int i = 0; i <= 14; i++)
            {
                Console.ForegroundColor = (ConsoleColor)i;
                Console.WriteLine(i);
            }
            Console.ResetColor();
            ConsoleColor clr = (ConsoleColor)int.Parse(Console.ReadLine());
            Console.Clear();

            int x = random.Next(1, 5);
            int y = random.Next(1, 5);

            Console.WriteLine("Get your ID");
            SendPacket(PacketInfo.ID);
            int id = ReceivePacket();
            Console.WriteLine($"Your ID is: {id}");
            Thread.Sleep(1000);
            Console.Clear();

            player = new Player(x, y, spr, clr, id);

            Task.Run(() => { while (true) ReceivePacket(); }); 
                
            while (true)
            {
                player.Draw();
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.LeftArrow: player.Remove(); player.X--; goto case 252;
                    case ConsoleKey.RightArrow: player.Remove(); player.X++; goto case 252;
                    case ConsoleKey.DownArrow: player.Remove(); player.Y++; goto case 252;
                    case ConsoleKey.UpArrow: player.Remove();   player.Y--; goto case 252;
                    case (ConsoleKey)252:
                        player.Draw();
                        SendPacket(PacketInfo.Position);
                        break;

                }
            }
            
        }
        enum PacketInfo
        {
            ID, Position
        }
        static void SendPacket(PacketInfo info)
        {
            ms.Position = 0;
            switch (info)
            {
                case PacketInfo.ID:
                    writer.Write(0);
                    socket.Send(ms.GetBuffer());
                    break;
                case PacketInfo.Position:
                    writer.Write(1);
                    writer.Write(player.ID);
                    writer.Write(player.X);
                    writer.Write(player.Y);
                    writer.Write(player.Sprite);
                    writer.Write((int)player.Color);
                    socket.Send(ms.GetBuffer());
                    break;
                default:
                    break;
            }
        }
        static int ReceivePacket()
        {
            ms.Position = 0;
            socket.Receive(ms.GetBuffer());
            int code = reader.ReadInt32();

            int id, x, y;
            char sprite;
            ConsoleColor color;

            switch (code)
            {
                case 0: return reader.ReadInt32();
                case 1: 
                    id = reader.ReadInt32(); 
                    x = reader.ReadInt32(); 
                    y = reader.ReadInt32();

                    Player plr = players.Find(p => p.ID == id);
                    if (plr != null)
                    {
                        plr.Remove();
                        plr.X = x;
                        plr.Y = y;
                        plr.Draw();
                    }
                    else
                    {
                        sprite = reader.ReadChar();
                        color = (ConsoleColor)reader.ReadInt32();
                        plr = new Player(x, y, sprite, color, id);
                        players.Add(plr);
                        plr.Draw();
                    }
                    break;
            }

            return -1;
        }

        class Player
        {
            public int X { get; set; }
            public int Y { get; set; }
            public char Sprite { get; private set; }
            public ConsoleColor Color { get; private set; }
            public int ID { get; private set; }

            public Player(int x, int y, char sprite, ConsoleColor color, int id)
            {
                X = x;
                Y = y;
                Color = color;
                Sprite = sprite;
                ID = id;
            }

            public void Draw()
            {
                Console.ForegroundColor = Color;
                Console.SetCursorPosition(X, Y);
                Console.Write(Sprite);
            }
            public void Remove()
            {
                Console.SetCursorPosition(X, Y);
                Console.Write(" ");
            }
        }
    }
}

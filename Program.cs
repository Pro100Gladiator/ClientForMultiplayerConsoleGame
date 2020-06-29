using System;
using System.Net.Sockets;

namespace Client
{
    class Program
    {
        //static Socket socket = new Socket()

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

        static Player player = new Player(1, 1, '@', ConsoleColor.Yellow, 0);
        static void Main(string[] args)
        {
            Console.Title = "Client";
            Console.CursorVisible = false;
                
            while (true)
            {
                player.Draw();
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.LeftArrow: player.Remove(); player.X--; break;
                    case ConsoleKey.RightArrow: player.Remove(); player.X++; break;
                    case ConsoleKey.DownArrow: player.Remove(); player.Y++; break;
                    case ConsoleKey.UpArrow: player.Remove();   player.Y--; break;

                }
            }
            
        }
    }
}

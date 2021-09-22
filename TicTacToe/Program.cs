using System;
using System.Collections.Generic;

namespace TicTacToe
{
    class Program
    {
        static void Main()
        {
            bool isRunning = true;
            List<string> menuItems = new List<string>()
            {
                "Main menu:",
                "1 - Start game (for 1 player)",
                "2 - Start game (for 2 players)",
                "0 - Exit",
                "Your choice: "
            };

            do
            {
                ConsoleIO.ShowMenu(menuItems);

                if (!int.TryParse(Console.ReadLine(), out int choice) || choice > 2 || choice < 0)
                {
                    ConsoleIO.OutputError("Incorrect choice selection!\n\n");
                    continue;
                }

                switch (choice)
                {
                    case 0:
                        isRunning = false;
                        break;

                    case 1:
                        new Game().StartGame(1);
                        break;

                    case 2:
                        new Game().StartGame(2);
                        break;
                }

            } while (isRunning);
        }
    }
}

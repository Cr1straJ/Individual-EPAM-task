using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe
{
    static class ConsoleIO
    {
        public static void OutputError(string text) => WriteColoredText(text, ConsoleColor.Red);

        public static void OutputSuccess(string text) => WriteColoredText(text, ConsoleColor.Green);

        public static void ShowMenu(List<string> menuItems) => Console.Write(string.Join('\n', menuItems));

        public static void WriteMatrix(int[,] matrix, string playerCharacter)
        {
            int size = matrix.GetLength(0);

            Console.Write("\n    {0}", string.Concat(Enumerable.Range(1, size).Select(i => string.Format(" {0} ", i))));
            Console.Write("\n    {0}\n", string.Concat(Enumerable.Range(1, size).Select(i => " | ")));

            for (int i = 0; i < size; i++)
            {
                Console.Write($"{i + 1} - ");

                for (int j = 0; j < size; j++)
                {
                    if (matrix[i, j] == 0)
                    {
                        Console.Write("[");
                        OutputSuccess("-");
                        Console.Write("]");
                    }
                    else
                    {
                        string _oponentCharacter = playerCharacter.Equals("x", StringComparison.CurrentCulture) ? "o" : "x";
                        Console.Write(string.Format("[{0}]", matrix[i, j] == 1 ? playerCharacter : _oponentCharacter));
                    }
                }

                Console.WriteLine();
            }
        }
        
        private static void WriteColoredText(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}

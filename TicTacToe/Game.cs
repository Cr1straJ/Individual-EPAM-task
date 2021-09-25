using System;
using System.Collections.Generic;

namespace TicTacToe
{
    public class Game
    {
        private int[][] gameBoard = null;
        private int size = 3;
        private int currentPlayer = 1;
        private string playerCharacter = "x";
        private List<(int, int)> availableMoves = null;

        public bool ExistMoves
        {
            get
            {
                return this.availableMoves.Count > 0;
            }
        }

        public void InitializeGame(int size, int firstPlayer, string playerCharacter)
        {
            this.size = size;
            this.gameBoard = new int[size][];
            for (int i = 0; i < size; i++)
            {
                this.gameBoard[i] = new int[size];
            }

            this.currentPlayer = firstPlayer;
            this.playerCharacter = playerCharacter;
            this.availableMoves = new List<(int, int)>();

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    this.availableMoves.Add((i, j));
                }
            }
        }

        public void StartGame(int countPlayers)
        {
            int player = GetPlayerMovesFirst();
            string character = GetPlayerCharacter();
            this.InitializeGame(GetGameBoardSize(), player, character);

            this.GetBotMove(countPlayers);

            while (this.ExistMoves)
            {
                ConsoleIO.WriteMatrix(this.gameBoard, this.playerCharacter);

                if (countPlayers == 2)
                {
                    ConsoleIO.OutputSuccess($"\n{(this.currentPlayer == 1 ? "First" : "Second")} player:");
                }

                var move = GetMove(this.gameBoard);
                this.MakeMove(move.Item1, move.Item2);

                if (!this.ContainsCombinations())
                {
                    ConsoleIO.OutputError("Win combinations not exist!");
                    break;
                }

                this.GetBotMove(countPlayers);

                if (!this.ContainsCombinations())
                {
                    ConsoleIO.OutputError("There are no winning combinations!");
                    break;
                }
            }

            Console.WriteLine("\nGame is finished!");
            ConsoleIO.WriteMatrix(this.gameBoard, this.playerCharacter);
            var gameResult = this.GetScore();
            Console.WriteLine($"\nScore:\nPlayer X: {gameResult.Item1}\nPlayer O: {gameResult.Item2}\n\n");
        }

        public void MakeMove(int moveX, int moveY)
        {
            this.availableMoves.RemoveAll(position => position.Item1 == moveX && position.Item2 == moveY);
            this.gameBoard[moveX][moveY] = this.currentPlayer;
            this.currentPlayer = (this.currentPlayer == 1) ? 2 : 1;
        }

        public void GetBotMove(int countPlayers)
        {
            if (this.currentPlayer == 2 && countPlayers == 1 && this.ExistMoves)
            {
                Random random = new Random();
                int index = random.Next(0, this.availableMoves.Count - 1);
                this.MakeMove(this.availableMoves[index].Item1, this.availableMoves[index].Item2);
            }
        }

        public (int, int) GetScore()
        {
            int firstPlayer = 1;
            int secondPlayer = 2;

            if (this.playerCharacter.Equals("o", StringComparison.CurrentCulture))
            {
                (firstPlayer, secondPlayer) = (2, 1);
            }

            int scoreX = this.FindСombinationsInLines(firstPlayer) + this.FindСombinationsInDiagonals(firstPlayer);
            int scoreY = this.FindСombinationsInLines(secondPlayer) + this.FindСombinationsInDiagonals(secondPlayer);

            return (scoreX, scoreY);
        }

        private static (int, int) GetMove(int[][] gameBoard)
        {
            int size = gameBoard.GetLength(0), moveX, moveY;

            do
            {
                Console.WriteLine("\nChoosing an entry position.");
                moveX = GetPositionParameter("Line: ", size);
                moveY = GetPositionParameter("Column: ", size);

                if (gameBoard[--moveX][--moveY] == 0)
                {
                    break;
                }

                ConsoleIO.OutputError("The selected position is already taken!\n");
            }
            while (true);

            return (moveX, moveY);
        }

        private static int GetPlayerMovesFirst()
        {
            int choice = GetChoiceFromMenu("\nChoosing the player who makes the move first.", new List<string>() { "1 - The first player to move is", "2 - Select the player who makes the first move randomly" });

            return choice == 2 ? new Random().Next(1, 3) : 1;
        }

        private static int GetChoiceFromMenu(string message, List<string> menuItems)
        {
            int choice;

            do
            {
                Console.WriteLine(message);
                ConsoleIO.ShowMenu(menuItems);
                Console.Write("\nYour choice: ");

                if (int.TryParse(Console.ReadLine(), out choice) && choice > 0 && choice <= menuItems.Count)
                {
                    break;
                }

                ConsoleIO.OutputError("Incorrect choice selection!\n");
            }
            while (true);

            return choice;
        }

        private static int GetGameBoardSize()
        {
            int size;

            while (true)
            {
                Console.Write("\nEnter the size of game board: ");

                if (!int.TryParse(Console.ReadLine(), out size) || size < 3)
                {
                    ConsoleIO.OutputError("Incorrect size of the game board!\n");
                    continue;
                }

                if ((size & 1) == 1)
                {
                    break;
                }
                else
                {
                    ConsoleIO.OutputError("The size of the game board must be an odd number!\n");
                }
            }

            return size;
        }

        private static string GetPlayerCharacter()
        {
            int choice = GetChoiceFromMenu("\nSelecting a player character.", new List<string>() { "1 - X", "2 - O" });

            return choice == 1 ? "x" : "o";
        }

        private static int GetPositionParameter(string message, int length)
        {
            do
            {
                Console.Write(message);

                if (!int.TryParse(Console.ReadLine(), out int parameter))
                {
                    ConsoleIO.OutputError("Incorrect value!\n");
                    continue;
                }

                if (parameter > 0 && parameter <= length)
                {
                    return parameter;
                }

                ConsoleIO.OutputError("The parameter goes beyond the boundaries of the matrix!\n");
            }
            while (true);
        }

        private bool ContainsCombinations()
        {
            int currentPlayerScore;
            int newPlayerScore = this.FindСombinationsInLines(this.currentPlayer, true) + this.FindСombinationsInDiagonals(this.currentPlayer, true);

            if (this.playerCharacter.Equals("o", StringComparison.CurrentCulture))
            {
                currentPlayerScore = this.GetScore().Item1;
            }
            else
            {
                currentPlayerScore = this.GetScore().Item2;
            }

            return currentPlayerScore < newPlayerScore;
        }

        private int FindСombinationsInLines(int player, bool includeEmptyCells = false)
        {
            int count = 0;

            for (int i = 0; i < this.size; i++)
            {
                int countInRow = 0, countInColumn = 0;
                for (int j = 0; j < this.size; j++)
                {
                    if (this.gameBoard[i][j] == player || (includeEmptyCells && this.gameBoard[i][j] == 0))
                    {
                        countInRow++;
                    }
                    else
                    {
                        count += countInRow / 3;
                        countInRow = 0;
                    }

                    if (this.gameBoard[j][i] == player || (includeEmptyCells && this.gameBoard[j][i] == 0))
                    {
                        countInColumn++;
                    }
                    else
                    {
                        count += countInColumn / 3;
                        countInColumn = 0;
                    }
                }

                count += countInRow / 3;
                count += countInColumn / 3;
            }

            return count;
        }

        private int FindСombinationsInDiagonals(int player, bool includeEmptyCells = false)
        {
            int count = 0;

            for (int i = this.size; i >= -this.size; i--)
            {
                int firstDiagonal = 0; // Diagonal that starts at the upper left corner and ends at the lower right corner. Example: \
                int secondDiagonal = 0; // Diagonal that starts at the upper right corner and ends at the lower left corner. Example: /

                for (int j = 0; j < this.size - i; j++)
                {
                    int index = i + j;

                    if (index < this.size && j < this.size && index >= 0 && j >= 0)
                    {
                        if (this.gameBoard[j][index] == player || (includeEmptyCells && this.gameBoard[j][index] == 0))
                        {
                            firstDiagonal++;
                        }
                        else
                        {
                            count += firstDiagonal / 3;
                            firstDiagonal = 0;
                        }

                        if (this.gameBoard[j][this.size - 1 - index] == player || (includeEmptyCells && this.gameBoard[j][this.size - 1 - index] == 0))
                        {
                            secondDiagonal++;
                        }
                        else
                        {
                            count += secondDiagonal / 3;
                            secondDiagonal = 0;
                        }
                    }
                }

                count += firstDiagonal / 3;
                count += secondDiagonal / 3;
            }

            return count;
        }
    }
}
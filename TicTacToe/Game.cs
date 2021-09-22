using System;
using System.Collections.Generic;

namespace TicTacToe
{
    public class Game
    {
        private int[,] _gameBoard = null;
        private int _size = 3;
        private int _currentPlayer = 1;
        private string _playerCharacter = "x";
        private List<(int, int)> _availableMoves = null;

        public bool ExistMoves
        {
            get
            {
                return _availableMoves.Count > 0;
            }
        }

        public void InitializeGame(int size, int firstPlayer, string playerCharacter)
        {
            _size = size;
            _gameBoard = new int[size, size];
            _currentPlayer = firstPlayer;
            _playerCharacter = playerCharacter;
            _availableMoves = new List<(int, int)>();

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    _availableMoves.Add((i, j));
                }
            }
        }

        private static (int, int) GetMove(int[,] gameBoard)
        {
            int size = gameBoard.GetLength(0), moveX, moveY;

            do
            {
                Console.WriteLine("\nChoosing an entry position.");
                moveX = GetPositionParameter("Line: ", size);
                moveY = GetPositionParameter("Column: ", size);

                if (gameBoard[--moveX, --moveY] == 0)
                {
                    break;
                }

                ConsoleIO.OutputError("The selected position is already taken!\n");

            } while (true);

            return (moveX, moveY);
        }

        private static int GetPositionParameter(string message, int length)
        {
            do
            {
                Console.Write(message);

                if (int.TryParse(Console.ReadLine(), out int parameter) && parameter > 0 && parameter <= length)
                {
                    return parameter;
                }

                ConsoleIO.OutputError("The parameter goes beyond the boundaries of the matrix!\n");

            } while (true);
        }

        public void StartGame(int countPlayers)
        {
            int player = GetPlayerMovesFirst();
            string character = GetPlayerCharacter();
            InitializeGame(GetGameBoardSize(), player, character);

            GetBotMove(countPlayers);

            while (ExistMoves)
            {
                ConsoleIO.WriteMatrix(_gameBoard, _playerCharacter);

                if (countPlayers == 2)
                {
                    ConsoleIO.OutputSuccess($"\n{(_currentPlayer == 1 ? "First" : "Second")} player:");
                }

                var move = GetMove(_gameBoard);
                MakeMove(move.Item1, move.Item2);
                GetBotMove(countPlayers);
            }

            Console.WriteLine("\nGame is finished!");
            ConsoleIO.WriteMatrix(_gameBoard, _playerCharacter);
            var gameResult = GetScore();
            Console.WriteLine($"\nScore:\nPlayer X: {gameResult.Item1}\nPlayer O: {gameResult.Item2}\n\n");
        }

        public void MakeMove(int moveX, int moveY)
        {
            _availableMoves.RemoveAll(position => position.Item1 == moveX && position.Item2 == moveY);
            _gameBoard[moveX, moveY] = _currentPlayer;
            _currentPlayer = (_currentPlayer == 1) ? 2 : 1;
        }

        public void GetBotMove(int countPlayers)
        {
            if (_currentPlayer == 2 && countPlayers == 1 && ExistMoves)
            {
                Random random = new Random();
                int index = random.Next(0, _availableMoves.Count - 1);
                MakeMove(_availableMoves[index].Item1, _availableMoves[index].Item2);
            }
        }

        public (int, int) GetScore()
        {
            int firstPlayer = 1;
            int secondPlayer = 2;

            if (_playerCharacter.Equals("o", StringComparison.CurrentCulture))
            {
                (firstPlayer, secondPlayer) = (2, 1);
            }

            int scoreX = FindСombinationsInLines(firstPlayer) + FindСombinationsInDiagonals(firstPlayer);
            int scoreY = FindСombinationsInLines(secondPlayer) + FindСombinationsInDiagonals(secondPlayer);

            return (scoreX, scoreY);
        }

        private int FindСombinationsInLines(int player)
        {
            int count = 0;

            for (int i = 0; i < _size; i++)
            {
                int countInRow = 0, countInColumn = 0;
                for (int j = 0; j < _size; j++)
                {
                    if (_gameBoard[i, j] == player)
                    {
                        countInRow++;
                    }
                    else
                    {
                        count += countInRow / 3;
                        countInRow = 0;
                    }

                    if (_gameBoard[j, i] == player)
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

        private int FindСombinationsInDiagonals(int player)
        {
            int count = 0;

            for (int i = _size; i >= -_size; i--)
            {
                int firstDiagonal = 0; // Diagonal that starts at the upper left corner and ends at the lower right corner. Example: \
                int secondDiagonal = 0; // Diagonal that starts at the upper right corner and ends at the lower left corner. Example: /

                for (int j = 0; j < _size - i; j++)
                {
                    int index = i + j;

                    if (index < _size && j < _size && index >= 0 && j >= 0)
                    {
                        if (_gameBoard[j, index] == player)
                        {
                            firstDiagonal++;
                        }
                        else
                        {
                            count += firstDiagonal / 3;
                            firstDiagonal = 0;
                        }

                        if (_gameBoard[j, _size - 1 - index] == player)
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

            } while (true);

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
    }
}
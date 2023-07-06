using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1;

public class Board
{
    private const int BoardSize = 8;
    private char[,] board;
    private char currentPlayer;

    public Board()
    {
        board = new char[BoardSize, BoardSize];
        currentPlayer = 'W'; // White moves first
    }

    private char GetOpponentPiece()
    {
        return (currentPlayer == 'W') ? 'R' : 'W';
    }

    public void PrintBoard()
    {
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                Console.Write(board[row, col] + " ");
            }
            Console.WriteLine();
        }
    }

    public void InitializeBoard()
    {
        // Populate the board with initial piece positions
        // 'W' for white pieces, 'R' for red pieces, '.' for empty spaces
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                if ((row + col) % 2 != 0)
                {
                    if (row < 3)
                        board[row, col] = 'W';
                    else if (row > 4)
                        board[row, col] = 'R';
                    else
                        board[row, col] = '.';
                }
                else
                {
                    board[row, col] = ' ';
                }
            }
        }
    }

    public void ApplyMovesFromFile(string filePath)
    {
        try
        {
            string[] moves = File.ReadAllLines(filePath);

            foreach (string move in moves)
            {
                if (!IsValidMove(move))
                {
                    Console.WriteLine($"Invalid move: {move}");
                    return;
                }

                int[] moveCoordinates = Array.ConvertAll(move.Split(','), int.Parse);
                int startX = moveCoordinates[0];
                int startY = moveCoordinates[1];
                int endX = moveCoordinates[2];
                int endY = moveCoordinates[3];

                if (!IsValidPosition(startX, startY) || !IsValidPosition(endX, endY))
                {
                    Console.WriteLine($"Invalid position in move: {move}");
                    return;
                }

                if (!IsLegalMove(startX, startY, endX, endY))
                {
                    Console.WriteLine($"Illegal move: {move}");
                    return;
                }

                PerformMove(startX, startY, endX, endY);

                if (HasWon())
                {
                    Console.WriteLine($"Player {currentPlayer} wins!");
                    return;
                }

                currentPlayer = (currentPlayer == 'W') ? 'R' : 'W'; // Switch player
                Console.WriteLine("yes");
            }

            Console.WriteLine("Incomplete game");
            PrintBoard();
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("File not found: " + filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    private bool IsValidMove(string move)
    {
        string[] coordinates = move.Split(',');
        return coordinates.Length == 4;
    }

    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < BoardSize && y >= 0 && y < BoardSize;
    }

    private bool HasWon()
    {
        int whiteCount = 0;
        int redCount = 0;

        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                if (board[row, col] == 'W')
                    whiteCount++;
                else if (board[row, col] == 'R')
                    redCount++;
            }
        }
       
        return whiteCount == 0 || redCount == 0;
    }

    private void PerformMove(int startX, int startY, int endX, int endY)
    {
        char piece = board[startY, startX];
        board[startY, startX] = '.';
        board[endY, endX] = piece;
    }

    private bool IsLegalMove(int startX, int startY, int endX, int endY)
    {
        int direction = (currentPlayer == 'W') ? 1 : -1; // Determine the direction based on the current player

        // Check if the move is in the correct diagonal direction
        if ((endY - startY) * direction <= 0)
        {
            Console.WriteLine("not valid Direction");
            return false;
        }


        // Check if the move is within the bounds of the board
        if (!IsValidPosition(startX, startY) || !IsValidPosition(endX, endY))
        {
            Console.WriteLine("not valid position");
            return false;
        }
            

        // Check if the destination position is empty
        if (board[endY, endX] != '.')
        {
            Console.WriteLine("Destination position is not empty");
            return false;
        }
        

        int diffX = Math.Abs(endX - startX);
        int diffY = Math.Abs(endY - startY);

        // Check if it's a regular move (non-jump)
        if (diffX == 1 && diffY == 1)
            return true;

        // Check if it's a jump move
        if (diffX == 2 && diffY == 2)
        {
            // Calculate the position of the jumped piece
            int jumpedX = (endX + startX) / 2;
            int jumpedY = (endY + startY) / 2;

            // Check if the jumped position contains an opponent's piece
            if (board[jumpedY, jumpedX] == GetOpponentPiece())
            {
                // Check if the destination position after the jump is within the bounds of the board
                if (IsValidPosition(endX, endY))
                {
                    // Check if the destination position after the jump is empty
                    if (board[endY, endX] == '.')
                    {
                        // Remove the jumped piece from the board
                        board[jumpedY, jumpedX] = '.';

                        return true;
                    }
                }
            }
        }

        return false;
    }

}

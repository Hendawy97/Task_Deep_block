namespace ConsoleApp1;

public class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Invalid number of arguments. Usage: ConsoleApp1 <file_path>");
            return;
        }

        string filePath = args[0];

        Board board = new Board();
        board.InitializeBoard();
        board.PrintBoard();
        board.ApplyMovesFromFile(filePath);
    }

}
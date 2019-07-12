using System;

namespace EightPuzzleSolver
{
    public class Program
    {
        public static void Main()
        {
            // Hard coded initial state
            int[] initialState = new int[9];

            initialState[0] = 2;
            initialState[1] = 8;
            initialState[2] = 3;
            initialState[3] = 1;
            initialState[4] = 6;
            initialState[5] = 4;
            initialState[6] = 7;
            initialState[7] = 0;
            initialState[8] = 5;

            Console.WriteLine("1 Manhattan Distance");
            Console.WriteLine("2 Misplaced tiles");
            Console.WriteLine("3 Nilsson's distance");

            Console.Write("Choose your heuristic: ");
            var heuristic = Console.Read();

            Solver puzzle = new Solver(initialState, heuristic);
            puzzle.SolvePuzzle();

            Console.ReadKey();
        }
    }
}
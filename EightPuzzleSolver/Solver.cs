using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EightPuzzleSolver
{
    public class Solver
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly int[][] _movingRules;
        private readonly int[] _solution = new int[9];
        private readonly int[] _startState = new int[9];
        private readonly int _noOfTiles;
        private readonly int _noOfMoves;
        private readonly int _kindOfHeuristic;
        private readonly List<TreeNode> _fringe = new List<TreeNode>();
        private bool _done;

        public Solver(int[] givenStartState, int heuristic)
        {
            int[][] rules =
            {
                new[] {1, 3, -9, -9},
                new[] {0, 2, 4, -9}, 
                new[] {1, 5, -9, -9},
                new[] {0, 4, 6, -9}, 
                new[] {1, 3, 5, 7}, 
                new[] {2, 4, 8, -9},
                new[] {3, 7, -9, -9},
                new[] {4, 6, 8, -9}, 
                new[] {5, 7, -9, -9} 
            };

            _noOfTiles = 9;
            _noOfMoves = 4;

            _movingRules = new[]
            {
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0}
            };

            for (int i = 0; i < _noOfTiles; i++)
            {
                for (int j = 0; j < _noOfMoves; j++)
                {
                    _movingRules[i][j] = rules[i][j];
                }
            }

            _solution[0] = 1;
            _solution[1] = 2;
            _solution[2] = 3;
            _solution[3] = 8;
            _solution[4] = 0;
            _solution[5] = 4;
            _solution[6] = 7;
            _solution[7] = 6;
            _solution[8] = 5;

            for (int i = 0; i < _noOfTiles; i++)
            {
                _startState[i] = givenStartState[i];
            }

            _kindOfHeuristic = heuristic;
            _done = false;
        }

        int GetHeuristic(int kind, TreeNode node)
        {
            // Manhattan distance
            if (kind == 1)
            {
                int manhattan = 0;

                int[][] distances = new int[9][]
                {
                    new[] {0, 1, 2, 1, 2, 3, 2, 3, 4},
                    new[] {1, 0, 1, 2, 1, 2, 3, 2, 3},
                    new[] {2, 1, 0, 3, 2, 1, 4, 3, 2},
                    new[] {1, 2, 3, 0, 1, 2, 1, 2, 3},
                    new[] {2, 1, 2, 1, 0, 1, 2, 1, 2},
                    new[] {3, 2, 1, 2, 1, 0, 3, 2, 1},
                    new[] {2, 3, 4, 1, 2, 3, 0, 1, 2},
                    new[] {3, 2, 3, 2, 1, 2, 1, 0, 1},
                    new[] {4, 3, 2, 3, 2, 1, 2, 1, 0}
                };

                for (int i = 0; i < _noOfTiles; i++)
                {
                    manhattan += distances[i][node.State[i]];
                }

                return manhattan;
            }

            // Misplaced tiles
            if (kind == 2)
            {
                int heuristic = 0;

                for (int i = 0; i < _noOfTiles; i++)
                {
                    if (node.State[i] != _solution[i])
                    {
                        heuristic++;
                    }
                }

                return heuristic;
            }

            // Nilsson's distance
            if (kind == 3)
            {
                int manhattan = 0;
                int sequenceScore = 0;

                int[][] distances = new[]
                {
                    new[] {0, 1, 2, 1, 2, 3, 2, 3, 4},
                    new[] {1, 0, 1, 2, 1, 2, 3, 2, 3},
                    new[] {2, 1, 0, 3, 2, 1, 4, 3, 2},
                    new[] {1, 2, 3, 0, 1, 2, 1, 2, 3},
                    new[] {2, 1, 2, 1, 0, 1, 2, 1, 2},
                    new[] {3, 2, 1, 2, 1, 0, 3, 2, 1},
                    new[] {2, 3, 4, 1, 2, 3, 0, 1, 2},
                    new[] {3, 2, 3, 2, 1, 2, 1, 0, 1},
                    new[] {4, 3, 2, 3, 2, 1, 2, 1, 0}
                };

                for (int i = 0; i < _noOfTiles; i++)
                {
                    manhattan += distances[i][node.State[i]];
                }

                for (int i = 0; i < 8; i++)
                {
                    int first = node.State[i];
                    int second = node.State[i + 1];

                    if ((first + 1) != second)
                    {
                        sequenceScore++;
                    }
                }

                if (node.State[8] != _solution[8])
                {
                    sequenceScore++;
                }

                return manhattan + sequenceScore;
            }

            return 0;
        }

        public void SolvePuzzle()
        {
            TreeNode root = CreateNewNode();

            Initialize(root.State);
            root.Score = 10000;
            _fringe.Add(root);

            _stopwatch.Start();

            while (_done == false)
            {
                Expand(root);
            }
        }

        private void StopSolving(TreeNode node)
        {
            _stopwatch.Stop();

            Console.WriteLine(
                $"At depth= {node.Depth} with nodes= {_fringe.Count} in time {_stopwatch.ElapsedMilliseconds}");

            _done = true;
            PrintSolution(node);
        }

        private TreeNode CreateNewNode()
        {
            TreeNode node = new TreeNode();

            for (int i = 0; i < _noOfTiles; i++)
            {
                node.State[i] = i;
            }

            for (int i = 0; i < _noOfMoves; i++)
            {
                node.Children[i] = null;
            }

            node.Parent = null;
            node.Move = -1;
            node.NoOfChildren = 0;
            node.Heuristic = 0;
            node.PathCost = 0;
            node.Score = 0;
            node.Depth = 0;
            return node;
        }

        private void Initialize(int[] state)
        {
            for (int i = 0; i < _noOfTiles; i++)
            {
                state[i] = _startState[i];
            }
        }

        private void Expand(TreeNode node)
        {
            if (node == null)
            {
                Console.WriteLine("Node points to NULL.");
                Environment.Exit(-1);
            }

            if (node.Children[0] == null
                && node.Children[1] == null
                && node.Children[2] == null
                && node.Children[3] == null)
            {
                MakeChildren(node);
            }
            else
            {
                var temp = _fringe.First();

                _fringe.RemoveAt(0);

                if (IsSolutionFound(temp))
                {
                    Console.WriteLine("\nSOLUTION WAS FOUND!");

                    StopSolving(_fringe.First());
                    if (_done == true) return;
                }

                Console.WriteLine($"nodes in memory= {_fringe.Count}");
                Console.WriteLine($"exploring depth= {temp.Depth}");

                Console.WriteLine("expanding node:");

                PrintBoard(temp.State);
                Console.WriteLine($"with F(N)={temp.Score}");

                Expand(temp);
            }
        }

        private int CheckRepeated(TreeNode node)
        {
            TreeNode grandpa = node.Parent.Parent;
            if (grandpa == null) return 0;

            for (int i = 0; i < _noOfTiles; i++)
            {
                if (grandpa.State[i] != node.State[i])
                {
                    return 1;
                }
            }

            return 2;
        }

        private int SetDepth(TreeNode node, int count)
        {
            if (node.Parent == null)
            {
                return count;
            }

            count++;

            return SetDepth(node.Parent, count);
        }

        private TreeNode Create(TreeNode node, int index)
        {
            int holePosition = 0;

            for (int i = 0; i < _noOfTiles; i++)
            {
                if (node.State[i] == 0)
                {
                    holePosition = i;
                    break;
                }
            }

            var piecePosition = _movingRules[holePosition][index];

            // If move is illegal return null
            if (piecePosition == -9) return null;

            // Otherwise go on with child creation
            TreeNode child = CreateNewNode();
            child.Parent = node;

            child.Depth = SetDepth(node, 0);

            for (int i = 0; i < _noOfTiles; i++)
            {
                child.State[i] = node.State[i];
            }

            child.State[holePosition] = node.State[piecePosition];
            child.State[piecePosition] = node.State[holePosition];
            child.Move = node.State[piecePosition];

            Evaluate(child);

            return child;
        }

        private bool IsSolutionFound(TreeNode node)
        {
            for (int i = 0; i < _noOfTiles; i++)
            {
                if (node.State[i] != _solution[i])
                {
                    return false;
                }
            }

            return true;
        }

        private void Evaluate(TreeNode node)
        {
            // Path cost g(n)
            node.PathCost = node.Depth;
            
            // h(n) = 
            node.Heuristic = GetHeuristic(_kindOfHeuristic, node);
            
            // F(n) =
            node.Score = node.PathCost + node.Heuristic;
        }

        private void MakeChildren(TreeNode node)
        {
            for (int i = 0; i < _noOfMoves; i++)
            {
                var child = Create(node, i);

                if (child == null)
                {
                    node.Children[i] = null;
                    continue;
                }

                // Check if solution is found
                if (IsSolutionFound(child))
                {
                    Console.WriteLine("\nSOLUTION WAS FOUND!");

                    StopSolving(child);
                    if (_done) return;
                }

                node.Children[i] = child;
                node.NoOfChildren++;

                // add to fringe if not repeated
                if (CheckRepeated(child) != 2)
                {
                    _fringe.Add(child);
                }
            }
        }

        private void PrintBoard(int[] state)
        {
            Console.WriteLine($"| {state[0]} | {state[1]} | {state[2]} |");
            Console.WriteLine("| - | - | - |");
            Console.WriteLine($"| {state[3]} | {state[4]} | {state[5]} |");
            Console.WriteLine("| - | - | - |");
            Console.WriteLine($"| {state[6]} | {state[7]} | {state[8]} |");
        }

        private void PrintSolution(TreeNode node)
        {
            if (node == null) return;

            if (node.Move != -1)
            {
                Console.Write("Expanded node. ");
                Console.WriteLine($"Move: {node.Move}");

                PrintBoard(node.State);

                Console.WriteLine($"Node's F(N)={node.Score}");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Initial state: ");
                PrintBoard(node.State);
            }

            PrintSolution(node.Parent);
        }
    }
}
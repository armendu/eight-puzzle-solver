namespace EightPuzzleSolver
{
    public class TreeNode
    {
        public int[] State { get; set; } = new int[9];
        public TreeNode[] Children { get; set; } = new TreeNode[4];
        public TreeNode Parent { get; set; }
        public int PathCost { get; set; }
        public int Heuristic { get; set; }
        public int Score { get; set; }
        public int Move { get; set; }
        public int NoOfChildren { get; set; }
        public int Depth { get; set; }
    }
}
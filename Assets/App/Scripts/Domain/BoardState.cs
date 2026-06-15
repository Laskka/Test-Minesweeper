namespace App.Domain
{
    public readonly struct BoardState
    {
        public readonly Cell[,] Cells;
        public readonly int RemainingMines;
        
        public BoardState(Cell[,] updates, int remaining)
        {
            Cells = updates;
            RemainingMines = remaining;
        }
    }
}
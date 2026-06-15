namespace App.Domain
{
    public readonly struct Cell
    {
        public bool IsMine { get; }
        public byte Adjacent { get; }
        public CellState State { get; }

        public Cell(bool isMine, byte adjacent, CellState state)
        {
            IsMine = isMine;
            Adjacent = adjacent;
            State = state;
        }

        public bool IsHidden => State == CellState.Hidden;
        public bool IsRevealed => State == CellState.Revealed;
        public bool IsFlagged => State == CellState.Flagged;

        public Cell WithState(CellState state) => new(IsMine, Adjacent, state);
        public Cell WithMine(bool isMine) => new(isMine, Adjacent, State);
        public Cell WithAdjacent(byte adjacent) => new(IsMine, adjacent, State);
    }
}

using System;
using System.Collections.Generic;
using App.Domain;
using R3;

namespace App.Application
{
    public class BoardSystem
    {
        private ReactiveProperty<BoardState> _state;
        public ReadOnlyReactiveProperty<BoardState> State => _state;

        private readonly GameConfig _config;

        public BoardSystem(GameConfig config)
        {
            _config = config;
            _state = new ReactiveProperty<BoardState>(new BoardState(new Cell[config.Width, config.Height], config.MineCount));
        }

        public void Initialize(int firstX, int firstY)
        {
            var state = new BoardState(new Cell[_config.Width, _config.Height], _config.MineCount);
            PlaceMines(firstX, firstY, state);
            CalculateAdjacent(state);
            _state.Value = state;
        }

        public void Reveal(int x, int y)
        {
            if (_state.Value.Cells[x, y].State != CellState.Hidden) 
                return;

            var newState = new BoardState((Cell[,])_state.Value.Cells.Clone(), _state.Value.RemainingMines);

            if (newState.Cells[x, y].IsMine)
            {
                RevealAllMines(newState);
                newState = new BoardState(newState.Cells, newState.RemainingMines);
                _state.Value = newState;
                return;
            }

            FloodFill(x, y, newState);

            newState = new BoardState(newState.Cells, newState.RemainingMines);
            _state.Value = newState;
        }

        public void Flag(int x, int y)
        {
            var newState = new BoardState((Cell[,])_state.Value.Cells.Clone(), _state.Value.RemainingMines);
            ref Cell cell = ref newState.Cells[x, y];
            var remainingMines = newState.RemainingMines;
            
            if (cell.State == CellState.Revealed)
                return;

            if (cell.State == CellState.Flagged)
            {
                cell = cell.WithState(CellState.Hidden);
                remainingMines++;
            }
            else if(remainingMines > 0)
            {
                cell = cell.WithState(CellState.Flagged);
                remainingMines--;
            }

            newState = new BoardState(newState.Cells, remainingMines);
            _state.Value = newState;
        }

        public void Reset()
        {
            _state.Value = new BoardState(new Cell[_config.Width, _config.Height], _config.MineCount);
        }

        private void PlaceMines(int firstX, int firstY, BoardState state)
        {
            var forbidden = new HashSet<(int, int)> { (firstX, firstY) };

            var candidates = new List<(int x, int y)>(_config.Width * _config.Height);
            for (int y = 0; y < _config.Height; y++)
            for (int x = 0; x < _config.Width; x++)
                if (!forbidden.Contains((x, y)))
                    candidates.Add((x, y));

            var rng = new Random();
            for (int i = candidates.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
            }

            int count = Math.Min(_config.MineCount, candidates.Count);
            for (int i = 0; i < count; i++)
            {
                var (mx, my) = candidates[i];
                state.Cells[mx, my] = state.Cells[mx, my].WithMine(true);
            }
        }

        private void CalculateAdjacent(BoardState state)
        {
            Cell[,] grid = state.Cells;
            for (int y = 0; y < _config.Height; y++)
            {
                for (int x = 0; x < _config.Width; x++)
                {
                    if (state.Cells[x, y].IsMine) continue;

                    byte count = 0;
                    foreach (var (nx, ny) in GetNeighbors(x, y))
                        if (state.Cells[nx, ny].IsMine)
                            count++;

                    grid[x, y] = grid[x, y].WithAdjacent(count);
                }
            }
        }

        private IEnumerable<(int x, int y)> GetNeighbors(int x, int y)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx >= 0 && nx < _config.Width && ny >= 0 && ny < _config.Height)
                        yield return (nx, ny);
                }
            }
        }

        private void FloodFill(int startX, int startY, BoardState state)
        {
            var queue = new Queue<(int x, int y)>();
            queue.Enqueue((startX, startY));

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                ref Cell cell = ref state.Cells[x, y];

                if (cell.State == CellState.Revealed) continue;
                if (cell.State == CellState.Flagged)  continue;
                if (cell.IsMine)                       continue;

                cell = cell.WithState(CellState.Revealed);

                if (cell.Adjacent == 0)
                    foreach (var (nx, ny) in GetNeighbors(x, y))
                        if (state.Cells[nx, ny].State == CellState.Hidden)
                            queue.Enqueue((nx, ny));
            }
        }

        private void RevealAllMines(BoardState state)
        {
            for (int y = 0; y < _config.Height; y++)
            for (int x = 0; x < _config.Width;  x++)
            {
                ref Cell cell = ref state.Cells[x, y];
                if (cell.IsMine && cell.State != CellState.Flagged)
                {
                    cell = cell.WithState(CellState.Revealed);
                }
            }
        }
    }
}
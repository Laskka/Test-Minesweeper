using System;
using System.Collections.Generic;
using App.Domain;
using App.Application;
using NUnit.Framework;

namespace App.Tests
{
    public sealed class BoardSystemTests
    {
        private const int Width = 9;
        private const int Height = 9;
        private const int Mines = 10;

        private static GameConfig Config() => new GameConfig(Width, Height, Mines);

        // ─── Расстановка мин ─────────────────────────────────────────────

        [Test]
        public void FirstClick_IsNeverAMine()
        {
            var board = new BoardSystem(Config());

            board.Initialize(4, 4);

            Assert.IsFalse(board.State.CurrentValue.Cells[4, 4].IsMine);
        }

        [Test]
        public void Initialize_PlacesExactMineCount()
        {
            var board = new BoardSystem(Config());

            board.Initialize(0, 0);

            Assert.AreEqual(Mines, CountMines(board.State.CurrentValue.Cells));
        }

        [Test]
        public void Adjacent_EqualsNeighbouringMineCount()
        {
            var board = new BoardSystem(Config());

            board.Initialize(0, 0);
            var cells = board.State.CurrentValue.Cells;

            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                if (cells[x, y].IsMine) continue;
                Assert.AreEqual(
                    CountMineNeighbours(cells, x, y),
                    cells[x, y].Adjacent,
                    $"Неверное число соседних мин в ({x},{y})");
            }
        }

        // ─── Открытие / flood fill ───────────────────────────────────────

        [Test]
        public void Reveal_OnSafeClick_NeverRevealsAMine()
        {
            var board = new BoardSystem(Config());
            board.Initialize(4, 4);

            board.Reveal(4, 4);

            var cells = board.State.CurrentValue.Cells;
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                if (cells[x, y].IsRevealed)
                    Assert.IsFalse(cells[x, y].IsMine, $"Открылась мина в ({x},{y})");
        }

        [Test]
        public void FloodFill_EveryRevealedEmptyCell_HasAllNeighboursRevealed()
        {
            var board = new BoardSystem(Config());
            board.Initialize(4, 4);

            board.Reveal(4, 4);

            // Инвариант цепочки: у каждой открытой клетки с 0 соседних мин
            // все соседи тоже должны быть открыты.
            var cells = board.State.CurrentValue.Cells;
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                if (!cells[x, y].IsRevealed || cells[x, y].Adjacent != 0) continue;
                foreach (var (nx, ny) in Neighbours(x, y))
                    Assert.IsTrue(cells[nx, ny].IsRevealed,
                        $"Сосед ({nx},{ny}) пустой клетки ({x},{y}) не открыт");
            }
        }

        [Test]
        public void Reveal_AlreadyRevealedCell_IsNoOp()
        {
            var board = new BoardSystem(Config());
            board.Initialize(4, 4);
            board.Reveal(4, 4);
            var before = board.State.CurrentValue.Cells;

            board.Reveal(4, 4);

            Assert.AreSame(before, board.State.CurrentValue.Cells,
                "Состояние изменилось при повторном открытии уже открытой клетки");
        }

        [Test]
        public void Reveal_FlaggedCell_DoesNotReveal()
        {
            var board = new BoardSystem(Config());
            board.Initialize(4, 4);
            board.Flag(0, 0);

            board.Reveal(0, 0);

            var cell = board.State.CurrentValue.Cells[0, 0];
            Assert.IsFalse(cell.IsRevealed);
            Assert.IsTrue(cell.IsFlagged);
        }

        [Test]
        public void RevealingAMine_RevealsAllMines()
        {
            var board = new BoardSystem(Config());
            board.Initialize(4, 4);
            var (mx, my) = FindFirstMine(board.State.CurrentValue.Cells);

            board.Reveal(mx, my);

            var cells = board.State.CurrentValue.Cells;
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                if (cells[x, y].IsMine)
                    Assert.IsTrue(cells[x, y].IsRevealed, $"Мина ({x},{y}) не открыта после проигрыша");
        }

        // ─── Флажки ──────────────────────────────────────────────────────

        [Test]
        public void Flag_TogglesStateAndRemainingMines()
        {
            var board = new BoardSystem(Config());
            board.Initialize(4, 4);
            int before = board.State.CurrentValue.RemainingMines;

            board.Flag(0, 0);
            Assert.IsTrue(board.State.CurrentValue.Cells[0, 0].IsFlagged);
            Assert.AreEqual(before - 1, board.State.CurrentValue.RemainingMines);

            board.Flag(0, 0);
            Assert.IsTrue(board.State.CurrentValue.Cells[0, 0].IsHidden);
            Assert.AreEqual(before, board.State.CurrentValue.RemainingMines);
        }

        [Test]
        public void Flag_OnRevealedCell_IsNoOp()
        {
            var board = new BoardSystem(Config());
            board.Initialize(4, 4);
            board.Reveal(4, 4);
            int before = board.State.CurrentValue.RemainingMines;

            board.Flag(4, 4);

            Assert.IsTrue(board.State.CurrentValue.Cells[4, 4].IsRevealed);
            Assert.AreEqual(before, board.State.CurrentValue.RemainingMines);
        }

        // ─── Хелперы ─────────────────────────────────────────────────────

        private static int CountMines(Cell[,] cells)
        {
            int n = 0;
            foreach (var c in cells)
                if (c.IsMine) n++;
            return n;
        }

        private static int CountMineNeighbours(Cell[,] cells, int x, int y)
        {
            int n = 0;
            foreach (var (nx, ny) in Neighbours(x, y))
                if (cells[nx, ny].IsMine) n++;
            return n;
        }

        private static (int x, int y) FindFirstMine(Cell[,] cells)
        {
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                if (cells[x, y].IsMine) return (x, y);
            throw new InvalidOperationException("На поле нет ни одной мины.");
        }

        private static IEnumerable<(int x, int y)> Neighbours(int x, int y)
        {
            for (int dy = -1; dy <= 1; dy++)
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx, ny = y + dy;
                if (nx >= 0 && nx < Width && ny >= 0 && ny < Height)
                    yield return (nx, ny);
            }
        }
    }
}

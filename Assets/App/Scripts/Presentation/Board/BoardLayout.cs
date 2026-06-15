using UnityEngine;

namespace App.Presentation.Board
{
    public readonly struct BoardLayout
    {
        public readonly Vector2 Origin;   
        public readonly float   CellSize;
        public readonly int     Width;
        public readonly int     Height;

        public BoardLayout(int width, int height, float cellSize)
        {
            Width    = width;
            Height   = height;
            CellSize = cellSize;
            Origin   = new Vector2(
                -width  * cellSize / 2f,
                -height * cellSize / 2f
            );
        }

        public Vector3 CellToWorld(int x, int y) => new Vector3(
            Origin.x + (x + 0.5f) * CellSize,
            Origin.y + (y + 0.5f) * CellSize,
            0f
        );

        public bool TryWorldToCell(Vector3 worldPos, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPos.x - Origin.x) / CellSize);
            y = Mathf.FloorToInt((worldPos.y - Origin.y) / CellSize);
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }
    }
}
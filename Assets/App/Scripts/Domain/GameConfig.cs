using System;

namespace App.Domain
{
    public readonly struct GameConfig
    {
        public int Width { get; }
        public int Height { get; }
        public int MineCount { get; }

        public int CellCount => Width * Height;

        public GameConfig(int width, int height, int mineCount)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
            if (mineCount < 0) throw new ArgumentOutOfRangeException(nameof(mineCount));

            Width = width;
            Height = height;
            
            MineCount = Math.Min(mineCount, width * height - 1);
        }
    }
}

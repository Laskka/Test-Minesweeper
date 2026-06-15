using System;
using App.Domain;
using R3;

namespace App.Application
{
    public enum GameResult 
    { 
        None, 
        Won, 
        Lost
    }
    
    public class GameResultDetector : IDisposable
    {
        public Observable<GameResult> Result => _result;
        
        private readonly Subject<GameResult> _result = new();
        private readonly GameConfig _config;
        private DisposableBag _disposableBag;
        
        public GameResultDetector(BoardSystem state, GameConfig config)
        {
            _config = config;
            _disposableBag = new DisposableBag();
            _disposableBag.Add(state.State.Skip(1).Subscribe(Evaluate));
        }

        private void Evaluate(BoardState state)
        {
            for (int y = 0; y < _config.Height; y++)
            for (int x = 0; x < _config.Width;  x++)
            {
                var cell = state.Cells[x, y];
                if (!cell.IsMine || cell.State != CellState.Revealed) 
                    continue;
                _result.OnNext(GameResult.Lost);
                return;
            }

            for (int y = 0; y < _config.Height; y++)
            for (int x = 0; x < _config.Width;  x++)
                if (!state.Cells[x, y].IsMine && state.Cells[x, y].State != CellState.Revealed)
                    return;

            _result.OnNext(GameResult.Won);
        }


        public void Dispose()
        {
            _disposableBag.Dispose();
        }
    }
}
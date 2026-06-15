using System;
using App.Domain;
using R3;

namespace App.Application
{
    public enum GamePhase
    {
        AwaitingFirstMove,
        Playing,
        GameOver
    }
    
    public sealed class GameSession : IDisposable
    {
        public ReadOnlyReactiveProperty<GamePhase> Phase => _phase;
        public ReadOnlyReactiveProperty<BoardState> BoardState => _board.State;
        
        private readonly ReactiveProperty<GamePhase> _phase = new(GamePhase.AwaitingFirstMove);
        
        private readonly BoardSystem _board;
        private DisposableBag _disposableBag;

        
        public GameSession(BoardSystem board, GameResultDetector gameResultDetector)
        {
            _disposableBag.Add(gameResultDetector.Result.Subscribe(OnGameResult));
            _board = board;
        }

        public void OnGameResult(GameResult result)
        {
            switch (result)
            {
                case GameResult.Lost:
                    _phase.Value = GamePhase.GameOver;
                    break;
                case GameResult.Won:
                    _phase.Value = GamePhase.GameOver;
                    break;
            }
        }

        public void Reveal(int x, int y)
        {
            switch (_phase.Value)
            {
                case GamePhase.AwaitingFirstMove:
                    _board.Initialize(x, y);
                    _phase.Value = GamePhase.Playing;
                    _board.Reveal(x, y);
                    break;

                case GamePhase.Playing:
                    _board.Reveal(x, y);
                    break;

                case GamePhase.GameOver:
                    break;
            }
        }

        public void Flag(int x, int y)
        {
            if (_phase.Value == GamePhase.Playing)
                _board.Flag(x, y);
        }
        
        public void Reset()
        {
            _board.Reset();
            _phase.Value = GamePhase.AwaitingFirstMove;
        }

        public void Dispose() => _disposableBag.Dispose();
    }
}
using System;
using R3;

namespace App.Application
{
    public enum ScreenState
    {
        MainMenu,
        InGame,
        Paused,
        GameOver
    }

    public sealed class GameFlow : IDisposable
    {
        private readonly ReactiveProperty<ScreenState> _state = new(ScreenState.MainMenu);
        public ReadOnlyReactiveProperty<ScreenState> State => _state;

        private readonly GameSession _session;
        private DisposableBag _disposableBag;

        public GameFlow(GameSession session)
        {
            _session = session;

            _disposableBag.Add(
                _session.Phase
                    .Where(phase => phase == GamePhase.GameOver)
                    .Subscribe(_ => _state.Value = ScreenState.GameOver));
        }

        public void StartGame()
        {
            if (_state.Value != ScreenState.MainMenu) return;
            _session.Reset();
            _state.Value = ScreenState.InGame;
        }

        public void Pause()
        {
            if (_state.Value != ScreenState.InGame) return;
            _state.Value = ScreenState.Paused;
        }

        public void Resume()
        {
            if (_state.Value != ScreenState.Paused) return;
            _state.Value = ScreenState.InGame;
        }

        public void Restart()
        {
            if (_state.Value != ScreenState.Paused && _state.Value != ScreenState.GameOver && _state.Value != ScreenState.InGame) return;
            _session.Reset();
            _state.Value = ScreenState.InGame;
        }

        public void ToMenu()
        {
            if (_state.Value != ScreenState.Paused && _state.Value != ScreenState.GameOver) return;
            _state.Value = ScreenState.MainMenu;
        }

        public void Dispose() => _disposableBag.Dispose();
    }
}
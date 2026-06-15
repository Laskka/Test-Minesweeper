using System;
using App.Application;
using R3;
using VContainer.Unity;

namespace App.Presentation.UI
{
    public sealed class ScreenRouter : IStartable, IDisposable
    {
        private readonly GameFlow _flow;
        private readonly MenuView _menu;
        private readonly HudView _hud;
        private readonly PauseView _pause;
        private readonly GameOverView _gameOver;

        private IDisposable _subscription;
        private readonly ViewRoot _world;

        public ScreenRouter(GameFlow flow, MenuView menu, HudView hud,
            PauseView pause, GameOverView gameOver, ViewRoot viewRoot)
        {
            _flow = flow;
            _menu = menu;
            _hud = hud;
            _pause = pause;
            _gameOver = gameOver;
            _world = viewRoot;
        }

        public void Start() => _subscription = _flow.State.Subscribe(Render);

        private void Render(ScreenState state)
        {
            bool inSession = state is ScreenState.InGame or ScreenState.Paused or ScreenState.GameOver;
            
            _menu.SetVisible(state == ScreenState.MainMenu);
            _world.SetVisible(inSession);
            _hud.SetVisible(inSession);
            _pause.SetVisible(state == ScreenState.Paused);
            _gameOver.SetVisible(state == ScreenState.GameOver);
        }

        public void Dispose() => _subscription?.Dispose();
    }
}
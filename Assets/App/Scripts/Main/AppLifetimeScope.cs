using App.Domain;
using App.Main;
using App.Application;
using App.Presentation;
using App.Presentation.UI;
using App.Presentation.Board;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class AppLifetimeScope : LifetimeScope
{
    private const float CellSize = 0.6f;

    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private Camera _camera;
    
    [SerializeField] private ViewRoot _viewRoot;
    [SerializeField] private BoardInputHandler _boardInputHandler;
    [SerializeField] private SessionTimer _timer;
    
    [Header("UI")]
    [SerializeField] private MenuView _menuView;
    [SerializeField] private HudView _hudView;
    [SerializeField] private PauseView _pauseView;
    [SerializeField] private GameOverView _gameOverView;
    
    protected override void Configure(IContainerBuilder builder)
    {
        //Config
        builder.RegisterInstance<GameConfig>(gameSettings.Config);

        //Gameplay
        builder.Register<BoardSystem>(Lifetime.Singleton);
        builder.Register<GameResultDetector>(Lifetime.Singleton);
        builder.Register<GameSession>(Lifetime.Singleton);
        builder.RegisterInstance<BoardInputHandler>(_boardInputHandler);

        //View
        builder.RegisterInstance<SessionTimer>(_timer);
        builder.RegisterInstance<ViewRoot>(_viewRoot);
        builder.RegisterInstance<Camera>(_camera);
        builder.Register<BoardLayout>(resolver =>
        {
            var cfg = resolver.Resolve<GameConfig>();
            return new BoardLayout(width: cfg.Width, height: cfg.Height, cellSize: CellSize);
        }, Lifetime.Singleton);

        builder.RegisterBuildCallback(resolver =>
        {
            var session = resolver.Resolve<GameSession>();
            var flow = resolver.Resolve<GameFlow>();
            _boardInputHandler.OnReveal
                .Where(_ => flow.State.CurrentValue == ScreenState.InGame)
                .Subscribe(c => session.Reveal(c.x, c.y))
                .AddTo(_boardInputHandler);
            _boardInputHandler.OnFlag
                .Where(_ => flow.State.CurrentValue == ScreenState.InGame)
                .Subscribe(c => session.Flag(c.x, c.y))
                .AddTo(_boardInputHandler);
        });
        
        //UI
        builder.Register<GameFlow>(Lifetime.Singleton);

        builder.RegisterInstance<MenuView>(_menuView);
        builder.RegisterInstance<HudView>(_hudView);
        builder.RegisterInstance<PauseView>(_pauseView);
        builder.RegisterInstance<GameOverView>(_gameOverView);

        builder.RegisterEntryPoint<ScreenRouter>();
    }
}
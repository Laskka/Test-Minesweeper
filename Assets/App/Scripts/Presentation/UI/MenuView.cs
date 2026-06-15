using App.Application;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace App.Presentation.UI
{
    public sealed class MenuView : UiScreen
    {
        [SerializeField] private Button _startButton;

        [Inject]
        public void Construct(GameFlow flow) =>
            _startButton.OnClickAsObservable().Subscribe(_ => flow.StartGame()).AddTo(this);
    }
}
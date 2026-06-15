using App.Application;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace App.Presentation.UI
{
    public sealed class PauseView : UiScreen
    {
        [SerializeField] private Button _resume, _restart, _menu;

        [Inject]
        public void Construct(GameFlow flow)
        {
            _resume.OnClickAsObservable().Subscribe(_ => flow.Resume()).AddTo(this);
            _restart.OnClickAsObservable().Subscribe(_ => flow.Restart()).AddTo(this);
            _menu.OnClickAsObservable().Subscribe(_ => flow.ToMenu()).AddTo(this);
        }
    }
}
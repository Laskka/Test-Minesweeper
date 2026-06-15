using App.Application;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace App.Presentation.UI
{
    public sealed class HudView : UiScreen
    {
        [SerializeField] private Button _pauseButton;
        [SerializeField] private TMP_Text _minesLabel, _timerLabel;

        [Inject]
        public void Construct(GameFlow flow, GameSession session, SessionTimer timer)
        {
            _pauseButton.OnClickAsObservable().Subscribe(_ => flow.Pause()).AddTo(this);
            session.BoardState
                .Subscribe(b => _minesLabel.text = b.RemainingMines.ToString())
                .AddTo(this);
            timer.Elapsed
                .Select(Mathf.FloorToInt)
                .DistinctUntilChanged()
                .Subscribe(s => _timerLabel.SetText(s.ToString()))
                .AddTo(this);
        }
    }
}
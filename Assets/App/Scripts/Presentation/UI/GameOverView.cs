using App.Application;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace App.Presentation.UI
{
    public sealed class GameOverView : UiScreen
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Button _restart, _menu;

        [Inject]
        public void Construct(GameFlow flow, GameResultDetector detector)
        {
            _restart.OnClickAsObservable().Subscribe(_ => flow.Restart()).AddTo(this);
            _menu.OnClickAsObservable().Subscribe(_ => flow.ToMenu()).AddTo(this);
            detector.Result
                .Subscribe(DisplayGameResultMessage)
                .AddTo(this);
        }

        private void DisplayGameResultMessage(GameResult result)
        {
            if (result == GameResult.Won)
            {
                _title.text = "Победа!";
                _title.color = Color.green;
            }
            else
            {
                _title.text = "Игра окончена";
                _title.color = Color.red;
            }
        }
    }
}
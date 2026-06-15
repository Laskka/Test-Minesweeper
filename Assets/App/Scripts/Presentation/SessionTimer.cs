using App.Application;
using R3;
using UnityEngine;
using VContainer;

namespace App.Presentation.UI
{
    public sealed class SessionTimer : MonoBehaviour
    {
        public ReadOnlyReactiveProperty<float> Elapsed => _elapsed;
        private readonly ReactiveProperty<float> _elapsed = new(0f);

        private GameFlow _flow;
        private GameSession _session;

        [Inject]
        public void Construct(GameFlow flow, GameSession session)
        {
            _flow = flow;
            _session = session;

            _session.Phase
                .Where(phase => phase == GamePhase.AwaitingFirstMove)
                .Subscribe(_ => _elapsed.Value = 0f)
                .AddTo(this);
        }

        private void Update()
        {
            if (_flow.State.CurrentValue == ScreenState.InGame
                && _session.Phase.CurrentValue == GamePhase.Playing)
                _elapsed.Value += Time.deltaTime;
        }

        private void OnDestroy() => _elapsed.Dispose();
    }
}
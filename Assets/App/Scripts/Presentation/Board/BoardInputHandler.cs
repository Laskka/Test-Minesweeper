using R3;
using UnityEngine;
using VContainer;

namespace App.Presentation.Board
{
    public class BoardInputHandler : MonoBehaviour
    {
        public Observable<Vector2Int> OnReveal => _onReveal;
        public Observable<Vector2Int> OnFlag => _onFlag;

        private readonly Subject<Vector2Int> _onReveal = new();
        private readonly Subject<Vector2Int> _onFlag = new();

        private Camera _camera;
        private BoardLayout _layout;

        [Inject]
        public void Construct(Camera camera, BoardLayout layout)
        {
            _camera = camera;
            _layout = layout;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && TryGetCell(out var cell))
                _onReveal.OnNext(cell);

            else if (Input.GetMouseButtonDown(1) && TryGetCell(out cell))
                _onFlag.OnNext(cell);
        }

        private bool TryGetCell(out Vector2Int cell)
        {
            var world = _camera.ScreenToWorldPoint(Input.mousePosition);
            var hit = _layout.TryWorldToCell(world, out int x, out int y);
            cell = new Vector2Int(x, y);
            return hit;
        }

        private void OnDestroy()
        {
            _onReveal.Dispose();
            _onFlag.Dispose();
        }
    }
}
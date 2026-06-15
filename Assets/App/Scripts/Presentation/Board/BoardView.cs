using System;
using App.Application;
using App.Domain;
using R3;
using UnityEngine;
using VContainer;

namespace App.Presentation.Board
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private GridCellView _cellViewPrefab;

        private GridCellView[,] _cellViews;
        private BoardLayout _layout;
        private IDisposable _subscription;

        [Inject]
        public void Construct(BoardSystem boardState, GameConfig config, BoardLayout layout)
        {
            _layout = layout;
            
            SpawnGrid(config);

            _subscription = boardState.State
                .Subscribe(OnBoardStateChanged)
                .AddTo(this);
        }

        private void OnBoardStateChanged(BoardState state)
        {
            for (int y = 0; y < _layout.Height; y++)
            for (int x = 0; x < _layout.Width; x++)
                _cellViews[x, y].Render(state.Cells[x, y]);
        }

        private void SpawnGrid(GameConfig config)
        {
            ClearGrid();
            _cellViews = new GridCellView[config.Width, config.Height];

            for (int y = 0; y < config.Height; y++)
            for (int x = 0; x < config.Width; x++)
            {_cellViews[x, y] = Instantiate(
                    _cellViewPrefab,
                    _layout.CellToWorld(x, y),
                    Quaternion.identity,
                    transform
                );
                _cellViews[x, y].transform.localScale = Vector3.one * _layout.CellSize;
            }
        }

        private void ClearGrid()
        {
            if (_cellViews == null) return;
            foreach (var view in _cellViews)
                if (view != null)
                    Destroy(view.gameObject);
            _cellViews = null;
        }

        private void OnDestroy() => _subscription?.Dispose();
    }
}
using App.Domain;
using TMPro;
using UnityEngine;

namespace App.Presentation.Board
{
    public class GridCellView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _background;
        [SerializeField] private TextMeshPro _label;

        [SerializeField] private Sprite _spriteHidden;
        [SerializeField] private Sprite _spriteRevealed;
        [SerializeField] private Sprite _spriteMine;
        [SerializeField] private Sprite _spriteFlag;
    
        private static readonly Color[] NumberColors =
        {
            Color.clear,
            new Color(0f, 0f, 1f),
            new Color(0f, 0.5f, 0f),
            new Color(1f, 0f, 0f),
            new Color(0f, 0f, 0.5f),
            new Color(0.5f, 0f, 0f),
            new Color(0f, 0.5f, 0.5f),
            new Color(0f, 0f, 0f),
            new Color(0.5f, 0.5f, 0.5f),
        };

        public void Render(Cell cell)
        {
            switch (cell.State)
            {
                case CellState.Hidden:
                    _background.sprite = _spriteHidden;
                    _background.color = Color.gray6;
                    _label.gameObject.SetActive(false);
                    break;

                case CellState.Flagged:
                    _background.sprite = _spriteFlag;
                    _background.color = Color.burlywood;
                    _label.gameObject.SetActive(false);
                    break;

                case CellState.Revealed when cell.IsMine:
                    _background.sprite = _spriteMine;
                    _background.color = Color.saddleBrown;
                    _label.gameObject.SetActive(false);
                    break;

                case CellState.Revealed:
                    _background.sprite = _spriteRevealed;
                    _background.color = Color.gray8;
                    bool hasNumber = cell.Adjacent > 0;
                    _label.gameObject.SetActive(hasNumber);
                    if (hasNumber)
                    {
                        _label.text = cell.Adjacent.ToString();
                        _label.color = NumberColors[cell.Adjacent];
                    }
                    break;
            }
        }
    }
}
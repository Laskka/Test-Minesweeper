using App.Domain;
using App.Presentation.Board;
using UnityEngine;
using VContainer;

namespace App.Presentation
{
    public class CameraFitter : MonoBehaviour
    {
        [SerializeField] private float _cameraPadding = 1f;

        private GameConfig _config;
        private Camera _camera;
        private BoardLayout _layout;

        [Inject]
        public void Construct(GameConfig config, Camera camera, BoardLayout layout)
        {
            _config = config;
            _camera = camera;
            _layout = layout;
        }

        private void Start()
        {
            FitCamera(_camera, _config);
        }

        private void FitCamera(Camera cam, GameConfig config)
        {
            float vertical = (config.Height * _layout.CellSize + _cameraPadding) / 2f;
            float horizontal = (config.Width * _layout.CellSize + _cameraPadding) / 2f / cam.aspect;
            cam.orthographicSize = Mathf.Max(vertical, horizontal);
        }
    }
}
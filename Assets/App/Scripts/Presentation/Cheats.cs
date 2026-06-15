using App.Application;
using UnityEngine;
using VContainer;

namespace App.Presentation
{
    public sealed class Cheats : MonoBehaviour
    {
        private GameFlow _flow;

        [Inject]
        public void Construct(GameFlow flow) => _flow = flow;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                _flow.Restart();
        }
    }
}
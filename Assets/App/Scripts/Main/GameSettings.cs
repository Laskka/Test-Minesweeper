using App.Domain;
using UnityEngine;

namespace App.Main
{
    [CreateAssetMenu(menuName = "App/Create GameSettings", fileName = "GameSettings", order = 0)]
    public sealed class GameSettings : ScriptableObject
    {
        [field: SerializeField] public int Width { get; private set; }
        [field: SerializeField] public int Height { get; private set; }
        [field: SerializeField] public int MineCount{ get; private set; }
        
        public GameConfig Config => new GameConfig(Width, Height, MineCount);
    }
}

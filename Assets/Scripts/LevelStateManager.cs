using QFramework;
using UnityEngine;

namespace DefaultNamespace
{
    public class LevelStateManager : MonoSingleton<LevelStateManager>
    {
        public Transform respawnPoint;
        public GameObject character;
    }
}
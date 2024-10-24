using UnityEngine;

namespace GAME
{
    public class PlayerSystem : MonoBehaviour
    {
        private static PlayerSystem Instance;

        private static void CheckInstance()
        {
            if (Instance == null)
                Instance = FindAnyObjectByType<PlayerSystem>();
        }

        public static PlayerSystemSettings Settings
        {
            get
            {
                CheckInstance();
                return Instance.settings ?? (Instance.settings = new PlayerSystemSettings());
            }
        }

        public static PlayerSystemData Data
        {
            get
            {
                CheckInstance();
                return Instance.data ?? (Instance.data = new PlayerSystemData());
            }
        }

        public static PlayerSystemEvents Events
        {
            get
            {
                CheckInstance();
                return Instance.events ?? (Instance.events = new PlayerSystemEvents());
            }
        }

        [SerializeField] private PlayerSystemSettings settings;
        [SerializeField] private PlayerSystemData data;
        private PlayerSystemEvents events;

    }
}


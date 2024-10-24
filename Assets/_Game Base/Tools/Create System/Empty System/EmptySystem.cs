using UnityEngine;

namespace GAME
{
    public class EmptySystem : MonoBehaviour
    {
        private static EmptySystem Instance;

        private static void CheckInstance()
        {
            if (Instance == null)
                Instance = FindAnyObjectByType<EmptySystem>();
        }

        public static EmptySystemSettings Settings
        {
            get
            {
                CheckInstance();
                return Instance.settings ?? (Instance.settings = new EmptySystemSettings());
            }
        }

        public static EmptySystemData Data
        {
            get
            {
                CheckInstance();
                return Instance.data ?? (Instance.data = new EmptySystemData());
            }
        }

        public static EmptySystemEvents Events
        {
            get
            {
                CheckInstance();
                return Instance.events ?? (Instance.events = new EmptySystemEvents());
            }
        }

        [SerializeField] private EmptySystemSettings settings;
        [SerializeField] private EmptySystemData data;
        private EmptySystemEvents events;

    }
}

using UnityEngine;

namespace ExamineSystem
{
    public class ExamineInputManager : MonoBehaviour
    {
        [Header("Raycast Pickup Input")]
        public KeyCode interactKey;

        [Header("Rotation Key Inputs")]
        public KeyCode rotateKey;
        public KeyCode dropKey;

        [Header("Should persist?")]
        [SerializeField] private bool persistAcrossScenes = true;

        public static ExamineInputManager instance;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                if (persistAcrossScenes)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
        }
    }
}

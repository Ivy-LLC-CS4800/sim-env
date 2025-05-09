using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;

namespace ExamineSystem
{
    public class ExamineDisableManager : MonoBehaviour
    {
        [SerializeField] private ExamineInteractor interactorScript = null;
        [SerializeField] private FirstPersonController player = null;
        [SerializeField] private BlurOptimized blur = null;

        [Header("Should persist?")]
        [SerializeField] private bool persistAcrossScenes = true;

        public static ExamineDisableManager instance;

        void Awake()
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

        public void DisablePlayer(bool disable)
        {
            if (disable)
            {
                if(player != null)
                {
                    player.enabled = false;
                }
                else
                {
                    print("Disable Manager: You will need to add the included player character here but if you have your own, you will need to change the reference");
                }

                interactorScript.enabled = false;

                blur.enabled = true;
                ExamineUIManager.instance.EnableCrosshair(false);

            }
            else
            {
                if (player != null) 
                {
                    player.enabled = true;
                }
                else 
                {
                    print("Disable Manager: You will need to add the included player character here but if you have your own, you will need to change the reference");
                }

                interactorScript.enabled = true;

                blur.enabled = false;
                ExamineUIManager.instance.EnableCrosshair(true);
            }
        }
    }
}

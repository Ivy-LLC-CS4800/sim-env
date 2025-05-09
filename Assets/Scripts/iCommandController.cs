using UnityEngine;
using UnityEngine.UI;

public class iCommandController : MonoBehaviour {
    public GameObject[] allTaskCards; // Array of all task cards in the left blue panel
    public GameObject[] inProgressTaskCards; // Array of task cards in the middle grey panel
    public GameObject[] completedTaskCards; // Array of task cards in the yellow panel

    void Start() {
        // Initially set all task cards to be visible in both left blue and middle grey panels,
        // and hidden in yellow completed tasks panel.
        foreach (GameObject card in allTaskCards) {
            card.SetActive(true);
        }//end foreach
        
        foreach (GameObject card in inProgressTaskCards) {
            card.SetActive(true);
        }//end foreach

        foreach (GameObject card in completedTaskCards) {
            card.SetActive(false);
        }//end foreach
    }//end Start()

    public void CompleteDebris(string debrisType) {
        bool firstCardHidden = false;

        for (int i = 0; i < inProgressTaskCards.Length; i++) {
            if (inProgressTaskCards[i].name.Contains(debrisType) && !firstCardHidden) {
                if (!inProgressTaskCards[i].activeSelf) continue;

                inProgressTaskCards[i].SetActive(false);
                firstCardHidden = true;
                
                for (int j = 0; j < completedTaskCards.Length; j++) {
                    if (completedTaskCards[j].name == inProgressTaskCards[i].name) {
                        completedTaskCards[j].SetActive(true);
                        break;
                    }//end if
                }//end for-loop
                break;
            }//end if
        }//end for-loop
    }//end CompleteDebris()
}//end iCommandController

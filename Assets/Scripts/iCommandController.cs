using UnityEngine;
using UnityEngine.UI;

public class iCommandController : MonoBehaviour {
    public GameObject[] allTaskCards; // Array of all task cards in the left blue panel
    public GameObject[] inProgressTaskCards; // Array of task cards in the middle grey panel
    public GameObject[] completedTaskCards; // Array of task cards in the yellow panel
    private Dictionary<string, int> debrisTypeCount; // Dictionary to keep track of the number of times each debris type has been trashed

    void Start() {
       
// Initialize the dictionary
        debrisTypeCount = new Dictionary<string, int>();

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
        // Increment the count for the debris type
        if (!debrisTypeCount.ContainsKey(debrisType)) {
            debrisTypeCount[debrisType] = 0;
        }//end if
        debrisTypeCount[debrisType]++;

        // Determine the index of the task card to hide and show
        int taskIndex = debrisTypeCount[debrisType] - 1;

        // Hide the task card in the in-progress panel and show it in the completed panel
        for (int i = 0; i < inProgressTaskCards.Length; i++) {
            if (inProgressTaskCards[i].name.Contains(debrisType) && i == taskIndex) {
                inProgressTaskCards[i].SetActive(false);
                
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

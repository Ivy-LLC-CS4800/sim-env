
/* This script is written by Shubham Vine of Eniv Studios. 
   Item Interaction Kit :- V1.1.3
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnivStudio
{
    // This script handles interacting with box and taking syringe out of it
    public class CustomFunction : MonoBehaviour
    {
        [SerializeField] GameObject syringe;
        Animator anim;
        bool boxOpen = false;
        private void Start()
        {
            anim = GetComponent<Animator>();
        }
        
        public void ButtonClicked()
        {
            anim.enabled = true;
            BoxOpen();
        }
        void BoxOpen()
        {
            if (!boxOpen) { anim.Play("BoxOpen");boxOpen = true; }
            else { anim.Play("BoxClose"); boxOpen = false; }
        }
        public void Destroy()
        {
            if (boxOpen)
            {
                Debug.Log("Syringe Picked");
                Destroy(syringe);
            }
        }
    }
}

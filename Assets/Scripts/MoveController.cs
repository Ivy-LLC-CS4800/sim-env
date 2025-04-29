using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    Animator animator;

    int isWalkingHash;
    int isRunningHash;
    int isWalkingLeftHash;
    int isWalkingRightHash;
    int isWalkingBackwardsHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isWalkingLeftHash = Animator.StringToHash("isWalkingLeft");
        isWalkingRightHash = Animator.StringToHash("isWalkingRight");
        isWalkingBackwardsHash = Animator.StringToHash("isWalkingBackwards");
    }

    void Update()
    {

        #region Movement
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        bool isWalkingLeft = animator.GetBool(isWalkingLeftHash);
        bool isWalkingRight = animator.GetBool(isWalkingRightHash);
        bool isWalkingBackwards = animator.GetBool(isWalkingBackwardsHash);

        bool forwardPressed = Input.GetKey("w");
        bool runPressed = Input.GetKey("left shift");
        bool leftPressed = Input.GetKey("a");
        bool rightPressed = Input.GetKey("d");
        bool backwardsPressed = Input.GetKey("s");

        // walking forward
        if (!isWalking && forwardPressed)
        {
            animator.SetBool(isWalkingHash, true);
        }
        if (isWalking && !forwardPressed)
        {
            animator.SetBool(isWalkingHash, false);
        }
        // walking left
        if (!isWalking && leftPressed)
        {
            animator.SetBool(isWalkingHash, true);
            animator.SetBool(isWalkingLeftHash, true);
        }
        if (isWalking && !leftPressed)
        {
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isWalkingLeftHash, false);
        }
        // walking right
        if (!isWalking && rightPressed)
        {
            animator.SetBool(isWalkingHash, true);
            animator.SetBool(isWalkingRightHash, true);
        }
        if (isWalking && !rightPressed)
        {
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isWalkingRightHash, false);
        }
        // walking backwards
        if (!isWalking && backwardsPressed)
        {
            animator.SetBool(isWalkingHash, true);
            animator.SetBool(isWalkingBackwardsHash, true);
        }
        if (isWalking && !backwardsPressed)
        {
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isWalkingBackwardsHash, false);
        }


        // running
        if (!isRunning && (forwardPressed && runPressed) )
        {
            animator.SetBool(isRunningHash, true);
        }
        if (isRunning && (!forwardPressed || !runPressed) )
        {
            animator.SetBool(isRunningHash, false);
        }

        #endregion
    }
}

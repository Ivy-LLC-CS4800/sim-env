
/* This script is written by Shubham Vine of Eniv Studios. 
   Item Interaction Kit :- V1.1.3
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnivStudios
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Player Movement")]
        [SerializeField] float movementSpeed;
        [SerializeField] [Range(0,1)] float smoothTime = 0.08f;

        [Header("Mouse Look")]
        [SerializeField] float mouseLookSensitivity;
        [SerializeField] [Range(0, 1)] float lookSmoothTime = 0.01f;

        [Header("Jump Settings")]
        [SerializeField] float jumpSpeed;
        [SerializeField] float gravity = -13;
        [SerializeField] AnimationCurve jumpFall;

        //Private Variables
        CharacterController characterController;
        float cameraPitch = 0;
        float velocityY = 0;
        Camera _cam;
        bool isJumping;
        Vector2 currentDir = Vector2.zero;
        Vector2 currentDirVelocity = Vector2.zero;
        Vector2 currentMouseDelta = Vector2.zero;
        Vector2 currentMouseDeltaVelocity = Vector2.zero;
        private void Awake()
        {
            _cam = GetComponentInChildren<Camera>();
            characterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
        }
        void Update()
        {
            Move();
            CameraRotation();
        }
        private void Move()
        {
            Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            targetDir.Normalize();

            currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity,smoothTime);
            if (characterController.isGrounded) { velocityY = 0; }
            velocityY += gravity * Time.deltaTime;

            Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * movementSpeed + Vector3.up * velocityY;
            characterController.Move(velocity * Time.deltaTime);

            Jump();
        }
        void CameraRotation()
        {
            if (Time.timeScale != 0)
            {
                Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, lookSmoothTime);
                cameraPitch -= currentMouseDelta.y * mouseLookSensitivity;
                cameraPitch = Mathf.Clamp(cameraPitch, -90, 90);
                _cam.transform.localEulerAngles = Vector3.right * cameraPitch;
                transform.Rotate(Vector3.up * currentMouseDelta.x * mouseLookSensitivity);
            }
        }
        void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
            {
                isJumping = true;
                StartCoroutine(JumpEvent());
            }
        }
        IEnumerator JumpEvent()
        {
            characterController.slopeLimit = 90;
            float timeInAir = 0;
            do
            {
                float jumpForce = jumpFall.Evaluate(timeInAir);
                characterController.Move(Vector3.up * jumpForce * jumpSpeed * Time.deltaTime);
                timeInAir += Time.deltaTime;
                yield return null;
            } while (!characterController.isGrounded && characterController.collisionFlags != CollisionFlags.Above);
            characterController.slopeLimit = 45;
            isJumping = false;
        }
    }
}

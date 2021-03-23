using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loyality
{
    public class Player : Character
    {
        [Header("Player")]
        [Tooltip("The speed of the character")]
        public float Speed;

        [SyncVar]
        [Tooltip("The metal of the character")]
        public float Metal;

        [Tooltip("The jump height of the character")]
        public float JumpHeight;

        [Tooltip("Applied Gravity to the character")]
        public float Gravity;

        [Header("Backend Of Player")]
        [Tooltip("The Velocity of the character")]
        private Vector3 Velocity;

        [Tooltip("The player's camera")]
        public GameObject Camera;

        [Tooltip("The character's character controller")]
        public CharacterController characterController;

        [Header("Ground Detection")]
        [Tooltip("This is for checking to see if the player is on the ground")]
        public Collider GroundCheckPoint;

        [Tooltip("The layer mask for checking to see if character is on the ground")]
        public LayerMask GroundMask;

        [Tooltip("The layer mask for checking to see if the player can jump")]
        public LayerMask JumpMask;

        [Tooltip("Is the character grounded?")]
        private bool IsGrounded;

        [Tooltip("Is the character on a surface it can jump on?")]
        private bool IsJumpable;

        [Header("Player environment interation")]
        [Tooltip("The maxmium range a player can click a tower or place a tower")]
        public float MaxInteractionRange = 5;

        [Tooltip("The layermask for interacting")]
        public LayerMask InteractMask;
   
        private new void Start()
        {
            SetupCamera();
            if (isServer)
            {
                base.Start();
            }

            if (isLocalPlayer)
            {
  
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

            }
        }

        private void SetupCamera()
        {
            if (isLocalPlayer)
            {
                Camera.SetActive(true);
            } else
            {
                Camera.SetActive(false);
            }
        }

        private void Update()
        {
            if (hasAuthority)
            {
                CheckForGrounded();
                CheckForJumpSurface();
                Move();
                ApplyGravity();

                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    RotateBody();
                    RotateCamera();
                }

                Attack();
            }
        }

        [Command(ignoreAuthority = true)]
        public void SetMetal(float value)
        {
            Metal = value;
        }

        public float GetMetal()
        {
            return Metal;
        }

        private void CheckForGrounded()
        {
            IsGrounded = Physics.CheckBox(GroundCheckPoint.bounds.center, GroundCheckPoint.bounds.extents, transform.rotation, GroundMask);
        }

        private void CheckForJumpSurface()
        {
            IsJumpable = Physics.CheckBox(GroundCheckPoint.bounds.center, GroundCheckPoint.bounds.extents, transform.rotation, JumpMask);
        }

        private void Move()
        {
            Vector3 direction = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
            characterController.Move(direction * Speed * Time.deltaTime);

            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }

        }

        private void ApplyGravity()
        {
            if (!(IsGrounded && Velocity.y < 0))
            {
                Velocity.y += Gravity * Time.deltaTime;
                characterController.Move(Velocity * Time.deltaTime);
            } else
            {
                Velocity.y = -2f;
                characterController.Move(Velocity * Time.deltaTime);
            }
        }

        private void RotateBody()
        {
            transform.Rotate(Vector3.up * Input.GetAxis("Mouse X"));
        }

        private void RotateCamera()
        {
            float xRotation = Camera.transform.localRotation.eulerAngles.x;

            xRotation -= Input.GetAxis("Mouse Y");

            Camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        private void Jump()
        {
            if (IsJumpable)
            {
                Velocity.y = Mathf.Sqrt(JumpHeight * -2 * Gravity); //TODO: Store this into a variable because this is computationally costly
            }
        }

        private void Attack()
        {

        }

        protected override void OnObjectDestroy() 
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}



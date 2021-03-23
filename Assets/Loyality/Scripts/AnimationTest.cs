using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loyality
{
    public class AnimationTest : NetworkBehaviour
    {

        public Animator animator;

        // Update is called once per frame
        void Update()
        {
            if (hasAuthority)
            {
                animator.SetFloat("Forward", Input.GetAxis("Horizontal"));
                animator.SetFloat("Sideways", Input.GetAxis("Vertical"));
                
            }
        }
    }
}



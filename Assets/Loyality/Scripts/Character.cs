using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loyality
{
    public class Character : Object
    {
        [Header("Character Properties")]
        [Tooltip("The max health of this character")]
        public float MaxHealth;

        [SyncVar]
        [Tooltip("The current health of this character")]
        private float Health;


        protected void Start()
        {
            if (isServer)
            {
                if (NetworkServer.connections.Count > 1)
                {
                    SetHealth(MaxHealth);
                } else
                {
                    Health = MaxHealth;
                }
            }

        }

        [Command(ignoreAuthority = true)]
        public void SetHealth(float health)
        {
            Health = health;

            if (Health <= 0)
            {
                CharacterDied();
            }
        }

        public float GetHealth()
        {
            return Health;
        }

        protected virtual void CharacterDied() 
        {
            ServerDestroy();
        }
    }
}


using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// NEEDS TO HAVE A RIGID BODY (No gravity Kinematic), PLUS NETWORK TRANSFORM
/// </summary>

namespace Loyality
{
    public class Projectile : Object
    {
        [Header("Projectile")]
        [Tooltip("Speed of projectile")]
        public float Speed;

        [Tooltip("Damage of projectile")]
        public float Damage;

        [Tooltip("The Range of the projectile")]
        public float MaxRange;

        [Tooltip("The tag's this projectile will look for when trying to damage")]
        public string DamageTag;

        [Tooltip("The starting location of the projectile")]
        public Vector3 startPosition;

        [Tooltip("What the arrow can interact with in order to destory itself")]
        public LayerMask HitMask;

        private void Start()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            if (isServer)
            {
                CheckDistance();
                Move();
            }
        }

        public void SetAttackTag(string tag)
        {
            DamageTag = tag;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isServer && other != null)
            {
                if (other.transform.parent != null)
                {
                    other.transform.parent.gameObject.TryGetComponent(out Character temp);

                    if (other.tag == DamageTag)
                    {
                        if (temp != null)
                        {
                            temp.SetHealth(temp.GetHealth() - Damage);
                            ServerDestroy();
                        }

                    }


                    if (HitMask == (HitMask | (1 << other.gameObject.layer)))
                    {
                        ServerDestroy();
                    }
                }
            }
        }

        private void CheckDistance()
        {
            if (Vector3.Distance(transform.position, startPosition) > MaxRange)
            {
                ServerDestroy();
            }

        }

        protected virtual void Move() {}

    }
}



using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Loyality
{
    public class Tower : Character
    {
        [Header("Tower Properties")]
        [Tooltip("How far this tower can see")]
        public float Range;

        [Tooltip("The Rate Of Fire")]
        public float RateOfFire = Mathf.Infinity;

        /*        [Tooltip("The angle that the tower can see")]
                public float VisionDegree;*/

        [Tooltip("The cost of this tower")]
        public float Cost;

        [Tooltip("The tag's this tower will look for when targeting")]
        public string AttackTag;

        [Tooltip("The list of characters it can see")]
        private List<GameObject> characters = new List<GameObject>();

        [Tooltip("For when enemies enter or exit the range of the tower")]
        public SphereCollider sphereCollider;

        [Tooltip("Layermask to see what the tower can see through")]
        public LayerMask VisionMask;

        [Tooltip("The metal object to spawn once the tower is destroyed")]
        public GameObject Metal;

        [Tooltip("Keeps track if the tower is firing or not")]
        private bool IsAttacking = false;

        [Tooltip("The health gui")]
        public TextMeshProUGUI Health;

        private void OnTriggerEnter(Collider other)
        {
            if (isServer)
            {
                if (other.tag == AttackTag)
                {
                    characters.Add(other.gameObject);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (isServer)
            {
                if (other.tag == AttackTag)
                {
                    characters.Remove(other.gameObject);
                }
            }
        }

        protected void Update()
        {
            if (isServer)
            {
                if (RateOfFire != Mathf.Infinity && characters.Count != 0 && !IsAttacking)
                {
                    IsAttacking = true;
                    StartCoroutine("Attack");
                }
            }

            if (isClient)
            {
                Health.text = GetHealth() + "/" + MaxHealth;
            }
        }

        /// <summary>
        /// Helper function to call the LaunchAttack.
        /// </summary>
        private IEnumerator Attack()
        {
            GameObject target = GetClosestAimableTarget();

            if (target != null)
            {
                LaunchAttack(target);
            }

            yield return new WaitForSeconds(RateOfFire);
            IsAttacking = false;
        }

        protected virtual void LaunchAttack(GameObject target) {} 

        protected GameObject GetClosestAimableTarget()
        {
            float closestRange = Mathf.Infinity;
            GameObject closest = null;

            for (int i = 0; i < characters.Count; i++)
            {
                GameObject character = characters[i];

                if (character == null)
                {
                    characters.RemoveAt(i);
                    continue;
                }

                float distance = Vector3.Distance(transform.position, character.transform.position);
                if (distance < closestRange)
                {
                    if (!Physics.Linecast(transform.position, character.transform.position, VisionMask))
                    {
                        closestRange = distance;
                        closest = character;
                    }
                }
            }

            return closest;
        }

        protected override void CharacterDied()
        {
            GameObject newObject = Instantiate(Metal);
            newObject.transform.position = SpawnOffset.position;

            NetworkServer.Spawn(newObject);

            ServerDestroy();
        }
    }
}



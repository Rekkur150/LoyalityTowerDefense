using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

namespace Loyality
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : Character
    {
        [Header("Enemy")]
        [Tooltip("The nav mesh for the enemy")]
        private NavMeshAgent Agent;

        [Tooltip("The list of characters it can see")]
        private List<GameObject> characters = new List<GameObject>();

        [Tooltip("The position that the enemy ultimately wants to be")]
        private Vector3 finalPosition;

        [Tooltip("The tag's this enemy will look for when targeting")]
        public string AttackTag;

        [Tooltip("Layermask to see what the tower can see through")]
        public LayerMask VisionMask;

        [Tooltip("The current enemy target")]
        private GameObject Target;

        [Tooltip("Attack Range")]
        public float AttackRange;

        [Tooltip("Attack Rate")]
        public float AttackRate;

        [Tooltip("Is the Enemy Attacking")]
        private bool IsAttacking = false;

        [Tooltip("The metal object to spawn once the enemy dies")]
        public GameObject Metal;

        [Tooltip("The health gui")]
        public TextMeshProUGUI Health;

        // Start is called before the first frame update
        private new void Start()
        {
            if (isServer)
            {
                base.Start();
                Agent = GetComponent<NavMeshAgent>();

                finalPosition = MapController.singleton.Crystal.transform.position;

                Agent.SetDestination(finalPosition);
            }
        }

        private void Update()
        {
            if (isServer)
            {
                UpdateNavAgent();
                UpdateAttackTarget();
            }

            if (isClient)
            {
                Health.text = GetHealth() + "/" + MaxHealth;
            }
        }

        private void UpdateAttackTarget()
        {
            if (Target != null)
            {
                float distance = Vector3.Distance(transform.position, Target.transform.position);
                if (distance < AttackRange)
                {
                    if (!IsAttacking)
                    {
                        IsAttacking = true;
                        StartCoroutine("Attack");
                    }
                }
            }
        }

        private IEnumerator Attack()
        {
            LaunchAttack(Target);
            yield return new WaitForSeconds(AttackRate);
            IsAttacking = false;
        }

        private void UpdateNavAgent()
        {
            if (characters.Count <= 0)
            {
                if (Agent.destination != finalPosition)
                {
                    Agent.SetDestination(finalPosition);
                }
            }
            else
            {
                if (Target == null)
                {
                    Target = GetClosestAimableTarget();
                }
                else
                {
                    GameObject newTarget = GetClosestAimableTarget();
                    if (Target != newTarget)
                    {
                        Target = newTarget;
                    }


                    if (Agent.destination != Target.transform.position)
                    {
                        Agent.SetDestination(Target.transform.position);
                    }

                    if (Agent.hasPath && Agent.pathStatus == NavMeshPathStatus.PathPartial)
                    {
                        Debug.Log(Target.name);
                        characters.Remove(Target);
                        Target = null;
                    }
                }
            }

        }

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

        protected virtual void LaunchAttack(GameObject target) { }

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


        protected override void OnObjectDestroy()
        {
            GameObject newObject = Instantiate(Metal);
            newObject.transform.position = SpawnOffset.position;

            NetworkServer.Spawn(newObject);
        }
    }
}



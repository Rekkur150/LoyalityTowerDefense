using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loyality
{

    /// <summary>
    /// Requires Sphereical Collider
    /// </summary>
    public class AreaOfEffect : Object
    {
        [Header("AreaOfEffect")]
        [Tooltip("The Range of the AreaOfEffect")]
        public float Range;

        [Tooltip("The list of characters that are in range")]
        protected List<GameObject> characters = new List<GameObject>();

        [Tooltip("The tag's this projectile will look for when trying to damage")]
        public string DamageTag;

        private void OnTriggerEnter(Collider other)
        {
            if (isServer)
            {
                if (other.tag == DamageTag)
                {
                    characters.Add(other.gameObject);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (isServer)
            {
                if (other.tag == DamageTag)
                {
                    characters.Remove(other.gameObject);
                }
            }
        }

        public void SetAttackTag(string tag)
        {
            DamageTag = tag;
        }

        public void ApplyAreaOfEffect() {
            foreach (GameObject character in characters)
            {
                Effect(character);
            }
            ApplyedAreaOfEffect();
        }

        protected virtual void Effect(GameObject character) {}

        protected virtual void ApplyedAreaOfEffect() {}
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loyality
{
    public class GhostObject : Object
    {
        [Header("Ghost Object")]

        [Tooltip("The collider we will use to get the bounds")]
        public new Collider collider;

        [Tooltip("The layer mask when checking for collisions")]
        public LayerMask ColliderMask;

        [Tooltip("The layer mask for when checking for the floor")]
        public LayerMask GroundMask;

        [Tooltip("The material to change all children to")]
        private Material toChangeMaterial;

        [Tooltip("The current material")]
        private Material CurrentMaterial;

        public bool IsColliding()
        {
            return Physics.CheckBox(collider.bounds.center, collider.bounds.extents, transform.rotation, ColliderMask);
        }

        public bool IsOnGround()
        {
            return Physics.CheckSphere(SpawnOffset.position, 0.1f, GroundMask);
        }

        public void ChangeMaterial(Material newMaterial)
        {
            if (newMaterial != CurrentMaterial)
            {
                toChangeMaterial = newMaterial;
                CurrentMaterial = newMaterial;
                ChangeMaterialRecursive(gameObject);
            }
        }

        private void ChangeMaterialRecursive(GameObject obj)
        {
            if (obj == null) return;

            MeshRenderer temp = obj.GetComponent<MeshRenderer>();

            if (temp != null) temp.material = toChangeMaterial;

            foreach (Transform child in obj.transform)
            {
                if (child == null) continue;

                ChangeMaterialRecursive(child.gameObject);
            }
        } 

    }

}


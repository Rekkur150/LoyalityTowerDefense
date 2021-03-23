using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loyality
{
    public class Object : NetworkBehaviour
    {
        [Header("Object")]
        [Tooltip("How to identity different prefabs")]
        public string UnqiueName;

        [Tooltip("This point will adjust placement, so if spawned wouldn't spawn in the floor")]
        public Transform SpawnOffset;

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public Quaternion GetRotation()
        {
            return transform.rotation;
        }

        public void OffsetPosition()
        {
            if (SpawnOffset != null)
            {
                transform.position -= SpawnOffset.localPosition;
            }
        }

        public void ServerDestroy()
        {
            OnObjectDestroy();
            NetworkServer.Destroy(gameObject);
        }

        protected virtual void OnObjectDestroy() {}
    }
}


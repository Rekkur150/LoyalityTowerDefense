using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loyality
{
    public class MeleeAttacker : Enemy
    {
        [Header("MeleeAttacker")]
        [Tooltip("The AOE prefab to do damage")]
        public GameObject AOEPrefab;

        [Tooltip("The spawn location of the AOE")]
        public Transform AOESpawnLocation;


        protected override void LaunchAttack(GameObject target) 
        {
            GameObject newObject = Instantiate(AOEPrefab);
            newObject.transform.position = AOESpawnLocation.position;

            ArrowAOE temp = newObject.GetComponent<ArrowAOE>();
            temp.SetAttackTag(AttackTag);

            NetworkServer.Spawn(newObject);
        }
    }
}


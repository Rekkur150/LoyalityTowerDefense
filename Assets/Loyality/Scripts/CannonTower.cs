using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loyality
{
    public class CannonTower : LookingTower
    {
        [Header("Cannon Tower Properties")]
        [Tooltip("The projectile this tower will spawn")]
        public GameObject projectile;

        protected override void LaunchAttack(GameObject target)
        {

            GameObject newArrow = Instantiate(projectile);
            newArrow.transform.position = SpawnPosition.position;
            newArrow.transform.LookAt(target.transform);

            CannonProjectile temp = newArrow.GetComponent<CannonProjectile>();
            temp.SetAttackTag(AttackTag);

            NetworkServer.Spawn(newArrow);
        }
    }
}




using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loyality
{
    public class CannonProjectile : Projectile
    {
        [Header("Cannon Projectile")]
        public GameObject AOEPrefabTest;

        protected override void Move()
        {
            transform.position += transform.forward * Speed * Time.deltaTime;
        }

        protected override void OnObjectDestroy()
        {
            GameObject newArrow = Instantiate(AOEPrefabTest);
            newArrow.transform.position = transform.position;

            ArrowAOE temp = newArrow.GetComponent<ArrowAOE>();
            temp.SetAttackTag(DamageTag);

            NetworkServer.Spawn(newArrow);

        }
    }
}
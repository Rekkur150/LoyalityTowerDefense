using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loyality
{
    public class LookingTower : Tower
    {
        [Header("Look Tower")]
        [Tooltip("Where the rotation will happen horizontaly")]
        public GameObject HorizontalTurret;

        [Tooltip("Where the rotation will happen looking up and down")]
        public GameObject VerticalTurret;

        [Tooltip("Where the projectiles will spawn from ")]
        public Transform SpawnPosition;

        private new void Update()
        {
            base.Update();
            GameObject temp = GetClosestAimableTarget();
            if (temp != null)
            {
                if (HorizontalTurret == VerticalTurret)
                {
                    HorizontalTurret.transform.LookAt(temp.transform.position);
                } else
                {
                    Vector3 modifiedForHorizontal = temp.transform.position;
                    modifiedForHorizontal.y = HorizontalTurret.transform.position.y;
                    HorizontalTurret.transform.LookAt(modifiedForHorizontal);

                    VerticalTurret.transform.LookAt(temp.transform.position);
                }

            }
        }
    }
}



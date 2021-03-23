using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loyality 
{

    public class SummonObject : NetworkBehaviour
    {

        public GameObject enemyPrefab;
        public Transform spawnEnemy;

        // Update is called once per frame
        void Update()
        {
            if (isServer)
            {
                if (Input.GetKeyDown("b"))
                {
                    GameObject newArrow = Instantiate(enemyPrefab);
                    newArrow.transform.position = spawnEnemy.position;

                    NetworkServer.Spawn(newArrow);
                }
            }
        }
    }
}



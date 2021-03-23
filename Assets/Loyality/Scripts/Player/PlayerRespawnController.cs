using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loyality
{
    public class PlayerRespawnController : NetworkBehaviour
    {
        public GameObject Player;

        private float RespawnTimer = 10f;

        public static PlayerRespawnController singleton;

        void Start()
        {
            if (singleton == null)
            {
                singleton = this;
            }
            else if (singleton != this)
            {
                Destroy(this);
            }
        }

        public void Died()
        {
            Debug.Log("respawning");
            DespawnPlayer(Player);
            StartCoroutine("Respawn");
        }

        [Command(ignoreAuthority = true)]
        private void DespawnPlayer(GameObject player)
        {
            NetworkServer.UnSpawn(player);
        }

        private IEnumerator Respawn()
        {
            yield return new WaitForSeconds(RespawnTimer);
            Debug.Log("Respawning");
            RespawnPlayer(Player);
            
        }

        [Command(ignoreAuthority = true)]
        private void RespawnPlayer(GameObject player)
        {
            NetworkServer.Spawn(player);
        }
    }
}



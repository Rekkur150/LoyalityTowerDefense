using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Loyality
{
    public class Wave : MonoBehaviour
    {
        [Header("Wave")]
        [Tooltip("The information for this wave")]
        public List<WaveSpawningStructure> WaveSpawningStructures;

        [Tooltip("The list of all enemies that this wave spawned")]
        private List<GameObject> spawnedEnemies = new List<GameObject>();

        [Tooltip("Is the wave finished")]
        private bool WaveFinished = false;

        public void StartWave()
        {
            WaveFinished = false;
            StartCoroutine("WaveHelper");
        }

        private IEnumerator WaveHelper()
        {
            foreach (WaveSpawningStructure info in WaveSpawningStructures)
            {
                SpawnEnemy(info.enemy, info.spawnLocation);
                yield return new WaitForSeconds(info.waitBeforeSpawn);
            }

            CheckSpawningFinished();
        }

        private void CheckSpawningFinished()
        {
            StartCoroutine("CheckWaveDead");

        }

        private IEnumerator CheckWaveDead()
        {
            for(; ; )
            {
                for (int i = 0; i < spawnedEnemies.Count; i++)
                {
                    if (spawnedEnemies[i] == null)
                    {
                        spawnedEnemies.RemoveAt(i);
                    }
                }

                yield return new WaitForSeconds(1f);

                if (spawnedEnemies.Count <= 0)
                {
                    Debug.Log("Wave Finished!");
                    WaveFinished = true;
                    break;
                }

            }
        }

        public bool IsWaveFinished()
        {
            return WaveFinished;
        }

        public void SpawnEnemy(GameObject enemyPrefab, Transform position)
        {
            GameObject newObject = Instantiate(enemyPrefab);

            newObject.TryGetComponent(out NavMeshAgent navMesh);
            navMesh.Warp(position.position);

            spawnedEnemies.Add(newObject);
            NetworkServer.Spawn(newObject);

        }

    }
}



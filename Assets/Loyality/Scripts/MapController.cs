using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Loyality
{
    public class MapController : NetworkBehaviour
    {
        [Header("Map Controller")]
        [Tooltip("The crystal for the map")]
        public Character Crystal;

        [Tooltip("The wave information")]
        public List<Wave> Waves;

        [Tooltip("The singleton of this mapcontroller")]
        public static MapController singleton;

        [Tooltip("The name of the scene that will be loaded after this map is completed")]
        public string NextMapName;

        [SyncVar]
        [Tooltip("The current wave")]
        private int CurrentWave = 0;

        [SyncVar]
        [Tooltip("Is the current wave finished")]
        private bool CurrentWaveFinished = true;

        [Tooltip("The number of player's ready for the next wave")]
        private List<int> PlayersReady = new List<int>();

        [SyncVar]
        [Tooltip("The number of ready players")]
        private int NumberOfReadyPlayers = 0;

        [SyncVar]
        [Tooltip("The number of players connected")]
        private int NumberOfPlayersConnected;

        public void Start()
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

        private void Update()
        {
            if (isServer)
            {
                if (NumberOfPlayersConnected != NetworkServer.connections.Count)
                {
                    NumberOfPlayersConnected = NetworkServer.connections.Count;
                }
                if (Crystal.GetHealth() <= 0)
                {
                    GameOver();
                }
            }
        }

        public void StartNextWave()
        {
            if (CurrentWave < Waves.Count && CurrentWaveFinished)
            {
                Debug.Log("Starting new wave");
                CurrentWaveFinished = false;
                NumberOfReadyPlayers = 0;
                PlayersReady.Clear();
                Waves[CurrentWave++].StartWave();
                StartCoroutine("CheckWaveFinished");
            } else if (CurrentWave >= Waves.Count)
            {
                NetworkManager.singleton.ServerChangeScene(NextMapName);
            }
        }

        private IEnumerator CheckWaveFinished()
        {
            for (; ;)
            {
                if (Waves[CurrentWave-1].IsWaveFinished())
                {
                    PlayersReady.Clear();
                    Debug.Log("wave finished new wave");
                    CurrentWaveFinished = true;
                    break;
                }

                yield return new WaitForSeconds(1f);
            }
        }

        public bool IsCurrentWaveOver()
        {
            return CurrentWaveFinished;
        }

        public int GetCurrentWave()
        {
            return CurrentWave;
        }

        public int GetNumberOfWaves()
        {
            return Waves.Count;
        }

        [Command(ignoreAuthority = true)]
        public void ReadyUp(NetworkConnectionToClient conn = null)
        {
            if (CurrentWaveFinished)
            {
                if (conn != null)
                {
                    bool found = false;

                    for (int i = 0; i < PlayersReady.Count; i++)
                    {
                        if (PlayersReady[i] == conn.connectionId)
                        {

                            found = true;
                            PlayersReady.RemoveAt(i);
                        }
                    }

                    if (!found)
                    {
                        PlayersReady.Add(conn.connectionId);
                    }
                }

                NumberOfReadyPlayers = PlayersReady.Count;

                if (PlayersReady.Count >= NetworkServer.connections.Count)
                {
                    StartNextWave();
                }
            }
        }

        public int GetNumberOfReadyPlayers()
        {
            return NumberOfReadyPlayers;
        }

        public int GetNumberOfConnectedPlayers()
        {
            return NumberOfPlayersConnected;
        }

        private void GameOver()
        {
            NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
        }

    }
}



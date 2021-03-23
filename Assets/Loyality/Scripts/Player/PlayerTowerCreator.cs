using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loyality
{
    [RequireComponent(typeof(Player))]
    public class PlayerTowerCreator : NetworkBehaviour
    {
        [Header("Player Tower Creator")]
        [Tooltip("The current ghost tower")]
        private GameObject GhostObject;

        [Tooltip("The last spawn object")]
        private GameObject LastSpawnObject;

        [Tooltip("The stack to store unused ghost objects")]
        private Queue<GameObject> UnusedGhostStack = new Queue<GameObject>();

        [Tooltip("The player object")]
        private Player Player;

        [Tooltip("The Player gui")]
        private PlayerGUI PlayerGUI;

        [Tooltip("Checks to see if we are expecting a ghost tower to by replied in the UpdateLastSpawnedTower function")]
        private bool PlacingGhostTower = false;

        [Tooltip("Keeping track to see if we are deleting towers")]
        private bool DeletingTowers = false;

        [Tooltip("The list of towers the player can spawn (order matters)")]
        public List<Tower> TowersToSpawn;

        [Tooltip("The list of ghost towers corresponding to the towers to spawn list")]
        public List<GhostObject> GhostTowersToSpawn;

        [Tooltip("The current selected tower")]
        private int SelectedTower = -1;

        [Header("Player environment interation")]
        [Tooltip("The maxmium range a player can click a tower or place a tower")]
        public float MaxInteractionRange = 5;

        [Tooltip("The layermask for interacting")]
        public LayerMask InteractMask;

        [Tooltip("The layermask for interacting with towers")]
        public LayerMask TowerInteractionMask;

        [Header("Ghost Prefab")]
        [Tooltip("The material for when the tower can be placed")]
        public Material PlaceableMaterial;

        [Tooltip("The material for when the tower cannot be placed")]
        public Material NonPlaceableMaterial;

        [Tooltip("The material for when the tower is looked over")]
        public Material DeleteMaterial;

        [Tooltip("The current highlighted tower")]
        private GameObject HighlightedTower;

        [Tooltip("Highlighted Tower Color")]
        private Color HighlightedDefaultColor;

        private void Start()
        {
            if (hasAuthority)
            {
                Player = GetComponent<Player>();
                PlayerGUI = GetComponent<PlayerGUI>();
            }
        }

        private void Update()
        {
            if (hasAuthority)
            {
                PlayerInput();
                MoveGhostTower();
                HighlightDeletingTower();
                ClearNextGhost();
                UpdateGUI();
            }
        }

        private void UpdateGUI()
        {
            if (PlayerGUI)
            {
                if (DeletingTowers)
                {
                    PlayerGUI.SelectedTask(TowersToSpawn.Count);
                } else
                {
                    PlayerGUI.SelectedTask(SelectedTower);
                }
            }

        }

        private void PlayerInput()
        {
            if (Input.GetKeyDown("g"))
            {
                MapController.singleton.ReadyUp();
            }

            if (Input.GetButtonDown("Tower_Option_A"))
            {
                if (SelectedTower == 0)
                {
                    ResetGhostTower();
                } else
                {
                    SwitchTower(0);
                }
            } 
            else if (Input.GetButtonDown("Tower_Option_B"))
            {
                if (SelectedTower == 1)
                {
                    ResetGhostTower();
                }
                else
                {
                    SwitchTower(1);
                }
            } 
            else if (Input.GetButtonDown("Tower_Option_C"))
            {
                if (SelectedTower == 2)
                {
                    ResetGhostTower();
                }
                else
                {
                    SwitchTower(2);
                }
            } 
            else if (Input.GetButtonDown("Tower_Option_D"))
            {
                if (SelectedTower == 3)
                {
                    ResetGhostTower();
                }
                else
                {
                    SwitchTower(3);
                }
            }
            else if (Input.GetButtonDown("Tower_Option_E"))
            {
                if (SelectedTower == 4)
                {
                    ResetGhostTower();
                }
                else
                {
                    SwitchTower(4);
                }
            }

            if (Input.GetButtonDown("Delete"))
            {
                if (DeletingTowers)
                {
                    ResetGhostTower();
                }
                else
                {
                    ResetGhostTower();
                    DeletingTowers = true;
                }
            }

            if (Input.GetButtonDown("Cancel"))
            {
                ResetGhostTower();
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (DeletingTowers)
                {
                    Transform DeletingGameObject = GetTowerLookingAt();
                    if (DeletingGameObject != null)
                    {
                        DeletingGameObject.parent.gameObject.TryGetComponent(out Tower TowerObject);

                        if (TowerObject != null) DestoryTower(DeletingGameObject.parent.gameObject, true);
                    }
                } else if (SelectedTower != -1)
                {
                    if (PlacingGhostTower)
                    {
                        if (CanGhostTowerBePlaced())
                        {
                            if (TowersToSpawn[SelectedTower] != null)
                            {
                                ServerCanGhostTowerBePlaced(GhostObject, Player, TowersToSpawn[SelectedTower].Cost);
                            }
                        }
                    }
                }
            }
        }

        private Transform GetTowerLookingAt()
        {
            Camera cam = Player.Camera.GetComponent<Camera>();

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, TowerInteractionMask))
            {
                return hit.transform;
            }

            return null;
        }

        private void HighlightDeletingTower()
        {
            //Todo implement this
/*            if (DeletingTowers)
            {
                Transform temp = GetTowerLookingAt();

                if (temp != null)
                {
                    if (temp.gameObject != HighlightedTower)
                    {
                        MeshRenderer meshRender = HighlightedTower.GetComponent<MeshRenderer>();
                        meshRender.material.color = HighlightedDefaultColor;
                        HighlightedTower = temp.gameObject;
                        meshRender = HighlightedTower.GetComponent<MeshRenderer>();
                        HighlightedDefaultColor = meshRender.material.color;
                    }
                } else
                {
                    MeshRenderer meshRender = HighlightedTower.GetComponent<MeshRenderer>();
                    meshRender.material.color = HighlightedDefaultColor;
                    HighlightedTower = null;
                }
            }*/
        }


        private void ClearNextGhost()
        {
            if (UnusedGhostStack.Count > 1 || (UnusedGhostStack.Count > 0 && GhostObject == null))
            {
                DestoryTower(UnusedGhostStack.Dequeue(), false);
            }
        }

        private void MoveGhostTower()
        {
            if (PlacingGhostTower)
            {
                if (GhostObject != null)
                {
                    Vector3 pos = GetMouseWorldPosition();
                    ///TODO: This can be improved if we keep track of the object class if its new we change it instead of finding it every time
                    GhostObject.transform.position = pos;
                    GhostObject.transform.rotation = transform.rotation;
                    GhostObject.TryGetComponent(out Object ObjectClass);
                    ObjectClass.OffsetPosition();

                    ServerChangeGhostMaterial(GhostObject, CanGhostTowerBePlaced());
                }
            } 
        }

        [Command]
        private void ServerChangeGhostMaterial(GameObject obj, bool isValid)
        {
            if (obj != null)
            {
                ClientsChangeGhostMaterial(obj, isValid);
            }

        }

        [ClientRpc]
        private void ClientsChangeGhostMaterial(GameObject obj, bool isValid)
        {
            if (obj != null)
            {

                Material todoMaterial;

                if (isValid)
                {
                    todoMaterial = PlaceableMaterial;
                }
                else
                {
                    todoMaterial = NonPlaceableMaterial;
                }

                //TODO: This is terribly ineffecient, check to see if they are the same before calling ChangeMaterial function
                GhostObject temp = obj.GetComponent<GhostObject>();

                if (temp != null)
                {
                    temp.ChangeMaterial(todoMaterial);
                }
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            Camera cam = Player.Camera.GetComponent<Camera>();

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, InteractMask))
            {
                return hit.point;
            }

            return new Vector3();
        }

        private bool CanGhostTowerBePlaced()
        {
            if (GhostObject != null)
            {
                bool temp = true;

                float GhostObjectDistance = Vector3.Distance(transform.position, GhostObject.transform.position);
                if (GhostObjectDistance < MaxInteractionRange)
                {
                    temp = true;
                }
                else
                {
                    temp = false;
                }

                GhostObject GhostObjectObject = GhostObject.GetComponent<GhostObject>();
                if (GhostObjectObject != null && (GhostObjectObject.IsColliding() || !GhostObjectObject.IsOnGround()))
                {
                    temp = false;
                }

                if (Player.GetMetal() < TowersToSpawn[SelectedTower].Cost)
                {
                    temp = false;
                }



                return temp;
            }

            return false;
        }
        

        [Command]        //This is poorly named
        private void ServerCanGhostTowerBePlaced(GameObject GhostObject, Player player, float cost, NetworkConnectionToClient conn = null)
        {

            
            if (GhostObject != null && player != null)
            {
                bool canBePlaced = true;

                GhostObject GhostObjectObject = GhostObject.GetComponent<GhostObject>();
                if (GhostObjectObject != null && (GhostObjectObject.IsColliding() || !GhostObjectObject.IsOnGround()))
                {
                    canBePlaced = false;
                }

                if (player.GetMetal() < cost)
                {
                    canBePlaced = false;
                }

                ReturnCanGhostTowerBePlaced(canBePlaced);
                return;
            }
            ReturnCanGhostTowerBePlaced(false);
        }

        [TargetRpc]//This is poorly named
        private void ReturnCanGhostTowerBePlaced(bool CanBePlaced)
        {
            if (CanBePlaced)
            {
                if (GhostObject != null && Player != null && TowersToSpawn[SelectedTower] != null)
                {
                    Player.SetMetal(Player.GetMetal() - TowersToSpawn[SelectedTower].Cost);
                    SpawnTower(GhostObject.transform.position, GhostObject.transform.rotation, TowersToSpawn[SelectedTower].UnqiueName, false, false);
                    ResetGhostTower();
                }
            }
        }

        private void SwitchTower(int TowerIndex)
        {
            ResetGhostTower();

            SelectedTower = TowerIndex;
            SpawnTower(GetMouseWorldPosition(), transform.rotation, GhostTowersToSpawn[TowerIndex].UnqiueName, true, true);
            PlacingGhostTower = true;
        }

        private void ResetGhostTower()
        {
            if (GhostObject != null)
            {
                GhostObject = null;
            }

            SelectedTower = -1;
            PlacingGhostTower = false;
            DeletingTowers = false;
        }

        [Command]
        private void DestoryTower(GameObject obj, bool ReturnValue, NetworkConnectionToClient conn = null)
        {
            if (obj != null)
            {
                obj.TryGetComponent(out Tower TowerObject);

                if (TowerObject != null && ReturnValue) DestroyedTower(TowerObject.Cost);

                obj.TryGetComponent(out Object ObjectClass);
                if (ObjectClass != null) ObjectClass.ServerDestroy();
            }
        }

        [TargetRpc]
        private void DestroyedTower(float value)
        {
            Player.SetMetal(Player.GetMetal() + value);
        }

        [Command]
        private void SpawnTower(Vector3 position, Quaternion rotation, string UniqueName, bool UseOffset, bool isTemp, NetworkConnectionToClient conn = null)
        {

            foreach (GameObject spawnablePrefab in NetworkManager.singleton.spawnPrefabs)
            {

                spawnablePrefab.TryGetComponent(out Object obj);

                if (obj != null && obj.UnqiueName == UniqueName)
                {
                    GameObject newTower = Instantiate(spawnablePrefab);
                    newTower.transform.position = position;
                    newTower.transform.rotation = rotation;
                    if (UseOffset)
                    {
                        newTower.TryGetComponent(out Object spawnableObjectClass);
                        spawnableObjectClass.OffsetPosition();
                    }

                    if (conn != null)
                    {

                        NetworkServer.Spawn(newTower, conn);
                        UpdateLastSpawnedTower(newTower, isTemp);
                    }
                    else
                    {

                        NetworkServer.Spawn(newTower);
                    }

                    return;
                }
            }

            Debug.Log("No spawnable prefab with the UnqiueName " + UniqueName);

        }

        [TargetRpc]
        private void UpdateLastSpawnedTower(GameObject spawnedTower, bool isTemp)
        {
            if (spawnedTower != null)
            {
                if (isTemp)
                {
                    GhostObject = spawnedTower;
                    UnusedGhostStack.Enqueue(GhostObject);
                    MoveGhostTower();
                }
                else
                {
                    LastSpawnObject = spawnedTower;
                }
            }
        }
    }
}



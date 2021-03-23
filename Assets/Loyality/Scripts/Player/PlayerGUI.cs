using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Loyality
{
    [RequireComponent(typeof(Player))]
    public class PlayerGUI : NetworkBehaviour
    {
        [Header("Player GUI")]

        public GameObject Canvas; 

        [Tooltip("The gui text box for metal")]
        public TextMeshProUGUI MetalText;

        [Tooltip("The gui text for health")]
        public TextMeshProUGUI HealthText;

        [Tooltip("The gui text for the wave number")]
        public TextMeshProUGUI WaveNumber;

        [Tooltip("The gui text for the core health")]
        public TextMeshProUGUI CoreHealthText;

        [Tooltip("The gui text for the finished wave text")]
        public GameObject FinishedWave;

        [Tooltip("The gui text for the wave number")]
        public TextMeshProUGUI FinishedWaveText;

        [Tooltip("The panel for the options menu")]
        public GameObject OptionMenu;

        [Tooltip("The connected player object")]
        private Player player;

        [Tooltip("To store if the menu is open or not")]
        private bool IsMenuOpen = false;

        [Tooltip("Buttons for host only")]
        public GameObject HostOnly;

        [Tooltip("The current depressed button")]
        private int CurrentlySelectedButton = -1;

        [Tooltip("The list of pressable buttons with keypresses")]
        public List<Button> PressableButtons;


        void Start()
        {
            if (isLocalPlayer)
            {
                Canvas.SetActive(true);
                player = GetComponent<Player>();
            } else
            {
                Canvas.SetActive(false);
            }

            if (isServer)
            {
                HostOnly.SetActive(true);
            }
        }

        void Update()
        {
            if (hasAuthority)
            {
                UpdateHudInformation();
                LookForKeyPresses();
                UpdateTowerMenu();
            }
        }

        private void UpdateHudInformation()
        {
            MetalText.text = "Metal: " + player.Metal;
            HealthText.text = "Health: " + player.GetHealth() + "/" +  player.MaxHealth;
            WaveNumber.text = "Wave: " + MapController.singleton.GetCurrentWave() + "/" + MapController.singleton.GetNumberOfWaves();
            FinishedWave.SetActive(MapController.singleton.IsCurrentWaveOver());
            CoreHealthText.text = "Core Health: " + MapController.singleton.Crystal.GetHealth() + "/" + MapController.singleton.Crystal.MaxHealth;

            if (MapController.singleton.GetCurrentWave() == MapController.singleton.GetNumberOfWaves())
            {
                FinishedWaveText.text = "Congratulations you beat this map! Ready to go to next map? (" + MapController.singleton.GetNumberOfReadyPlayers() + " / " + MapController.singleton.GetNumberOfConnectedPlayers() + ")";
            } else
            {
                FinishedWaveText.text = "Press 'G' to start next wave (" + MapController.singleton.GetNumberOfReadyPlayers() + "/" + MapController.singleton.GetNumberOfConnectedPlayers() + ")";
            }

            
        }

        private void LookForKeyPresses()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                if (IsMenuOpen)
                {
                    CloseMenu();
                } else
                {
                    OpenMenu();
                }
            }
        }

        private void OpenMenu()
        {
            IsMenuOpen = true;
            OptionMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void CloseMenu()
        {
            IsMenuOpen = false;
            OptionMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void PlayerDisconnect()
        {
            if (isServer)
            {
                NetworkManager.singleton.StopHost();
            }else if (hasAuthority)
            {
                NetworkManager.singleton.StopClient();
            }
        }

        private void UpdateTowerMenu()
        {
/*            if (CurrentlySelectedButton != -1)
            {
                if (PressableButtons[CurrentlySelectedButton] != null)
                {
                    PressableButtons[CurrentlySelectedButton].Select();
                }
            }*/
        }

        public void SelectedTask(int selected)
        {
            CurrentlySelectedButton = selected;

        }

        public void RestartScene()
        {
            if (isServer)
            {
                NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}


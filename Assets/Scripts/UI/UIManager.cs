using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TurnAMint.UI.Inventory;

namespace TurnAMint.UI
{
    public class UIManager : MonoBehaviour
    {

        public int index;
        public Crosshair activeCrosshair;
        public Crosshair[] crosshairs;

        Player.StateManager state;

        public Color positiveColor, negativeColor;

        public Text contextText;
        public AmmoCountText ammoText;

        public HealthBar healthBar;

        public InventoryUI invUI;

        public static UIManager instance;
        public static UIManager GetInstance()
        {
            return instance;
        }

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            for (int i = 0; i < crosshairs.Length; i++)
            {
                crosshairs[i].gameObject.SetActive(false);
            }

            invUI.gameObject.SetActive(true);
            invUI.gameObject.SetActive(false);

            crosshairs[index].gameObject.SetActive(true);
            activeCrosshair = crosshairs[index];
        }

        public void DefineCrosshairByIndex(int findIndex)
        {
            activeCrosshair = crosshairs[findIndex];
        }

        public void DefineCrosshairByName(string name)
        {
            for (int i = 0; i < crosshairs.Length; i++)
            {
                if (string.Equals(crosshairs[i].name, name))
                {
                    activeCrosshair = crosshairs[i];
                    break;
                }
            }
        }

        public void AssignPlayerState(TurnAMint.Player.StateManager newState)
        {
            state = newState;
            healthBar.hm = state.playerHealth;
            ammoText.state = state;
            invUI.InventoryHandler = state.inventoryHandler;
        }

        public void EnableAmmoCountText(bool val)
        {
            ammoText.gameObject.SetActive(val);
        }

        public void SetContextText(string newStr, bool isAGoodThing = true)
        {
            contextText.color = isAGoodThing ? positiveColor : negativeColor;
            contextText.text = newStr;
        }

        public void EnableContextText(bool val)
        {
            contextText.gameObject.SetActive(val);
        }

    }

}
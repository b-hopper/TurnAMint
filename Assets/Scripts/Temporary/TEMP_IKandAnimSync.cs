using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurnAMint.Player.Animation;

namespace TurnAMint.Temporary
{
    public class TEMP_IKandAnimSync : MonoBehaviour
    {

        I_IKHandler ikHandler;

        I_IKHandler drivingIKHandler;

        public GameObject drivingPlayer;

        private void Awake()
        {
            ikHandler = GetComponent<I_IKHandler>();
        }

        private void Start()
        {
            drivingIKHandler = drivingPlayer.GetComponent<I_IKHandler>();
        }

        private void FixedUpdate()
        {
            ikHandler.IKValues = drivingIKHandler.IKValues;
        }
    }
}
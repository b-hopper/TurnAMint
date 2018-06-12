using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurnAMint.Management;

namespace TurnAMint.Player.Health
{
    public class CharacterHitEffects : MonoBehaviour
    {

        public GameObject[] hitEffects;
        StateManager state;
        int objPoolID;

        private void Awake()
        {
            state = GetComponent<StateManager>();
        }

        private void Start()
        {
            objPoolID = GameManager.Instance.ObjectPool.AddNewPool(hitEffects, 2, state.PlayerName + "_bloodPool");
        }

        public void PlayEffect(Vector3 position, Vector3 direction)
        {
            GameObject hitEffect = GameManager.Instance.ObjectPool.GetNewObj(objPoolID);

            if (hitEffect != null)
            {
                hitEffect.transform.position = position;
                hitEffect.transform.rotation = Quaternion.LookRotation(-direction);
            }
        }
    }
}
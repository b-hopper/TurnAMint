using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHitEffects : MonoBehaviour {

    public HitEffectsList[] hitEffects;
    ObjectPool[] objPool;

    private void Start()
    {
        objPool = new ObjectPool[hitEffects.Length];

        for (int i = 0; i < hitEffects.Length; i++)
        {
            objPool[i] = new ObjectPool(hitEffects[i].effect, 2, hitEffects[i].name);
        }
    }

    public void PlayEffect(int index, Vector3 position, Vector3 direction)
    {
        GameObject hitEffect = null;
        if (hitEffects[index] != null)
        {
            hitEffect = objPool[index].GetNewObj();
        }
        if (hitEffect != null)
        {
            Debug.Log(hitEffect, hitEffect);
            hitEffect.transform.position = transform.position + position;
            hitEffect.transform.rotation = Quaternion.LookRotation(-direction);
            hitEffect.SetActive(true);
        }
    }

    public void PlayEffect(string name, Vector3 position, Vector3 direction)
    {
        GameObject hitEffect = null;

        for (int i = 0; i < hitEffects.Length; i++)
        {
            if (string.Equals(hitEffects[i].name, name))
            {
                hitEffect = objPool[i].GetNewObj();
                break;
            }
        }

        if (hitEffect != null)
        {
            Debug.Log(hitEffect, hitEffect);
            hitEffect.transform.position = transform.position + position;
            hitEffect.transform.rotation = Quaternion.LookRotation(-direction);
            hitEffect.SetActive(true);
        }
    }
}

[System.Serializable]
public class HitEffectsList
{
    public string name;
    public GameObject[] effect;
}

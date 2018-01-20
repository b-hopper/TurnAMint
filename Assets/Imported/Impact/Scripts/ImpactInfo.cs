﻿using UnityEngine;

[System.Serializable]
public struct ImpactInfo
{
	public enum Type
	{
		Material,
		Terrain,
        Tag
	}

	[SerializeField]
	Type impactType;

	[SerializeField]
	Material material;
	[SerializeField]
	Texture texture;
    [SerializeField]
    string tag;
	[SerializeField]
	GameObject[] impactPrefabs;

	public Type ImpactType {
		get
        {
			return impactType;
		}
	}

	public Material Material {
		get
        {
			return material;
		}
	}

	public Texture Texture {
		get
        {
			return texture;
		}
	}

    public string Tag
    {
        get
        {
            return tag;
        }
    }
    
	public GameObject GetRandomPrefab ()
	{
		int length = impactPrefabs.Length;

		if (length == 0) {
			Debug.LogWarning ("Please assign at least one impact prefab for material '{0}'");
			return null;
		} else if (length == 1) {
			return impactPrefabs [0];
		}

		return impactPrefabs [Random.Range (0, length)];
	}
}

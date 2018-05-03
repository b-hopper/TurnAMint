using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    List<GameObject[]> ObjPool;
    Dictionary<GameObject, Rigidbody> RBPool;

    private void Awake()
    {
        ObjPool = new List<GameObject[]>();
        RBPool = new Dictionary<GameObject, Rigidbody>();
    }

    public int AddNewPool(GameObject[] newObjArray, int objsPerPool, string name)
    {
        int ret = -1;
        if (newObjArray.Length != 0)
        {
            GameObject[] tmp = new GameObject[objsPerPool * newObjArray.Length];

            GameObject parent = new GameObject(name);
            parent.transform.parent = transform;

            tmp = PopulatePool(newObjArray, objsPerPool, parent.transform);

            ObjPool.Add(tmp);            

            ret = ObjPool.IndexOf(tmp);
        }
        return ret;
    }

    public int AddNewPool(GameObject newObj, int objsPerPool, string name)
    {
        int ret = -1;
        if (newObj != null)
        {
            GameObject[] tmp = new GameObject[objsPerPool];

            GameObject parent = new GameObject(name);
            parent.transform.parent = transform;

            tmp = PopulatePool(newObj, objsPerPool, parent.transform);

            ObjPool.Add(tmp);

            ret = ObjPool.IndexOf(tmp);
        }
        return ret;
    }

    private GameObject[] PopulatePool(GameObject[] newObjArray, int amount, Transform newParent)
    {
        GameObject[] array = new GameObject[newObjArray.Length * amount];
        int k = 0;
        for (int i = 0; i < newObjArray.Length; i++)
        {
            for (int j = 0; j < amount; j++)
            {
                GameObject tmp = Instantiate(newObjArray[i]);
                tmp.transform.parent = newParent;
                array[k] = tmp;
                RBPool.Add(tmp, tmp.GetComponent<Rigidbody>());
                tmp.SetActive(false);
                k++;
            }
        }
        return array;
    }

    private GameObject[] PopulatePool(GameObject newObj, int amount, Transform newParent)
    {
        GameObject[] array = new GameObject[amount];
        int k = 0;
        for (int j = 0; j < amount; j++)
        {
            GameObject tmp = Instantiate(newObj);
            tmp.transform.parent = newParent;
            array[k] = tmp;
            RBPool.Add(tmp, tmp.GetComponent<Rigidbody>());
            tmp.SetActive(false);
            k++;
        }
        return array;
    }

    /// <summary>
    /// Returns a random object from the pool, selected by ID number (assigned when pool is created)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public GameObject GetNewObj(int id)
    {
        GameObject ret;
        if (id > ObjPool.Count || id < 0)
        {
            ret = null;
        }
        else
        {
            ret = ObjPool[id][Random.Range(0, ObjPool[id].Length)];
            ret.SetActive(true);
        }

        return ret;
    }

    public GameObject GetNewObj(int id, int index)
    {
        GameObject ret;
        if (id > ObjPool.Count || id < 0)
        {
            ret = null;
        }
        else
        {
            ret = ObjPool[id][index];
            ret.SetActive(true);
        }

        return ret;
    }    

    public Rigidbody GetRigidBody(GameObject obj)
    {
        Rigidbody rb;
        RBPool.TryGetValue(obj, out rb);
        return rb;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager {
    
    private GameObject gameObject;

    #region GameManager Instance
    private static GameManager m_Instance;
    public static GameManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new GameManager();
                m_Instance.gameObject = new GameObject("_gameManager");
                m_Instance.gameObject.AddComponent<Timer>();
                m_Instance.gameObject.AddComponent<Respawner>();
                GameObject tmp = new GameObject("_objPool");
                tmp.transform.parent = m_Instance.gameObject.transform;
                tmp.gameObject.AddComponent<ObjectPool>();
            }
            return m_Instance;
        }
    }
    #endregion

    #region Local Player Reference

    public StateManager LocalPlayerReference;

    #endregion

    #region Camera References

    private CameraReferences m_CamRefs;
    public CameraReferences CamRefs
    {
        get
        {
            if (m_CamRefs == null)
            {
                m_CamRefs = GameObject.FindObjectOfType<CameraReferences>();
            }
            return m_CamRefs;
        }
    }

    #endregion

    #region Timer

    private Timer m_Timer;
    public Timer Timer {
        get
        {
            if (m_Timer == null)
            {
                m_Timer = gameObject.GetComponent<Timer>();
            }
            return m_Timer;
        }
    }

    #endregion

    #region UIManager
    private UIManager m_UIManager;
    public UIManager UIManager
    {
        get
        {
            if (m_UIManager == null)
            {
                m_UIManager = GameObject.FindObjectOfType<UIManager>();
            }
            return m_UIManager;
        }
    }
    #endregion

    #region Object Pool
    private ObjectPool m_ObjectPool;
    public ObjectPool ObjectPool
    {
        get
        {
            if(m_ObjectPool == null)
            {
                m_ObjectPool = gameObject.GetComponentInChildren<ObjectPool>();
            }
            return m_ObjectPool;
        }
    }
    #endregion

    #region Respawner
    private Respawner m_Respawner;
    public Respawner Respawner
    {
        get
        {
            if (m_Respawner == null)
            {
                m_Respawner = gameObject.GetComponent<Respawner>();
            }
            return m_Respawner;
        }
    }
    #endregion
}

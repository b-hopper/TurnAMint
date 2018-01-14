using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Scanner : MonoBehaviour {

    [SerializeField] float scanSpeed;
    [SerializeField, Range(0, 360)] float fieldOfView;
    [SerializeField] LayerMask mask;

    SphereCollider rangeTrigger;
    List<Player> targets;
    Player m_selectedTarget;
    Player selectedTarget
    {
        get { return m_selectedTarget; }
        set
        {
            m_selectedTarget = value;
            if (m_selectedTarget == null)
            {
                return;
            }
            if (OnTargetSelected != null)
            {
                OnTargetSelected(m_selectedTarget.transform.position);
            }
        }
    }


    public event System.Action<Vector3> OnTargetSelected;
    
    private void Start()
    {
        rangeTrigger = GetComponent<SphereCollider>();
        PrepareScan();
    }

    void PrepareScan()
    {
        if (selectedTarget != null)
        {
            return;
        }

        GameManager.Instance.Timer.Add(ScanForTargets, scanSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        if (selectedTarget != null)
        {
            Gizmos.DrawLine(transform.position, selectedTarget.transform.position);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + GetViewAngle(fieldOfView * 0.5f) * GetComponent<SphereCollider>().radius);
        Gizmos.DrawLine(transform.position, transform.position + GetViewAngle(-fieldOfView * 0.5f) * GetComponent<SphereCollider>().radius);
    }

    Vector3 GetViewAngle(float angle)
    {
        float radian = (angle + transform.eulerAngles.y) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0, Mathf.Cos(radian));
    }

    void ScanForTargets()
    {
        targets = new List<Player>();
        Collider[] results = Physics.OverlapSphere(transform.position, rangeTrigger.radius);

        for (int i = 0; i < results.Length; i++)
        {
            var player = results[i].GetComponent<Player>();
            if (player == null)
            {
                continue;
            }
            if (!IsInLineOfSight(Vector3.up, player.transform.position))
            {
                continue;
            }
            targets.Add(player);
        }

        if (targets.Count == 1)
        {
            selectedTarget = targets[0];
        }
        else
        {
            float closestTarget = rangeTrigger.radius;
            foreach(var possibleTarget in targets)
            {
                if (Vector3.Distance(transform.position, possibleTarget.transform.position) < closestTarget)
                {
                    selectedTarget = possibleTarget;
                }
            }
        }
        PrepareScan();
    }

    bool IsInLineOfSight(Vector3 eyeHeight, Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;

        if (Vector3.Angle(transform.forward, direction.normalized) < fieldOfView * 0.5f)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetPos);
            if (Physics.Raycast(transform.position + eyeHeight, direction.normalized, distanceToTarget, mask))
            {
                return false;
            }
            return true;
        }
        return false;
    }
}

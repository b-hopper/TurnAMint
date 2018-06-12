using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnAMint.Temporary
{
    public class tracer_TEMP : MonoBehaviour
    {

        LineRenderer line;

        Vector3 lastPos, posBeforeLast;

        private void Awake()
        {
            line = GetComponent<LineRenderer>();
        }

        private void Start()
        {
            line.SetPosition(0, Vector3.zero);
            line.SetPosition(1, Vector3.zero);
        }

        private void OnEnable()
        {
            posBeforeLast = transform.position;

        }

        private void Update()
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, posBeforeLast);
            lastPos = transform.position;
            posBeforeLast = lastPos;
        }

    }
}
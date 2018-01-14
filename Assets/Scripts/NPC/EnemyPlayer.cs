using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pathfinder))]
[RequireComponent(typeof(Scanner))]
public class EnemyPlayer : MonoBehaviour {

    Pathfinder pathfinder;
    Scanner scanner;

    [SerializeField] public Animator animator;
    

    private void Start()
    {
        pathfinder = GetComponent<Pathfinder>();
        scanner = GetComponent<Scanner>();
        scanner.OnTargetSelected += Scanner_OnTargetSelected;
    }

    private void Scanner_OnTargetSelected(Vector3 position)
    {
        pathfinder.SetTarget(position);
    }

    private void Update()
    {
        animator.SetFloat("Vertical", pathfinder.Agent.velocity.z);
    }
}

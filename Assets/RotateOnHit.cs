using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnHit : MonoBehaviour {

    [SerializeField] float secondsDown;
    [SerializeField] float rotationSpeed;

    Quaternion initialRotation, targetRotation;

    bool needsRotation;

    bool isHit;

    private void Awake()
    {
        initialRotation = transform.rotation;
    }

    public void Rotate()
    {
        if (!isHit)
        {
            isHit = true;
            needsRotation = true;
            targetRotation = Quaternion.Euler(transform.right * 90);


            GameManager.Instance.Timer.Add(() =>
            {
                needsRotation = true;
                targetRotation = initialRotation;
                isHit = false;
            }, secondsDown);
        }
    }

    private void Update()
    {
        if (!needsRotation)
        {
            return;
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (transform.rotation == targetRotation)
        {
            needsRotation = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

    [SerializeField]
    Vector3 cameraOffset;

    [SerializeField]
    float damping;

    Transform cameraLookTarget;
    Player LocalPlayer;

    private void Awake()
    {
        GameManager.Instance.OnLocalPlayerJoined += HandleOnLocalPlayerJoined;
    }

    void HandleOnLocalPlayerJoined(Player player)
    {
        LocalPlayer = player;
        cameraLookTarget = LocalPlayer.transform.Find("CameraLookTarget");

        if (cameraLookTarget == null)
        {
            cameraLookTarget = LocalPlayer.transform;
        }
    }
    
    void Update () {
        Vector3 targetPosition = cameraLookTarget.position 
            + LocalPlayer.transform.forward * cameraOffset.z
            + LocalPlayer.transform.up * cameraOffset.y
            + LocalPlayer.transform.right * cameraOffset.x;

        Quaternion targetRotation = Quaternion.LookRotation(cameraLookTarget.position - targetPosition, Vector3.up);

        transform.position = Vector3.Lerp(transform.position, targetPosition, damping * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, damping * Time.deltaTime);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [System.Serializable]
    public class CameraRig
    {
        public Vector3 CameraOffset;
        public float CrouchHeight;
        public float Damping;
    }

    // 0, 0.8, -2
    // Damping: 50
    [SerializeField] CameraRig defaultCamera;
    [SerializeField] CameraRig aimCamera;
    [SerializeField] CameraRig firstPersonCamera;
    [SerializeField] LayerMask firstPersonMask;
    LayerMask thirdPersonMask;

    Camera thisCam;

    float yMousePos;

    Transform previousLookTargetTransform;

    Transform cameraLookOrigin, cameraLookTarget;
    Player localPlayer;

    private void Awake()
    {
        GameManager.Instance.OnLocalPlayerJoined += HandleOnLocalPlayerJoined;

        thisCam = GetComponent<Camera>();
    }

    void HandleOnLocalPlayerJoined(Player player)
    {
        localPlayer = player;
        cameraLookOrigin = localPlayer.transform.Find("CameraLookObjs/CameraLookOrigin");
        cameraLookTarget = localPlayer.transform.Find("CameraLookObjs/CameraLookTarget");
                
        if (cameraLookOrigin == null)
        {
            cameraLookOrigin = localPlayer.transform;
        }
    }
    
    void Update () {

        if (localPlayer == null)
        {
            return;
        }

        //Vector3 lookTarget = new Vector3(cameraLookTarget.localPosition.x,
            //cameraLookTarget.localPosition.y - (Mathf.Sin(localPlayer.playerAim.GetAngle() * Mathf.Deg2Rad) * Vector3.Distance(cameraLookOrigin.position, cameraLookTarget.position)),
            //cameraLookTarget.localPosition.z + (Mathf.Cos(localPlayer.playerAim.GetAngle() * Mathf.Deg2Rad) * Vector3.Distance(cameraLookOrigin.position, cameraLookTarget.position)));
            
        //////////
        //
        //  Tutorial I'm following doesn't cover vertical camera movement, only vertical crosshair movement.
        //  Will be modifying later - this is just remnants of that, to be revisited later on.
        //
        //////////


        //yMousePos += GameManager.Instance.InputController.MouseInput.y;

        CameraRig cameraRig = defaultCamera;

        if (localPlayer.PlayerState.WeaponState == PlayerState.EWeaponState.AIMING || localPlayer.PlayerState.WeaponState == PlayerState.EWeaponState.AIMEDFIRING)
        {
            cameraRig = aimCamera;
        }

        //if (localPlayer.PlayerState.CameraState == PlayerState.ECameraState.FIRSTPERSON)
        //{
          //  cameraRig = firstPersonCamera;
        //}


        Vector3 targetPosition = cameraLookOrigin.position
            + localPlayer.transform.forward * cameraRig.CameraOffset.z
            + localPlayer.transform.up * (cameraRig.CameraOffset.y + (localPlayer.PlayerState.MoveState == PlayerState.EMoveState.CROUCHING ? cameraRig.CrouchHeight : 0))
            + localPlayer.transform.right * cameraRig.CameraOffset.x;

        Quaternion targetRotation = Quaternion.LookRotation(cameraLookTarget.position/*.TransformPoint(lookTarget)*/ - cameraLookOrigin.position, Vector3.up);

        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraRig.Damping * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, cameraRig.Damping * Time.deltaTime);
	}

    public void SetFirstPersonCameraParent(Transform newParent)
    {
        previousLookTargetTransform = cameraLookOrigin.parent;

        thirdPersonMask = thisCam.cullingMask;

        thisCam.cullingMask = firstPersonMask;

        cameraLookOrigin.parent = newParent;
    }

    public void SetThirdPersonCameraParent()
    {
        thisCam.cullingMask = thirdPersonMask;
        cameraLookOrigin.parent = previousLookTargetTransform;
    }
}

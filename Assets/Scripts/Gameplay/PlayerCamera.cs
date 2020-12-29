using Mirror;
using UnityEngine;

namespace Gameplay
{
    public class PlayerCamera : NetworkBehaviour
    {
        [SerializeField] private GameObject cameraMountPoint;
        [SerializeField] private GameObject headBone; // TODO: Bone to force camera to follow player head while animating

        public override void OnStartLocalPlayer()
        {
            if (Camera.main is null)
                return;

            if (!isServer)
                SetCameraForClient();
        }

        private void SetCameraForClient()
        {
            var cameraTransform = Camera.main.gameObject.transform;

            cameraTransform.SetParent(cameraMountPoint.transform);
            cameraTransform.SetPositionAndRotation(cameraMountPoint.transform.position, cameraMountPoint.transform.rotation);

            //cameraMountPoint.transform.SetParent(headBone.transform);
        }
    }
}

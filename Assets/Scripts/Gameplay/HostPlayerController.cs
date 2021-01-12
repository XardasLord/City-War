using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    public class HostPlayerController : NetworkBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private Vector2 lookInputs; // X for rotate left/right, Y for look up/down

        // TODO: Moving camera

        [Header("Looking")]
        [SerializeField] private float lookAngleRange = 60f; // 60' up, 60' down
        [SerializeField] private float turnSpeed = 100f; // multiplier for turning while looking
        private float _camRotation; // current camera up/down rotation value

        // TODO: Scrolling camera

        #region Receive Input Values
        // Call these functions from the PlayerInput component as set up by this guide:
        // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/QuickStartGuide.html#getting-input-indirectly-through-an-input-action
        // This is designed for you to use "Action Responses" when 
        // Player Input Behaviour is set to "Invoke Unity Events", 
        // and then you can point those events to these functions.

        // Because the CallbackContext object is generic, you must call ReadValue<T>
        // and specify the type that you're expecting. Read more on C# generics here:
        // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/

        public void UpdateLookInputs(InputAction.CallbackContext context)
        {
            lookInputs = context.ReadValue<Vector2>();
        }
        #endregion

        private Camera _camera;

        public override void OnStartLocalPlayer()
            => enabled = isServer;

        private void Awake() 
            => _camera = Camera.main;


        private void Update() 
            => Look();

        private void Look()
        {
            if (!hasAuthority)
                return;

            if (lookInputs == Vector2.zero)
                return;

            var cameraPosition = _camera.transform.position;
            var lookInputsV3 = new Vector3(lookInputs.x, 0, lookInputs.y);

            _camera.transform.position = new Vector3(
                cameraPosition.x + (lookInputsV3.x * turnSpeed * Time.deltaTime), 
                cameraPosition.y, 
                cameraPosition.z + (lookInputsV3.y * turnSpeed * Time.deltaTime));
        }
    }
}

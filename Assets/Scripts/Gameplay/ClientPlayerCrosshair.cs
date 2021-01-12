using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class ClientPlayerCrosshair : NetworkBehaviour
    {
        [SerializeField] private Image crosshairImage;

        public override void OnStartLocalPlayer() 
            => crosshairImage.enabled = !isServer;
    }
}

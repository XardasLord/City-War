using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    public class ClientPlayerCrosshairUI : NetworkBehaviour
    {
        [SerializeField] private Image crosshairImage;

        public override void OnStartLocalPlayer() 
            => crosshairImage.enabled = !isServer;
    }
}

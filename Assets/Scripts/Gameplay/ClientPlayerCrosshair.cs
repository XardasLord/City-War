using Mirror;
using UnityEngine;

namespace Gameplay
{
    public class ClientPlayerCrosshair : NetworkBehaviour
    {
        [SerializeField] private GameObject crosshairImage;

        public override void OnStartLocalPlayer()
            => crosshairImage.SetActive(!isServer);
    }
}

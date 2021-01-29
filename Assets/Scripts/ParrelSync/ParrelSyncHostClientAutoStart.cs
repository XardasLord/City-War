#if UNITY_EDITOR
using Mirror;
using ParrelSync;
using UnityEngine;

namespace Assets.Scripts.ParrelSync
{
    public class ParrelSyncHostClientAutoStart : MonoBehaviour
    {
        private void Start()
        {
            var networkManager = GetComponent<NetworkManager>();
            networkManager.networkAddress = "localhost";

            if (ClonesManager.IsClone())
                networkManager.StartClient();
            else
                networkManager.StartHost();
        }
    }
}
#endif
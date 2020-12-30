#if UNITY_EDITOR
using Mirror;
using UnityEngine;

namespace ParrelSync
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
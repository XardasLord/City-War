using Mirror;
using UnityEngine;

namespace Networking
{
    public class GameNetworkManager : NetworkManager
    {
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            Debug.Log($"Player with ID {conn.connectionId} connected to the server! Active numbers of players on the server: {numPlayers}");
        }
    }
}
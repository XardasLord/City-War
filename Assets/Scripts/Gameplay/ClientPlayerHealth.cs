using Mirror;
using UnityEngine;

namespace Gameplay
{
    public class ClientPlayerHealth : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnTookDamage))] 
        public int health = 100;

        public override void OnStartLocalPlayer() 
            => enabled = !isServer;

        [Server]
        public void Hit(int damage)
        {
            if (health - damage <= 0)
                health = 0;
            else
                health -= damage;
        }

        private void OnTookDamage(int oldValue, int newValue)
        {
            // TODO: Destroy the game object on the server and load death screen for the player when health <= 0
            Debug.Log($"Player got hit. Current health - {newValue}");
        }
    }
}

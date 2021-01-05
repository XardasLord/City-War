using Mirror;
using UnityEngine;

namespace Gameplay.Weapons
{
    public class Projectile : NetworkBehaviour
    {
        [SerializeField] private float projectileLifeInSeconds = 3f;

        public override void OnStartServer() 
            => Invoke(nameof(DestroySelf), projectileLifeInSeconds);

        [Server]
        private void DestroySelf() 
            => NetworkServer.Destroy(gameObject);

        [ServerCallback]
        private void OnTriggerEnter(Collider collider)
        {
            Debug.Log("Projectile hit!");
            NetworkServer.Destroy(gameObject);
        }
    }
}

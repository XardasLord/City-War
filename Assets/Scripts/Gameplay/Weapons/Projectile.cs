using Assets.Scripts.Gameplay;
using Mirror;
using UnityEngine;

namespace Gameplay.Weapons
{
    public class Projectile : NetworkBehaviour
    {
        private Weapon _shotFromWeapon;

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), _shotFromWeapon.weaponProjectileLifeInSeconds);
            GetComponent<Rigidbody>().AddForce(transform.forward * _shotFromWeapon.weaponProjectileSpeed, ForceMode.VelocityChange);
        }

        [Server]
        private void DestroySelf() 
            => NetworkServer.Destroy(gameObject);

        [Server]
        public void SetWeapon(Weapon projectileShotFromWeapon)
            => _shotFromWeapon = projectileShotFromWeapon;

        [Server]
        public void SetDestination(Vector3 destinationPoint)
            => transform.LookAt(destinationPoint);

        [ServerCallback]
        private void OnTriggerEnter(Collider collider)
        {
            Debug.Log(collider);
            if (collider.gameObject.CompareTag("Player"))
            {
                // TODO: Detect body part and apply some additional damage multiplier value based on that
                Debug.Log($"Projectile hit player with damage - {_shotFromWeapon.weaponDamage}");
                
                // TODO: Particle effect of hit

                var playerHealthScript = collider.transform.root.GetComponent<ClientPlayerHealth>();
                playerHealthScript.Hit(_shotFromWeapon.weaponDamage);
            }

            NetworkServer.Destroy(gameObject);
        }

    }
}

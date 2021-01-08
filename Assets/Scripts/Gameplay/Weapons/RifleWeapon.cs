using Mirror;
using UnityEngine;

namespace Gameplay.Weapons
{
    public class RifleWeapon : Weapon
    {
        [Server]
        public override void Fire(Vector3 destinationPoint)
        {
            weaponAmmo -= 1;

            var projectile = Instantiate(weaponProjectile, weaponFirePosition.position, weaponFirePosition.rotation);

            projectile.SetDestination(destinationPoint);
            projectile.SetWeapon(this);

            NetworkServer.Spawn(projectile.gameObject);
        }
    }
}

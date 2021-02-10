using Gameplay.Weapons;
using Mirror;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Weapons
{
    public class Ak47Weapon : Weapon
    {
        [Server]
        public override void Fire(Vector3 destinationPoint)
        {
            weaponAmmo -= 1;

            var projectile = Instantiate(weaponProjectile, weaponFirePosition.position, weaponFirePosition.rotation);

            projectile.SetDestination(destinationPoint);
            projectile.SetWeapon(this);

            NetworkServer.Spawn(projectile.gameObject);
            
            var shootEffect = Instantiate(weaponFireEffect, weaponFirePosition.position, weaponFirePosition.rotation);
            NetworkServer.Spawn(shootEffect.gameObject);
        }
    }
}

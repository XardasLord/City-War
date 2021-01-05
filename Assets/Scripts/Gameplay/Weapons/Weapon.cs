using Mirror;
using UnityEngine;

namespace Gameplay.Weapons
{
    public abstract class Weapon : NetworkBehaviour
    {
        public float weaponProjectileSpeed = 15.0f;
        public float weaponCooldown = 1.0f;
        public int weaponAmmo = 15;

        public Projectile weaponProjectile;
        public Transform weaponFirePosition;

        public abstract void Fire(Vector3 destinationPoint);
    }
}

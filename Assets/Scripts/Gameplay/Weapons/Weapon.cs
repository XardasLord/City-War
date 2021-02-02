using Mirror;
using UnityEngine;

namespace Gameplay.Weapons
{
    public abstract class Weapon : NetworkBehaviour
    {
        [Header("Projectile details")]
        public float weaponProjectileSpeed = 15.0f;
        public float weaponProjectileLifeInSeconds = 3f;
        public Projectile weaponProjectile;

        [Header("Weapon details")]
        public int weaponDamage = 10;
        public float weaponCooldown = 1.0f;
        public int weaponAmmo = 15;
        public Transform weaponFirePosition;
        public MeshRenderer[] weaponRenderer;

        [Header("Weapon Effects")] 
        public ParticleSystem weaponFireEffect;

        public abstract void Fire(Vector3 destinationPoint);
    }
}

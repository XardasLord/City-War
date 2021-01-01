using Mirror;
using UnityEngine;

namespace Gameplay.Weapons
{
    public abstract class Weapon : NetworkBehaviour
    {
        public float weaponSpeed = 15.0f;
        public float weaponLife = 3.0f;
        public float weaponCooldown = 1.0f;
        public int weaponAmmo = 15;

        public GameObject weaponBullet;
        public Transform weaponFirePosition;

        public abstract void Fire(Vector3 destinationPoint);
    }
}

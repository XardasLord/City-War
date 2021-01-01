using Mirror;
using UnityEngine;

namespace Gameplay.Weapons
{
    public class RifleWeapon : Weapon
    {
        [Server]
        public override void Fire(Vector3 destinationPoint)
        {
            Debug.Log("Fire from RifleWeapon script!");
            weaponAmmo -= 1;

            var bullet = Instantiate(weaponBullet, weaponFirePosition.position, weaponFirePosition.rotation);

            bullet.transform.LookAt(destinationPoint); // TODO: Bullet responsibility
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * weaponSpeed, ForceMode.VelocityChange);

            NetworkServer.Spawn(bullet);

            if (bullet)
                Destroy(bullet, weaponLife); // TODO: Destroy on server
        }
    }
}

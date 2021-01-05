using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Weapons
{
    public class ClientPlayerWeaponHandler : NetworkBehaviour
    {
        [SerializeField] private Weapon activeWeapon;
        [SerializeField] private List<Weapon> availableWeapons;

        private float _weaponCooldownTime;
        private Camera _camera;

        public override void OnStartLocalPlayer() 
            => enabled = !isServer;

        public void OnFire(InputAction.CallbackContext context)
        {
            if (!hasAuthority || !isLocalPlayer || isServer)
                return;

            if (context.performed)
            {
                if (CanShoot())
                {
                    var screenCenterRay = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

                    Vector3 targetPoint;
                    if (Physics.Raycast(screenCenterRay, out var hit))
                        targetPoint = hit.point;
                    else
                        targetPoint = screenCenterRay.GetPoint(1000);

                    CmdFire(targetPoint);
                }
            }
        }

        [ClientCallback]
        private void Awake() 
            => _camera = Camera.main;

        #region Server

        [Command]
        public void CmdFire(Vector3 targetPoint)
        {
            if (!CanShoot())
                return;
            
            _weaponCooldownTime = Time.time + activeWeapon.weaponCooldown;

            activeWeapon.Fire(targetPoint);
        }

        #endregion

        private bool CanShoot()
            => activeWeapon != null &&
               activeWeapon.weaponAmmo > 0 &&
               Time.time > _weaponCooldownTime;
    }
}

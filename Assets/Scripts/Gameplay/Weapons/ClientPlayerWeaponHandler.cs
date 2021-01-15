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

        [SyncVar(hook = nameof(OnWeaponChanged))] 
        public int activeWeaponIndex = 0;

        private float _weaponCooldownTime;
        private Camera _camera;

        public override void OnStartLocalPlayer() 
            => enabled = !isServer;

        #region InputAction Events

        public void OnFire(InputAction.CallbackContext context)
        {
            if (!hasAuthority || !isLocalPlayer || isServer)
                return;

            if (!context.performed)
                return;

            if (!CanShoot())
                return;

            var targetPoint = GetTargetShootPoint();
            CmdFire(targetPoint);
        }

        public void OnWeaponSwitch(InputAction.CallbackContext context)
        {
            if (!hasAuthority || !isLocalPlayer || isServer)
                return;

            if (!context.performed)
                return;

            if (!CanSwitchWeapon())
                return;

            var direction = context.ReadValue<Vector2>().y;

            if (direction == 0f)
                return; // I have no idea why this event is triggered always twice

            var weaponSwitchDirection = Mathf.Sign(direction);
            if (weaponSwitchDirection == 1f)
            {
                CmdSwitchToNextWeapon();
            }
            else
            {
                Debug.Log("Previous weapon");
            }
        }

        #endregion


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

        [Command]
        public void CmdSwitchToNextWeapon()
        {
            if (!CanSwitchWeapon())
                return;

            var currentWeaponIndex = availableWeapons.IndexOf(activeWeapon);

            if (currentWeaponIndex == availableWeapons.Count - 1)
                activeWeaponIndex = 0;
            else
                activeWeaponIndex = currentWeaponIndex + 1;

            Debug.Log($"Weapon changed to: {activeWeapon.name}");
        }

        #endregion

        #region Client

        private void OnWeaponChanged(int oldIndex, int newIndex)
        {
            Debug.Log("SyncVar activeWeaponIndex changed");

            activeWeapon.weaponRenderer.enabled = false;

            activeWeapon = availableWeapons[newIndex];

            // Enable renderer for new weapon
            activeWeapon.weaponRenderer.enabled = true;

            //// disable old weapon
            //// in range and not null
            //if (0 < _Old && _Old < weaponArray.Length && weaponArray[_Old] != null)
            //{
            //    weaponArray[_Old].SetActive(false);
            //}

            //// enable new weapon
            //// in range and not null
            //if (0 < _New && _New < weaponArray.Length && weaponArray[_New] != null)
            //{
            //    weaponArray[_New].SetActive(true);
            //}
        }

        #endregion

        private bool CanShoot()
            => activeWeapon != null &&
               activeWeapon.weaponAmmo > 0 &&
               Time.time > _weaponCooldownTime;

        private Vector3 GetTargetShootPoint()
        {
            var screenCenterRay = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            Vector3 targetPoint;
            if (Physics.Raycast(screenCenterRay, out var hit))
                targetPoint = hit.point;
            else
                targetPoint = screenCenterRay.GetPoint(1000);

            return targetPoint;
        }

        private bool CanSwitchWeapon() 
            => availableWeapons.Count > 1;
    }
}

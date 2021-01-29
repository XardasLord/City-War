using Mirror;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Gameplay.UI
{
    public class ClientPlayerWeaponUI : NetworkBehaviour
    {
        [SerializeField] private GameObject weaponInformation;
        [SerializeField] private TMP_Text weaponNameText;

        public override void OnStartLocalPlayer()
            => weaponInformation.SetActive(!isServer);

        public void ChangeWeaponName(string weaponName)
            => weaponNameText.text = weaponName;
    }
}

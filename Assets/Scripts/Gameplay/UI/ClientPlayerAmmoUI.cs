using Mirror;
using TMPro;
using UnityEngine;

namespace Gameplay.UI
{
    public class ClientPlayerAmmoUI : NetworkBehaviour
    {
        [SerializeField] private GameObject ammoInformation;
        [SerializeField] private TMP_Text availableAmmoText;

        public override void OnStartLocalPlayer() 
            => ammoInformation.SetActive(!isServer);

        public void ChangeAvailableAmmo(int availableAmmo) 
            => availableAmmoText.text = availableAmmo.ToString();
    }
}

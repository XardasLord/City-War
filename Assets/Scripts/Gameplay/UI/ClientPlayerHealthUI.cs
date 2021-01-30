using Mirror;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Gameplay.UI
{
	public class ClientPlayerHealthUI : NetworkBehaviour
	{
		[SerializeField] private GameObject healthInformation;
		[SerializeField] private TMP_Text healthTextValue;

		public override void OnStartLocalPlayer()
			=> healthInformation.SetActive(!isServer);

		public void ChangeHealthValue(int health)
			=> healthTextValue.text = $"{health}%";
	}
}

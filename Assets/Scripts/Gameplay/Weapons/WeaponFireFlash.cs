using Mirror;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Weapons
{
	public class WeaponFireFlash : NetworkBehaviour
	{
		[SerializeField] private float destroyAfterSeconds = 0.1f;

		public override void OnStartServer() 
			=> Invoke(nameof(DestroySelf), destroyAfterSeconds);

		[Server]
		private void DestroySelf()
			=> NetworkServer.Destroy(gameObject);
	}
}

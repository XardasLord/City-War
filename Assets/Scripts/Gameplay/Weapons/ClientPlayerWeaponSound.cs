using Mirror;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Weapons
{
	public class ClientPlayerWeaponSound : NetworkBehaviour
	{
		private AudioSource _audioSource;

		public override void OnStartLocalPlayer()
		{
			if (isLocalPlayer)
				_audioSource = GetComponent<AudioSource>();
		}

		public void PlayFireSound(AudioClip fireAudioClip) 
			=> _audioSource.PlayOneShot(fireAudioClip);
	}
}

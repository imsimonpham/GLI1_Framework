using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private AudioClip _scopeInSound;
    [SerializeField] private AudioSource _audioSource;

    public void PlayScopeInSound()
    {
        _audioSource.PlayOneShot(_scopeInSound, 1f);
    }

    public void StopScopeInSound()
    {
        _audioSource.Stop();
    }
}

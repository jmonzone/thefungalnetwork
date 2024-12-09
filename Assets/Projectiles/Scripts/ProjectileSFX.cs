using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Projectile))]
[RequireComponent(typeof(AudioSource))]
public class ProjectileSFX : MonoBehaviour
{
    [Serializable]
    public class SoundEffect
    {
        public AudioClip audioClip;
        public float pitch;
    }

    [SerializeField] private List<SoundEffect> sounds;

    private void Awake()
    {
        var audioSource = GetComponent<AudioSource>();
        var randomIndex = UnityEngine.Random.Range(0, sounds.Count);
        var randomSound = sounds[randomIndex];
        audioSource.clip = randomSound.audioClip;
        audioSource.pitch = randomSound.pitch;
        audioSource.Play();

        var projectile = GetComponent<Projectile>();
        projectile.OnDissipateUpdate += time =>
        {
            audioSource.volume -= Time.deltaTime;
        };
    }
}

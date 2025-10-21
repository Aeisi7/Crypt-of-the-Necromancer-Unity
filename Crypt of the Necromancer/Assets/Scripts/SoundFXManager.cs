using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance; // create soundFXmanager singleton

    [SerializeField] private AudioSource soundFXObject;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // spawn in gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        // assign the audioclip
        audioSource.clip = audioClip;

        // assign the volume
        audioSource.volume = volume;

        // play sound
        audioSource.Play();

        // get length of sond fx clip
        float clipLength = audioSource.clip.length;

        // Debug.Log($"Destroying in {clipLength} seconds for {audioSource.clip.name}");

        // destroy clip after playing
        Destroy(audioSource.gameObject, clipLength );
    }
}

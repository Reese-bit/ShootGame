using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : PersistenSingleton<AudioManager>
{
    [SerializeField] private AudioSource SFXPlayer;

    private const float MIN_PICTH = 0.9f;
    private const float MAX_PITCH = 1.1f;

    //suitable for playing UI SFX
    public void PlaySFX(AudioData audioData)
    {
        SFXPlayer.PlayOneShot(audioData.audioClip,audioData.volumn);
    }

    //suitable for playing repeat-play SFX
    public void PlayRandomSFX(AudioData audioData)
    {
        SFXPlayer.pitch = Random.Range(MIN_PICTH, MAX_PITCH);
        PlaySFX(audioData);
    }

    public void PlayRandomSFX(AudioData[] audioDatas)
    {
        PlayRandomSFX(audioDatas[Random.Range(0,audioDatas.Length)]);
    }
}

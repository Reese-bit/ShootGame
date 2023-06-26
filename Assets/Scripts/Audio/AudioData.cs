using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioData
{
    public AudioClip audioClip;
    
    [Range(0,1)]
    public float volumn;
}

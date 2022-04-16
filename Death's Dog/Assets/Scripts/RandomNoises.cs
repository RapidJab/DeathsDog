using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Random Noises")]
public class RandomNoises : ScriptableObject
{ 
    public AudioClip[] platformerSongs;
    public AudioClip[] memorySongs;
    public AudioClip[] fallingPlatformSounds;
}

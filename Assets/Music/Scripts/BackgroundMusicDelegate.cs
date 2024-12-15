using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class BackgroundMusicDelegate : ScriptableObject
{
    public event UnityAction<AudioClip> OnMusicRequested;

    public void RequestMusic(AudioClip audioClip)
    {
        OnMusicRequested?.Invoke(audioClip);
    }
}

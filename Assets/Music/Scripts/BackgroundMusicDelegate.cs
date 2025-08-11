using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class BackgroundMusicDelegate : ScriptableObject
{
    public event UnityAction<AudioClip> OnMusicRequested;
    public event UnityAction OnMusicHide;

    public void RequestMusic(AudioClip audioClip)
    {
        OnMusicRequested?.Invoke(audioClip);
    }

    public void HideMusic()
    {
        OnMusicHide?.Invoke();
    }
}

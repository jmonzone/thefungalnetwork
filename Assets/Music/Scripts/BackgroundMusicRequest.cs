using UnityEngine;

public class BackgroundMusicRequest : MonoBehaviour
{
    [SerializeField] private BackgroundMusicDelegate backgroundMusicReference;
    [SerializeField] private AudioClip audioClip;

    private void Start()
    {
        backgroundMusicReference.RequestMusic(audioClip);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class AudioUtility
{
    public static AudioSource PlayClip(AudioClip clip, float volume)
    {
        var go = new GameObject("OneShotAudio " + clip.name);
        var source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.Play();
        Object.Destroy(go, clip.length);
        return source;
    }
}
public class ButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI text;

    private AudioSource audioSource;

    private void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Initialize(string label, UnityAction action)
    {
        //Debug.Log($"{text} {label}");
        //Debug.Log($"{text.text}");
        if (!text) text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = label;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            //Debug.Log("clicked " + action == null);
            action?.Invoke();
            OnClick();
        });
    }

    private void OnClick()
    {
        AudioUtility.PlayClip(audioSource.clip, audioSource.volume);
    }
}

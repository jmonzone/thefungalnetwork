using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(AudioSource))]
public class ButtonSFX : MonoBehaviour
{
    private void Awake()
    {
        var button = GetComponent<Button>();
        var audioSource = GetComponent<AudioSource>();

        button.onClick.AddListener(() =>
        {   
            audioSource.Play();
        });
    }
}

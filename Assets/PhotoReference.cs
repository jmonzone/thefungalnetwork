using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class PhotoReference : ScriptableObject
{
    [Header("References")]
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference cameraView;
    [SerializeField] private ViewReference screenshotView;

    [Header("Runtime")]
    [SerializeField] private Texture texture;

    public Texture Texture => texture;

    public event UnityAction OnPhotoStart;
    public event UnityAction OnPhotoExit;
    public event UnityAction OnPhotoTaken;

    public void StartPhotoView()
    {
        navigation.Navigate(cameraView);
        OnPhotoStart?.Invoke();
    }

    public void ExitPhotoView()
    {
        navigation.GoBack();
        OnPhotoExit?.Invoke();
    }

    public void TakePhoto(Texture texture)
    {
        this.texture = texture;
        navigation.Navigate(screenshotView);
        OnPhotoTaken?.Invoke();
    }
}

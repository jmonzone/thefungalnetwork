using System.Collections.Generic;
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
    [SerializeField] private Texture lastPhoto;
    [SerializeField] private List<Texture> allPhotos;
    
    public Texture LastPhoto => lastPhoto;
    public List<Texture> AllPhotos => allPhotos;

    public event UnityAction OnPhotoStart;
    public event UnityAction OnPhotoExit;
    public event UnityAction OnPhotoTaken;

    public void Initialize()
    {
        allPhotos = new List<Texture>();
    }

    public void StartPhotoView()
    {
        allPhotos = new List<Texture>();

        navigation.Navigate(cameraView);
        OnPhotoStart?.Invoke();
    }

    public void ExitPhotoView()
    {
        navigation.GoBack();
        OnPhotoExit?.Invoke();
    }

    public void TakePhoto(Texture photo)
    {
        lastPhoto = photo;
        allPhotos.Add(photo);
        navigation.Navigate(screenshotView);
        OnPhotoTaken?.Invoke();
    }
}

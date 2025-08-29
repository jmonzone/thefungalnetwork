using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoScreenshotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PhotoReference photoReference;
    [SerializeField] private Navigation navigation;
    [SerializeField] private RawImage screenshotImage;
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private Button keepButton;

    private void Awake()
    {
        tryAgainButton.onClick.AddListener(() =>
        {
            navigation.GoBack();
        });

        keepButton.onClick.AddListener(() =>
        {
            navigation.GoBack();
        });
    }

    private void OnEnable()
    {
        photoReference.OnPhotoTaken += PhotoReference_OnPhotoTaken;
    }

    private void PhotoReference_OnPhotoTaken()
    {
        screenshotImage.texture = photoReference.Texture;
    }
}

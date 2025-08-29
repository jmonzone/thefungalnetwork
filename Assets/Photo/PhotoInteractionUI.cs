using UnityEngine;

public class PhotoInteractionUI : MonoBehaviour
{
    [SerializeField] private PhotoReference photoReference;
    [SerializeField] private PassTheSpore passTheSpore;
    [SerializeField] private PhotoInteractionButton photoInteractionButton;

    private void Awake()
    {
        passTheSpore.OnGameStart += PassTheSpore_OnGameStart;
        passTheSpore.OnGameComplete += PassTheSpore_OnGameComplete;
        photoInteractionButton.OnClick += PhotoInteractionButton_OnClick;
    }

    private void PassTheSpore_OnGameStart()
    {
        StartCoroutine(photoInteractionButton.Show(passTheSpore.AnchorPosition));
    }

    private void PassTheSpore_OnGameComplete()
    {
        StartCoroutine(photoInteractionButton.Hide());
    }

    private void PhotoInteractionButton_OnClick()
    {
        photoReference.StartPhotoView();
        StartCoroutine(photoInteractionButton.Hide());
    }
}
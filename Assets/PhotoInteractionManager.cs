using UnityEngine;

public class PhotoInteractionManager : MonoBehaviour
{
    [SerializeField] private PassTheSpore passTheSpore;
    [SerializeField] private PhotoInteractionButton photoInteractionButton;

    private void Awake()
    {
        passTheSpore.OnGameStart += PassTheSpore_OnGameStart;
    }

    private void PassTheSpore_OnGameStart()
    {
        StartCoroutine(photoInteractionButton.Show(passTheSpore.AnchorPosition));
    }
}

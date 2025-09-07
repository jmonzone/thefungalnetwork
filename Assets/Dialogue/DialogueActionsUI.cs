using UnityEngine;
using UnityEngine.UI;

public class DialogueActionsUI : MonoBehaviour
{
    [SerializeField] private DialogueReference dialogueReference;
    [SerializeField] private PhotoReference photoReference;

    [SerializeField] private Button chatButton;
    [SerializeField] private Button photoButton;

    private void Awake()
    {
        chatButton.onClick.AddListener(dialogueReference.StartChat);
        photoButton.onClick.AddListener(StartPhoto);
    }

    private void StartPhoto()
    {
        photoReference.SetLookTarget(dialogueReference.Unit.transform);
        photoReference.StartPhotoView();
    }
}

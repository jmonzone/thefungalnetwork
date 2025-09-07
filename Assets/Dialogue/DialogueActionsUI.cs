using UnityEngine;
using UnityEngine.UI;

public class DialogueActionsUI : MonoBehaviour
{
    [SerializeField] private DialogueReference dialogueReference;

    [SerializeField] private Button chatButton;
    [SerializeField] private Button photoButton;
    [SerializeField] private Button giveButton;
    [SerializeField] private Button followButton;

    private void Awake()
    {
        chatButton.onClick.AddListener(dialogueReference.StartChat);
        photoButton.onClick.AddListener(dialogueReference.StartPhoto);
        giveButton.onClick.AddListener(dialogueReference.StartGive);
        followButton.onClick.AddListener(dialogueReference.StartFollow);
    }

   
}

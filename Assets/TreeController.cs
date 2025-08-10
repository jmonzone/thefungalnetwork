using Cinemachine;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private Animator eyeballAnimator;
    [SerializeField] private DialogueReference dialogue;

    [SerializeField] private bool isSelected = false;

    public bool IsSelected => isSelected;

    private void Awake()
    {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        eyeballAnimator = GetComponentInChildren<Animator>(true);

        dialogue.OnDialogeClosed += Dialogue_OnDialogeClosed;
    }

    private void Dialogue_OnDialogeClosed()
    {
        Debug.Log("Unselected");

        virtualCamera.Priority = 0;
        eyeballAnimator.gameObject.SetActive(false);
        eyeballAnimator.enabled = false;

        Invoke(nameof(Unselect), 2f);
    }

    private void Unselect()
    {
        isSelected = false;
    }

    public void OnSelect()
    {
        Debug.Log("Selected");
        virtualCamera.Priority = 100;
        eyeballAnimator.gameObject.SetActive(true);
        eyeballAnimator.enabled = true;
        dialogue.SetSpeaker("The Tree");
        dialogue.SetDialogue("Where's the party??? what happened. We have to bring it back.");
        dialogue.ShowDialogue();
        isSelected = true;
    }
}

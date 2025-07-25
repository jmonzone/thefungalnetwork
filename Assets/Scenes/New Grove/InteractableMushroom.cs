using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InteractableMushroom : MonoBehaviour, IInteractable, ICollectable
{
    public ScaleController scaleController;
    public ScaleController indicatorScaleController;

    public float respawnDelay = 5f;

    private bool isInteractable = true;

    public event UnityAction OnInteractionStart;
    public event UnityAction OnInteractionComplete;
    public event UnityAction OnCollect;


    public void OnBaseInteraction()
    {
        if (!isInteractable) return;

        isInteractable = false;
        StartCoroutine(HandleInteraction());
    }

    private IEnumerator HandleInteraction()
    {
        OnInteractionStart?.Invoke();
        yield return scaleController.ScaleDown();
        

        OnInteractionComplete?.Invoke();
        OnCollect?.Invoke();
        yield return new WaitForSeconds(respawnDelay);
        yield return scaleController.ScaleUp();


        isInteractable = true;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InteractableMushroom : MonoBehaviour, IInteractable, ICollectable
{
    public ScaleController scaleController;

    public float respawnDelay = 5f;

    private bool isInteractable = true;

    public bool IsInteractable => isInteractable;
    public Transform Transform => transform;

    public event UnityAction OnCollect;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnBaseInteraction()
    {
        if (!isInteractable) return;

        isInteractable = false;
        StartCoroutine(HandleInteraction());
    }

    private IEnumerator HandleInteraction()
    {
        yield return scaleController.ScaleDown();

        OnCollect?.Invoke();
        audioSource.Play();

        yield return new WaitForSeconds(respawnDelay);
        yield return scaleController.ScaleUp();

        isInteractable = true;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TarotCardUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Transform front;
    [SerializeField] private Transform back;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private bool isFaceDown = true;

    private void Awake()
    {
        //button.onClick.AddListener(OnClick);
    }

    public void StartFlipCard(UnityAction onComplete)
    {
        StartCoroutine(FlipCardRoutine(onComplete));
    }

    private IEnumerator FlipCardRoutine(UnityAction onComplete)
    {
        var i = 0f;
        while(i < 90f)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            i += rotationSpeed * Time.deltaTime;

            yield return null;
        }

        FlipCard();

        while (i < 180f)
        {
            transform.Rotate(Vector3.down, rotationSpeed * Time.deltaTime);
            i += rotationSpeed * Time.deltaTime;

            yield return null;
        }

        onComplete?.Invoke();
    }

    private void FlipCard()
    {
        isFaceDown = !isFaceDown;

        front.gameObject.SetActive(!isFaceDown);
        back.gameObject.SetActive(isFaceDown);
    }
}

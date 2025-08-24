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

    public void Reset()
    {
        FlipCard(true);
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

        FlipCard(!isFaceDown);

        while (i < 180f)
        {
            transform.Rotate(Vector3.down, rotationSpeed * Time.deltaTime);
            i += rotationSpeed * Time.deltaTime;

            yield return null;
        }

        onComplete?.Invoke();
    }

    private void FlipCard(bool isFaceDown)
    {
        this.isFaceDown = isFaceDown;

        front.gameObject.SetActive(!this.isFaceDown);
        back.gameObject.SetActive(this.isFaceDown);
    }
}

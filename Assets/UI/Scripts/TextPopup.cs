using TMPro;
using UnityEngine;

public class TextPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private float timer = 0;

    public void ShowText(string text)
    {
        timer = 0;
        gameObject.SetActive(true);
        this.text.text = text;
    }

    private void Update()
    {
        if (timer > 1f) gameObject.SetActive(false);
        else
        {
            timer += Time.deltaTime;
            transform.position += 100 * Time.deltaTime * Vector3.up;
        }
    }
}
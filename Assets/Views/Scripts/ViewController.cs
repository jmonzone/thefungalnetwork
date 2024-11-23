using UnityEngine;
using UnityEngine.UI;

public class ViewController : MonoBehaviour
{
    [SerializeField] private ViewReference viewReference;
    [SerializeField] private GameObject canvas;

    private void Awake()
    {
        viewReference.OnOpened += () =>
        {
            canvas.SetActive(true);
        };

        viewReference.OnClosed += () =>
        {
            canvas.SetActive(false);
        };

        canvas.SetActive(false);
    }
}

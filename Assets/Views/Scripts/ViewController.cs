using UnityEngine;

public class ViewController : MonoBehaviour
{
    [SerializeField] private ViewReference viewReference;
    [SerializeField] private GameObject canvas;

    private void OnEnable()
    {
        viewReference.OnOpened += Show;
        viewReference.OnClosed += Hide;
    }

    private void OnDisable()
    {
        viewReference.OnOpened -= Show;
        viewReference.OnClosed -= Hide;
    }

    private void Show()
    {
        canvas.SetActive(true);
    }

    private void Hide()
    {
        canvas.SetActive(false);
    }
}

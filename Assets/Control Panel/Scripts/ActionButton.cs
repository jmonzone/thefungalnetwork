using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image actionImage;
    [SerializeField] private Image background;

    private bool isVisible;

    private Vector3 hiddenPosition;
    private Vector3 visiblePosition;

    public event UnityAction OnClicked;

    private void Awake()
    {
        button.onClick.AddListener(() => OnClicked?.Invoke());
        visiblePosition = transform.position;
        hiddenPosition = transform.position + Vector3.left * 500f;
        Debug.Log($"{visiblePosition} {hiddenPosition}");
    }

    public void SetInteraction(Sprite sprite, Color color)
    {
        actionImage.sprite = sprite;
        background.color = color;
    }

    public void SetVisible(bool value)
    {
        isVisible = value;
    }

    private void Update()
    {
        var targetPosition = isVisible ? visiblePosition : hiddenPosition;
        var direction = targetPosition - transform.position;
        if (direction.magnitude > 10f)
        {
            direction.y = 0;
            transform.position += 5f * Time.deltaTime * direction + direction.normalized;
        }
        else transform.position = targetPosition;
    }

}

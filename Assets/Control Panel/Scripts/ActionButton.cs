using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    [SerializeField] private Image actionImage;
    [SerializeField] private Image background;

    private bool isVisible;

    private Vector3 hiddenPosition;
    private Vector3 visiblePosition;

    public void Initialize(Sprite sprite, Color color)
    {
        actionImage.sprite = sprite;
        background.color = color;

        visiblePosition = transform.position;
        hiddenPosition = transform.position + Vector3.left * 500f;
    }

    public void SetVisible(bool value)
    {
        isVisible = value;
    }

    private void Update()
    {
        var targetPosition = isVisible ? visiblePosition : hiddenPosition;
        var direction = targetPosition - transform.position;
        if (direction.magnitude > 0.05f)
        {
            transform.position += 5f * Time.deltaTime * direction + direction.normalized;
        }
        else transform.position = targetPosition;
    }

}

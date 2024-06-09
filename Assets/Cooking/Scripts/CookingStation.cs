using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class CookingStation : EntityController
{
    [SerializeField] private Sprite actionImage;
    [SerializeField] private Color actionColor;
    [SerializeField] private CinemachineVirtualCamera camera;
    [SerializeField] private IngredientManager ingredientManager;
    [SerializeField] private SlideAnimation controlPanel;
    [SerializeField] private SlideAnimation cookingUIAnimation;
    [SerializeField] private Button backButton;
    [SerializeField] private Transform playerAnchor;
    [SerializeField] private MovementController playerController;

    [Header("Background References")]
    [SerializeField] private Image background;

    public override Sprite ActionImage => actionImage;

    public override Color ActionColor => actionColor;

    public override string ActionText => "Cook";

    private void Awake()
    {
        backButton.onClick.AddListener(() =>
        {
            StartCoroutine(LerpAlpha(0, () =>
            {
                camera.Priority = 0;
                controlPanel.IsVisible = true;
                cookingUIAnimation.IsVisible = false;
                ingredientManager.StopAllCoroutines();
                playerController.enabled = false;
            }));
        });
    }

    public override void UseAction()
    {
        camera.Priority = 2;
        controlPanel.IsVisible = false;
        playerController.enabled = true;
        playerController.SetTarget(playerAnchor);

        StartCoroutine(ShowBackground());
    }

    private IEnumerator ShowBackground()
    {
        yield return new WaitForSeconds(2);
        yield return LerpAlpha(0.75f);
        cookingUIAnimation.IsVisible = true;
        ingredientManager.SpawnIngredients();
    }

    private IEnumerator LerpAlpha(float target, UnityAction onComplete = null)
    {
        var startColor = background.color;
        var targetColor = startColor;
        targetColor.a = target;

        var i = 0f;
        while (i < 1)
        {
            background.color = Color.Lerp(startColor, targetColor, i);
            i += Time.deltaTime;
            yield return null;
        }

        onComplete?.Invoke();
    }
}

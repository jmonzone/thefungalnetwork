using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class CookingStation : EntityController
{
    [Header("Gameplay References")]
    [SerializeField] private CinemachineVirtualCamera camera;
    [SerializeField] private IngredientManager ingredientManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BladeController bladeController;

    [Header("Action References")]
    [SerializeField] private Sprite actionImage;
    [SerializeField] private Color actionColor;

    [Header("UI References")]
    [SerializeField] private ControlPanel controlPanel;
    [SerializeField] private SlideAnimation cookingUIAnimation;
    [SerializeField] private Button backButton;
    [SerializeField] private Image background;

    [Header("Position References")]
    [SerializeField] private Transform playerPositionAnchor;
    [SerializeField] private Transform playerLookTarget;
    [SerializeField] private Transform fungalPositionAnchor;
    [SerializeField] private Transform fungalLookTarget;

    public override Sprite ActionImage => actionImage;
    public override Color ActionColor => actionColor;
    public override string ActionText => "Cook";

    private FungalController fungal;

    private void Awake()
    {
        backButton.onClick.AddListener(() =>
        {
            bladeController.enabled = false;

            StartCoroutine(LerpAlpha(0, () =>
            {
                camera.Priority = 0;
                controlPanel.SetVisible(true);
                cookingUIAnimation.IsVisible = false;
                ingredientManager.StopAllCoroutines();

                if (fungal) fungal.Stop();
            }));
        });
    }

    public override void UseAction()
    {
        camera.Priority = 2;
        controlPanel.SetVisible(false);

        fungal = controlPanel.EscortedFungal;
        if (fungal)
        {
            controlPanel.UnescortFungal();
            fungal.MoveToPosition(fungalPositionAnchor.position, fungalLookTarget);
        }

        playerController.Movement.SetPosition(playerPositionAnchor.position, () =>
        {
            playerController.Movement.SetLookTarget(playerLookTarget);
        });

        bladeController.enabled = true;

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

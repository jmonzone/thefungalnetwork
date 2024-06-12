using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class CookingStation : JobStation
{
    [Header("Gameplay References")]
    [SerializeField] private IngredientManager ingredientManager;
    [SerializeField] private BladeController bladeController;

    [Header("UI References")]
    [SerializeField] private Image background;

    [Header("Position References")]
    [SerializeField] private Transform fungalPositionAnchor;
    [SerializeField] private Transform fungalLookTarget;

    public override string ActionText => "Cook";

    protected override void OnJobStarted()
    {
        if (Fungal)
        {
            Fungal.MoveToPosition(fungalPositionAnchor.position, fungalLookTarget);
        }

        bladeController.enabled = true;

        StartCoroutine(ShowBackground());
    }

    protected override void OnBackButtonClicked()
    {
        StopAllCoroutines();
        bladeController.enabled = false;

        StartCoroutine(LerpAlpha(0, () =>
        {
            EndAction();
            ingredientManager.StopAllCoroutines();
        }));
    }

    private IEnumerator ShowBackground()
    {
        yield return new WaitForSeconds(2);
        yield return LerpAlpha(0.75f);
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

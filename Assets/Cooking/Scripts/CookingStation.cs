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

    public override void SetFungal(FungalController fungal)
    {
        if (fungal)
        {
            fungal.MoveToPosition(fungalPositionAnchor.position, fungalLookTarget);
        }
    }

    protected override void OnJobStarted()
    {
        bladeController.enabled = true;

        IEnumerator ShowBackground()
        {
            yield return new WaitForSeconds(2);
            yield return background.LerpAlpha(0.75f);
            ingredientManager.enabled = true;
        }

        StartCoroutine(ShowBackground());
    }

    protected override void OnJobEnded()
    {
    }

    protected override void OnBackButtonClicked()
    {
        StopAllCoroutines();
        bladeController.enabled = false;

        StartCoroutine(background.LerpAlpha(0, () =>
        {
            EndAction();
            ingredientManager.enabled = false;
        }));
    }


}

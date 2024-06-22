using UnityEngine;
using UnityEngine.UI;

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

    protected override void OnFungalChanged(FungalController fungal)
    {
        if (fungal)
        {
            fungal.Movement.SetPosition(fungalPositionAnchor.position, () =>
            {
                fungal.Movement.SetLookTarget(fungalLookTarget);
            });
        }
    }

    protected override void OnJobStarted()
    {
    }

    protected override void OnCameraPrepared()
    {
        bladeController.enabled = true;

        StartCoroutine(background.LerpAlpha(0.75f, () =>
        {
            ingredientManager.enabled = true;
        }));
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

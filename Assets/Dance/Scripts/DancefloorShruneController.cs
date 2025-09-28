using UnityEngine;

public class DancefloorShruneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private DancefloorAuraController auraController;

    private void Awake()
    {
        if (auraController != null)
            auraController.OnHit += AuraController_OnHit;
    }

    private void AuraController_OnHit()
    {

        Debug.Log("Earn shrune!");
    }
}

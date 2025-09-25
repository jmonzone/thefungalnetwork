using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancefloorShruneController : MonoBehaviour
{
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private DancefloorAuraController auraController;
    [SerializeField] private DancefloorShruneUI shruneUI;
    [SerializeField] private float progress = 0;
    [SerializeField] private float maxProgress = 8;

    private void Awake()
    {
        auraController.OnHit += AuraController_OnHit;
    }

    private void AuraController_OnHit()
    {
        progress++;

        shruneUI.UpdateUI(progress, 0, maxProgress);

        if (progress >= maxProgress)
        {
            Debug.Log("Earn shrune");
            progress = 0;
        }


    }
}

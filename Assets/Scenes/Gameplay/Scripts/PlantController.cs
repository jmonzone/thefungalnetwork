using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantController : MonoBehaviour
{
    [SerializeField] private GameObject plantStage1;
    [SerializeField] private GameObject plantStage2;

    private void Awake()
    {
        var buildController = GetComponent<BuildController>();
        buildController.OnPlaced += BuildController_OnBuildComplete;
    }

    private void BuildController_OnBuildComplete()
    {
        if (plantStage1) StartCoroutine(GrowthPhase());
    }

    private IEnumerator GrowthPhase()
    {
        yield return new WaitForSeconds(10f);
        plantStage1.SetActive(false);
        plantStage2.SetActive(true);
    }
}

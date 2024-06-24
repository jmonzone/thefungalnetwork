using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EggSelection : MonoBehaviour
{
    [SerializeField] private GameObject instructions;

    public event UnityAction<EggController> OnEggSelected;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        instructions.SetActive(true);
    }

    private void OnDisable()
    {
        instructions.SetActive(false);
    }

    public void SetPets(List<FungalData> pets)
    {
        var eggControllers = GetComponentsInChildren<EggController>().ToList();

        for (var i = 0; i < eggControllers.Count && i < pets.Count; i++)
        {
            var egg = eggControllers[i];
            egg.Initialize(pets[i]);
            egg.OnHatch += () => OnEggSelected?.Invoke(egg);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 50f))
            {
                var egg = hit.transform.GetComponentInParent<EggController>();
                if (egg)
                {
                    egg.Hatch();
                    enabled = false;
                }
            }
        }
    }
}

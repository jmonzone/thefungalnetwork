using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EggSelection : MonoBehaviour
{
    private Camera mainCamera;

    private List<EggController> eggControllers;

    public event UnityAction<PetData> OnEggSelected;

    private void Awake()
    {
        mainCamera = Camera.main;
        eggControllers = GetComponentsInChildren<EggController>().ToList();
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
                    egg.Jump();
                    enabled = false;
                    OnEggSelected?.Invoke(egg.Pet);
                }
            }
        }
    }

    public void SetPets(List<PetData> pets)
    {
        for(var i = 0; i < eggControllers.Count && i < pets.Count; i++)
        {
            eggControllers[i].SetPet(pets[i]);
        }
    }
}

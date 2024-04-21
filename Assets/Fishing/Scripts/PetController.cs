using System.Collections.Generic;
using UnityEngine;

public class PetController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float idleRadius = 4f;
    [SerializeField] private float autoFishCooldown = 4f;
    [SerializeField] private float flightHeight = 5f;
    [SerializeField] private Vector3 offset;

    [Header("References")]
    [SerializeField] private GameObject placeholder;

    private Vector3 origin;
    private float timer;
    private FishController targetFish;
    private List<FishController> fish;

    private void Awake()
    {
        Destroy(placeholder);
        placeholder = null;

        origin = transform.position;
    }

    public void SetPet(Pet pet)
    {
        if (pet)
        {
            var petObject = Instantiate(pet.Prefab, transform);
            petObject.transform.localScale = Vector3.one * 3f;

            if (pet.Type == PetType.SKY) origin.y = 5;
        }
    }

    public void SetFish(List<FishController> fish)
    {
        this.fish = fish;
    }

    private Vector3 TargetPosition
    {
        get
        {
            if (targetFish)
            {
                return targetFish.transform.position;
            }
            else
            {
                var x = Mathf.Cos(timer);
                var z = Mathf.Sin(timer);
                return origin + offset - new Vector3(x, 0, z) * idleRadius;
            }
        }
    }

    private void Update()
    {
        var direction = TargetPosition - transform.position;
        if (direction.magnitude > 0.5f)
        {
            transform.position += speed * Time.deltaTime * direction.normalized;
            transform.forward = Vector3.Lerp(transform.forward, direction, rotationSpeed * Time.deltaTime);
        }
        else if (targetFish)
        {
            targetFish.Catch();
            targetFish = null;
            timer = 0;
        }

        if (!targetFish && fish.Count > 0 && timer > autoFishCooldown)
        {
            targetFish = fish[0];
        }

        timer += Time.deltaTime;
    }
}

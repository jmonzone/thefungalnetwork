using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FungalManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PositionAnchor positionAnchor;
    [SerializeField] private EggSelection eggSelection;

    [Header("Assets")]
    [SerializeField] private FungalController fungalControllerPrefab;
    [SerializeField] private EggController eggControllerPrefab;

    public List<FungalController> FungalControllers { get; private set; } = new List<FungalController>();

    public List<FungalModel> Fungals => GameManager.Instance.Fungals;

    public FungalController TalkingFungal { get; private set; }
    public FungalController EscortedFungal { get; private set; }

    public static FungalManager Instance { get; private set; }

    public event UnityAction<FungalController> OnInteractionStarted;

    private void Awake()
    {
        Instance = this;
    }

    //private void Start()
    //{
    //    if (Fungals.Count == 0)
    //    {
    //        eggSelection.SetPets(GameData.Fungals.GetRange(0, 3));

    //        eggSelection.OnEggSelected += egg =>
    //        {
    //            OnEggHatched(egg);
    //            eggSelection.gameObject.SetActive(false);
    //        };

    //        eggSelection.gameObject.SetActive(true);
    //    }

    //    if (Fungals.Count == 1 && Fungals[0].Level >= 10)
    //    {
    //        var availableFungals = GameData.Fungals.Where(fungal => fungal != Fungals[0].Data).ToList();
    //        var randomIndex = Random.Range(0, availableFungals.Count);
    //        var secondFungal = availableFungals[randomIndex];
    //        SpawnEgg(secondFungal);
    //    }
    //}

    private void OnEggHatched(EggController egg)
    {
        var fungal = ScriptableObject.CreateInstance<FungalModel>();
        fungal.Initialize(egg.Fungal);
        GameManager.Instance.AddFungal(fungal);
        SpawnFungal(fungal, egg.transform.position);
    }

    private void SpawnEgg(FungalData fungal)
    {
        var randomPosition = (Vector3)Random.insideUnitCircle.normalized * 4;
        randomPosition.z = Mathf.Abs(randomPosition.y);
        randomPosition.y = 1;

        var eggController = Instantiate(eggControllerPrefab, randomPosition, Quaternion.identity);
        eggController.Initialize(fungal);
        eggController.OnHatch += () => OnEggHatched(eggController);
    }

    private void SpawnFungal(FungalModel fungal, Vector3 spawnPosition)
    {
        var fungalController = Instantiate(fungalControllerPrefab, spawnPosition, Quaternion.identity);
        fungalController.Initialize(fungal, positionAnchor.Bounds);
        fungalController.transform.forward = Utility.RandomXZVector;
        FungalControllers.Add(fungalController);
        fungalController.OnInteractionStarted += () => OnInteractionStarted?.Invoke(fungalController);
    }

    public void SpawnFungals()
    {
        Debug.Log("spawning fungals");

        foreach (var fungal in Fungals)
        {
            var randomPosition = positionAnchor.Position;
            SpawnFungal(fungal, randomPosition);
        }
    }

    public void EndFungalTalk()
    {
        if (TalkingFungal != EscortedFungal) TalkingFungal.Stop();
        TalkingFungal = null;
    }
}

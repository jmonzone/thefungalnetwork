using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum FungalState
{
    RANDOM,
    ESCORT,
    TARGET,
}

public class FungalController : MonoBehaviour
{
    [SerializeField] private PlayerReference player;

    public FungalModel Model { get; private set; }
    public MovementController Movement { get; private set; }

    public event UnityAction OnInitialized;

    private void Awake()
    {
        Movement = GetComponent<MovementController>();
    }

    //todo: centralize between GroveManager and Launcher
    public void Initialize(FungalModel model, bool isGrove = true)
    {
        Debug.Log($"initializing fungal controller {model}");

        Model = model;

        if (model)
        {
            name = $"Fungal Controller - {model.name}";
            var render = Instantiate(model.Data.Prefab, transform);
            render.transform.localScale = Vector3.one;

            InitializeAnimations();
        }
    }

    public void InitializeAnimations()
    {
        var animator = GetComponentInChildren<Animator>();
        animator.speed = 0.25f;

        //Movement.OnJump += () => animator.Play("Jump");

        //var movementAnimations = GetComponentInChildren<MovementAnimations>();
        //movementAnimations.Initalize();
    }
}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FungalInfoUI : MonoBehaviour
{
    [SerializeField] private Transform fungalModelAnchor;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button playButton;

    [Header("Fungal Stats")]
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI staminaText;
    [SerializeField] private TextMeshProUGUI powerText;

    private FungalInstance fungal;
    private GameObject fungalModelView;
    private Camera mainCamera;

    private void Awake()
    {
        playButton.onClick.AddListener(GoToFishingGameplay);
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 50f))
            {
                var pet = hit.transform.GetComponentInParent<Animator>();
                if (pet)
                {
                    pet.Play("Attack");
                    StartCoroutine(RotatePet(pet));
                }
            }
        }
    }

    public void SetFungal(FungalInstance fungal)
    {
        balanceText.text = fungal.Balance.ToString();
        speedText.text = fungal.Speed.ToString();
        staminaText.text = fungal.Stamina.ToString();
        powerText.text = fungal.Power.ToString();

        if (this.fungal == fungal) return;

        if (fungalModelView)
        {
            Destroy(fungalModelView);
            fungalModelView = null;
        }

        this.fungal = fungal;
        nameText.text = fungal.Data.Name;
        typeText.text = fungal.Data.Type.ToString();
        levelText.text = $"Level {fungal.Level}";

        if (!fungalModelView)
        {
            fungalModelView = Instantiate(fungal.Data.Prefab, fungalModelAnchor);
            var animator = fungalModelView.GetComponentInChildren<Animator>();
            animator.speed = 0.25f;
        }
    }

    private IEnumerator RotatePet(Animator pet)
    {
        while (Input.GetMouseButton(0))
        {
            var startPosition = Input.mousePosition;
            yield return new WaitForEndOfFrame();
            var endPosition = Input.mousePosition;

            var inputDirection = endPosition - startPosition;
            pet.transform.Rotate(Vector3.up, -inputDirection.x * Time.deltaTime * 10f);
        }

    }

    private void GoToFishingGameplay()
    {
        SceneParameters.FungalIndex = fungal.Index;
        SceneManager.LoadScene(1);
    }
}

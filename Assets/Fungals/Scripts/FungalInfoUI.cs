using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FungalInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button closeButton; 

    [Header("Fungal Stats")]
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI staminaText;
    [SerializeField] private TextMeshProUGUI powerText;

    private FungalController fungal;
    private Camera mainCamera;

    public event UnityAction OnClose;

    private void Awake()
    {
        mainCamera = Camera.main;
        closeButton.onClick.AddListener(() => OnClose?.Invoke());
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
                }
            }
        }
    }

    public void SetFungal(FungalController fungal)
    {
        if (this.fungal && this.fungal != fungal)
        {
            this.fungal.SpotlightCamera.gameObject.SetActive(false);

            var defaultLayer = LayerMask.NameToLayer("Default");
            this.fungal.Render.layer = defaultLayer;

            foreach (Transform child in this.fungal.Render.transform)
            {
                child.gameObject.layer = defaultLayer;
            }
        }

        this.fungal = fungal;

        if (this.fungal)
        {
            fungal.SpotlightCamera.gameObject.SetActive(true);

            var fungalLayer = LayerMask.NameToLayer("Fungal");
            fungal.Render.layer = fungalLayer;

            foreach (Transform child in fungal.Render.transform)
            {
                child.gameObject.layer = fungalLayer;
            }

            nameText.text = fungal.Model.Data.Id;
            typeText.text = fungal.Model.Data.Type.ToString();
            levelText.text = $"Level {fungal.Model.Level}";

            balanceText.text = fungal.Model.Balance.ToString();
            speedText.text = fungal.Model.Speed.ToString();
            staminaText.text = fungal.Model.Stamina.ToString();
            powerText.text = fungal.Model.Power.ToString();
        }
    }
}

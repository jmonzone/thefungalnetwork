using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FungalInfoUI : MonoBehaviour
{
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
                }
            }
        }
    }

    public void SetFungal(FungalInstance fungal)
    {
        this.fungal = fungal;
        nameText.text = fungal.Data.Name;
        typeText.text = fungal.Data.Type.ToString();
        levelText.text = $"Level {fungal.Level}";

        balanceText.text = fungal.Balance.ToString();
        speedText.text = fungal.Speed.ToString();
        staminaText.text = fungal.Stamina.ToString();
        powerText.text = fungal.Power.ToString();
    }

    private void GoToFishingGameplay()
    {
        SceneParameters.FungalIndex = fungal.Index;
        SceneManager.LoadScene(1);
    }
}

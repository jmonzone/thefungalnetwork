using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PetInfoManager : MonoBehaviour
{
    [SerializeField] private Transform petModelAnchor;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button playButton;

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

    public void SetFungal(FungalInstance fungal)
    {
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
            fungalModelView = Instantiate(fungal.Data.Prefab, petModelAnchor);
            var animator = fungalModelView.GetComponentInChildren<Animator>();
            animator.speed = 0.25f;
        }
    }

    private void GoToFishingGameplay()
    {
        SceneParameters.FungalIndex = fungal.Index;
        SceneManager.LoadScene(1);
    }
}

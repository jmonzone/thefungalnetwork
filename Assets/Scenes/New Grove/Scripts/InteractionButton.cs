using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractionButton : MonoBehaviour
{
    [SerializeField] private int cost = 20;
    [SerializeField] private bool unlocked = false;

    [SerializeField] private Button button;
    [SerializeField] private GameObject costContainer;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI interactionLabel;
    [SerializeField] private Image lockImage;


    public int Cost => cost;
    public bool Unlocked => unlocked;

    public event UnityAction OnInteractionClicked;
    public event UnityAction OnUnlocked;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            if (!unlocked)
            {
                unlocked = true;
                costContainer.SetActive(false);
                interactionLabel.gameObject.SetActive(true);
                lockImage.gameObject.SetActive(false);
                OnUnlocked?.Invoke();
            }

            if (unlocked)
            {
                OnInteractionClicked?.Invoke();
            }
            
        });

        SetCost(cost);
    }


    public void SetCost(int cost)
    {
        this.cost = cost;
        costText.text = cost.ToString();
    }

    public void SetInteractable(bool value)
    {
        button.interactable = value;
    }
}

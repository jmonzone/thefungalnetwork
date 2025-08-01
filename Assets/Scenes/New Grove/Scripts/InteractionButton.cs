using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractionButton : MonoBehaviour
{
    [SerializeField] private Interaction interaction;

    [SerializeField] private Button button;
    [SerializeField] private GameObject costContainer;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI interactionLabel;
    [SerializeField] private Image lockImage;

    public Interaction Interaction => interaction;

    public event UnityAction OnInteractionClicked;
    public event UnityAction OnUnlocked;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            if (!interaction.unlocked)
            {
                interaction.unlocked = true;
                UpdateView();
                OnUnlocked?.Invoke();
            }

            if (interaction.unlocked)
            {
                OnInteractionClicked?.Invoke();
            }
            
        });
    }

    public void SetInteraction(Interaction interaction)
    {
        this.interaction = interaction;
        button.interactable = interaction.unlocked;
        costText.text = interaction.cost.ToString();
        UpdateView();
    }

    private void UpdateView()
    {
        if (interaction == null) return;
        interactionLabel.gameObject.SetActive(interaction.unlocked);
        lockImage.gameObject.SetActive(!interaction.unlocked);
        costContainer.SetActive(!interaction.unlocked);

    }
}

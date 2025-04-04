using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FungalLivesUI : MonoBehaviour
{
    [SerializeField] private MultiplayerReference multiplayer;
    [SerializeField] private Color emptyColor;

    private List<Image> livesIndicator = new List<Image>();

    private NetworkFungal fungal;
    private Color defaultColor;

    private void Awake()
    {
        gameObject.SetActive(multiplayer.GameMode == GameMode.ELIMINATION);

        GetComponentsInChildren(livesIndicator);
        defaultColor = livesIndicator[0].color;

        fungal = GetComponentInParent<NetworkFungal>();
        fungal.OnLivesChanged += UpdateView;

        UpdateView();
    }

    private void UpdateView()
    {
        for(var i = 0; i < livesIndicator.Count; i++)
        {
            livesIndicator[i].color = fungal.Lives > i ? defaultColor : emptyColor;
        }
    }
}

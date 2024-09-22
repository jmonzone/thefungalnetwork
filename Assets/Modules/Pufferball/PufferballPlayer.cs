using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class PufferballPlayer : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject fungal;

    public void Awake()
    {
        var partner = GameManager.Instance.GetPartner();

        if (partner)
        {
            SetRender(fungal);
        }
        else
        {
            SetRender(player);
        }
    }

    private void SetRender(GameObject render)
    {
        render.SetActive(true);

        var animator = render.GetComponent<Animator>();

        var movementAnimations = GetComponent<MovementAnimations>();
        movementAnimations.SetAnimatior(animator);

        var networkAnimator = GetComponent<NetworkAnimator>();
        networkAnimator.Animator = animator;
    }
}

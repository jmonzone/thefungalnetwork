using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableFish : MonoBehaviour
{
    public GameObject render;

    public void OnClick()
    {
        render.SetActive(false);
    }
}

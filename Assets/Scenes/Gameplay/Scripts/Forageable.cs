using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forageable : MonoBehaviour
{
    [SerializeField] private int sporeCount = 1;

    public int SporeCount => sporeCount;
}

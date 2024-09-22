using UnityEngine;

public class RabbitHole : MonoBehaviour
{
    private void Start()
    {
        var proximityAction = GetComponent<ProximityAction>();
        proximityAction.OnUse += () => Utility.LoadScene("Pufferball");
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class ProximitySceneNavigation : MonoBehaviour
{
    [SerializeField] private int sceneIndex;

    private void Awake()
    {
        var proximityAction = GetComponent<ProximityAction>();
        proximityAction.OnUse += () => SceneManager.LoadScene(sceneIndex);
    }
}

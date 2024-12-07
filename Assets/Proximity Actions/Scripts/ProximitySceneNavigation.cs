using UnityEngine;
using UnityEngine.SceneManagement;

public class ProximitySceneNavigation : MonoBehaviour
{
    [SerializeField] private int sceneIndex;
    [SerializeField] private SceneNavigation sceneNavigation;

    private void Awake()
    {
        var proximityAction = GetComponent<ProximityAction>();
        proximityAction.OnUse += () => sceneNavigation.NavigateToScene(sceneIndex);
    }
}

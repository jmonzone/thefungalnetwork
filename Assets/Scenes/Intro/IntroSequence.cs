using System.Collections;
using Cinemachine;
using UnityEngine;

public class IntroSequence : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private SceneNavigation sceneNavigation;

    public const string INTRO_COMPLETE_KEY = "introComplete";

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return GoToIntro();
    }

    private IEnumerator GoToIntro()
    {
        var dolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();

        while (dolly.m_PathPosition < 1f)
        {
            dolly.m_PathPosition += Time.deltaTime * 0.5f;
            yield return null;
        }

        PlayerPrefs.SetInt(INTRO_COMPLETE_KEY, 1);
        sceneNavigation.NavigateToScene(Launcher.MENU_SCENE_INDEX);
    }
}

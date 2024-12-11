using UnityEngine;

public class BossRoom : MonoBehaviour
{
    [SerializeField] private Controller playerReference;
    [SerializeField] private Attackable boss;
    [SerializeField] private ViewReference resultView;

    private void Awake()
    {
        boss.OnDeath += ShowResults;
    }

    private void OnEnable()
    {
        playerReference.Attackable.OnDeath += ShowResults;
    }

    private void OnDisable()
    {
        playerReference.Attackable.OnDeath -= ShowResults;
    }

    private void ShowResults()
    {
        Debug.Log("OnAttack");
        resultView.RequestShow();
    }
}

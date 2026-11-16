using UnityEngine;

public class MainObjective : MonoBehaviour
{
    [SerializeField] private Team team;

    private void Awake()
    {
        Health health = GetComponent<Health>();
        GameManager.Instance.AddMainObjective(team, health);
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Action<Team> OnGameWon;

    private Dictionary<Team, Health> mainObjectivesToDestroy;

    private void Awake()
    {
        mainObjectivesToDestroy = new Dictionary<Team, Health>();

        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject.transform.root);
        }
    }

    private void Start()
    {
        foreach (var obj in mainObjectivesToDestroy)
        {
            obj.Value.OnDeath += () => WinGame(obj.Key);
        }
    }

    public void AddMainObjective(Team team, Health health)
    {
        mainObjectivesToDestroy.Add(team, health);
    }

    private void WinGame(Team team)
    {
        OnGameWon?.Invoke(team);
        Debug.Log($"Game won by team {team}");
    }
}
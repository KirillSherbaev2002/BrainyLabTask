using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreScripts : MonoBehaviour
{
    [SerializeField] private int scorePlayer = 0;
    [SerializeField] private int scoreEnemy = 0;

    private void OnValidate()
    {
        if (scorePlayer < 0) scorePlayer = 0;
        if (scoreEnemy < 0) scoreEnemy = 0;
    }

    void Awake()
    {
        UpdateScore();
    }

    private void UpdateScore()
    {
        gameObject.GetComponent<Text>().text = scorePlayer+":"+ scoreEnemy;
    }

    public void AddValue(bool isPlayerKilled)
    {
        if (isPlayerKilled) scoreEnemy++;
        if (!isPlayerKilled) scorePlayer++;
        UpdateScore();
    }
}

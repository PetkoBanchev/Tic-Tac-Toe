using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private Text winnerText;

    // Not using Awake to prevent a null referrence exception
    private void Start()
    {
        GameManager.Instance.OnGameOver += GameIsOver;
        GameManager.Instance.OnNewGame += HideEndGamePanel;
    }
    private void GameIsOver(SquareOwner currentPlayer)
    {
        endGamePanel.SetActive(true);

        if (currentPlayer == SquareOwner.PLAYER1)
            winnerText.text = "Player 1 wins";
        else if (currentPlayer == SquareOwner.PLAYER2)
            winnerText.text = "Player 2 wins";
        else
            winnerText.text = "It's a draw";
    }

    private void HideEndGamePanel()
    {
        endGamePanel.SetActive(false);
    }
}

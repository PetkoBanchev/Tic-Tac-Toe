using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour
{

    private int x;
    private int y;

    [SerializeField] private SquareOwner squareOwner = SquareOwner.EMPTY;

    private Text squareText;

    public int X
    {
        get { return x; }
        set { x = value; }
    }

    public int Y
    {
        get { return y; }
        set { y = value; }
    }

    public SquareOwner SquareOwner
    {
        get { return squareOwner; }
        set { squareOwner = value; SetSquareText(); }
    }


    private void Awake()
    {
        squareText = GetComponentInChildren<Text>();
    }

    private void SetSquareText()
    {
        if(squareOwner == SquareOwner.PLAYER1)
            squareText.text = "X";
        else
            squareText.text = "O";

        transform.GetComponent<Button>().interactable = false;
    }

    public void SetSquareOwner()
    {
        SquareOwner = GameManager.Instance.CurrentPlayer;
        GameManager.Instance.ChangeCurrentPlayer();
    }
}

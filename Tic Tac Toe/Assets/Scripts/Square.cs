using System;
using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour
{
    #region Private Variables
    private int x;
    private int y;

    [SerializeField] private SquareOwner squareOwner = SquareOwner.EMPTY;

    private Text squareText;
    #endregion

    #region Events
    // Chose a static event so I don't need to referrence every square separately. Might lead to memory leaks. Definetely consult a senior developer about this.
    public static event Action<Square> OnSquareSet;
    #endregion

    #region Public Properties
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
        set { squareOwner = value; }
    }
    #endregion

    #region Private Methods
    private void Awake()
    {
        squareText = GetComponentInChildren<Text>();
    }

    /// <summary>
    /// Changes the text of the square to show which player owns it.
    /// </summary>
    private void SetSquareText()
    {
        if(squareOwner == SquareOwner.PLAYER1)
            squareText.text = "X";
        else
            squareText.text = "O";

        transform.GetComponent<Button>().interactable = false; // Disables the button, so the input cannot be overwritten
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Sets the square owner to the current player. 
    /// Invokes an event that passes the square as a parameter.
    /// </summary>
    public void SetSquareOwner()
    {
        SquareOwner = GameManager.Instance.CurrentPlayer;
        SetSquareText();
        OnSquareSet?.Invoke(this);
    }
    #endregion
}

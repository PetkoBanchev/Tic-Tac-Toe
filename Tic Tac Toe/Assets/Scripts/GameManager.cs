using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Private Variables
    private FieldGenerator fieldGenerator;
    private Square[,] field = new Square[3, 3];
    private SquareOwner currentPlayer = SquareOwner.PLAYER1;
    private bool isGameOver = false;
    private int filledSquaresCounter = 0;
    #endregion

    #region Events
    public event Action<SquareOwner> OnGameOver;
    public event Action OnNewGame;
    #endregion

    #region Public Properties
    public SquareOwner CurrentPlayer { get { return currentPlayer; } }
    #endregion

    #region Singleton
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    #endregion

    #region Private Methods
    private void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        // N.B! The win check must happen before the change of the current player! If it's the other way around no win can be detected. 
        // When an event has multiple subscribers, the event handlers are invoked synchronously when an event is raised.
        // Check for more info: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/events/
        Square.OnSquareSet += CheckForWinnerWithEvent;
        Square.OnSquareSet += ChangeCurrentPlayer;

        fieldGenerator = GetComponent<FieldGenerator>();
    }

    /// <summary>
    /// Checks for a winner in several steps.
    /// 1. Loops through all the neighbours and checks for a match.
    /// 2. If a match is found it determines on which of the 4 axes it belongs.
    /// 3. Checks along the axis in both directions for a match.
    /// 4. If there are 3 matches on the same axis a winner is declared.
    /// </summary>
    /// <param name="currentSquare"></param>
    private void CheckForWinnerWithEvent(Square currentSquare)
    {
        List<Square>[] axes = new List<Square>[4];
        int x = currentSquare.X;
        int y = currentSquare.Y;

        // Description of the 4 possible axes
        // List<Square> horizontal; (-1, 0) && (1, 0)  - 0
        // List<Square> vertical;   (0, -1) && (0, 1)  - 1
        // List<Square> ascending;  (-1, -1) && (1, 1) - 2
        // List<Square> descending; (-1, 1) && (1, -1) - 3

        for (var i = -1; i < 2; i++) // (y axis)
        {
            for (var j = -1; j < 2; j++)  // (x axis)
            {
                // Excluding currentSquare and all indexes outside array bounds
                if ((i == 0 && j == 0) || (x + j) < 0 || (x + j) > field.GetLength(0) - 1 || (y + i) < 0 || (y + i) > field.GetLength(1) - 1) 
                    continue;

                // Check if a neighbour belongs to current player
                if (field[x + j, y + i] != null && field[x + j, y + i].GetComponent<Square>().SquareOwner == currentPlayer) 
                {
                    switch ((j, i)) // Determing on which axis we found a matching neighbour
                    {
                        //Horizontal axis
                        case var valueNegativeDir when valueNegativeDir == (-1, 0):
                        case var valuePositiveDir when valuePositiveDir == (1, 0):
                            if (axes[0] == null)
                                axes[0] = new List<Square>();
                            axes[0].Add(currentSquare);
                            CheckAlongAxis(j, i, currentSquare, axes[0]);
                            break;

                        //Vertical axis
                        case var valueNegativeDir when valueNegativeDir == (0, -1):
                        case var valuePositiveDir when valuePositiveDir == (0, 1):
                            if (axes[1] == null)
                                axes[1] = new List<Square>();
                            axes[1].Add(currentSquare);
                            CheckAlongAxis(j, i, currentSquare, axes[1]);
                            break;

                        //Ascending axis
                        case var valueNegativeDir when valueNegativeDir == (-1, -1):
                        case var valuePositiveDir when valuePositiveDir == (1, 1):
                            if (axes[2] == null)
                                axes[2] = new List<Square>();
                            axes[2].Add(currentSquare);
                            CheckAlongAxis(j, i, currentSquare, axes[2]);
                            break;

                        //Descending axis
                        case var valueNegativeDir when valueNegativeDir == (-1, 1):
                        case var valuePositiveDir when valuePositiveDir == (1, -1):
                            if (axes[3] == null)
                                axes[3] = new List<Square>();
                            axes[3].Add(currentSquare);
                            CheckAlongAxis(j, i, currentSquare, axes[3]);
                            break;
                    }
                }
            }
        }

        // Draw check
        filledSquaresCounter++;
        CheckForDraw();

        // Nested helper method to check if there are 3 matching squares along a given axis
        void CheckAlongAxis(int xDir, int yDir, Square _curSquare, List<Square> axis)
        {
            int xCur = _curSquare.X;
            int yCur = _curSquare.Y;

            Ends _ends = new Ends();

            for (int k = 1; k < 3; k++) // Loops 2 times, since we already have the current square and we need a maximum of 2 neighbours in either direction
            {
                int x_Positive = xCur + (k * xDir); //It is the direction in which we found a matching neighbour
                int y_Positive = yCur + (k * yDir);

                int x_Negative = xCur - (k * xDir); //Inverts the direction of the search
                int y_Negative = yCur - (k * yDir);

                // Positive direction
                CheckNeighborsInDirection(x_Positive, y_Positive, _ends.PositiveEnd, Directions.POSITIVE);

                // Negative direction
                CheckNeighborsInDirection(x_Negative, y_Negative, _ends.NegativeEnd, Directions.NEGATIVE);

                // Breaks the loop when a win is detected
                if (axis.Count == 3)
                {
                    isGameOver = true;
                    DisableField();
                    OnGameOver?.Invoke(currentPlayer);
                    foreach(Square s in axis)
                        s.transform.GetComponent<Image>().color = Color.green;
                    break;
                }

                // Nested helper method to check if there are 2 matching neighbours in a given direction
                void CheckNeighborsInDirection(int _x, int _y, bool end, Directions dir)
                {
                    if ((_x < field.GetLength(0) && _x > -1) && (_y < field.GetLength(1) && _y > -1))
                    {
                        if (field[_x, _y] != null && field[_x, _y].SquareOwner == currentPlayer && !end)
                        {
                            axis.Add(field[_x, _y]);
                        }
                        else
                        {
                            if(dir == Directions.POSITIVE)
                                _ends.PositiveEnd = true;
                            else
                                _ends.NegativeEnd = true;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Simple draw check. If the field is full and there is no winner a draw is declared.
    /// SquareOwner.EMPTY is passed so the InputManager knows it is a draw.
    /// </summary>
    private void CheckForDraw()
    {
        if (filledSquaresCounter == 9 && !isGameOver) // A draw is declared only when there is no winner after the final square has been filled.
            OnGameOver?.Invoke(SquareOwner.EMPTY);
    }

    /// <summary>
    /// Disables every button in the field.
    /// </summary>
    private void DisableField()
    {
        foreach (Square s in field)
            s.transform.GetComponent<Button>().interactable = false; // Disables all the buttons after the game ends
    }

    /// <summary>
    /// Toggles the current player.
    /// </summary>
    /// <param name="_square"></param>
    private void ChangeCurrentPlayer(Square _square)
    {
        if (_square.SquareOwner == SquareOwner.PLAYER1)
            currentPlayer = SquareOwner.PLAYER2;
        else
            currentPlayer = SquareOwner.PLAYER1;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Resets the game parameters and gets a new field.
    /// </summary>
    public void NewGame()
    {
        currentPlayer = SquareOwner.PLAYER1;
        isGameOver = false;
        filledSquaresCounter = 0;
        OnNewGame?.Invoke();
        field = fieldGenerator.GenerateField();
    }
    #endregion

    #region Helper classes
    private class Ends
    {
        public bool PositiveEnd;
        public bool NegativeEnd;

        public Ends() { PositiveEnd = false; NegativeEnd = false; }
    }

    private enum Directions
    {
        POSITIVE,
        NEGATIVE
    }
    #endregion
}

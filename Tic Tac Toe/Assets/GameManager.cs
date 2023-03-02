using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Square[,] field = new Square[3, 3];

    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private Transform fieldHolder;

    private SquareOwner currentPlayer = SquareOwner.PLAYER1;
    private bool isWinner = false;

    public SquareOwner CurrentPlayer
    {
        get { return currentPlayer; }
        set { currentPlayer = value; }
    }


    private static GameManager instance;

    public static GameManager Instance { get { return instance; } }


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        Square.OnSquareSet += CheckForWinnerWithEvent;
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateField();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateField()
    {
        for(int y = 0; y < field.GetLength(1); y++)
        {
            for(int x = 0; x < field.GetLength(0); x++)
            {
                var square = Instantiate(squarePrefab, new Vector3(x*200, y*200, 0), Quaternion.identity).GetComponent<Square>();
                square.X = x;
                square.Y = y;
                field[x, y] = square;
                square.transform.SetParent(fieldHolder);
            }
        }

        fieldHolder.localPosition = new Vector3(60, 550, 0);
    }

    public void CheckForWinnerWithEvent(Square currentSquare)
    {
        List<Square>[] axes = new List<Square>[4];
        int x = currentSquare.X;
        int y = currentSquare.Y;

        //List<Square> horizontal; (-1, 0) && (1, 0)  - 0
        //List<Square> vertical;   (0, -1) && (0, 1)  - 1
        //List<Square> ascending;  (-1, -1) && (1, 1) - 2
        //List<Square> descending; (-1, 1) && (1, -1) - 3

        for (var i = -1; i < 2; i++) // (y axis)
        {
            for (var j = -1; j < 2; j++)  // (x axis)
            {
                if ((i == 0 && j == 0) || (x + j) < 0 || (x + j) > field.GetLength(0) - 1 || (y + i) < 0 || (y + i) > field.GetLength(1) - 1) //excludes currentSquare and all indexes outside array bounds
                {
                    continue;
                }

                if (isWinner)
                {
                    foreach(Square s in field)
                        s.transform.GetComponent<Button>().interactable = false;
                    //endGamePanel.SetActive(true);
                    //playerTurnText.text = " ";
                    if (currentPlayer == SquareOwner.PLAYER1)
                    {
                        Debug.Log("Player 1 wins");
                    }
                    //endGamePanel.GetComponentInChildren<Text>().text = "Player 1 wins";
                    else
                        Debug.Log("Player 2 wins");
                    //endGamePanel.GetComponentInChildren<Text>().text = "Player 2 wins";
                    break;
                }

                if (field[x + j, y + i] != null && field[x + j, y + i].GetComponent<Square>().SquareOwner == currentPlayer) //check if neighbour belongs to current player
                {
                    switch ((j, i))
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
                    isWinner = true;
                    foreach(Square s in axis)
                        s.transform.GetComponent<Image>().color = Color.green;
                    break;
                }

                // Helper method to keep the code more readable
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
    public void ChangeCurrentPlayer()
    {
        if (currentPlayer == SquareOwner.PLAYER1)
            currentPlayer = SquareOwner.PLAYER2;
        else
            currentPlayer = SquareOwner.PLAYER1;
    }

    public void NewGame()
    {
        Array.Clear(field, 0, field.Length);
        foreach (Transform child in fieldHolder.transform)
            Destroy(child.gameObject);
        currentPlayer = SquareOwner.PLAYER1;
        fieldHolder.localPosition = Vector3.zero;
        isWinner = false;
        GenerateField();
    }

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
}

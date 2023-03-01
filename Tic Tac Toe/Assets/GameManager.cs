using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //public void CheckForWinner(int x, int y)
    //{
    //    List<Square>[] axes = new List<Square>[4];
    //    Square currentSquare = field[x, y].GetComponent<Square>();

    //    //List<Pluck> horizontal; (-1, 0) && (1, 0)  - 0
    //    //List<Pluck> vertical;   (0, -1) && (0, 1)  - 1
    //    //List<Pluck> ascending;  (-1, -1) && (1, 1) - 2
    //    //List<Pluck> descending; (-1, 1) && (1, -1) - 3


    //    for (var i = -1; i < 2; i++) // loops through neigbours (y axis)
    //    {
    //        for (var j = -1; j < 2; j++)  // loops through neigbours (x axis)
    //        {
    //            if ((i == 0 && j == 0) || (x + j) < 0 || (x + j) > field.GetLength(0) - 1 || (y + i) < 0 || (y + i) > field.GetLength(1) - 1) //excludes _currentPluck and indexes outside array bounds
    //            {
    //                continue;
    //            }

    //            if (isWinner)
    //            {
    //                //endGamePanel.SetActive(true);
    //                //playerTurnText.text = " ";
    //                if (currentPlayer == SquareOwner.PLAYER1)
    //                {

    //                }
    //                    //endGamePanel.GetComponentInChildren<Text>().text = "Player 1 wins";
    //                else
    //                    //endGamePanel.GetComponentInChildren<Text>().text = "Player 2 wins";
    //                break;
    //            }

    //            if (field[x + j, y + i] != null && field[x + j, y + i].GetComponent<Square>().SquareOwner == currentPlayer) //check if neighbour belongs to current player
    //            {
    //                switch ((j, i))
    //                {
    //                    case var value when value == (-1, 0):
    //                    case var value1 when value1 == (1, 0):
    //                        //Debug.Log("Horizontal");
    //                        if (axes[0] == null)
    //                            axes[0] = new List<Square>();
    //                        axes[0].Add(currentSquare);
    //                        CheckFurtherNeighbours(j, i, currentSquare, axes[0]);
    //                        break;

    //                    case var value when value == (0, -1):
    //                    case var value1 when value1 == (0, 1):
    //                        //Debug.Log("Vertical");
    //                        if (axes[1] == null)
    //                            axes[1] = new List<Square>();
    //                        axes[1].Add(currentSquare);
    //                        CheckFurtherNeighbours(j, i, currentSquare, axes[1]);
    //                        break;

    //                    case var value when value == (-1, -1):
    //                    case var value1 when value1 == (1, 1):
    //                        //Debug.Log("Ascending");
    //                        if (axes[2] == null)
    //                            axes[2] = new List<Square>();
    //                        axes[2].Add(currentSquare);
    //                        CheckFurtherNeighbours(j, i, currentSquare, axes[2]);
    //                        break;

    //                    case var value when value == (-1, 1):
    //                    case var value1 when value1 == (1, -1):
    //                        //Debug.Log("Descending");
    //                        if (axes[3] == null)
    //                            axes[3] = new List<Square>();
    //                        axes[3].Add(currentSquare);
    //                        CheckFurtherNeighbours(j, i, currentSquare, axes[3]);
    //                        break;
    //                }
    //            }
    //        }
    //    }

    //    void CheckFurtherNeighbours(int xDir, int yDir, Square _curSquare, List<Square> axis)
    //    {
    //        int xCur = _curSquare.X;
    //        int yCur = _curSquare.Y;

    //        bool end_Positive = false;
    //        bool end_Negative = false;

    //        for (int l = 1; l < 3; l++) // Loops 2 times, since we already have the current pluck and we need a maximum of 2 neighbours in either direction
    //        {
    //            int x_Positive = xCur + (l * xDir); //It is the direction in which we found a matching neighbour
    //            int y_Positive = yCur + (l * yDir);

    //            int x_Negative = xCur - (l * xDir); //Inverts the direction of the search
    //            int y_Negative = yCur - (l * yDir);

    //            //Positive direction
    //            if ((x_Positive < field.GetLength(0) && x_Positive > -1) && (y_Positive < field.GetLength(1) && y_Positive > -1))
    //            {
    //                if (field[x_Positive, y_Positive] != null && field[x_Positive, y_Positive].SquareOwner == currentPlayer && !end_Positive)
    //                {
    //                    axis.Add(field[x_Positive, y_Positive]);
    //                }
    //                else
    //                {
    //                    end_Positive = true;
    //                }
    //            }

    //            //Negative direction
    //            if ((x_Negative < field.GetLength(0) && x_Negative > -1) && (y_Negative < field.GetLength(1) && y_Negative > -1))
    //            {
    //                if (field[x_Negative, y_Negative] != null && field[x_Negative, y_Negative].SquareOwner == currentPlayer && !end_Negative)
    //                {
    //                    axis.Add(field[x_Negative, y_Negative]);
    //                }
    //                else
    //                {
    //                    end_Negative = true;
    //                }
    //            }

    //            if (axis.Count == 3)
    //            {
    //                Debug.Log("Winner is: " + currentPlayer);
    //                isWinner = true;
    //                break;
    //            }
    //        }
    //    }
    //}

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
        GenerateField();

    }
}

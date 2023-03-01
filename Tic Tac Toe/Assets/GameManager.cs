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
    private bool isFirstGeneration = false;

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

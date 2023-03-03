using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private Transform fieldHolder;

    public Square[,] GenerateField()
    {
        DestroyField(); //Destroys the old field if there is one

        var field = new Square[3, 3];
        for (int y = 0; y < field.GetLength(1); y++)
        {
            for (int x = 0; x < field.GetLength(0); x++)
            {
                var square = Instantiate(squarePrefab, new Vector3(x * 200, y * 200, 0), Quaternion.identity, fieldHolder).GetComponent<Square>();
                square.X = x;
                square.Y = y;
                field[x, y] = square;
            }
        }

        fieldHolder.localPosition = new Vector3(60, 550, 0); // Ensures the field is centred

        return field;
    }

    private void DestroyField()
    {
        fieldHolder.transform.localPosition = Vector3.zero; // Ensures the field is centred
        foreach (Transform child in fieldHolder.transform)
            Destroy(child.gameObject);
    }
}

using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    #region Private Variables
    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private Transform fieldHolder;
    #endregion
    
    #region Private Methods
    /// <summary>
    /// Resets the position of the field holder.
    /// Removes all children, essentially deleting the field.
    /// </summary>
    private void DestroyField()
    {
        fieldHolder.transform.localPosition = Vector3.zero; // Ensures the field is centred
        foreach (Transform child in fieldHolder.transform)
            Destroy(child.gameObject);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Creates a new field. The field is represented by a 2D array
    /// </summary>
    /// <returns></returns>
    public Square[,] GenerateField()
    {
        DestroyField(); //Destroys the old field if there is one

        var field = new Square[3, 3];
        for (int y = 0; y < field.GetLength(1); y++)
        {
            for (int x = 0; x < field.GetLength(0); x++)
            {
                var square = Instantiate(squarePrefab, new Vector3(x * .33f, y * .33f, 0), Quaternion.identity, fieldHolder).GetComponent<Square>(); // * .33f ensure proper spacing between the squares
                square.X = x;
                square.Y = y;
                field[x, y] = square;
            }
        }

        fieldHolder.localPosition = new Vector3(-345, 0, 0); // Ensures the field is centred

        return field;
    }
    #endregion

}

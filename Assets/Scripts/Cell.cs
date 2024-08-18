using Unity.VisualScripting;
using UnityEngine;

public class Cell : MonoBehaviour
{
    int cellX, cellY;

    // von Neumann Neighbours
    Cell topNeighbour, bottomNeighbour, leftNeighbour, rightNeighbour;

    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCoords(int x, int y)
    {
        cellX = x;
        cellY = y;
    }

    public void VonNeumannNeighbours(Cell top, Cell bottom, Cell left, Cell right)
    {
        topNeighbour = top;
        bottomNeighbour = bottom;
        leftNeighbour = left;
        rightNeighbour = right;
    }
}

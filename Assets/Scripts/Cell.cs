using Unity.VisualScripting;
using UnityEngine;

public class Cell : MonoBehaviour
{
    // Coordinates of this cell
    int cellX, cellY;

    // 1 = Alive, 0 = Dead
    public int state = -1;

    public bool changedState = true;

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

    // Mark if the state changed in the last cycle
    public void UpdateState(int newState)
    {
        if(newState == state)
        {
            changedState = false;
        }
        else
        {
            // If the state changed, store the new state
            state = newState;
            changedState = true;
        }
    }

    // Once the material has been updated, this information can be reset
    public void UpdatedMaterial()
    {
        changedState = false;
    }
}

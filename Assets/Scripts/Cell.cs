using UnityEngine;

public class Cell : MonoBehaviour
{
    // Coordinates of this cell
    public int cellX, cellY;

    // 1 = Alive, 0 = Dead
    public int state = -1;

    public bool changedState = true;

    // von Neumann Neighbours
    // Indexes: 0 = Top, 1 = Bottom, 2 = Left, 3 = Right
    public Cell[] neighbours;

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
        neighbours = new Cell[4];

        neighbours[0] = top;
        neighbours[1] = bottom;
        neighbours[2] = left;
        neighbours[3] = right;
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

    public void GameOfLifeGeneration(int currentLiveNeighbours)
    {
        if(state == 1)
        {
            if(currentLiveNeighbours < 2 || currentLiveNeighbours > 3)
            {
                UpdateState(0);
            }
        }
        else
        {
            if(currentLiveNeighbours == 3)
            {
                UpdateState(1);
            }
        }
    }
}

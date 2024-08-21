public class Cell
{
    public int x, y;

    // 1 = Alive, 0 = Dead
    public int state = -1;

    public bool changedState = true;

    // Indexes
    // 0 = Top, 1 = Bottom, 2 = Left, 3 = Right
    // 4 = TopLeft, 5 = TopRight, 6 = BottomLeft, 7 = BottomRight 
    public Cell[] neighbours;

    public Cell(int xLocation)
    {
        x = xLocation;
    }

    public Cell(int xLocation, int yLocation)
    {
        x = xLocation;
        y = yLocation;
    }

    public void SetLinearNeighbours(Cell[] newNeighbours)
    {
        neighbours = new Cell[2];

        neighbours[0] = newNeighbours[0];
        neighbours[1] = newNeighbours[1];
    }

    public void VonNeumannNeighbours(Cell[] newNeighbours)
    {
        neighbours = new Cell[4];

        neighbours[0] = newNeighbours[0];
        neighbours[1] = newNeighbours[1];
        neighbours[2] = newNeighbours[2];
        neighbours[3] = newNeighbours[3];
    }

    public void MooreNeighbours(Cell[] newNeighbours)
    {
        neighbours = new Cell[8];

        neighbours[0] = newNeighbours[0];
        neighbours[1] = newNeighbours[1];
        neighbours[2] = newNeighbours[2];
        neighbours[3] = newNeighbours[3];
        neighbours[4] = newNeighbours[4];
        neighbours[5] = newNeighbours[5];
        neighbours[6] = newNeighbours[6];
        neighbours[7] = newNeighbours[7];
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

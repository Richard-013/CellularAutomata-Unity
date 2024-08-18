using UnityEngine;

public class AutomataManager : MonoBehaviour
{
    public Cell prefabCell;

    public int horizontalSize = 50;
    public int verticalSize = 50;

    public Cell[,] automataGrid;

    void Awake()
    {
        setupGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setupGrid()
    {
        automataGrid = new Cell[horizontalSize, verticalSize];

        for(int x = 0; x < horizontalSize; x++)
        {
            for(int y = 0; y < verticalSize; y++)
            {
                automataGrid[x, y] = Instantiate<Cell>(prefabCell);
            }
        }

        for(int x = 0; x < horizontalSize; x++)
        {
            for(int y = 0; y < verticalSize; y++)
            {
                Cell currentCell = automataGrid[x, y];
                currentCell.name = "Cell " + x + ", " + y;
                currentCell.SetCoords(x, y);
                currentCell.transform.position = new Vector3(x, y, 0);

                Cell top, bottom, left, right;
                
                if(x == 0)
                {
                    left = null;
                    right = automataGrid[x+1, y];
                }
                else if(x == horizontalSize-1)
                {
                    left = automataGrid[x-1, y];
                    right = null;
                }
                else
                {
                    left = automataGrid[x-1, y];
                    right = automataGrid[x+1, y];
                }

                if(y == 0)
                {
                    top = automataGrid[x, y+1];
                    bottom = null;
                }
                else if(y == verticalSize-1)
                {
                    top = null;
                    bottom = automataGrid[x, y-1];
                }
                else
                {
                    top = automataGrid[x, y+1];
                    bottom = automataGrid[x, y-1];
                }

                currentCell.VonNeumannNeighbours(top, bottom, left, right);
            }
        }
        
    }
}

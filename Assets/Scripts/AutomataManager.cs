using UnityEngine;

public class AutomataManager : MonoBehaviour
{
    public Cell prefabCell;
    public Material aliveMaterial, deadMaterial;

    public int horizontalSize = 50;
    public int verticalSize = 50;

    public Cell[,] automataGrid;

    // Time tracking
    private float time = 0.0f;
    public float interpolationPeriod = 0.5f;

    void Awake()
    {
        SetupGrid();
        UpdateMaterials();
    }

    // Update is called once per frame
    void Update()
    {
        // Update how much time has passed since last frame
        time += Time.deltaTime;

        // If enough time has passed, update the grid
        if (time >= interpolationPeriod)
        {
            time = time - interpolationPeriod;
            RandomiseState();
            UpdateMaterials();
        }
    }

    void SetupGrid()
    {
        automataGrid = new Cell[horizontalSize, verticalSize];

        for(int x = 0; x < horizontalSize; x++)
        {
            for(int y = 0; y < verticalSize; y++)
            {
                automataGrid[x, y] = Instantiate<Cell>(prefabCell);
            }
        }

        SetCellNeighbours();
    }

    // Set the neighbours of each Cell based on its position
    void SetCellNeighbours()
    {
        for(int x = 0; x < horizontalSize; x++)
        {
            for(int y = 0; y < verticalSize; y++)
            {
                Cell currentCell = automataGrid[x, y];
                currentCell.name = "Cell " + x + ", " + y;
                currentCell.SetCoords(x, y);
                currentCell.transform.position = new Vector3(x, y, 0);
                currentCell.UpdateState(Random.Range(0,2));

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

    // Randomise the state of all cells in the grid
    void RandomiseState()
    {
        for(int x = 0; x < horizontalSize; x++)
        {
            for(int y = 0; y < verticalSize; y++)
            {
                automataGrid[x, y].UpdateState(Random.Range(0,2));
            }
        }
    }

    // Update the material of each Cell to match its state
    void UpdateMaterials()
    {
        for(int x = 0; x < horizontalSize; x++)
        {
            for(int y = 0; y < verticalSize; y++)
            {
                Cell currentCell = automataGrid[x, y];

                // Only update the material if state changed
                if(currentCell.changedState)
                {
                    if(currentCell.state == 1)
                    {
                        currentCell.GetComponent<Renderer>().material = aliveMaterial;
                    }
                    else
                    {
                        currentCell.GetComponent<Renderer>().material = deadMaterial;
                    }

                    // Tell the cell the material has been updated to match the state
                    currentCell.UpdatedMaterial();
                }
            }
        }
    }
}

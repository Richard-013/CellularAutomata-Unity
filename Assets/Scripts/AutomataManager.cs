using UnityEngine;

public class AutomataManager : MonoBehaviour
{
    // References
    public Cell prefabCell;
    public Material aliveMaterial, deadMaterial;

    // Grid size
    public int horizontalSize = 50;
    public int verticalSize = 50;

    // Grid
    public Cell[,] automataGrid;

    // Shadow Grid of current states
    public int[,] shadowGrid;

    // Time tracking
    private float time = 0.0f;
    public float interpolationPeriod = 0.5f;

    const int VON_NEUMANN_NEIGHBOURS = 4;
    const int MOORE_NEIGHBOURS = 8;

    bool enableMooreMode = false;
    int numNeighbours;

    int generation = 0;

    void Awake()
    {
        enableMooreMode = true;
        SetNeighbourMode();
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
            RunGameOfLifeGeneration();
            
            if(generation % 10 == 0)
            {
                UpdateMaterials();
            }
        }
    }

    void SetNeighbourMode()
    {
        if(enableMooreMode)
        {
            numNeighbours = MOORE_NEIGHBOURS;
        }
        else
        {
            numNeighbours = VON_NEUMANN_NEIGHBOURS;
        }
    }

    void SetupGrid()
    {
        // Initialise the grid
        automataGrid = new Cell[horizontalSize, verticalSize];
        shadowGrid = new int[horizontalSize, verticalSize];

        // Add a Cell instance to every grid cell
        for(int x = 0; x < horizontalSize; x++)
        {
            for(int y = 0; y < verticalSize; y++)
            {
                automataGrid[x, y] = Instantiate<Cell>(prefabCell, this.transform);
            }
        }

        SetupCell();
    }

    // Set the properties of each Cell
    void SetupCell()
    {
        for(int x = 0; x < horizontalSize; x++)
        {
            for(int y = 0; y < verticalSize; y++)
            {
                Cell currentCell = automataGrid[x, y];
                currentCell.name = "Cell " + x + ", " + y;
                currentCell.transform.position = new Vector3(x, y, 0);
                int newState = Random.Range(0,2);
                currentCell.UpdateState(newState);
                shadowGrid[x, y] = newState;

                SetupCellNeighbours(currentCell, x, y);
            }
        }
    }

    void SetupCellNeighbours(Cell currentCell, int x, int y)
    {
        if(enableMooreMode)
        {
            // Set the neighbours of the cell based on its grid position
            int[] top, bottom, left, right, topLeft, topRight, bottomLeft, bottomRight;
            
            if(x == 0)
            {
                // If the cell is on the left edge, set no left neighbour
                left = new int[2] {-1, -1}; // Null
                right = new int[2] {x+1, y};

                if(y == 0)
                {
                    // If the cell is on the bottom edge, set no bottom neighbours
                    top = new int[2] {x, y+1};
                    bottom = new int[2] {-1, -1}; // Null

                    topLeft = new int[2] {-1, -1}; // Null
                    bottomLeft = new int[2] {-1, -1}; // Null
                    bottomRight = new int[2] {-1, -1}; // Null
                    topRight = new int[2] {x+1, y+1};
                }
                else if(y == verticalSize-1)
                {
                    // If the cell is on the top edge, set no top neighbours
                    top = new int[2] {-1, -1}; // Null
                    bottom = new int[2] {x, y-1};

                    topLeft = new int[2] {-1, -1}; // Null
                    bottomLeft = new int[2] {-1, -1}; // Null
                    bottomRight = new int[2] {x+1, y-1};
                    topRight = new int[2] {-1, -1}; // Null
                }
                else
                {
                    // Set remaining neighbours
                    top = new int[2] {x, y+1};
                    bottom = new int[2] {x, y-1};

                    topLeft = new int[2] {-1, -1}; // Null
                    bottomLeft = new int[2] {-1, -1}; // Null
                    bottomRight = new int[2] {x+1, y-1};
                    topRight = new int[2] {x+1, y+1};
                }
            }
            else if(x == horizontalSize-1)
            {
                // If the cell is on the right edge, set no right neighbour
                left = new int[2] {x-1, y};
                right = new int[2] {-1, -1}; // Null

                if(y == 0)
                {
                    // If the cell is on the bottom edge, set no bottom neighbours
                    top = new int[2] {x, y+1};
                    bottom = new int[2] {-1, -1}; // Null

                    topLeft = new int[2] {x-1, y+1};
                    bottomLeft = new int[2] {-1, -1}; // Null
                    bottomRight = new int[2] {-1, -1}; // Null
                    topRight = new int[2] {-1, -1}; // Null
                }
                else if(y == verticalSize-1)
                {
                    // If the cell is on the top edge, set no top neighbours
                    top = new int[2] {-1, -1}; // Null
                    bottom = new int[2] {x, y-1};

                    topLeft = new int[2] {-1, -1}; // Null
                    bottomLeft = new int[2] {x-1, y-1};
                    bottomRight = new int[2] {-1, -1}; // Null
                    topRight = new int[2] {-1, -1}; // Null
                }
                else
                {
                    // Set remaining neighbours
                    top = new int[2] {x, y+1};
                    bottom = new int[2] {x, y-1};

                    topLeft = new int[2] {x-1, y+1};
                    bottomLeft = new int[2] {x-1, y-1};
                    bottomRight = new int[2] {-1, -1}; // Null
                    topRight = new int[2] {-1, -1}; // Null
                }
            }
            else
            {
                left = new int[2] {x-1, y};
                right = new int[2] {x+1, y};

                if(y == 0)
                {
                    // If the cell is on the bottom edge, set no bottom neighbours
                    top = new int[2] {x, y+1};
                    bottom = new int[2] {-1, -1}; // Null
                    
                    topLeft = new int[2] {x-1, y+1};
                    bottomLeft = new int[2] {-1, -1}; // Null
                    bottomRight = new int[2] {-1, -1}; // Null
                    topRight = new int[2] {x+1, y+1};
                }
                else if(y == verticalSize-1)
                {
                    // If the cell is on the top edge, set no top neighbours
                    top = new int[2] {-1, -1}; // Null
                    bottom = new int[2] {x, y-1};

                    topLeft = new int[2] {-1, -1}; // Null
                    bottomLeft = new int[2] {x-1, y-1};
                    bottomRight = new int[2] {x+1, y-1};
                    topRight = new int[2] {-1, -1}; // Null
                }
                else
                {
                    // Set remaining neighbours
                    top = new int[2] {x, y+1};
                    bottom = new int[2] {x, y-1};
                    
                    topLeft = new int[2] {x-1, y+1};
                    bottomLeft = new int[2] {x-1, y-1};
                    bottomRight = new int[2] {x+1, y-1};
                    topRight = new int[2] {x+1, y+1};
                }
            }

            // Tell the cell its neighbours
            currentCell.MooreNeighbours(top, bottom, left, right, topLeft, topRight, bottomLeft, bottomRight);
        }
        else
        {
            // Set the neighbours of the cell based on its grid position
            int[] top, bottom, left, right;
            
            if(x == 0)
            {
                // If the cell is on the left edge, set no left neighbour
                left = new int[2] {-1, -1}; // Null
                right = new int[2] {x+1, y};
            }
            else if(x == horizontalSize-1)
            {
                // If the cell is on the right edge, set no right neighbour
                left = new int[2] {x-1, y};
                right = new int[2] {-1, -1}; // Null
            }
            else
            {
                left = new int[2] {x-1, y};
                right = new int[2] {x+1, y};
            }

            if(y == 0)
            {
                // If the cell is on the bottom edge, set no bottom neighbour
                top = new int[2] {x, y+1};
                bottom = new int[2] {-1, -1}; // Null
            }
            else if(y == verticalSize-1)
            {
                // If the cell is on the top edge, set no top neighbour
                top = new int[2] {-1, -1}; // Null
                bottom = new int[2] {x, y-1};
            }
            else
            {
                top = new int[2] {x, y+1};
                bottom = new int[2] {x, y-1};
            }

            // Tell the cell its neighbours
            currentCell.VonNeumannNeighbours(top, bottom, left, right);
        }
    }

    // Randomise the state of all cells in the grid
    void RandomiseState()
    {
        for(int x = 0; x < horizontalSize; x++)
        {
            for(int y = 0; y < verticalSize; y++)
            {
                int newState = Random.Range(0,2);
                automataGrid[x, y].UpdateState(newState);
                shadowGrid[x, y] = newState;
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

    void RunGameOfLifeGeneration()
    {
        for(int x = 0; x < horizontalSize; x++)
        {
            for(int y = 0; y < verticalSize; y++)
            {
                Cell currentCell = automataGrid[x, y];

                currentCell.GameOfLifeGeneration(TotalLiveNeighbours(currentCell));
            }
        }

        for(int x = 0; x < horizontalSize; x++)
        {
            for(int y = 0; y < verticalSize; y++)
            {
                shadowGrid[x, y] = automataGrid[x, y].state;
            }
        }

        generation++;
    }

    int TotalLiveNeighbours(Cell currentCell)
    {
        int liveNeighbours = 0;

        for(int i = 0; i < numNeighbours; i++)
        {
            if(currentCell.neighbours[i] != null)
            {
                if(currentCell.neighbours[i][0] == -1)
                {
                    continue;
                }

                if(shadowGrid[currentCell.neighbours[i][0], currentCell.neighbours[i][1]] == 1)
                {
                    liveNeighbours++;
                }
            }
        }

        return liveNeighbours;
    }
}

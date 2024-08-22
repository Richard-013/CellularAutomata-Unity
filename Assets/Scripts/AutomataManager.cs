using UnityEngine;

public class AutomataManager : MonoBehaviour
{
    const int VON_NEUMANN_NEIGHBOURS = 4;
    const int MOORE_NEIGHBOURS = 8;

    // Draw every n generations
    const int GENERATION_DRAW_INTERVAL = 1;

    // Set in Editor
    public Material displayMaterial;

    // Texture to apply to render automata on
    Texture2D automatonTexture;

    // Cell colors
    Color deadColour = new Color(0.2415f, 0.2415f, 0.2415f, 1f);
    Color dyingColour = new Color(0.8372f, 0.3745f, 0.3745f, 1f);
    Color aliveColour = new Color(0.3745f, 0.6843f, 0.8372f, 1f);

    // Grid size
    public int horizontalSize = 200;
    public int verticalSize = 200;

    // Grid
    Cell[,] automataGrid;

    // Shadow Grid of current states
    int[,] shadowGrid;
    int[,] shadowInfectionGrid;

    // Time tracking
    private float time = 0.0f;
    public float interpolationPeriod = 0.5f;

    // Select Cellular Automata
    // Index
    // 0 = Game of Life, 1 = Seeds, 2 = Brian's Brain, 3 = Belosouv-Zhabotinsky
    public int automataMode = 0;

    // Set neighbourhood type to use
    // 0 = Moore, 1 = von Neumann
    bool vonNeumannMode = false;
    int numNeighbours;

    // Initial board is alway Generation 0
    int generation = 0;

    // Infection rate for Belosouv-Zhabotinsky Reaction
    public int infectivity = 15;
    int initialInfectionAmount = 20;
    int infectionChance = 10;

    void Awake()
    {
        SetupGrid();
        SetupTexture();
    }

    void Update()
    {
        // Update how much time has passed since last frame
        time += Time.deltaTime;

        // If enough time has passed, update the grid
        if (time >= interpolationPeriod)
        {
            time = time - interpolationPeriod;

            CellularAutomata.RunAutomatonGeneration(automataMode, automataGrid, shadowGrid, shadowInfectionGrid, horizontalSize, verticalSize, numNeighbours, infectivity);
            generation++;
            
            if(generation % GENERATION_DRAW_INTERVAL == 0)
            {
                UpdateTexture();
            }
        }
    }

    void SetupTexture()
    {
        automatonTexture = new Texture2D(horizontalSize, verticalSize);
        automatonTexture.filterMode = FilterMode.Point;

        UpdateTexture();

        displayMaterial.SetTexture("_BaseMap", automatonTexture);
    }

    void UpdateTexture()
    {
        if(automataMode == 3)
        {
            UpdateTextureInfection();
            return;
        }

        Color[] pixels = automatonTexture.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            // Convert array index into X, Y then retrieve cell state
            int state = shadowGrid[i % horizontalSize, i / verticalSize];

            switch(state)
            {
                case 0:
                    // Dead cell
                    pixels[i] = deadColour;
                    break;
                case 1:
                    // Alive cell
                    pixels[i] = aliveColour;
                    break;
                case 2:
                    // Dying cell
                    pixels[i] = dyingColour;
                    break;
                default:
                    pixels[i] = deadColour;
                    break;
            }
        }

        automatonTexture.SetPixels(pixels);
        automatonTexture.Apply();
    }

    void UpdateTextureInfection()
    {
        /*Gradient infectionGradient = new Gradient();
        GradientColorKey[] gradientColours = new GradientColorKey[2];

        gradientColours[0] = new GradientColorKey(aliveColour, 0f);
        gradientColours[1] = new GradientColorKey(dyingColour, 1f);

        GradientAlphaKey[] gradientAlphas = new GradientAlphaKey[2];
        gradientAlphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        gradientAlphas[1] = new GradientAlphaKey(1.0f, 1.0f);

        infectionGradient.SetKeys(gradientColours, gradientAlphas);*/

        Color[] pixels = automatonTexture.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            int state = shadowGrid[i % horizontalSize, i / verticalSize];

            if(state == 0)
            {
                pixels[i] = aliveColour;
            }
            else if(state == 2)
            {
                pixels[i] = dyingColour;
            }
            else
            {
                if(shadowInfectionGrid[i % horizontalSize, i / verticalSize] < 50)
                {
                    pixels[i] = new Color(1f, 1f, 1f, 1f);
                }
                else
                {
                    pixels[i] = deadColour;
                }
            }
        }

        automatonTexture.SetPixels(pixels);
        automatonTexture.Apply();
    }

    void SetupGrid()
    {
        // Initialise the grid
        automataGrid = new Cell[horizontalSize, verticalSize];
        shadowGrid = new int[horizontalSize, verticalSize];

        if(automataMode == 3)
        {
            shadowInfectionGrid = new int[horizontalSize, verticalSize];
        }

        // Add a Cell instance to every grid cell
        for(int x = 0; x < horizontalSize; x++)
        {
            for(int y = 0; y < verticalSize; y++)
            {
                automataGrid[x, y] = new Cell(x, y);
            }
        }

        switch(automataMode)
        {
            case 0:
                SetupCellsRandom();
                break;
            case 1:
                SetupCellsCentral();
                break;
            case 2:
                SetupCellsCentral();
                break;
            case 3:
                SetupCellsRandomInfection();
                break;
            default:
                SetupCellsRandom();
                break;
        }
    }

    // Set the properties of each Cell
    // Give every cell a random state
    void SetupCellsRandom()
    {
        for(int x = 0; x < horizontalSize; x++)
        {
            for(int y = 0; y < verticalSize; y++)
            {
                Cell currentCell = automataGrid[x, y];
                int newState = Random.Range(0, 2);
                currentCell.UpdateState(newState);
                shadowGrid[x, y] = newState;

                if(automataMode == 3)
                {
                    shadowInfectionGrid[x, y] = currentCell.infection;
                }

                SetupCellNeighbours(currentCell, x, y);
            }
        }
    }

    void SetupCellsRandomInfection()
    {
        for(int x = 0; x < horizontalSize; x++)
        {
            for(int y = 0; y < verticalSize; y++)
            {
                Cell currentCell = automataGrid[x, y];

                int chance = Random.Range(0, 100);
                int newState = 0;
                int currentInfection = 0;

                if(chance < infectionChance) // Cell starts infected
                {
                    currentInfection = initialInfectionAmount;
                    newState = 1;
                }
                else // Cell is not infected
                {
                    currentCell.infection = 0;
                }

                currentCell.infection = currentInfection;
                currentCell.UpdateState(newState);
                shadowGrid[x, y] = newState;
                shadowInfectionGrid[x, y] = currentInfection;

                SetupCellNeighbours(currentCell, x, y);
            }
        }
    }

    // Set the properties of each Cell
    // Set up a cluster of central cells with random states
    void SetupCellsCentral()
    {
        for(int x = 0; x < horizontalSize; x++)
        {
            for(int y = 0; y < verticalSize; y++)
            {
                Cell currentCell = automataGrid[x, y];
                int newState = 0;
                
                if(x >= ((horizontalSize/2)-2) && x <= ((horizontalSize/2)+2))
                {
                    if(y >= ((verticalSize/2)-1) && y <= ((verticalSize/2)+1))
                    {
                        newState = Random.Range(0,2);
                    }
                }
                
                currentCell.UpdateState(newState);
                shadowGrid[x, y] = newState;

                SetupCellNeighbours(currentCell, x, y);
            }
        }
    }

    void SetupCellNeighbours(Cell currentCell, int x, int y)
    {
        if(vonNeumannMode)
        {
            numNeighbours = VON_NEUMANN_NEIGHBOURS;
            SetVonNeumannNeighbours(currentCell, x, y);
        }
        else
        {
            numNeighbours = MOORE_NEIGHBOURS;
            SetMooreNeighbours(currentCell, x, y);
        }
    }

    void SetVonNeumannNeighbours(Cell currentCell, int x, int y)
    {
        // Indexes
        // 0 = Top, 1 = Bottom, 2 = Left, 3 = Right
        Cell[] neighbours = new Cell[numNeighbours];
        
        if(x == 0) // If the cell is on the left edge, set no left neighbour
        {
            neighbours[2] = null;
            neighbours[3] = automataGrid[currentCell.x+1, currentCell.y];
        }
        else if(x == horizontalSize-1) // If the cell is on the right edge, set no right neighbour
        {
            neighbours[2] = automataGrid[currentCell.x-1, currentCell.y];
            neighbours[3] = null;
        }
        else
        {
            neighbours[2] = automataGrid[currentCell.x-1, currentCell.y];
            neighbours[3] = automataGrid[currentCell.x+1, currentCell.y];
        }

        if(y == 0) // If the cell is on the bottom edge, set no bottom neighbour
        {
            neighbours[0] = automataGrid[currentCell.x, currentCell.y+1];
            neighbours[1] = null;
        }
        else if(y == verticalSize-1) // If the cell is on the top edge, set no top neighbour
        {
            neighbours[0] = null;
            neighbours[1] = automataGrid[currentCell.x, currentCell.y-1];
        }
        else
        {
            neighbours[0] = automataGrid[currentCell.x, currentCell.y+1];
            neighbours[1] = automataGrid[currentCell.x, currentCell.y-1];
        }

        // Tell the cell its neighbours
        currentCell.VonNeumannNeighbours(neighbours);
    }

    void SetMooreNeighbours(Cell currentCell, int x, int y)
    {
        // Indexes
        // 0 = Top, 1 = Bottom, 2 = Left, 3 = Right
        // 4 = TopLeft, 5 = TopRight, 6 = BottomLeft, 7 = BottomRight
        Cell[] neighbours = new Cell[numNeighbours];
        
        if(x == 0) // If the cell is on the left edge, set no left neighbours
        {
            neighbours[2] = null;
            neighbours[3] = automataGrid[currentCell.x+1, currentCell.y];

            if(y == 0) // If the cell is on the bottom edge, set no bottom neighbours
            {
                neighbours[0] = automataGrid[currentCell.x, currentCell.y+1];
                neighbours[1] = null;

                neighbours[4] = null;
                neighbours[6] = null;
                neighbours[7] = null;
                neighbours[5] = automataGrid[currentCell.x+1, currentCell.y+1];
            }
            else if(y == verticalSize-1) // If the cell is on the top edge, set no top neighbours
            {
                neighbours[0] = null;
                neighbours[1] = automataGrid[currentCell.x, currentCell.y-1];

                neighbours[4] = null;
                neighbours[6] = null;
                neighbours[7] = automataGrid[currentCell.x+1, currentCell.y-1];
                neighbours[5] = null;
            }
            else // If the cell is not on the top or bottom edge
            {
                neighbours[0] = automataGrid[currentCell.x, currentCell.y+1];
                neighbours[1] = automataGrid[currentCell.x, currentCell.y-1];

                neighbours[4] = null;
                neighbours[6] = null;
                neighbours[7] = automataGrid[currentCell.x+1, currentCell.y-1];
                neighbours[5] = automataGrid[currentCell.x+1, currentCell.y+1];
            }
        }
        else if(x == horizontalSize-1) // If the cell is on the right edge, set no right neighbour
        {
            neighbours[2] = automataGrid[currentCell.x-1, currentCell.y];
            neighbours[3] = null;

            if(y == 0) // If the cell is on the bottom edge, set no bottom neighbours
            {
                neighbours[0] = automataGrid[currentCell.x, currentCell.y+1];
                neighbours[1] = null;

                neighbours[4] = automataGrid[currentCell.x-1, currentCell.y+1];
                neighbours[6] = null;
                neighbours[7] = null;
                neighbours[5] = null;
            }
            else if(y == verticalSize-1) // If the cell is on the top edge, set no top neighbours
            {
                neighbours[0] = null;
                neighbours[1] = automataGrid[currentCell.x, currentCell.y-1];

                neighbours[4] = null;
                neighbours[6] = automataGrid[currentCell.x-1, currentCell.y-1];
                neighbours[7] = null;
                neighbours[5] = null;
            }
            else // If the cell is not on the top or bottom edge
            {
                neighbours[0] = automataGrid[currentCell.x, currentCell.y+1];
                neighbours[1] = automataGrid[currentCell.x, currentCell.y-1];

                neighbours[4] = automataGrid[currentCell.x-1, currentCell.y+1];
                neighbours[6] = automataGrid[currentCell.x-1, currentCell.y-1];
                neighbours[7] = null;
                neighbours[5] = null;
            }
        }
        else // If the cell is not on the left or right edge
        {
            neighbours[2] = automataGrid[currentCell.x-1, currentCell.y];
            neighbours[3] = automataGrid[currentCell.x+1, currentCell.y];

            if(y == 0)
            {
                // If the cell is on the bottom edge, set no bottom neighbours
                neighbours[0] = automataGrid[currentCell.x, currentCell.y+1];
                neighbours[1] = null;
                
                neighbours[4] = automataGrid[currentCell.x-1, currentCell.y+1];
                neighbours[6] = null;
                neighbours[7] = null;
                neighbours[5] = automataGrid[currentCell.x+1, currentCell.y+1];
            }
            else if(y == verticalSize-1)
            {
                // If the cell is on the top edge, set no top neighbours
                neighbours[0] = null;
                neighbours[1] = automataGrid[currentCell.x, currentCell.y-1];

                neighbours[4] = null;
                neighbours[6] = automataGrid[currentCell.x-1, currentCell.y-1];
                neighbours[7] = automataGrid[currentCell.x+1, currentCell.y-1];
                neighbours[5] = null;
            }
            else // If the cell is not on the top or bottom edge, set all neighbours
            {
                neighbours[0] = automataGrid[currentCell.x, currentCell.y+1];
                neighbours[1] = automataGrid[currentCell.x, currentCell.y-1];
                
                neighbours[4] = automataGrid[currentCell.x-1, currentCell.y+1];
                neighbours[6] = automataGrid[currentCell.x-1, currentCell.y-1];
                neighbours[7] = automataGrid[currentCell.x+1, currentCell.y-1];
                neighbours[5] = automataGrid[currentCell.x+1, currentCell.y+1];
            }
        }

        // Tell the cell its neighbours
        currentCell.MooreNeighbours(neighbours);
    }
}

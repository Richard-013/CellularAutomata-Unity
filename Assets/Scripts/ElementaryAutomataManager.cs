using UnityEngine;

public class ElementaryAutomataManager : MonoBehaviour
{
    const int LINEAR_NEIGHBOURS = 2;

    // Set in Editor
    public Material displayMaterial;

    // Texture to apply to render automata on
    Texture2D automatonTexture;

    // Cell colors
    Color deadColour = new Color(0.2415f, 0.2415f, 0.2415f, 1f);
    Color aliveColour = new Color(0.3745f, 0.6843f, 0.8372f, 1f);

    // Grid size
    public int horizontalSize = 150;
    public int verticalSize = 150;

    // Grid
    Cell[] automataGrid;

    // Shadow Grid of current states
    int[] shadowGrid;

    // Time tracking
    private float time = 0.0f;
    public float interpolationPeriod = 0.5f;

    // Always start at Generation 0
    int generation = 0;

    public int ruleSet = 222;

    void Awake()
    {
        SetupGrid();
        SetupTexture();
    }

    void Update()
    {
    }

    void SetupTexture()
    {
        automatonTexture = new Texture2D(horizontalSize, verticalSize, TextureFormat.RGB24, false);
        automatonTexture.filterMode = FilterMode.Point;

        Color[] pixels = automatonTexture.GetPixels();

        // Blank the texture to dead cell colour
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = deadColour;
        }

        automatonTexture.SetPixels(pixels);
        automatonTexture.Apply();

        UpdateTexture();
        
        displayMaterial.SetTexture("_BaseMap", automatonTexture);
    }

    void UpdateTexture()
    {
        Color[] pixels = automatonTexture.GetPixels();

        // Draw from top of texture downwards
        int startOfRow = verticalSize*(horizontalSize-(generation+1));

        for (int i = startOfRow; i < startOfRow+horizontalSize; i++)
        {
            // Convert array index into X then retrieve cell state
            int state = automataGrid[i % horizontalSize].state;

            if(state == 0)
            {
                pixels[i] = deadColour;
            }
            else
            {
                pixels[i] = aliveColour;
            }
        }

        automatonTexture.SetPixels(pixels);
        automatonTexture.Apply();

        // Increment generation as the current generation has been rendered
        generation++;
    }

    void SetupGrid()
    {
        // Initialise the grid
        automataGrid = new Cell[horizontalSize];
        shadowGrid = new int[horizontalSize];

        // Add a Cell instance to every grid cell
        for(int x = 0; x < horizontalSize; x++)
        {
            automataGrid[x] = new Cell(x);
        }

        SetupCell();
    }

    // Set the properties of each Cell
    void SetupCell()
    {
        for(int x = 0; x < horizontalSize; x++)
        {
            Cell currentCell = automataGrid[x];
            
            int newState;

            if(x == horizontalSize / 2)
            {
                newState = 1;
            }
            else
            {
                newState = 0;
            }

            currentCell.UpdateState(newState);
            shadowGrid[x] = newState;

            SetLinearNeighbours(currentCell, x);
        }
    }

    void SetLinearNeighbours(Cell currentCell, int x)
    {
        // Indexes
        // 0 = Left, 1 = Right
        Cell[] neighbours = new Cell[LINEAR_NEIGHBOURS];

        if(x == 0) // If the cell is on the left edge, set no left neighbour
        {
            neighbours[0] = null;
            neighbours[1] = automataGrid[currentCell.x+1];
        }
        else if(x == horizontalSize-1) // If the cell is on the right edge, set no right neighbour
        {
            neighbours[0] = automataGrid[currentCell.x-1];
            neighbours[1] = null;
        }
        else // If the cell is not on the left or right edge set both neighbours
        {
            neighbours[0] = automataGrid[currentCell.x-1];
            neighbours[1] = automataGrid[currentCell.x+1];
        }

        currentCell.SetLinearNeighbours(neighbours);
    }
}

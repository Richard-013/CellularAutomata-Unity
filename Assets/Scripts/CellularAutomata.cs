public static class CellularAutomata
{
    public static void RunAutomatonGeneration(int automataType, Cell[,] cells, int[,] previous, int xSize, int ySize, int neighbours)
    {
        switch(automataType)
            {
                case 0:
                    RunGameOfLifeGeneration(cells, previous, xSize, ySize, neighbours);
                    break;
                case 1:
                    RunSeedsGeneration(cells, previous, xSize, ySize, neighbours);
                    break;
                case 2:
                    RunBriansBrainGeneration(cells, previous, xSize, ySize, neighbours);
                    break;
                default:
                    RunGameOfLifeGeneration(cells, previous, xSize, ySize, neighbours);
                    break;
            }
    }

    public static void RunGameOfLifeGeneration(Cell[,] cells, int[,] previous, int xSize, int ySize, int neighbours)
    {
        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                Cell currentCell = cells[x, y];

                currentCell.GameOfLifeGeneration(TotalLiveNeighbours(currentCell, previous, neighbours));
            }
        }

        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                previous[x, y] = cells[x, y].state;
            }
        }
    }

    public static void RunSeedsGeneration(Cell[,] cells, int[,] previous, int xSize, int ySize, int neighbours)
    {
        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                Cell currentCell = cells[x, y];

                currentCell.SeedsGeneration(TotalLiveNeighbours(currentCell, previous, neighbours));
            }
        }

        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                previous[x, y] = cells[x, y].state;
            }
        }
    }

    public static void RunBriansBrainGeneration(Cell[,] cells, int[,] previous, int xSize, int ySize, int neighbours)
    {
        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                Cell currentCell = cells[x, y];

                currentCell.BriansBrainGeneration(TotalLiveNeighbours(currentCell, previous, neighbours));
            }
        }

        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                previous[x, y] = cells[x, y].state;
            }
        }
    }

    public static int TotalLiveNeighbours(Cell currentCell, int[,] previousStates, int neighbours)
    {
        int liveNeighbours = 0;

        for(int i = 0; i < neighbours; i++)
        {
            // Check if the neighbour exists
            if(currentCell.neighbours[i] != null)
            {
                // Check if the neighbour was alive in the last generation
                if(previousStates[currentCell.neighbours[i].x, currentCell.neighbours[i].y] == 1)
                {
                    liveNeighbours++;
                }
            }
        }

        return liveNeighbours;
    }
}

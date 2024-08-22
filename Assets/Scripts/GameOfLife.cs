public static class GameOfLife
{
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

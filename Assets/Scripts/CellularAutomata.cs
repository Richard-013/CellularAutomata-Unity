using System;

public static class CellularAutomata
{
    public static void RunAutomatonGeneration(int automataType, Cell[,] cells, int[,] previous, int[,] previousInfection, int xSize, int ySize, int neighbours, int infect)
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
                case 3:
                    RunBelosouvZhabotinskyGeneration(cells, previous, previousInfection, xSize, ySize, neighbours, infect);
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

    public static void RunBelosouvZhabotinskyGeneration(Cell[,] cells, int[,] previous, int[,] previousInfection, int xSize, int ySize, int neighbours, int infectivity)
    {
        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                Cell currentCell = cells[x, y];

                currentCell.BelosouvZhabotinskyGeneration(CalculateInfection(currentCell, infectivity, neighbours));
            }
        }

        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                previous[x, y] = cells[x, y].state;
                previousInfection[x, y] = cells[x, y].infection;
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

    public static int CalculateInfection(Cell currentCell, int infectivity, int neighbours)
    {
        // If a cell is already ill, don't calculate infection
        if(currentCell.state == 2)
        {
            return 101;
        }

        int infectionAmount;
        int numInfectedNeighbours = 0;
        int numIllNeighbours = 0;
        int infectionSum = currentCell.infection;

        // Calculate number of infected and ill neighbours
        for(int i = 0; i < neighbours; i++)
        {
            if(currentCell.neighbours[i] == null)
            {
                continue;
            }

            if(currentCell.neighbours[i].state == 1)
            {
                numInfectedNeighbours++;
            }
            else if(currentCell.neighbours[i].state == 2)
            {
                numIllNeighbours++;
            }
            
            infectionSum += currentCell.neighbours[i].infection;
        }

        if(currentCell.state == 0) // Healthy cell
        {
            infectionAmount = (numInfectedNeighbours/currentCell.resistanceInfection) + (numIllNeighbours/currentCell.resistanceIllness);
        }
        else // Infected cell
        {

            infectionAmount = (infectionSum/(numInfectedNeighbours+numIllNeighbours+1)) + infectivity;
        }

        return infectionAmount;
    }
}

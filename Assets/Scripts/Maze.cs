using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public int Rows = 4;
    public int Columns = 4;
    public GameObject Wall;
    public GameObject Floor;

    private MazeCell[,] grid;
    private int currentRow = 0;
    private int currentColumn = 0;

    // Start is called before the first frame update
    void Start()
    {
        // First, we create the grid with all the walls and floor.
        CreateGrid();

        // Then we run the algorithm to carve the path top-left to bottom-right.
        HuntAndKill();
    }

    void CreateGrid()
    {
        float size = Wall.transform.localScale.x;
        grid = new MazeCell[Rows, Columns];

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {

                GameObject floor = Instantiate(Floor, new Vector2(j, -i), Quaternion.identity);
                floor.name = "floor " + i + "-" + j;

                GameObject upwall = Instantiate(Wall, new Vector2(j, -i + 0.5f), Quaternion.identity);
                upwall.name = "upWall " + i + "-" + j;

                GameObject downwall = Instantiate(Wall, new Vector2(j, -i - 0.4f), Quaternion.identity);
                downwall.name = "downWall " + i + "-" + j;

                GameObject leftwall = Instantiate(Wall, new Vector3(j - 0.39f, -i, 1), Quaternion.Euler(0, 0, 90));
                leftwall.name = "leftWall " + i + "-" + j;

                GameObject rightwall = Instantiate(Wall, new Vector3(j + 0.5f, -i, 1), Quaternion.Euler(0, 0, 90));
                rightwall.name = "rightwall " + i + "-" + j;

                grid[i, j] = new MazeCell();
                grid[i, j].UpWall = upwall;
                grid[i, j].DownWall = downwall;
                grid[i, j].LeftWall = leftwall;
                grid[i, j].RightWall = rightwall;

                floor.transform.parent = transform;
                upwall.transform.parent = transform;
                downwall.transform.parent = transform;
                leftwall.transform.parent = transform;
                rightwall.transform.parent = transform;

            }
        }
    }

    bool AreThereUnvisitedNeighbors()
    {
        // Check up
        if (IsCellUnvisitedAndWithinBundaries(currentRow - 1, currentColumn))
        {
            return true;
        }
        
        // Check down
        if (IsCellUnvisitedAndWithinBundaries(currentRow + 1, currentColumn))
        {
            return true;
        }
        
        // Check left
        if (IsCellUnvisitedAndWithinBundaries(currentRow, currentColumn + 1))
        {
            return true;
        }
        
        // Check left
        if (IsCellUnvisitedAndWithinBundaries(currentRow, currentColumn - 1))
        {
            return true;
        }

        return false;
    }

    // Do a boundary check and unvisited check
    bool IsCellUnvisitedAndWithinBundaries(int row, int column)
    {
        if (row >= 0 && row < Rows && column >= 0 && column < Columns && !grid[row, column].Visited)
        {
            return true;
        }
        return false;
    }

    void HuntAndKill()
    {
        // Mark the first cell of the random walk as visited.
        grid[currentRow, currentColumn].Visited = true;

        while (AreThereUnvisitedNeighbors())
        {
            // Then go to a random direction.
            int direction = Random.Range(0, 4);

            // Check up
            if (direction == 0)
            {

                if (IsCellUnvisitedAndWithinBundaries(currentRow - 1, currentColumn))
                {
                    if (grid[currentRow, currentColumn].UpWall)
                    {
                        Destroy(grid[currentRow, currentColumn].UpWall);
                    }

                    currentRow--;
                    grid[currentRow, currentColumn].Visited = true;

                    if (grid[currentRow, currentColumn].DownWall)
                    {
                        Destroy(grid[currentRow, currentColumn].DownWall);
                    }
                }
            }

            // Check down
            else if (direction == 1)
            {
                if (IsCellUnvisitedAndWithinBundaries(currentRow + 1, currentColumn))
                {
                    if (grid[currentRow, currentColumn].DownWall)
                    {
                        Destroy(grid[currentRow, currentColumn].DownWall);
                    }

                    currentRow++;
                    grid[currentRow, currentColumn].Visited = true;

                    if (grid[currentRow, currentColumn].UpWall)
                    {
                        Destroy(grid[currentRow, currentColumn].UpWall);
                    }
                }
            }

            // Check left
            else if (direction == 2)
            {
                if (IsCellUnvisitedAndWithinBundaries(currentRow, currentColumn - 1))
                {
                    if (grid[currentRow, currentColumn].LeftWall)
                    {
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                    }

                    currentColumn--;
                    grid[currentRow, currentColumn].Visited = true;

                    if (grid[currentRow, currentColumn].RightWall)
                    {
                        Destroy(grid[currentRow, currentColumn].RightWall);
                    }
                }
            }

            // Check right
            else if (direction == 3)
            {
                if (IsCellUnvisitedAndWithinBundaries(currentRow, currentColumn + 1))
                {
                    if (grid[currentRow, currentColumn].RightWall)
                    {
                        Destroy(grid[currentRow, currentColumn].RightWall);
                    }

                    currentColumn++;
                    grid[currentRow, currentColumn].Visited = true;

                    if (grid[currentRow, currentColumn].LeftWall)
                    {
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                    }
                }
            }
        }
    }
}

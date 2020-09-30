using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Maze : MonoBehaviour
{
    public int Rows = 2;
    public int Columns = 2;
    public GameObject Wall;
    public GameObject Floor;
    public GameObject Goal;
    public InputField HeightField;
    public InputField WidthField;

    // robot2
    public GameObject Robot2;
    public Robot robot;

    private MazeCell[,] grid;
    private int currentRow = 0;
    private int currentColumn = 0;
    private bool scanComplete = false;

    // Start is called before the first frame update
    void Start()
    {
        // First, we create the grid with all the walls and floor.
        //CreateGrid();
    }

    void Update()
    {
                
    }

    void GenerateGrid()
    {
        // Destroy all the childern of this transform.
        foreach (Transform transform in transform)
        {
            Destroy(transform.gameObject);
        }

        // first, we create the grid with all the walls and floors.
        CreateGrid();

        // reset the algorithm variables.
        currentRow = 0;
        currentColumn = 0;
        scanComplete = false;

        // then we run the algorithm to carve the paths.
        HuntAndKill();
    }

    public void CreateGrid()
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

                if (i == 0 && j == 0)
                {
                    // create robot2
                    GameObject robot2 = Instantiate(Robot2, new Vector3(j, -i, 1), Quaternion.identity);
                    robot2.name = "robot2";
                    robot2.transform.parent = transform;
                    //    destroy(leftwall);
                }

                if (i == (Rows - 1) && j == (Columns - 1))
                {
                    // create goal
                    GameObject goal = Instantiate(Goal, new Vector2(j, -i), Quaternion.identity);
                    goal.name = "goal";
                    goal.transform.parent = transform;
                    Destroy(rightwall);
                }
            }
        }
    }

    void HuntAndKill()
    {
        // mark the first cell of the random walk as visited.
        grid[currentRow, currentColumn].Visited = true;

        while (!scanComplete)
        {
            Walk();
            Hunt();
        }
    }

    void Walk()
    {
        while (AreThereUnvisitedNeighbors())
        {
            // then go to a random direction.
            int direction = Random.Range(0, 4);

            // check up.
            if (direction == 0)
            {
                // make sure the above cell is unvisited and within grid boundaries.
                if (IsCellUnvisitedAndWithinBoundaries(currentRow - 1, currentColumn))
                {
                    // Debug.Log("Went up.");

                    // destroy the up wall of this cell if there's any.
                    if (grid[currentRow, currentColumn].UpWall)
                    {
                        Destroy(grid[currentRow, currentColumn].UpWall);
                    }

                    currentRow--;
                    grid[currentRow, currentColumn].Visited = true;

                    // destroy the down wall of the cell above if there's any.
                    if (grid[currentRow, currentColumn].DownWall)
                    {
                        Destroy(grid[currentRow, currentColumn].DownWall);
                    }
                }
            }
            // check down.
            else if (direction == 1)
            {
                // make sure the below cell is unvisited and within grid boundaries.
                if (IsCellUnvisitedAndWithinBoundaries(currentRow + 1, currentColumn))
                {
                    // Debug.Log("Went down.");

                    // destroy the down wall of this cell if there's any.
                    if (grid[currentRow, currentColumn].DownWall)
                    {
                        Destroy(grid[currentRow, currentColumn].DownWall);
                    }

                    currentRow++;
                    grid[currentRow, currentColumn].Visited = true;

                    // destroy the up wall of the cell below if there's any.
                    if (grid[currentRow, currentColumn].UpWall)
                    {
                        Destroy(grid[currentRow, currentColumn].UpWall);
                    }
                }
            }
            // check left.
            else if (direction == 2)
            {
                // make sure the left cell is unvisited and within grid boundaries.
                if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn - 1))
                {
                    // Debug.Log("Went left.");

                    // destroy the left wall of this cell if there's any.
                    if (grid[currentRow, currentColumn].LeftWall)
                    {
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                    }

                    currentColumn--;
                    grid[currentRow, currentColumn].Visited = true;

                    // destroy the right wall of the cell at the left if there's any.
                    if (grid[currentRow, currentColumn].RightWall)
                    {
                        Destroy(grid[currentRow, currentColumn].RightWall);
                    }
                }
            }
            // check right.
            else if (direction == 3)
            {
                // make sure the right cell is unvisited and within grid boundaries.
                if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn + 1))
                {
                    // Debug.Log("Went right.");

                    // destroy the right wall of this cell if there's any.
                    if (grid[currentRow, currentColumn].RightWall)
                    {
                        Destroy(grid[currentRow, currentColumn].RightWall);
                    }

                    currentColumn++;
                    grid[currentRow, currentColumn].Visited = true;

                    // destroy the left wall of the cell at the right if there's any.
                    if (grid[currentRow, currentColumn].LeftWall)
                    {
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                    }
                }
            }
        }
    }

    // after random walk is complete, we run Hunt.
    void Hunt()
    {
        // assume the scan is complete.
        scanComplete = true;

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                // if the condition is satisfied that a cell is unvisited and it has a visited neighbour, do another random walk from new cell.
                if (!grid[i, j].Visited && AreThereVisitedNeighbors(i, j))
                {
                    // scan is not actually complete.
                    scanComplete = false;
                    // set the new current row and column.
                    currentRow = i;
                    currentColumn = j;
                    // mark it as visited.
                    grid[currentRow, currentColumn].Visited = true;
                    // and create a passage (by destroying wall/s) between the new current cell and any adjacent cell.
                    DestroyAdjacentWall();

                    return;
                }
            }
        }
    }

    void DestroyAdjacentWall()
    {
        bool destroyed = false;

        while (!destroyed)
        {
            // pick a random adjacent cell that is visited and within boundaries,
            // and destroy the wall/s between the current cell and adjacent cell.
            int direction = Random.Range(0, 4);

            // check up.
            if (direction == 0)
            {
                if (currentRow > 0 && grid[currentRow - 1, currentColumn].Visited)
                {
                    // Debug.Log("Destroyed down wall of " + (currentRow - 1) + " " + currentColumn
                    //             + " and up wall of " + currentRow + " " + currentColumn);

                    if (grid[currentRow, currentColumn].UpWall)
                    {
                        Destroy(grid[currentRow, currentColumn].UpWall);
                    }

                    if (grid[currentRow - 1, currentColumn].DownWall)
                    {
                        Destroy(grid[currentRow - 1, currentColumn].DownWall);
                    }

                    destroyed = true;
                }
            }
            // check down.
            else if (direction == 1)
            {
                if (currentRow < Rows - 1 && grid[currentRow + 1, currentColumn].Visited)
                {
                    // Debug.Log("Destroyed up wall of " + (currentRow + 1) + " " + currentColumn
                    //             + " and down wall of " + currentRow + " " + currentColumn);

                    if (grid[currentRow, currentColumn].DownWall)
                    {
                        Destroy(grid[currentRow, currentColumn].DownWall);
                    }

                    if (grid[currentRow + 1, currentColumn].UpWall)
                    {
                        Destroy(grid[currentRow + 1, currentColumn].UpWall);
                    }

                    destroyed = true;
                }
            }
            // check left.
            else if (direction == 2)
            {
                if (currentColumn > 0 && grid[currentRow, currentColumn - 1].Visited)
                {
                    // Debug.Log("Destroyed right wall of " + currentRow + " " + (currentColumn - 1)
                    //         + " and left wall of " + currentRow + " " + currentColumn);

                    if (grid[currentRow, currentColumn].LeftWall)
                    {
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                    }

                    if (grid[currentRow, currentColumn - 1].RightWall)
                    {
                        Destroy(grid[currentRow, currentColumn - 1].RightWall);
                    }

                    destroyed = true;
                }
            }
            // check right.
            else if (direction == 3)
            {
                if (currentColumn < Columns - 1 && grid[currentRow, currentColumn + 1].Visited)
                {
                    // Debug.Log("Destroyed left wall of " + currentRow + " " + (currentColumn + 1)
                    //         + " and right wall of " + currentRow + " " + currentColumn);

                    if (grid[currentRow, currentColumn].RightWall)
                    {
                        Destroy(grid[currentRow, currentColumn].RightWall);
                    }

                    if (grid[currentRow, currentColumn + 1].LeftWall)
                    {
                        Destroy(grid[currentRow, currentColumn + 1].LeftWall);
                    }

                    destroyed = true;
                }
            }
        }
    }

    bool AreThereUnvisitedNeighbors()
    {
        // check up.
        if (IsCellUnvisitedAndWithinBoundaries(currentRow - 1, currentColumn))
        {
            return true;
        }

        // check down.
        if (IsCellUnvisitedAndWithinBoundaries(currentRow + 1, currentColumn))
        {
            return true;
        }

        // check left.
        if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn + 1))
        {
            return true;
        }

        // check right.
        if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn - 1))
        {
            return true;
        }

        return false;
    }

    public bool AreThereVisitedNeighbors(int row, int column)
    {
        // check up.
        if (row > 0 && grid[row - 1, column].Visited)
        {
            return true;
        }

        // check down.
        if (row < Rows - 1 && grid[row + 1, column].Visited)
        {
            return true;
        }

        // check left.
        if (column > 0 && grid[row, column - 1].Visited)
        {
            return true;
        }

        // check right.
        if (column < Columns - 1 && grid[row, column + 1].Visited)
        {
            return true;
        }

        return false;
    }

    // do a boundary check and unvisited check.
    bool IsCellUnvisitedAndWithinBoundaries(int row, int column)
    {
        if (row >= 0 && row < Rows && column >= 0 && column < Columns
            && !grid[row, column].Visited)
        {
            return true;
        }

        return false;
    }

    public void Regenerate()
    {
        int rows = 2;
        int columns = 2;

        if (int.TryParse(HeightField.text, out rows))
        {
            // set the minimum rows to 2.
            Rows = Mathf.Max(2, rows);
        }

        if (int.TryParse(WidthField.text, out columns))
        {
            // set the minimum columns to 2.
            Columns = Mathf.Max(2, columns);
        }

        GenerateGrid();
    }

    public int Choose()
    {
        Debug.Log("work");
        return 1;
        
    }
}

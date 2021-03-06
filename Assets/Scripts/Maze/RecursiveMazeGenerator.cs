using UnityEngine;
using System.Collections;

public class RecursiveMazeGenerator
{
    public int RowCount { get; }
    public int ColumnCount { get; }

    public MazeCell[,] Maze
    {
        get => _maze;
    }

    public int GoalCount;
    private MazeCell[,] _maze;

    public RecursiveMazeGenerator(int rows, int columns)
    {
        RowCount = Mathf.Abs(rows);
        ColumnCount = Mathf.Abs(columns);
        if (RowCount == 0)
        {
            RowCount = 1;
        }
        if (ColumnCount == 0)
        {
            ColumnCount = 1;
        }
        _maze = new MazeCell[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                _maze[row, column] = new MazeCell();
            }
        }
    }

    public void GenerateMaze()
    {
        VisitCell(0, 0, Direction.Start);        
    }

    private void VisitCell(int row, int column, Direction moveMade)
    {
        Direction[] movesAvailable = new Direction[4];
        int movesAvailableCount = 0;

        do
        {
            movesAvailableCount = 0;

            //check move right
            if (column + 1 < ColumnCount && !GetMazeCell(row, column + 1).IsVisited)
            {
                movesAvailable[movesAvailableCount] = Direction.Right;
                movesAvailableCount++;
            }
            else if (!GetMazeCell(row, column).IsVisited && moveMade != Direction.Left)
            {
                GetMazeCell(row, column).WallRight = true;
            }
            //check move forward
            if (row + 1 < RowCount && !GetMazeCell(row + 1, column).IsVisited)
            {
                movesAvailable[movesAvailableCount] = Direction.Front;
                movesAvailableCount++;
            }
            else if (!GetMazeCell(row, column).IsVisited && moveMade != Direction.Back)
            {
                GetMazeCell(row, column).WallFront = true;
            }
            //check move left
            if (column > 0 && column - 1 >= 0 && !GetMazeCell(row, column - 1).IsVisited)
            {
                movesAvailable[movesAvailableCount] = Direction.Left;
                movesAvailableCount++;
            }
            else if (!GetMazeCell(row, column).IsVisited && moveMade != Direction.Right)
            {
                GetMazeCell(row, column).WallLeft = true;
            }
            //check move backward
            if (row > 0 && row - 1 >= 0 && !GetMazeCell(row - 1, column).IsVisited)
            {
                movesAvailable[movesAvailableCount] = Direction.Back;
                movesAvailableCount++;
            }
            else if (!GetMazeCell(row, column).IsVisited && moveMade != Direction.Front)
            {
                GetMazeCell(row, column).WallBack = true;
            }

            if (movesAvailableCount == 0 && !GetMazeCell(row, column).IsVisited&& GoalCount>0)
            {
                GetMazeCell(row, column).IsGoal = true;
                GoalCount--;
            }

            GetMazeCell(row, column).IsVisited = true;

            if (movesAvailableCount > 0)
            {
                switch (movesAvailable[Random.Range(0, movesAvailableCount)])
                {
                    case Direction.Start:
                        break;
                    case Direction.Right:
                        VisitCell(row, column + 1, Direction.Right);
                        break;
                    case Direction.Front:
                        VisitCell(row + 1, column, Direction.Front);
                        break;
                    case Direction.Left:
                        VisitCell(row, column - 1, Direction.Left);
                        break;
                    case Direction.Back:
                        VisitCell(row - 1, column, Direction.Back);
                        break;
                }
            }
        } while (movesAvailableCount > 0);
    }

    public MazeCell GetMazeCell(int row, int column)
    {
        if (row >= 0 && column >= 0 && row < RowCount && column < ColumnCount)
        {
            return _maze[row, column];
        }
        else
        {
            Debug.Log(row + " " + column);
            throw new System.ArgumentOutOfRangeException();
        }
    }
}

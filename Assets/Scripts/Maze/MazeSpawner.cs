using System.Collections.Generic;
using UnityEngine;

public enum TypesMap
{
    Cube,
    Arbitrary
}

public class MazeSpawner : MonoBehaviour
{
    [Header("Maze Prefabs")]
    public GameObject Floor = null;
    public GameObject EmptyFloor = null;
    public GameObject Wall = null;
    public GameObject EmptyWall = null;
    public GameObject Final = null;
    public GameObject Empty = null;
    public GameObject GoalPrefab = null;
    public GameObject PeoplePrefab = null;

    [Header("Size Prefabs")]
    public float CellWidth = 4;
    public float FenceWidth = 2;
    public float CellHeight = 4;

    [Header("Goal Count")]
    public int GoalCount = 1;
    public int FinalCount = 1;
    public int PeopleAmount = 3;
    [Range(0, 1)]
    public float ChanceOfGeneratingPeople = 0.3f;

    [Header("Size Map")]
    public TypesMap TypeMap = TypesMap.Cube;
    public bool EmptyMap = false;
    [HideInInspector] public int AmountOfCellsInRow = 5;
    [HideInInspector] public int LengthCells = 5;
    [HideInInspector] public int WidthCells = 5;
    [HideInInspector] public int HeightCells = 5;

    private int _currentPeopleAmount = 0;

    private GameObject _level;
    private GameObject _people;
    private GameObject _levelEmpties;
    private GameObject _levelWalls;
    private GameObject _levelFloor;
    private List<GameObject> _floors = new List<GameObject>();
    private List<GameObject> _walls = new List<GameObject>();
    private List<GameObject> _levels = new List<GameObject>();
    private Dictionary<GameObject, List<GameObject>> _bakingLayers = new Dictionary<GameObject, List<GameObject>>();

    private RecursiveMazeGenerator _mazeGenerator = null;

    private GameObject InstantiatePartOfMaze(GameObject gameObject, Vector3 position, Quaternion quaternion, Transform parent)
    {
        GameObject tmp;
        tmp = Instantiate(gameObject, position, quaternion) as GameObject;
        tmp.transform.parent = parent;
        return tmp;
    }

    private void CheckAndSpawnCell(int row, int column, int level)
    {
        float x = column * CellWidth;
        float z = row * CellHeight;
        float y = -level * FenceWidth;
        MazeCell cell = _mazeGenerator.GetMazeCell(row, column);

        if (GoalPrefab != null)
        {
            if (EmptyMap == false)
            {
                if (!cell.IsGoal)
                {
                    var floor = InstantiatePartOfMaze(Floor, new Vector3(x, y, z), Quaternion.Euler(0, 0, 0), _level.transform);
                    _floors.Add(floor);
                    floor.transform.parent = _levelFloor.transform;
                    if (_currentPeopleAmount > 0)
                    {
                        if (Random.Range(0, 100) <= ChanceOfGeneratingPeople * 100)
                        {
                            _currentPeopleAmount--;
                            InstantiatePartOfMaze(PeoplePrefab, new Vector3(x, y, z), Quaternion.Euler(0, Random.Range(0, 359), 0), _people.transform);
                        }
                    }
                }
                else if ((TypeMap == TypesMap.Cube && level + 1 == AmountOfCellsInRow) || (TypeMap == TypesMap.Arbitrary && level + 1 == HeightCells))
                    InstantiatePartOfMaze(Final, new Vector3(x, y, z), Quaternion.Euler(0, 0, 0), _levelEmpties.transform);
                else
                    InstantiatePartOfMaze(Empty, new Vector3(x, y, z), Quaternion.Euler(0, 0, 0), _levelEmpties.transform);

                if (cell.WallRight)
                {
                    var wall = InstantiatePartOfMaze(Wall, new Vector3(x + CellWidth / 2, y, z) + Wall.transform.position, Quaternion.Euler(0, Wall.transform.rotation.y - 90, 0), _level.transform);
                    _walls.Add(wall);
                    wall.transform.parent = _levelWalls.transform;
                }

                if (cell.WallFront)
                {
                    var wall = InstantiatePartOfMaze(Wall, new Vector3(x, y, z + CellHeight / 2) + Wall.transform.position, Quaternion.Euler(0, Wall.transform.rotation.y - 0, 0), _level.transform);
                    _walls.Add(wall);
                    wall.transform.parent = _levelWalls.transform;
                }

                if (cell.WallLeft)
                {
                    var wall = InstantiatePartOfMaze(Wall, new Vector3(x - CellWidth / 2, y, z) + Wall.transform.position, Quaternion.Euler(0, Wall.transform.rotation.y - 270, 0), _level.transform);
                    _walls.Add(wall);
                    wall.transform.parent = _levelWalls.transform;
                }

                if (cell.WallBack)
                {
                    var wall = InstantiatePartOfMaze(Wall, new Vector3(x, y, z - CellHeight / 2) + Wall.transform.position, Quaternion.Euler(0, Wall.transform.rotation.y - 180, 0), _level.transform);
                    _walls.Add(wall);
                    wall.transform.parent = _levelWalls.transform;
                }
            }
            else
            {
                var wall0 = InstantiatePartOfMaze(EmptyWall, new Vector3(x + CellWidth / 2, y, z) + EmptyWall.transform.position, Quaternion.Euler(0, EmptyWall.transform.rotation.y - 90, 0), _level.transform);
                _walls.Add(wall0);
                wall0.transform.parent = _levelWalls.transform;
                var wall1 = InstantiatePartOfMaze(EmptyWall, new Vector3(x, y, z + CellHeight / 2) + EmptyWall.transform.position, Quaternion.Euler(0, EmptyWall.transform.rotation.y - 0, 0), _level.transform);
                _walls.Add(wall1);
                wall1.transform.parent = _levelWalls.transform;
                var wall2 = InstantiatePartOfMaze(EmptyWall, new Vector3(x - CellWidth / 2, y, z) + EmptyWall.transform.position, Quaternion.Euler(0, EmptyWall.transform.rotation.y - 270, 0), _level.transform);
                _walls.Add(wall2);
                wall2.transform.parent = _levelWalls.transform;
                var wall3 = InstantiatePartOfMaze(EmptyWall, new Vector3(x, y, z - CellHeight / 2) + EmptyWall.transform.position, Quaternion.Euler(0, EmptyWall.transform.rotation.y - 180, 0), _level.transform);
                _walls.Add(wall3);
                wall3.transform.parent = _levelWalls.transform;
                var floor = InstantiatePartOfMaze(EmptyFloor, new Vector3(x, y, z), Quaternion.Euler(0, 0, 0), _level.transform);
                _floors.Add(floor);
                floor.transform.parent = _levelFloor.transform;
            }
        }
    }

    public void SpawnMaze()
    {
#if UNITY_EDITOR
        _currentPeopleAmount = PeopleAmount;
        _levels = new List<GameObject>();
        if (TypeMap == TypesMap.Cube)
            SpawnMaze(AmountOfCellsInRow);
        else if (TypeMap == TypesMap.Arbitrary)
            SpawnMaze(LengthCells, WidthCells, HeightCells);
#endif
    }

    public List<GameObject> GetMazeLevels()
    {
        return _levels;
    }

    private void SpawnMaze(int amountOfCellsInRow)
    {
        for (int level = 0; level < amountOfCellsInRow; level++)
        {
            CreateLevelHierarchy(level);
            SpawnLevel(amountOfCellsInRow, amountOfCellsInRow, amountOfCellsInRow, level);
            AddBakingLayer(level);
        }
    }

    private void SpawnMaze(int lengthCell, int widthCell, int heightCell)
    {
        for (int level = 0; level < heightCell; level++)
        {
            CreateLevelHierarchy(level);
            SpawnLevel(lengthCell, widthCell, heightCell, level);
            AddBakingLayer(level);
        }
    }

    public void BakeLevel(int amountCalls)
    {
        int numLevel = 0;
        foreach (var part in _bakingLayers)
        {
            GameObject bakedObject = part.Key;
#if UNITY_EDITOR
            MeshBaker.BakeMeshes(part.Value.ToArray(), bakedObject, amountCalls);
#endif
            bakedObject.transform.parent = _levels[numLevel / 2].transform;
            bakedObject.AddComponent<MeshCollider>();
            numLevel++;
        }
    }

    private void SpawnLevel(int rows, int columns, int height, int currentLevel)
    {
        _mazeGenerator = new RecursiveMazeGenerator(rows, columns);
        if (currentLevel + 1 == height)
            _mazeGenerator.GoalCount = FinalCount;
        else
            _mazeGenerator.GoalCount = GoalCount;
        _mazeGenerator.GenerateMaze();
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                CheckAndSpawnCell(row, column, currentLevel);
            }
        }
    }

    private void CreateLevelHierarchy(int level)
    {
        _people = new GameObject("People");
        _level = new GameObject($"Level {level + 1}");
        _level.transform.parent = transform;
        _people.transform.parent = _level.transform;
        _levels.Add(_level);
        _levelEmpties = new GameObject($"Empties {level + 1}");
        _levelEmpties.transform.parent = _level.transform;
        _levelWalls = new GameObject($"Walls {level + 1}");
        _levelWalls.transform.parent = _level.transform;
        _levelFloor = new GameObject($"Floor {level + 1}");
        _levelFloor.transform.parent = _level.transform;
        _floors = new List<GameObject>();
        _walls = new List<GameObject>();
    }

    private void AddBakingLayer(int level)
    {
        if (_bakingLayers.Keys.Count > level * 2)
            _bakingLayers.Clear();
        _bakingLayers.Add(_levelFloor, _floors);
        _bakingLayers.Add(_levelWalls, _walls);
    }
}

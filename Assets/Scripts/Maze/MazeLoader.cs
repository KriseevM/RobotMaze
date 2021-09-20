using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeLoader : MonoBehaviour
{
    public string MapName = "Map 0";
    public static string MapsPath = "";
    public static int MapID=2;

    private static MazeSpawner _mazeSpawner;
    private static MazeLoader _mazeLoader;
    private static int _amountOfLevels;
    private static int _amountOfStartLevels = 3;
    private static int _currentLevel = 0;
    private static Queue<GameObject> _activeLevels = new Queue<GameObject>();

    public static void GetMapInfo()
    {
        GameObject map = (GameObject)Resources.Load($"Levels/{_mazeLoader.MapName}/{_mazeLoader.MapName}");
        _mazeSpawner = map.GetComponent<MazeSpawner>();
    }

    public static void LoadMap()
    {
        var map = Instantiate(Resources.Load($"Levels/{_mazeLoader.MapName}/{_mazeLoader.MapName}")) as GameObject;
    }

    public static void LoadNextMap()
    {
        UnLoadLevel();
        _currentLevel = 0;
        MapID++;
        _mazeLoader.MapName = "Map "+ MapID;
        GetMapInfo();
        LoadStartLevels();
    }

    public static void LoadLevel(int numberLevel)
    {
        Debug.Log($"Levels/{_mazeLoader.MapName}/Level {numberLevel}");
        try
        {
            var map = Instantiate(Resources.Load($"Levels/{_mazeLoader.MapName}/Level {numberLevel}")) as GameObject;
            _activeLevels.Enqueue(map);
            _currentLevel = numberLevel;
        }
        catch {}
    }

    public static void UnLoadLevel()
    {
        Destroy(_activeLevels.Dequeue(), 1);
    }

    public static void LoadNextLevel()
    {
        if (_amountOfLevels > 3)
        {
            _currentLevel++;
            Debug.Log(_currentLevel);
            Debug.Log(_amountOfLevels + "-");
            LoadLevel(_currentLevel);
            UnLoadLevel();
        }
    }

    private static void LoadStartLevels()
    {
        if (_mazeSpawner.TypeMap == TypesMap.Cube)
            _amountOfLevels = _mazeSpawner.AmountOfCellsInRow;
        else if (_mazeSpawner.TypeMap == TypesMap.Arbitrary)
            _amountOfLevels = _mazeSpawner.HeightCells;

        if (_amountOfLevels > _amountOfStartLevels)
        {
            for (int numberLevel = 1; numberLevel <= _amountOfStartLevels; numberLevel++)
                LoadLevel(numberLevel);
            _currentLevel = _amountOfStartLevels;
        }
        else
        {
            for (int numberLevel = 1; numberLevel <= _amountOfLevels; numberLevel++)
                LoadLevel(numberLevel);
            _currentLevel = _amountOfLevels;
        }
    }

    private void Start()
    {
        _mazeLoader = this;
        MapName = "Map " + MapID;
        GetMapInfo();
        LoadStartLevels();
    }
}

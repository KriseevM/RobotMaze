using System.IO;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
[CustomEditor(typeof(MazeSpawner)), CanEditMultipleObjects]
public class MazeEditor : Editor
{
    private MazeSpawner _mapGenerator;

    private static int _amountSavedMaps = 0;
    private bool _isBaked = false;

    public override void OnInspectorGUI()
    {
        _mapGenerator = (MazeSpawner)target;
        if (DrawDefaultInspector())
        {
        }
        if (_mapGenerator.TypeMap == TypesMap.Cube)
        {
            _mapGenerator.AmountOfCellsInRow = EditorGUILayout.IntField("Amount Of Cells In Row:", _mapGenerator.AmountOfCellsInRow);
        }
        else if (_mapGenerator.TypeMap == TypesMap.Arbitrary)
        {
            _mapGenerator.LengthCells = EditorGUILayout.IntField("Length Cells:", _mapGenerator.LengthCells);
            _mapGenerator.WidthCells = EditorGUILayout.IntField("Width Cells:", _mapGenerator.WidthCells);
            _mapGenerator.HeightCells = EditorGUILayout.IntField("Height Cells:", _mapGenerator.HeightCells);
        }      
        if (GUILayout.Button("Generate"))
        {
            GenerateMapInEditor();
        }
        if (GUILayout.Button("Edit"))
        {            
            
        }        
        if (GUILayout.Button("Destroy"))
        {
            ClearMapInEditor();
        }
        if (GUILayout.Button("Bake"))
        {
            BakeMapInEditor();
        }
        if (GUILayout.Button("Save"))
        {
            SaveMapInEditor();
        }
        if (GUILayout.Button("Load"))
        {
            MazeLoader.LoadMap();
        }
    }

    private void GenerateMapInEditor()
    {
        if (_mapGenerator.transform.childCount > 0)
            ClearMapInEditor();
        _isBaked = false;
        _mapGenerator.SpawnMaze();
    }

    private void ClearMapInEditor()
    {
        while (_mapGenerator.transform.childCount > 0)
        {
            DestroyImmediate(_mapGenerator.transform.GetChild(0).gameObject);
        }
    }

    private void BakeMapInEditor()
    {
        if (_mapGenerator.transform.childCount > 0)
            _mapGenerator.BakeLevel(_amountSavedMaps);
        _isBaked = true;
    }

    private void SaveMapInEditor()
    {
        if (_mapGenerator.transform.childCount > 0)
        {
            BakeMapInEditor();
            var levels = _mapGenerator.GetMazeLevels();
            string path = $"Assets/MazeGenerator/Prefabs/Resources/Levels/Map {_amountSavedMaps}";
            Directory.CreateDirectory(path);
            for (int num = 0; num < levels.Count; num++)
            {
                PrefabUtility.SaveAsPrefabAsset(levels[num].gameObject, path+$"/Level {num+1}.prefab");
            }
            PrefabUtility.SaveAsPrefabAsset(_mapGenerator.gameObject, path+$"/Map {_amountSavedMaps}.prefab");
            _amountSavedMaps++;
            EditorUtility.DisplayDialog("", "The map was saved", "ok");
        }
        else
        {
            EditorUtility.DisplayDialog("", "The map do not generate!!! Generate please it :-)", "ok");
        }
    }

}
#endif
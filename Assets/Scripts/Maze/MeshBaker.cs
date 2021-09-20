using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
public class MeshBaker : MonoBehaviour
{
    enum Resolving
    {
        Not,
        Remove,
        Create
    }

    private const int MAX_VERTEX_COUNT_PER_ONE_OBJECT = 65000;

    private static Resolving colorResolving;
    private static Resolving normalsResolving;
    private static Resolving tangentsResolving;
    private static Resolving uvResolving;

    private static int objectNum;
    private static int _amountCalls = 0;

    private static GameObject bakedGO;

    public static GameObject BakeMeshes(GameObject[] menuCommand, GameObject gameObject, int amountCalls = 0)
    {
        List<Vector3> vertexes = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector4> tangents = new List<Vector4>();
        List<Color> colors = new List<Color>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        Dictionary<Material, List<Mesh>> meshesWithMaterials = new Dictionary<Material, List<Mesh>>();
        List<Mesh> meshesWithoutMaterials = new List<Mesh>();

        _amountCalls= amountCalls;
        bakedGO = gameObject;
        objectNum = 0;
        if (!FillDataAndCheckResolving(meshesWithMaterials, meshesWithoutMaterials, menuCommand))
        {
            gameObject = null;
            return gameObject;
        }

        foreach (var meshToBake in meshesWithMaterials)
        {
            foreach (var mesh in meshToBake.Value)
            {
                Bake(mesh, vertexes, normals, tangents, colors, uvs, triangles, meshToBake.Key);
            }
            gameObject = CreateObject(vertexes, normals, tangents, colors, uvs, triangles, meshToBake.Key);
            vertexes.Clear();
            normals.Clear();
            tangents.Clear();
            colors.Clear();
            uvs.Clear();
            triangles.Clear();
        }

        foreach (GameObject selected in menuCommand)
        {
            if (selected != null)
            {
                Undo.DestroyObjectImmediate(selected);
            }
        }
        return gameObject;
    }

    private static bool FillDataAndCheckResolving(Dictionary<Material, List<Mesh>> meshesWithMaterials, List<Mesh> meshesWithoutMaterials, GameObject[] gameObjects)
    {
        colorResolving = Resolving.Not;
        normalsResolving = Resolving.Not;
        uvResolving = Resolving.Not;

        bool anyHasColors = false;
        bool anyHasNormals = false;
        bool anyHasUVs = false;
        bool anyHasNotColors = false;
        bool anyHasNotNormals = false;
        bool anyHasNotUVs = false;

        HashSet<Transform> transforms = new HashSet<Transform>();

        foreach (GameObject selected in gameObjects)
        {
            MeshFilter[] meshFilters = selected.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshFilters)
            {
                if (transforms.Contains(meshFilter.transform))
                {
                    continue;
                }
                Material material = null;
                var mr = meshFilter.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    material = mr.sharedMaterial;
                }
                Mesh mesh = Instantiate(meshFilter.sharedMesh);
                HandleMesh(meshesWithMaterials, meshesWithoutMaterials, mesh, meshFilter.transform, material, transforms, ref anyHasNotNormals, ref anyHasNotColors, ref anyHasNotUVs, ref anyHasNormals, ref anyHasColors, ref anyHasUVs);
            }
        }

        return SetResolving(anyHasNormals, anyHasNotNormals, ref normalsResolving, "normals") &&
               SetResolving(anyHasColors, anyHasNotColors, ref colorResolving, "colors") &&
               SetResolving(anyHasUVs, anyHasNotUVs, ref uvResolving, "uvs");
    }

    private static bool FillDataAndCheckResolving(Dictionary<Material, List<Mesh>> meshesWithMaterials, List<Mesh> meshesWithoutMaterials)
    {
        colorResolving = Resolving.Not;
        normalsResolving = Resolving.Not;
        uvResolving = Resolving.Not;

        bool anyHasColors = false;
        bool anyHasNormals = false;
        bool anyHasUVs = false;
        bool anyHasNotColors = false;
        bool anyHasNotNormals = false;
        bool anyHasNotUVs = false;

        HashSet<Transform> transforms = new HashSet<Transform>();

        foreach (GameObject selected in Selection.gameObjects)
        {
            MeshFilter[] meshFilters = selected.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshFilters)
            {
                if (transforms.Contains(meshFilter.transform))
                {
                    continue;
                }
                Material material = null;
                var mr = meshFilter.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    material = mr.sharedMaterial;
                }
                Mesh mesh = Instantiate(meshFilter.sharedMesh);
                HandleMesh(meshesWithMaterials, meshesWithoutMaterials, mesh, meshFilter.transform, material, transforms, ref anyHasNotNormals, ref anyHasNotColors, ref anyHasNotUVs, ref anyHasNormals, ref anyHasColors, ref anyHasUVs);
            }
        }

        return SetResolving(anyHasNormals, anyHasNotNormals, ref normalsResolving, "normals") &&
               SetResolving(anyHasColors, anyHasNotColors, ref colorResolving, "colors") &&
               SetResolving(anyHasUVs, anyHasNotUVs, ref uvResolving, "uvs");
    }

    private static void HandleMesh(Dictionary<Material, List<Mesh>> meshesWithMaterials, List<Mesh> meshesWithoutMaterials, Mesh mesh,
        Transform transform, Material material, HashSet<Transform> transforms, ref bool anyHasNotNormals,
        ref bool anyHasNotColors, ref bool anyHasNotUVs, ref bool anyHasNormals, ref bool anyHasColors, ref bool anyHasUVs)
    {
        mesh.vertices = mesh.vertices.Select(transform.TransformPoint).ToArray();
        mesh.normals = mesh.normals.Select(transform.TransformDirection).ToArray();

        if (material == null)
        {
            meshesWithoutMaterials.Add(mesh);
        }
        else
        {
            if (meshesWithMaterials.ContainsKey(material))
            {
                meshesWithMaterials[material].Add(mesh);
            }
            else
            {
                meshesWithMaterials.Add(material, new List<Mesh>() { mesh });
            }
        }
        transforms.Add(transform);

        CheckMeshAttributes(mesh, ref anyHasNotNormals, ref anyHasNotColors, ref anyHasNotUVs, ref anyHasNormals,
            ref anyHasColors, ref anyHasUVs);
    }

    private static void CheckMeshAttributes(Mesh mesh, ref bool anyHasNotNormals, ref bool anyHasNotColors, ref bool anyHasNotUVs,
        ref bool anyHasNormals, ref bool anyHasColors, ref bool anyHasUVs)
    {
        bool hasNormals = mesh.vertexCount == mesh.normals.Length;
        bool hasColors = mesh.vertexCount == mesh.colors.Length;
        bool hasUVs = mesh.vertexCount == mesh.uv.Length;

        anyHasNotNormals |= !hasNormals;
        anyHasNotColors |= !hasColors;
        anyHasNotUVs |= !hasUVs;

        anyHasNormals |= hasNormals;
        anyHasColors |= hasColors;
        anyHasUVs |= hasUVs;
    }

    private static bool SetResolving(bool has, bool hasNot, ref Resolving resolving, string property)
    {
        if (has && hasNot)
        {
            var result = EditorUtility.DisplayDialogComplex("Simplest Mesh Baker",
                "Not all objects used " + property + ".",
                "Don't use " + property, //0
                "Cancel", //1
                "Create fake " + property //2
            );
            if (result == 1)
            {
                return false;
            }
            resolving = result == 0 ? Resolving.Remove : Resolving.Create;
        }
        return true;
    }

    private static GameObject CreateObject(List<Vector3> vertexes, List<Vector3> normals, List<Vector4> tangents, List<Color> colors,
        List<Vector2> uvs, List<int> triangles, Material material)
    {
        objectNum++;
        MeshFilter mf = bakedGO.AddComponent<MeshFilter>();
        MeshRenderer mr = bakedGO.AddComponent<MeshRenderer>();
        Mesh newMesh = new Mesh();
        newMesh.SetVertices(vertexes);
        if (normals.Count != 0 && normalsResolving != Resolving.Remove)
        {
            newMesh.SetNormals(normals);
        }
        if (tangents.Count != 0 && tangentsResolving != Resolving.Remove)
        {
            newMesh.SetTangents(tangents);
        }
        if (colors.Count != 0 && colorResolving != Resolving.Remove)
        {
            newMesh.SetColors(colors);
        }
        if (uvs.Count != 0 || uvResolving != Resolving.Remove)
        {
            newMesh.SetUVs(0, uvs);
        }        
        newMesh.SetTriangles(triangles, 0);

        string path = $"Assets/Mesh_{_amountCalls}";
        Directory.CreateDirectory(path);
        var prefab = EditorUtility.CreateEmptyPrefab(path+ ".prefab");
        AssetDatabase.AddObjectToAsset(newMesh, path + ".prefab");
        mf.sharedMesh = newMesh;
        mr.material = material;
        EditorUtility.ReplacePrefab(bakedGO, prefab, ReplacePrefabOptions.ReplaceNameBased);
        return bakedGO;
    }

    private static void Bake(Mesh mesh, List<Vector3> vertexes, List<Vector3> normals, List<Vector4> tangents,
        List<Color> colors, List<Vector2> uvs, List<int> triangles, Material material)
    {
        //mesh may not have more than 65000 vertices.
        if (vertexes.Count + mesh.vertexCount > MAX_VERTEX_COUNT_PER_ONE_OBJECT)
        {
            CreateObject(vertexes, normals, tangents, colors, uvs, triangles, material);
            vertexes.Clear();
            normals.Clear();
            tangents.Clear();
            colors.Clear();
            uvs.Clear();
            triangles.Clear();
        }

        int startCount = vertexes.Count;
        foreach (Vector3 vertex in mesh.vertices)
        {
            vertexes.Add(vertex);
        }
        foreach (int triangle in mesh.triangles)
        {
            triangles.Add(triangle + startCount);
        }

        FillOrResolve(mesh.normals, normals, mesh.vertices.Length, normalsResolving);
        FillOrResolve(mesh.tangents, tangents, mesh.tangents.Length, tangentsResolving);
        FillOrResolve(mesh.colors, colors, mesh.vertices.Length, colorResolving);
        FillOrResolve(mesh.uv, uvs, mesh.vertices.Length, uvResolving);
    }

    private static void FillOrResolve<T>(T[] source, List<T> distanation, int expectedCount,
        Resolving resolvingLogic)
    {
        if (source.Length == 0 && resolvingLogic == Resolving.Create)
        {
            for (int i = 0; i < expectedCount; i++)
            {
               distanation.Add(default(T));
            }
        }
        else
        {
            distanation.AddRange(source);
        }
    }
}
#endif
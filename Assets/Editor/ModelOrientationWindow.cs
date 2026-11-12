using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

public class ModelOrientationWindow : EditorWindow
{
    const float Step = 90f;

    [SerializeField] GameObject model;
    [SerializeField] Vector3 euler;

    [MenuItem("Tools/Model Orientation")]
    public static void Open()
    {
        GetWindow<ModelOrientationWindow>("Model Orientation");
    }

    [MenuItem("Assets/Reorient Model", false, 30)]
    static void OpenForSelection()
    {
        ModelOrientationWindow window = GetWindow<ModelOrientationWindow>("Model Orientation");
        window.AssignFromSelection();
    }

    [MenuItem("Assets/Reorient Model", true)]
    static bool ValidateSelection()
    {
        return TryGetImporter(Selection.activeObject, out _);
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Model Orientation", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Applies this rotation to the FBX mesh on import. The angle is stored in the model's .meta, so prefabs that use the mesh pick it up.",
            MessageType.Info);

        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        GameObject next = (GameObject)EditorGUILayout.ObjectField("Model", model, typeof(GameObject), false);
        if (EditorGUI.EndChangeCheck())
            SetModel(next);

        if (GUILayout.Button("Use Selected", GUILayout.Width(110)))
            AssignFromSelection();
        EditorGUILayout.EndHorizontal();

        bool valid = TryGetImporter(model, out ModelImporter importer);
        if (model != null && !valid)
            EditorGUILayout.HelpBox("Choose an imported model, such as an FBX.", MessageType.Warning);

        Vector3 saved = Vector3.zero;
        bool hasSaved = valid && ModelOrientationUtility.TryRead(importer.userData, out saved);

        using (new EditorGUI.DisabledScope(!valid))
        {
            EditorGUILayout.Space();
            DrawAxis("X", ref euler.x);
            DrawAxis("Y", ref euler.y);
            DrawAxis("Z", ref euler.z);

            EditorGUILayout.Space();
            if (hasSaved)
                EditorGUILayout.LabelField("Saved on model", $"{saved.x:0.##}, {saved.y:0.##}, {saved.z:0.##}");
            else if (valid)
                EditorGUILayout.LabelField("Saved on model", "None");

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(!valid || Approximately(euler, saved)))
            {
                if (GUILayout.Button("Apply"))
                    Apply(importer);
            }

            using (new EditorGUI.DisabledScope(!hasSaved))
            {
                if (GUILayout.Button("Reset"))
                    ResetOrientation(importer);
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    void DrawAxis(string label, ref float axis)
    {
        EditorGUILayout.BeginHorizontal();
        axis = EditorGUILayout.FloatField(label, axis);
        if (GUILayout.Button("-90", GUILayout.Width(48)))
            axis = WrapDegrees(axis - Step);
        if (GUILayout.Button("+90", GUILayout.Width(48)))
            axis = WrapDegrees(axis + Step);
        EditorGUILayout.EndHorizontal();
    }

    void AssignFromSelection()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        SetModel(AssetDatabase.LoadAssetAtPath<GameObject>(path));
    }

    void SetModel(GameObject next)
    {
        model = next;
        euler = Vector3.zero;
        if (TryGetImporter(model, out ModelImporter importer) && ModelOrientationUtility.TryRead(importer.userData, out Vector3 saved))
            euler = saved;
    }

    void Apply(ModelImporter importer)
    {
        importer.userData = ModelOrientationUtility.Write(importer.userData, euler);
        string name = model != null ? model.name : importer.assetPath;

        try
        {
            EditorUtility.DisplayProgressBar("Model Orientation", "Reimporting " + name, 0.5f);
            importer.SaveAndReimport();
            Debug.Log($"Set orientation of {name} to {euler.x:0.##}, {euler.y:0.##}, {euler.z:0.##}.", model);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorUtility.DisplayDialog("Model Orientation", "Reimport failed. See the Console for details.", "OK");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    void ResetOrientation(ModelImporter importer)
    {
        if (!EditorUtility.DisplayDialog("Reset model orientation", "Reimport this model with no baked rotation?", "Reset", "Cancel"))
            return;

        euler = Vector3.zero;
        Apply(importer);
    }

    static bool TryGetImporter(UnityEngine.Object asset, out ModelImporter importer)
    {
        importer = null;
        if (asset == null)
            return false;

        string path = AssetDatabase.GetAssetPath(asset);
        if (string.IsNullOrEmpty(path))
            return false;

        importer = AssetImporter.GetAtPath(path) as ModelImporter;
        return importer != null;
    }

    static float WrapDegrees(float degrees)
    {
        degrees %= 360f;
        if (degrees > 180f)
            degrees -= 360f;
        if (degrees < -180f)
            degrees += 360f;
        return degrees;
    }

    static bool Approximately(Vector3 a, Vector3 b)
    {
        return (a - b).sqrMagnitude < 0.0001f;
    }
}

public class ModelOrientationPostprocessor : AssetPostprocessor
{
    public override int GetPostprocessOrder()
    {
        return -100;
    }

    void OnPostprocessModel(GameObject root)
    {
        if (assetImporter is not ModelImporter importer)
            return;

        if (!ModelOrientationUtility.TryRead(importer.userData, out Vector3 euler))
            return;

        if (euler.sqrMagnitude < 0.0000001f)
            return;

        ModelOrientationUtility.Apply(root, Quaternion.Euler(euler));
    }
}

static class ModelOrientationUtility
{
    const string Prefix = "ModelOrientationEuler=";

    public static void Apply(GameObject root, Quaternion rotation)
    {
        var seen = new HashSet<Mesh>();
        Matrix4x4 rot = Matrix4x4.Rotate(rotation);
        Transform rootTransform = root.transform;

        foreach (MeshFilter filter in root.GetComponentsInChildren<MeshFilter>(true))
            Rotate(filter.sharedMesh, filter.transform, rootTransform, rot, seen);

        foreach (SkinnedMeshRenderer renderer in root.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            Rotate(renderer.sharedMesh, renderer.transform, rootTransform, rot, seen);

        foreach (MeshCollider collider in root.GetComponentsInChildren<MeshCollider>(true))
            Rotate(collider.sharedMesh, collider.transform, rootTransform, rot, seen);
    }

    public static bool TryRead(string userData, out Vector3 euler)
    {
        euler = Vector3.zero;
        if (string.IsNullOrEmpty(userData))
            return false;

        string[] lines = userData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (!trimmed.StartsWith(Prefix, StringComparison.Ordinal))
                continue;

            string[] parts = trimmed.Substring(Prefix.Length).Split(',');
            if (parts.Length != 3)
                return false;

            if (float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x)
                && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y)
                && float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
            {
                euler = new Vector3(x, y, z);
                return true;
            }
        }

        return false;
    }

    public static string Write(string userData, Vector3 euler)
    {
        var lines = new List<string>();
        if (!string.IsNullOrEmpty(userData))
        {
            string[] existing = userData.Split('\n');
            foreach (string line in existing)
            {
                string trimmed = line.Trim().TrimEnd('\r');
                if (trimmed.Length == 0 || trimmed.StartsWith(Prefix, StringComparison.Ordinal))
                    continue;

                lines.Add(trimmed);
            }
        }

        if (euler.sqrMagnitude > 0.0000001f)
        {
            lines.Add(string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1:0.######},{2:0.######},{3:0.######}",
                Prefix,
                euler.x,
                euler.y,
                euler.z));
        }

        return string.Join("\n", lines);
    }

    static void Rotate(Mesh mesh, Transform meshTransform, Transform root, Matrix4x4 rot, HashSet<Mesh> seen)
    {
        if (mesh == null || !seen.Add(mesh) || mesh.vertexCount == 0)
            return;

        Matrix4x4 meshToRoot = root.worldToLocalMatrix * meshTransform.localToWorldMatrix;
        Matrix4x4 xform = Mathf.Abs(meshToRoot.determinant) < 0.0000001f
            ? rot
            : meshToRoot.inverse * rot * meshToRoot;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = xform.MultiplyPoint3x4(vertices[i]);
        mesh.vertices = vertices;

        if (Mathf.Abs(xform.determinant) > 0.0000001f)
        {
            Matrix4x4 normalMatrix = xform.inverse.transpose;
            Vector3[] normals = mesh.normals;
            if (normals.Length == vertices.Length)
            {
                for (int i = 0; i < normals.Length; i++)
                    normals[i] = Normalize(normalMatrix.MultiplyVector(normals[i]), normals[i]);
                mesh.normals = normals;
            }

            Vector4[] tangents = mesh.tangents;
            if (tangents.Length == vertices.Length)
            {
                for (int i = 0; i < tangents.Length; i++)
                {
                    Vector3 tangent = Normalize(xform.MultiplyVector(tangents[i]), tangents[i]);
                    tangents[i] = new Vector4(tangent.x, tangent.y, tangent.z, tangents[i].w);
                }

                mesh.tangents = tangents;
            }
        }

        RotateBlendShapes(mesh, xform);
        mesh.RecalculateBounds();
    }

    static void RotateBlendShapes(Mesh mesh, Matrix4x4 xform)
    {
        int shapeCount = mesh.blendShapeCount;
        if (shapeCount == 0)
            return;

        int vertexCount = mesh.vertexCount;
        var frames = new List<BlendFrame>();
        Matrix4x4 normalMatrix = Mathf.Abs(xform.determinant) > 0.0000001f ? xform.inverse.transpose : xform;

        for (int shape = 0; shape < shapeCount; shape++)
        {
            string name = mesh.GetBlendShapeName(shape);
            int frameCount = mesh.GetBlendShapeFrameCount(shape);
            for (int frame = 0; frame < frameCount; frame++)
            {
                float weight = mesh.GetBlendShapeFrameWeight(shape, frame);
                var deltaVertices = new Vector3[vertexCount];
                var deltaNormals = new Vector3[vertexCount];
                var deltaTangents = new Vector3[vertexCount];
                mesh.GetBlendShapeFrameVertices(shape, frame, deltaVertices, deltaNormals, deltaTangents);

                for (int i = 0; i < vertexCount; i++)
                {
                    deltaVertices[i] = xform.MultiplyVector(deltaVertices[i]);
                    deltaNormals[i] = normalMatrix.MultiplyVector(deltaNormals[i]);
                    deltaTangents[i] = xform.MultiplyVector(deltaTangents[i]);
                }

                frames.Add(new BlendFrame(name, weight, deltaVertices, deltaNormals, deltaTangents));
            }
        }

        mesh.ClearBlendShapes();
        foreach (BlendFrame frame in frames)
            mesh.AddBlendShapeFrame(frame.Name, frame.Weight, frame.DeltaVertices, frame.DeltaNormals, frame.DeltaTangents);
    }

    static Vector3 Normalize(Vector3 value, Vector3 fallback)
    {
        float magnitude = value.magnitude;
        return magnitude > 0.0000001f ? value / magnitude : fallback;
    }

    readonly struct BlendFrame
    {
        public BlendFrame(string name, float weight, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents)
        {
            Name = name;
            Weight = weight;
            DeltaVertices = deltaVertices;
            DeltaNormals = deltaNormals;
            DeltaTangents = deltaTangents;
        }

        public string Name { get; }
        public float Weight { get; }
        public Vector3[] DeltaVertices { get; }
        public Vector3[] DeltaNormals { get; }
        public Vector3[] DeltaTangents { get; }
    }
}

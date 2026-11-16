#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>One-click setup for the existing Arena scene; safe to re-run.</summary>
public static class PrototypeSceneBuilder
{
    private const string PrefabFolder = "Assets/_Project/Prefabs";
    private const string MaterialFolder = "Assets/_Project/Materials";

    [MenuItem("Tools/Bullet Arena/Prepare Combat Prototype")]
    public static void Prepare()
    {
        GameObject player = GameObject.Find("Player");
        Camera camera = Camera.main;
        if (player == null || camera == null)
        {
            EditorUtility.DisplayDialog("Bullet Arena",
                "Once Arena sahnesini acin. Player ve Main Camera nesneleri gerekli.", "Tamam");
            return;
        }

        EnsureFolder("Assets/_Project", "Prefabs");
        EnsureFolder("Assets/_Project", "Materials");

        BulletProjectile projectile = CreateProjectilePrefab();
        EnemyAgent enemy = CreateEnemyPrefab();

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health == null) health = Undo.AddComponent<PlayerHealth>(player);
        PlayerShooter shooter = player.GetComponent<PlayerShooter>();
        if (shooter == null) shooter = Undo.AddComponent<PlayerShooter>(player);
        shooter.Configure(projectile);
        EditorUtility.SetDirty(shooter);

        WaveManager manager = Object.FindFirstObjectByType<WaveManager>();
        if (manager == null)
        {
            GameObject obj = new GameObject("WaveManager");
            Undo.RegisterCreatedObjectUndo(obj, "Create WaveManager");
            manager = obj.AddComponent<WaveManager>();
        }
        manager.Configure(player.transform, enemy, health);
        EditorUtility.SetDirty(manager);

        camera.orthographic = true;
        camera.orthographicSize = 12f;
        camera.transform.position = player.transform.position + new Vector3(-12f, 18f, -12f);
        camera.transform.LookAt(player.transform.position);
        EditorUtility.SetDirty(camera);

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        EditorUtility.DisplayDialog("Bullet Arena",
            "Prototip hazir! Play tusuna basin. WASD: hareket | Sol tik: ates.", "Tamam");
    }

    private static BulletProjectile CreateProjectilePrefab()
    {
        string path = PrefabFolder + "/Projectile.prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing.GetComponent<BulletProjectile>();

        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.name = "Projectile";
        obj.transform.localScale = Vector3.one * 0.24f;
        obj.GetComponent<SphereCollider>().isTrigger = true;
        Rigidbody body = obj.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.isKinematic = true;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        obj.AddComponent<BulletProjectile>();
        obj.GetComponent<Renderer>().sharedMaterial =
            CreateMaterial(MaterialFolder + "/Projectile.mat", Color.cyan);
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(obj, path);
        Object.DestroyImmediate(obj);
        return prefab.GetComponent<BulletProjectile>();
    }

    private static EnemyAgent CreateEnemyPrefab()
    {
        string path = PrefabFolder + "/EnemyChaser.prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing.GetComponent<EnemyAgent>();

        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        obj.name = "EnemyChaser";
        obj.transform.localScale = new Vector3(0.8f, 1f, 0.8f);
        obj.AddComponent<EnemyAgent>();
        obj.GetComponent<Renderer>().sharedMaterial =
            CreateMaterial(MaterialFolder + "/Enemy.mat", new Color(0.85f, 0.16f, 0.2f));
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(obj, path);
        Object.DestroyImmediate(obj);
        return prefab.GetComponent<EnemyAgent>();
    }

    private static Material CreateMaterial(string path, Color color)
    {
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) return existing;
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        var material = new Material(shader);
        material.color = color;
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static void EnsureFolder(string parent, string folder)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + folder))
            AssetDatabase.CreateFolder(parent, folder);
    }
}
#endif

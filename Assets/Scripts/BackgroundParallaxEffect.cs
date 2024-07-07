using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BackgroundsParallaxEffect : MonoBehaviour
{
    [SerializeField] public float bgVerticalHeight; // vertical offset for all backgrounds
    [SerializeField] public string[] bgPaths;
    [SerializeField] public float[] parallaxFactors;
    [SerializeField] public Vector2[] tilingScales;
}

#if UNITY_EDITOR
[CustomEditor(typeof(BackgroundsParallaxEffect))]
public class BackgroundsParallaxEffectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BackgroundsParallaxEffect script = (BackgroundsParallaxEffect)target;
        if (GUILayout.Button("Setup Backgrounds"))
        {
            SetupBackgrounds(script);
        }
    }

    private void SetupBackgrounds(BackgroundsParallaxEffect script)
    {
        if (script.bgPaths.Length != script.parallaxFactors.Length || (script.bgPaths.Length != script.tilingScales.Length && script.tilingScales.Length > 1))
        {
            Debug.LogError("Mismatch in number of paths, factors and scales.");
            return;
        }

        if (script.tilingScales.Length == 1)
        {
            // when no separate scales for each tile
            Vector2 singleScale = script.tilingScales[0];
            script.tilingScales = new Vector2[script.bgPaths.Length];
            for (int i = 0; i < script.bgPaths.Length; i++)
            {
                script.tilingScales[i] = singleScale;
            }
        }

        for (int i = 0; i < script.bgPaths.Length; i++)
        {
            GameObject layer = new GameObject("Layer_" + i);
            layer.transform.parent = script.transform;
            SpriteRenderer sr = layer.AddComponent<SpriteRenderer>();
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(script.bgPaths[i]);
            if (sprite == null)
            {
                Debug.LogError("Sprite not found at path: " + script.bgPaths[i]);
                continue;
            }

            TextureImporter textureImporter = AssetImporter.GetAtPath(script.bgPaths[i]) as TextureImporter;
            if (textureImporter != null)
            {
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                textureImporter.spritePixelsPerUnit = 100;
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.SaveAndReimport();
            }

            sr.sprite = sprite;
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.size = new Vector2(500, sr.size.y);
            sr.sortingOrder = -50 + i;

            layer.transform.localScale = new Vector3(script.tilingScales[i].x, script.tilingScales[i].y, 1);

            layer.transform.position = new Vector3(0, script.bgVerticalHeight, i * 0.1f);

            ParallaxLayer parallaxLayer = layer.AddComponent<ParallaxLayer>();
            parallaxLayer.setParallaxFactor(script.parallaxFactors[i]);
        }

        EditorUtility.SetDirty(script);
    }
}
#endif
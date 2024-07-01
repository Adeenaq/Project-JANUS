using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BackgroundsParallaxEffect : MonoBehaviour
{
    [SerializeField] private float bgVerticalHeight; // vertical offset for all backgrounds
    [SerializeField] private string[] bgPaths;
    [SerializeField] private float[] parallaxFactors;
    [SerializeField] private Vector2[] tilingScales;

    private Transform[] layers;
    private Vector3 previousCameraPosition;

    void Start()
    {
        if (bgPaths.Length != parallaxFactors.Length || (bgPaths.Length != tilingScales.Length && tilingScales.Length > 1))
        {
            Debug.LogError("Mismatch in number of paths, factors and scales.");
            return;
        }

        if (tilingScales.Length == 1)
        {
            // when no separate scales for each tile
            Vector2 singleScale = tilingScales[0];
            tilingScales = new Vector2[bgPaths.Length];
            for (int i = 0; i < bgPaths.Length; i++)
            {
                tilingScales[i] = singleScale;
            }
        }

        layers = new Transform[bgPaths.Length];
        previousCameraPosition = Camera.main.transform.position;

        for (int i = 0; i < bgPaths.Length; i++)
        {
            GameObject layer = new GameObject("Layer_" + i);
            layer.transform.parent = transform;
            SpriteRenderer sr = layer.AddComponent<SpriteRenderer>();
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(bgPaths[i]);
            if (sprite == null)
            {
                Debug.LogError("Sprite not found");
                continue;
            }

            TextureImporter textureImporter = AssetImporter.GetAtPath(bgPaths[i]) as TextureImporter;
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

            layer.transform.localScale = new Vector3(tilingScales[i].x, tilingScales[i].y, 1);

            layers[i] = layer.transform;
            layers[i].position = new Vector3(0, bgVerticalHeight, i * 0.1f);
        }
    }

    void Update()
    {
        Vector3 deltaMovement = Camera.main.transform.position - previousCameraPosition;

        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i] == null) continue;
            float parallaxEffect = parallaxFactors[i];
            layers[i].position += new Vector3(
                deltaMovement.x * parallaxEffect,
                deltaMovement.y * parallaxEffect, 
                0
            );
        }

        previousCameraPosition = Camera.main.transform.position;
    }
}

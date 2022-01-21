using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(TilemapToPng))]
public class TilemapToPngEditor : Editor {
    string name = "";
    public override void OnInspectorGUI() {
        TilemapToPng tilemapToPng = (TilemapToPng)target;
        //DrawDefaultInspector();
        if (tilemapToPng.result == null) {
            if (GUILayout.Button("Create PNG")) {
                tilemapToPng.CreatePng();
            }
        } else {
            GUILayout.Label("File Name");
            name = GUILayout.TextField(name);
            if (name.Length > 0) {
                if (GUILayout.Button("Export File")) {
                    tilemapToPng.ExportPng(name);
                }
            }
        }
    }
}
#endif
public class TilemapToPng : MonoBehaviour {
    Tilemap tileMap;
    int minX = 0;
    int maxX = 0;
    int minY = 0;
    int maxY = 0;
    [SerializeField] public Texture2D result;
    public void CreatePng() {
        tileMap = GetComponent<Tilemap>();
        Sprite randomSprite = null;
        for (int x = 0; x < tileMap.size.x; x++) //We find the smallest and largest point
        {
            for (int y = 0; y < tileMap.size.y; y++) {
                Vector3Int pos = new Vector3Int(-x, -y, 0);
                if (tileMap.GetSprite(pos) == null) {
                    print("there is no sprite in this position.");
                } else {
                    randomSprite = tileMap.GetSprite(pos); //we select any sprite to later know the dimensions of the sprites
                    if (minX > pos.x) {
                        minX = pos.x;
                    }
                    if (minY > pos.y) {
                        minY = pos.y;
                    }
                }
                pos = new Vector3Int(x, y, 0);
                if (tileMap.GetSprite(pos) == null) {
                    print("there is no sprite in this position");
                } else {
                    if (maxX < pos.x) {
                        maxX = pos.x;
                    }
                    if (maxY < pos.y) {
                        maxY = pos.y;
                    }
                }
            }
        }
        //We find the size of the sprite in pixels
        float width = randomSprite.rect.width;
        float height = randomSprite.rect.height;
        //we create a texture with the size multiplied by the number of cells
        Texture2D image = new Texture2D((int)width * tileMap.size.x, (int)height * tileMap.size.y);
        //We assign the entire invisible image
        Color[] invisible = new Color[image.width * image.height];
        for (int i = 0; i < invisible.Length; i++) {
            invisible[i] = new Color(0f, 0f, 0f, 0f);
        }
        image.SetPixels(0, 0, image.width, image.height, invisible);
        //Now we assign each block their respective pixels
        for (int x = minX; x <= maxX; x++) {
            for (int y = minY; y <= maxY; y++) {
                if (tileMap.GetSprite(new Vector3Int(x, y, 0)) == null) {
                    print("Bloque vacio");
                } else {
                    //we map the pixels so that the minX = 0 and minY = 0
                    image.SetPixels((x - minX) * (int)width, (y - minY) * (int)height, (int)width, (int)height, GetCurrentSprite(tileMap.GetSprite(new Vector3Int(x, y, 0))).GetPixels());
                }
            }
        }
        image.Apply();
        result = image; //We store the texture of the ready image
    }
    //method to obtain the trimmed sprite as we put it
    Texture2D GetCurrentSprite(Sprite sprite) {
        var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                         (int)sprite.textureRect.y,
                                         (int)sprite.textureRect.width,
                                         (int)sprite.textureRect.height);
        Texture2D texture = new Texture2D((int)sprite.textureRect.width,
                                         (int)sprite.textureRect.height);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    //method that exports as png
    public void ExportPng(string name) {
        byte[] bytes = result.EncodeToPNG();
        var dirPath = Application.dataPath + "/Exported Tilemaps/";
        if (!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + name + ".png", bytes);
        result = null;
    }
}

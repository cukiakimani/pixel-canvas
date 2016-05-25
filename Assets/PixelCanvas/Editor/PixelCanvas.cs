using UnityEditor;
using UnityEngine;
using System.IO;

public class PixelCanvas : EditorWindow
{
    Texture _alphaTexture;
    Texture2D _drawTexture;
    Vector2 _drawPos;

    Color _penColor = Color.black;

    int zoom = 10;
    int brushSize = 1;

    Rect canvasRect;
    Vector2 canvasSize = new Vector2(64, 64);

    // Add menu item named "Pixel Canvas" to the Window menu
    [MenuItem("Window/Pixel Canvas")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(PixelCanvas));
    }

    void OnEnable()
    {
        _alphaTexture = Resources.Load<Texture2D>("alpha_spriteDeformer");
        _drawTexture = new Texture2D(64, 64);
        _drawTexture.filterMode = FilterMode.Point;
        
        Color[] cols = _drawTexture.GetPixels();
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i] = Color.clear;
        }

        _drawTexture.SetPixels(0, 0, 64, 64, cols);
        _drawTexture.Apply();
    }

    void Update()
    {
        Repaint();
    }
    
    void OnGUI()
    {
        DrawCanvas();

        DrawUI();
    
        Event e = Event.current;

        if (canvasRect.Contains(e.mousePosition))
        {
            Vector2 cursorOffset = Vector2.one * zoom * brushSize * 0.5f;
            Vector2 pos = new Vector2(e.mousePosition.x, e.mousePosition.y) - cursorOffset;
            pos = SnapVector(pos, zoom * brushSize);
            Vector2 size = Vector2.one * brushSize * zoom;
            EditorGUI.DrawRect(new Rect(pos, size), _penColor);

            if (e.type == EventType.mouseDown || e.type == EventType.mouseDrag)
            {
                _drawPos = new Vector2(pos.x / zoom, pos.y / zoom);
                int x = (int)_drawPos.x;
                int y = (int)canvasSize.y - 1 - (int)_drawPos.y;
                
                // Color p = _drawTexture.GetPixel(x, y);
                // p += _penColor;

                _drawTexture.SetPixel(x, y, _penColor);
                _drawTexture.Apply();
            }
        }
    }

    void DrawCanvas()
    {
        canvasRect = new Rect(Vector2.zero, canvasSize * zoom);
        GUI.DrawTextureWithTexCoords(canvasRect, _alphaTexture, new Rect(0, 0, canvasSize.x / 2, canvasSize.y / 2));
        GUI.DrawTexture(canvasRect, _drawTexture, ScaleMode.ScaleToFit);
    }

    void DrawUI()
    {
        var colorChooserRect = new Rect(0, canvasSize.y * zoom, 80, 25);

        _penColor = EditorGUI.ColorField(colorChooserRect, "", _penColor);

        var saveButtonRect = colorChooserRect;
        saveButtonRect.y += 25;
        if (GUI.Button(saveButtonRect, "Save"))
        {
            byte[] bytes = _drawTexture.EncodeToPNG();
            string path = EditorUtility.SaveFilePanel("Save as PNG", Application.dataPath, "PixelCanvas_IMG.png", "");
            File.WriteAllBytes(path, bytes);
        }
    }

    public Vector3 SnapVector(Vector3 snapVector, float pixelSize)
    {
        var x1 = Mathf.Floor(snapVector.x / pixelSize) * pixelSize;
        var x2 = Mathf.Ceil(snapVector.x / pixelSize) * pixelSize;

        var y1 = Mathf.Ceil(snapVector.y / pixelSize) * pixelSize;
        var y2 = Mathf.Floor(snapVector.y / pixelSize) * pixelSize;

        var x = Mathf.Abs(snapVector.x - x1) < Mathf.Abs(snapVector.x - x2) ? x1 : x2;
        var y = Mathf.Abs(snapVector.y - y1) < Mathf.Abs(snapVector.y - y2) ? y1 : y2;

        return new Vector3(x, y, snapVector.z);
    }
}

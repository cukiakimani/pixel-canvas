using UnityEditor;
using UnityEngine;
using System.IO;

public class PixelCanvas : EditorWindow
{
    Texture _alphaTexture;
	Texture2D _drawTexture;
	Vector2 drawPos;

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
    	// draw canvas
    	int zoom = 10;
    	int brushSize = 1;
    	Vector2 canvasSize = new Vector2(64, 64);
        Rect canvasRect = new Rect(Vector2.zero, canvasSize * zoom);
        GUI.DrawTextureWithTexCoords(canvasRect, _alphaTexture, new Rect(0, 0, canvasSize.x / 2, canvasSize.y / 2));
    	GUI.DrawTexture(canvasRect, _drawTexture, ScaleMode.ScaleToFit);

    	if (GUI.Button(new Rect(0, canvasSize.y * zoom, 80, 25), "Save"))
    	{
			byte[] bytes = _drawTexture.EncodeToPNG();
			File.WriteAllBytes(Application.dataPath + "/../Assets/PixelCanvas/SavedScreen.png", bytes);
    	}
    
        Event e = Event.current;

        if (canvasRect.Contains(e.mousePosition))
        {
        	Vector2 cursorOffset = Vector2.one * zoom * brushSize * 0.5f;
        	Vector2 pos = new Vector2(e.mousePosition.x, e.mousePosition.y) - cursorOffset;
        	pos = SnapVector(pos, zoom * brushSize);
        	Vector2 size = Vector2.one * brushSize * zoom;
        	EditorGUI.DrawRect(new Rect(pos, size), Color.black);

        	if (e.type == EventType.mouseDown || e.type == EventType.mouseDrag)
        	{
        		drawPos = new Vector2(pos.x / zoom, pos.y / zoom);
        		_drawTexture.SetPixel((int)drawPos.x, 63 - (int)drawPos.y, Color.black);
        		_drawTexture.Apply();
        	}
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

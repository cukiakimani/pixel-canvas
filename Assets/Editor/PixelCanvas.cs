//C# Example
using UnityEditor;
using UnityEngine;

public class PixelCanvas : EditorWindow
{
    Texture _alphaTexture;
	
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
    }

    void Update()
    {
    	Repaint();
    }
    
    void OnGUI()
    {
    	// draw canvas
    	float zoom = 10f;
    	int brushSize = 1;
    	Vector2 canvasSize = new Vector2(64, 64);
        Rect canvasRect = new Rect(Vector2.zero, canvasSize * zoom);
        GUI.DrawTextureWithTexCoords(canvasRect, _alphaTexture, new Rect(0, 0, canvasSize.x / 2, canvasSize.y / 2));
    
        Event e = Event.current;

        if (canvasRect.Contains(e.mousePosition))
        {
        	Vector2 cursorOffset = Vector2.one * zoom * brushSize * 0.5f;
        	Vector2 pos = new Vector2(e.mousePosition.x, e.mousePosition.y) - cursorOffset;
        	pos = SnapVector(pos, zoom * brushSize);
        	Vector2 size = Vector2.one * brushSize * zoom;
        	EditorGUI.DrawRect(new Rect(pos, size), Color.black);
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

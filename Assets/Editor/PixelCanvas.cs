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
    	int brushSize = 2;
    	Vector2 canvasSize = new Vector2(64, 64);
        Rect canvasRect = new Rect(Vector2.zero, canvasSize * zoom);
        GUI.DrawTextureWithTexCoords(canvasRect, _alphaTexture, new Rect(0, 0, canvasSize.x / 2, canvasSize.y / 2));
    
        Event e = Event.current;

        if (canvasRect.Contains(e.mousePosition))
        {
        	Vector2 pos = new Vector2(e.mousePosition.x, e.mousePosition.y);
        	Vector2 size = Vector2.one * brushSize * zoom;
        	EditorGUI.DrawRect(new Rect(pos, size), Color.black);
        }
    }


}

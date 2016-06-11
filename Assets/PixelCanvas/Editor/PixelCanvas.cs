using UnityEditor;
using UnityEngine;
using System.IO;

public class PixelCanvas : EditorWindow
{
    public int MenuOption;
    public GUISkin Skin;

    public static Texture2D DrawTexture;
    public Texture2D AlphaTexture;

    public Vector2 DrawPosition;

    public Color BrushColor = Color.black;

    public float CanvasZoom = 10;
    public int BrushSize = 1;

    public Rect CanvasRect;

    public Vector2 CanvasSize;

    // 0 - Eraser
    // 1 - Pen
    public bool[] ToolToggle = new bool[2];

    [MenuItem("Pixel Canvas/New Canvas")]
    public static void ShowWindow()
    {
        var canvas = (PixelCanvas)EditorWindow.GetWindow(typeof(PixelCanvas), false, "Pixel Canvas");
        canvas.MenuOption = 1;
        canvas.Show();
    }

    [MenuItem("Pixel Canvas/Edit Sprite")]
    public static void EditSpriteShowWindow()
    {
        var canvas = (PixelCanvas)EditorWindow.GetWindow(typeof(PixelCanvas), false, "Pixel Canvas");
        canvas.MenuOption = 0;
        canvas.Show();
        canvas.OpenSpriteCanvas();
    }

    [MenuItem("Pixel Canvas/Save Sprite")]
    public static void SaveSprite()
    {
        byte[] bytes = DrawTexture.EncodeToPNG();
        string path = EditorUtility.SaveFilePanel("Save as PNG", Application.dataPath, "PixelCanvas_IMG.png", "");
        File.WriteAllBytes(path, bytes);
    }

    [MenuItem("Pixel Canvas/Save Sprite", true)]
    public static bool ValidateDrawTexture()
    {
        return DrawTexture != null;
    }

    void OnEnable()
    {
        // MenuOption = 0;
        CanvasSize = Vector2.one * 32;

        CreateBlankCanvas();
        MenuOption = 3;
        Skin = Resources.Load<GUISkin>("PixelCanvasSkin");
    }

    void Update()
    {
        Repaint();
    }

    void OnGUI()
    {
        switch (MenuOption)
        {
            case 0:
                NewWindowGUI();
                break;

            case 1:
                ChooseSizeCanvasGUI();
                break;

            case 2:
                NewWindowGUI();
                OpenSpriteCanvas();
                break;

            case 3:
                PaintCanvas();
                break;
        }
    }

    void NewWindowGUI()
    {
        var rect = new Rect(0, 0, 100, 20);
        EditorGUI.DrawRect(rect, Color.gray);

        GUI.Label(rect, "Pixel Canvas!");

        rect.y += rect.height;
        if (GUI.Button(rect, "Blank Canvas"))
        {
            MenuOption = 1;
        }

        rect.y += rect.height;
        if (GUI.Button(rect, "Edit Sprite"))
        {
            MenuOption = 2;
        }
    }

    void ChooseSizeCanvasGUI()
    {
        Rect r = new Rect(5, 5, 200, 25);
        CanvasSize.x = Mathf.Clamp(EditorGUI.IntField(r, "Width", (int)CanvasSize.x), 0f, Mathf.Infinity);

        r = new Rect(5, 35, 200, 25);
        CanvasSize.y = Mathf.Clamp(EditorGUI.IntField(r, "Height", (int)CanvasSize.y), 0f, Mathf.Infinity);

        r = new Rect(5, 65, 100, 25);
        if (GUI.Button(r, "Create Canvas"))
        {
            CreateBlankCanvas();
            MenuOption = 3;
        }
    }

    void CreateBlankCanvas()
    {
        AlphaTexture = Resources.Load<Texture2D>("alpha_spriteDeformer");
        DrawTexture = new Texture2D((int)CanvasSize.x, (int)CanvasSize.y);
        DrawTexture.filterMode = FilterMode.Point;
        CanvasRect = new Rect(new Vector2(70, 20), CanvasSize * CanvasZoom);

        Color[] cols = DrawTexture.GetPixels();
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i] = Color.clear;
        }

        DrawTexture.SetPixels(0, 0, (int)CanvasSize.x, (int)CanvasSize.y, cols);
        DrawTexture.Apply();
    }
    
    void OpenSpriteCanvas()
    {
        string path = EditorUtility.OpenFilePanel("Select Sprite", Application.dataPath, "png");

        if (path != "")
        {
            AlphaTexture = Resources.Load<Texture2D>("alpha_spriteDeformer");
            var bytes = File.ReadAllBytes(path);
            DrawTexture = new Texture2D(1, 1);
            DrawTexture.LoadImage(bytes);
            CanvasSize = new Vector2(DrawTexture.width, DrawTexture.height);
            DrawTexture.filterMode = FilterMode.Point;
            CanvasRect = new Rect(new Vector2(70, 20), CanvasSize * CanvasZoom);

            MenuOption = 3;
        }
        else
        {
            MenuOption = 0;
        }
    }

    void PaintCanvas()
    {
        DrawCanvas();

        DrawUI();
    
        Event e = Event.current;

        if (e.type == EventType.mouseDrag && e.button == 2)
        {
            CanvasRect.position += e.delta;
        }

        if (e.type == EventType.scrollWheel)
        {
            CanvasZoom = Mathf.Clamp(CanvasZoom + -e.delta.y, 0.5f, 40f);
        }

        if (CanvasRect.Contains(e.mousePosition))
        {
            Vector2 cursorOffset = Vector2.one * CanvasZoom * BrushSize * 0.5f;
            Vector2 pos = new Vector2(e.mousePosition.x, e.mousePosition.y) - cursorOffset;

            pos = SnapVector(pos, CanvasZoom);
            Vector2 size = Vector2.one * BrushSize * CanvasZoom;

            if (ToolToggle[0])
            {
                GUI.Box(new Rect(pos, size), "");
            }
            else
            {
                EditorGUI.DrawRect(new Rect(pos, size), BrushColor);
            }

            if (e.button == 0 && (e.type == EventType.mouseDown || e.type == EventType.mouseDrag))
            {
                pos -= CanvasRect.position;
                DrawPosition = new Vector2(pos.x / CanvasZoom, pos.y / CanvasZoom);
                

                int x = (int)DrawPosition.x;
                int brushPaintLen = BrushSize + x;
                x = x < 0 ? 0 : x;
                int y = (int)CanvasSize.y - 1 - (int)DrawPosition.y;
                

                var cols = DrawTexture.GetPixels();
                for (int i = x; i < brushPaintLen; i++)
                {
                    if (i >= (int)CanvasSize.x)
                        continue;

                    for (int j = y; j > y - BrushSize; j--)
                    {
                        int index = (int)CanvasSize.y * j + i;
                        int arraySize = Mathf.RoundToInt(CanvasSize.x * CanvasSize.y);
                        
                        if (index >= arraySize || index < 0)
                            continue;

                        Color bg = cols[index];
                        Color fg = BrushColor;

                        Color r = new Color();
                        r.a = 1 - (1 - fg.a) * (1 - bg.a);
                        
                        if (r.a < 1.0e-6)
                            continue; // Fully transparent -- R,G,B not important

                        r.r = fg.r * fg.a / r.a + bg.r * bg.a * (1 - fg.a) / r.a;
                        r.g = fg.g * fg.a / r.a + bg.g * bg.a * (1 - fg.a) / r.a;
                        r.b = fg.b * fg.a / r.a + bg.b * bg.a * (1 - fg.a) / r.a;

                        cols[index] = ToolToggle[0] ? Color.clear : r;
                    }
                }
                Undo.RecordObject(DrawTexture, "edit canvas");
                DrawTexture.SetPixels(cols);
                DrawTexture.Apply();
            }
        }
    }

    void DrawCanvas()
    {
        CanvasRect.size = CanvasSize * CanvasZoom;
        GUI.DrawTextureWithTexCoords(CanvasRect, AlphaTexture, new Rect(0, 0, CanvasSize.x / 2, CanvasSize.y / 2));
        GUI.DrawTextureWithTexCoords(CanvasRect, DrawTexture, new Rect(0, 0, 1, 1));
    }

    void DrawUI()
    {
        Color col = Color.black;
        ColorUtility.TryParseHtmlString("#383838FF", out col);

        EditorGUI.DrawRect(new Rect(5, 10, 60, 300), col);

        GUI.skin = Skin;

        var rect = new Rect(10, 20, 50, 50);
        BrushColor = EditorGUI.ColorField(rect, new GUIContent(""), BrushColor, false, true, false, null);

        rect.y += rect.height + 5;
        rect.size = new Vector2(22, 22);

        // Pen 
        ExclusiveGroupToggle(rect, 1, Skin.customStyles[1]);

        // Eraser
        rect.x += 27;
        ExclusiveGroupToggle(rect, 0, Skin.customStyles[2]);

        rect = new Rect(10, rect.y + rect.height + 5, 50, 22);
        BrushSize = Mathf.RoundToInt(GUI.HorizontalSlider(rect, BrushSize, 1f, CanvasSize.x));

        rect.y += rect.height - 8;
        EditorGUI.LabelField(rect, "" + BrushSize, Skin.customStyles[0]);

        // brushSizeRect.x += brushSizeRect.width;
        // GUI.Label(brushSizeRect, BrushSize + "");

        // brushSizeRect.x = 0;
        // brushSizeRect.width = 25;
        // brushSizeRect.y += brushSizeRect.height;
        // IsErasing = GUI.Toggle(brushSizeRect, IsErasing, "");

        // brushSizeRect.width = 80;
        // brushSizeRect.y += brushSizeRect.height;
        // if (GUI.Button(brushSizeRect, "" + CanvasZoom))
        // {
        //     CanvasZoom = 10f;
        // }

        // brushSizeRect.y += brushSizeRect.height;
        // EditorGUI.LabelField(brushSizeRect, MenuOption + "");
    }

    void ExclusiveGroupToggle(Rect r, int index, GUIStyle style)
    {
        bool delta = GUI.Toggle(r, ToolToggle[index], new GUIContent(""), style);

        if (!ToolToggle[index] && delta)
            ToolToggle[index] = delta;

        if (ToolToggle[index])
            ToolToggleExclusivity(index);
    }


    void ToolToggleExclusivity(int trueIndex)
    {
        for (int i = 0; i < ToolToggle.Length; i++)
        {
            if (i != trueIndex)
                ToolToggle[i] = false;
        }
    }

    Vector3 SnapVector(Vector3 snapVector, float pixelSize)
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

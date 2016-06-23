using UnityEditor;
using UnityEngine;
using System.IO;

public class PixelCanvas : EditorWindow
{
    public int MenuOption;

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
    // 2 - Eye Dropper
    public bool[] ToolToggle = new bool[3];
    
    private Texture2D _penIcon;
    private Texture2D _eraserIcon;
    private Texture2D _eyeDropperIcon;
    private Texture2D _eyeDropperCanvasIcon;
    private Texture2D _centerCanvasIcon;
    private Texture2D _penCursor;
    private bool[] _coloredPixels;
    private Vector2 _lastDrawPos;

    string _debugString;

    [MenuItem("Pixel Canvas/New Canvas")]
    public static void ShowWindow()
    {
        var canvas = (PixelCanvas)EditorWindow.GetWindow(typeof(PixelCanvas), false, "Pixel Canvas");
        PixelCanvas.DrawTexture = null;
        canvas.MenuOption = 1;
        canvas.InitializeUI();
        canvas.Show();
        PixelCanvas.DrawTexture = null;
    }

    [MenuItem("Pixel Canvas/Edit Sprite")]
    public static void EditSpriteShowWindow()
    {
        var canvas = (PixelCanvas)EditorWindow.GetWindow(typeof(PixelCanvas), false, "Pixel Canvas");
        PixelCanvas.DrawTexture = null;
        canvas.MenuOption = 0;
        canvas.Show();
        canvas.OpenSpriteCanvas();
        canvas.InitializeUI();
    }

    [MenuItem("Pixel Canvas/Save Sprite")]
    public static void SaveSprite()
    {
        byte[] bytes = DrawTexture.EncodeToPNG();
        string path = EditorUtility.SaveFilePanel("Save as PNG", Application.dataPath, "PixelCanvas_IMG.png", "");

        if (path != "")
            File.WriteAllBytes(path, bytes);
    }

    [MenuItem("Pixel Canvas/Save Sprite", true)]
    public static bool ValidateDrawTexture()
    {
        return DrawTexture != null;
    }

    void OnEnable()
    {
        MenuOption = 1;

        // MenuOption = 3;
        // CanvasSize = new Vector2(32, 32);
        // CreateBlankCanvas();
        // _coloredPixels = new bool[DrawTexture.width * DrawTexture.height];

        ToolToggle = new bool[3];
        ToolToggle[1] = true;

        InitializeUI();
    }

    void Update()
    {
        Repaint();
    }

    void OnGUI()
    {
        switch (MenuOption)
        {
            case 1:
                ChooseSizeCanvasGUI();
                break;

            case 2:
                OpenSpriteCanvas();
                break;

            case 3:
                PaintCanvas();
                break;
        }
    }

    void ChooseSizeCanvasGUI()
    {
        Rect r = new Rect(10, 10, 200, 25);
        CanvasSize.x = Mathf.Clamp(EditorGUI.IntField(r, "Width", (int)CanvasSize.x), 0f, Mathf.Infinity);

        r.y += r.height + 5;
        CanvasSize.y = Mathf.Clamp(EditorGUI.IntField(r, "Height", (int)CanvasSize.y), 0f, Mathf.Infinity);

        r.y += r.height + 5;
        r = new Rect(r.x, r.y, 100, 25);
        if (GUI.Button(r, "Create Canvas"))
        {
            CreateBlankCanvas();
            MenuOption = 3;
        }

        ToolToggle[1] = true;
    }

    void CreateBlankCanvas()
    {
        
        DrawTexture = new Texture2D((int)CanvasSize.x, (int)CanvasSize.y);
        DrawTexture.filterMode = FilterMode.Point;
        CanvasRect = new Rect(new Vector2(70, 10), CanvasSize * CanvasZoom);

        Color[] cols = DrawTexture.GetPixels();
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i] = Color.clear;
        }

        DrawTexture.SetPixels(0, 0, (int)CanvasSize.x, (int)CanvasSize.y, cols);
        DrawTexture.Apply();

        _coloredPixels = new bool[DrawTexture.width * DrawTexture.height];
    }
    
    void OpenSpriteCanvas()
    {
        string path = EditorUtility.OpenFilePanel("Select Sprite", Application.dataPath, "png");

        if (path != "")
        {
            var bytes = File.ReadAllBytes(path);
            DrawTexture = new Texture2D(1, 1);
            DrawTexture.LoadImage(bytes);
            CanvasSize = new Vector2(DrawTexture.width, DrawTexture.height);
            DrawTexture.filterMode = FilterMode.Point;
            CanvasRect = new Rect(new Vector2(70, 10), CanvasSize * CanvasZoom);

            _coloredPixels = new bool[DrawTexture.width * DrawTexture.height];

            MenuOption = 3;
        }
        else
        {
            MenuOption = 1;
        }

        ToolToggle[1] = true;
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

        if (e.command && e.type == EventType.scrollWheel && !ToolToggle[2])
        {

            var d = e.delta.y;
            d = Mathf.Abs(d) > 0f ? -Mathf.Sign(d) * 1f : 0f;
            BrushSize = (int)Mathf.Clamp(BrushSize + d, 1f, CanvasSize.x);
        }
        else if (e.type == EventType.scrollWheel)
        {
            CanvasZoom = Mathf.Clamp(CanvasZoom + -e.delta.y, 0.5f, 40f);
        }

        Cursor.visible = true;

        if (CanvasRect.Contains(e.mousePosition))
        {
            Cursor.visible = false;

            Vector2 cursorOffset = Vector2.one * CanvasZoom * BrushSize * 0.5f;
            Vector2 pos = new Vector2(e.mousePosition.x, e.mousePosition.y) - cursorOffset;

            var cSnap = SnapVector(CanvasRect.position, CanvasZoom);
            var delta = CanvasRect.position - cSnap;

            pos = SnapVector(pos, CanvasZoom) + delta;
            Vector2 size = Vector2.one * BrushSize * CanvasZoom;

            if (ToolToggle[0]) // Eraser
            {
                var col = (e.type == EventType.mouseDown || e.type == EventType.mouseDrag) ? Color.black : Color.white; 
                PaintingCursor(pos, Color.white);
                
                
                EditCanvas(e, pos);
            }
            else if (ToolToggle[1]) // Pen
            {
                EditCanvas(e, pos);
                EditorGUI.DrawRect(new Rect(pos, size), BrushColor);
                PaintingCursor(pos, BrushColor);
            }
            else if (ToolToggle[2]) // Eyedropper
            {
                cursorOffset = Vector2.one * CanvasZoom * 0.5f;
                pos = new Vector2(e.mousePosition.x, e.mousePosition.y) - cursorOffset;
                cSnap = SnapVector(CanvasRect.position, CanvasZoom);
                delta = CanvasRect.position - cSnap;
                pos = SnapVector(pos, CanvasZoom) + delta;
                
                var s = CanvasZoom * 0.75f;
                var p = pos + Vector2.right * CanvasZoom * 0.25f;

                GUI.DrawTextureWithTexCoords(new Rect(p.x, p.y, s, s), _eyeDropperCanvasIcon, new Rect(0, 0, 1, 1));
                
                if (e.button == 0 && e.type == EventType.mouseDown)
                {
                    var cols = DrawTexture.GetPixels();
                    pos -= CanvasRect.position;
                    pos = new Vector2(pos.x / CanvasZoom, pos.y / CanvasZoom);

                    int x = (int)pos.x;
                    int y = (int)CanvasSize.y - 1 - (int)pos.y;
        
                    int index = (int)CanvasSize.x * y + x;
                    int arraySize = Mathf.RoundToInt(CanvasSize.x * CanvasSize.y);

                    if (index <= arraySize && index > 0)
                    {
                        BrushColor = cols[index];
                    }

                }
            }
        }

        if (e.button == 0 && e.type == EventType.mouseUp)
        {
            _lastDrawPos = Vector2.one * -1;
            for (int i = 0; i < _coloredPixels.Length; i++)
            {
                _coloredPixels[i] = false;
            }
        }
    }

    void EditCanvas(Event e, Vector2 pos)
    {
        if (e.button == 0 && (e.type == EventType.mouseDown || e.type == EventType.mouseDrag))
        {
            pos -= CanvasRect.position;
            DrawPosition = new Vector2(pos.x / CanvasZoom, pos.y / CanvasZoom);

            PaintPixelBrushSize(DrawPosition);
            Vector2 dir = (DrawPosition - _lastDrawPos);

            if (_lastDrawPos.x > 0 && _lastDrawPos.y > 0)
            {
                for (int d = 1; d < dir.magnitude; d++)
                {
                    var p = _lastDrawPos + dir.normalized * d;
                    PaintPixelBrushSize(p);
                }
            }

            _lastDrawPos = DrawPosition;
        }
    }

    void PaintPixelBrushSize(Vector2 pos)
    {
        int x = (int)pos.x;
        int brushPaintLen = BrushSize + x;
        x = x < 0 ? 0 : x;
        int y = (int)CanvasSize.y - 1 - (int)pos.y;

        var cols = DrawTexture.GetPixels();
        for (int i = x; i < brushPaintLen; i++)
        {
            if (i >= (int)CanvasSize.x)
                continue;

            for (int j = y; j > y - BrushSize; j--)
            {
                PaintPixel(cols, new Vector2(i, j));
            }
        }

        Undo.RecordObject(DrawTexture, "edit canvas");
        DrawTexture.SetPixels(cols);
        DrawTexture.Apply();
    }

    void PaintPixel(Color[] pixels, Vector2 paintPos)
    {
        int index = (int)CanvasSize.x * (int)paintPos.y + (int)paintPos.x;
        int arraySize = Mathf.RoundToInt(CanvasSize.x * CanvasSize.y);

        if (index >= arraySize || index < 0)
            return;

        Color bg = pixels[index];
        Color fg = BrushColor;

        Color r = BlendColors(bg, fg);

        if (!_coloredPixels[index])
        {
            pixels[index] = ToolToggle[0] ? Color.clear : r;
            _coloredPixels[index] = true;
        }
    }

    Color BlendColors(Color bg, Color fg)
    {
        Color r = new Color();
        r.a = 1 - (1 - fg.a) * (1 - bg.a);
                        
        if (r.a < 1.0e-6)
            return bg; // Fully transparent -- R,G,B not important

        r.r = fg.r * fg.a / r.a + bg.r * bg.a * (1 - fg.a) / r.a;
        r.g = fg.g * fg.a / r.a + bg.g * bg.a * (1 - fg.a) / r.a;
        r.b = fg.b * fg.a / r.a + bg.b * bg.a * (1 - fg.a) / r.a;

        return r;
    }

    void DrawCanvas()
    {
        CanvasRect.size = CanvasSize * CanvasZoom;
        GUI.DrawTextureWithTexCoords(CanvasRect, AlphaTexture, new Rect(0, 0, CanvasSize.x / 2, CanvasSize.y / 2));
        GUI.DrawTextureWithTexCoords(CanvasRect, DrawTexture, new Rect(0, 0, 1, 1));
        CanvasBorder();
    }

    void DrawUI()
    {
    
        var rect = new Rect(10, 10, 50, 50);

        BrushColor = EditorGUI.ColorField(rect, new GUIContent(""), BrushColor, false, true, false, null);

        rect.y += rect.height + 5;
        rect.size = new Vector2(22, 22);

        // Pen
        ExclusiveGroupToggle(rect, 1, _penIcon);

        // Eraser
        rect.x += 27;
        ExclusiveGroupToggle(rect, 0, _eraserIcon);

        // Eye Dropper
        rect.x -= 27;
        rect.y += rect.height + 5;
        ExclusiveGroupToggle(rect, 2, _eyeDropperIcon);

        rect.x += 27;
        if (GUI.Button(rect, new GUIContent(_centerCanvasIcon)))
        {
            CanvasRect = new Rect(new Vector2(70, 10), CanvasSize * CanvasZoom);
            CanvasZoom = 10f;
        }

        rect = new Rect(10, rect.y + rect.height + 5, 50, 22);
        BrushSize = Mathf.RoundToInt(GUI.HorizontalSlider(rect, BrushSize, 1f, CanvasSize.x));

        rect.y += rect.height - 8;
        var style = GUI.skin.label;
        style.alignment = TextAnchor.UpperRight;
        EditorGUI.LabelField(rect, "" + BrushSize, style);

        // rect.y += rect.height + 10;
        // rect.width = 300;
        // rect.height = 500;

        // EditorGUI.LabelField(rect, _debugString);
    }

    void InitializeUI()
    {
        AlphaTexture = Resources.Load<Texture2D>("transparency_background");
        _penIcon = Resources.Load<Texture2D>("Icons/PenIcon");
        _eraserIcon = Resources.Load<Texture2D>("Icons/EraserIcon");
        _eyeDropperIcon = Resources.Load<Texture2D>("Icons/EyeDropperIcon");
        _eyeDropperCanvasIcon = Resources.Load<Texture2D>("Icons/EyeDropperCanvasIcon");
        _centerCanvasIcon = Resources.Load<Texture2D>("Icons/CenterCanvasIcon");
        _penCursor = Resources.Load<Texture2D>("cursor");
        _lastDrawPos = new Vector2(-1, -1);
    }

    void PaintingCursor(Vector2 pos, Color color, bool withBrushSize = true)
    {
        var b = withBrushSize ? BrushSize : 1f;
        var size = b * CanvasZoom;
        var outlineSize = (b * CanvasZoom) * 0.03f;
        outlineSize = Mathf.Clamp(outlineSize, 1f, Mathf.Infinity);

        EditorGUI.DrawRect(new Rect(pos.x, pos.y, size, outlineSize), color);
        EditorGUI.DrawRect(new Rect(pos.x, pos.y, outlineSize, size), color);
        EditorGUI.DrawRect(new Rect(pos.x + size - outlineSize, pos.y, outlineSize, size), color);
        EditorGUI.DrawRect(new Rect(pos.x, pos.y + size - outlineSize, size, outlineSize), color);

        var s = CanvasZoom;
        pos += Vector2.one * 0.5f * CanvasZoom * b;
        pos -= Vector2.one * 0.5f * s;
        GUI.DrawTextureWithTexCoords(new Rect(pos.x, pos.y, s, s), _penCursor, new Rect(0, 0, 1, 1));
    }

    void CanvasBorder()
    {
        var pos = CanvasRect.position;
        var size = CanvasSize * CanvasZoom;
        var color = Color.black;
        var outlineSize = size.magnitude * 0.002f;
        outlineSize = Mathf.Clamp(outlineSize, 1f, Mathf.Infinity);

        EditorGUI.DrawRect(new Rect(pos.x, pos.y - outlineSize, size.x + outlineSize, outlineSize), color);
        EditorGUI.DrawRect(new Rect(pos.x - outlineSize, pos.y - outlineSize, outlineSize, size.y + outlineSize * 2), color);
        EditorGUI.DrawRect(new Rect(pos.x + size.x, pos.y, outlineSize, size.y + outlineSize), color);
        EditorGUI.DrawRect(new Rect(pos.x, pos.y + size.y, size.x + outlineSize, outlineSize), color);
    }

    void ExclusiveGroupToggle(Rect r, int index, Texture2D icon)
    {
        bool delta = GUI.Toggle(r, ToolToggle[index], new GUIContent(icon), GUI.skin.button);

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

    Vector2 SnapVector(Vector3 snapVector, float pixelSize)
    {
        var x1 = Mathf.FloorToInt(snapVector.x / pixelSize) * pixelSize;
        var x2 = Mathf.CeilToInt(snapVector.x / pixelSize) * pixelSize;

        var y1 = Mathf.FloorToInt(snapVector.y / pixelSize) * pixelSize;
        var y2 = Mathf.CeilToInt(snapVector.y / pixelSize) * pixelSize;

        var x = Mathf.Abs(snapVector.x - x1) < Mathf.Abs(snapVector.x - x2) ? x1 : x2;
        var y = Mathf.Abs(snapVector.y - y1) < Mathf.Abs(snapVector.y - y2) ? y1 : y2;

        return new Vector2(x, y);
    }
}

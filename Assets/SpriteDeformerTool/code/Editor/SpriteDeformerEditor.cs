using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Medvedya.GeometryMath;
using UnityEditor.Sprites;
using UnityEditorInternal;
using System.Reflection;
namespace Medvedya.SpriteDeformerTools
{
    public enum CurrentTool { NONE, SELECT_RECT, MOVE_POINTS, MOVE_RIGHT, MOVE_UP, MOVE_PIVOT, SCALE_HORIZONTAL, SCALE_VERTICAL, ORIANT, ROTATE };
    public enum ColorSet { TRIANGLE, EDGE, SELECTED_RECTANGLE, POINT, DEFORMER_TOOL, BOUNDS_LINE, BOUNDS_DOT, LINE_TO_PAINT };

    public class SpriteDeformerEditor : Editor
    {

        DeformerTool deformerTool;

        public Dictionary<ColorSet, HandleColor> colorSets = new Dictionary<ColorSet, HandleColor>();
        private string[] mainToolBarNames = new string[] { "Main", "Points", "Bounds", "Paint" };
        public SpriteDeformer spriteDeformer;
        HandleColorSetting handleColorSetting = new HandleColorSetting();

       
        protected virtual void OnEnable()
        {
            spriteDeformer = (SpriteDeformer)target;
            selectedPoints = spriteDeformer.editorProps.selectedPoints;
            deformerTool = new DeformerTool(this);
            colorSets[ColorSet.EDGE] = new HandleColor(handleColorSetting, Color.green);
            colorSets[ColorSet.POINT] = new HandleColor(handleColorSetting, Color.green, Color.cyan, Color.yellow);
            colorSets[ColorSet.TRIANGLE] = new HandleColor(handleColorSetting, new Color(0.5f, 0.5f, 0.5f, 0.5f));
            colorSets[ColorSet.DEFORMER_TOOL] = new HandleColor(handleColorSetting, Color.red, Color.yellow);
            colorSets[ColorSet.BOUNDS_DOT] = new HandleColor(handleColorSetting, Color.green);
            colorSets[ColorSet.BOUNDS_LINE] = new HandleColor(handleColorSetting, Color.green);
            colorSets[ColorSet.SELECTED_RECTANGLE] = new HandleColor(handleColorSetting, new Color(0, 0, 1, 0.5f), Color.black);
            colorSets[ColorSet.LINE_TO_PAINT] = new HandleColor(handleColorSetting, new Color(1,1, 1,1 ), Color.black);
            spriteDeformer.CreateNewMesh(true);  
            upDateMeshDate(true);

        }
        void OnDisable()
        {
            if (Tools.current == Tool.None)
            {
                Tools.current = userTool;
            }
        }
        public List<SpritePoint> selectedPoints;
        public CurrentTool currentTool = CurrentTool.NONE;
        private Vector2 startSelectedRect;//in gui
        public virtual void doItAfterMovePoints(SpritePoint[] points)
        {

        }
   
        protected virtual void OnSceneGUI()
        {
            if (Event.current.type == EventType.mouseUp)
            {
                if (spriteDeformer.editorProps.autoFreezeScale)
                {
                    spriteDeformer.FreezeScale();
                    recalculeteBounds();
                }
            }
            if (Tools.current != Tool.None)
            {
                userTool = Tools.current;
            }
            if (spriteDeformer.editorProps.mainToolBar == MainToolBarInspector.STANDART && spriteDeformer.enabled)
            {
                if (Tools.current == Tool.None)
                {
                    Tools.current = userTool;
                }
            }
            if (spriteDeformer.editorProps.mainToolBar == MainToolBarInspector.EDIT_BOUNDS)
            {
                sceneBounds();
            }
            if (spriteDeformer.editorProps.mainToolBar == MainToolBarInspector.EDIT_VERTICS)
            {

                sceneEditor();
            }
            if (spriteDeformer.editorProps.mainToolBar == MainToolBarInspector.EDIT_COLOR)
            {
                sceneColor();
            }

            if (spriteDeformer.editorProps.mainToolBar == MainToolBarInspector.EDIT_BOUNDS
                ||
                spriteDeformer.editorProps.mainToolBar == MainToolBarInspector.EDIT_VERTICS)
                if (Event.current.type == EventType.mouseMove || Event.current.type == EventType.mouseUp || Event.current.type == EventType.mouseDrag || Event.current.type == EventType.mouseDown)
                {
                    HandleUtility.Repaint();
                }


            if (currentTool != CurrentTool.NONE && currentTool != CurrentTool.SELECT_RECT)
            {
                //spriteDeformer.dirty_points = true;
                if (Event.current.rawType == EventType.MouseDrag)
                    upDateMeshDate();
                //Event.current.Use();
            }

        }
        public Tool userTool = Tool.None;
        
        public void InspectorSpriteDeformer()
        {
            if (GUILayout.Button("Edit topology"))
            {
                UVEditorWindow uvEditorWindow = (UVEditorWindow)EditorWindow.GetWindow(typeof(UVEditorWindow), false, spriteDeformer.gameObject.name, false);
                uvEditorWindow.autoRepaintOnSceneChange = true;
                uvEditorWindow.spriteDeformer = spriteDeformer;
                uvEditorWindow.Repaint();
            }
            

        }
        // Get the sorting layer names
        public string[] GetSortingLayerNames()
        {
            System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            return (string[])sortingLayersProperty.GetValue(null, new object[0]);
        }

        // Get the unique sorting layer IDs -- tossed this in for good measure
        public int[] GetSortingLayerUniqueIDs()
        {
            System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
            return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
        }
        public void InspectorEditToolBar()
        {
            spriteDeformer.editorProps.inversGizmos = GUILayout.Toggle(spriteDeformer.editorProps.inversGizmos, "Inverse gizmos");
            handleColorSetting.inverse = spriteDeformer.editorProps.inversGizmos;

            MainToolBarInspector newMainTolbar = 
                (MainToolBarInspector)GUILayout.Toolbar((int)spriteDeformer.editorProps.mainToolBar, mainToolBarNames);

            if (newMainTolbar != spriteDeformer.editorProps.mainToolBar)
            {
                spriteDeformer.editorProps.mainToolBar = newMainTolbar;
                //HandleUtility.Repaint();
                //this.Repaint();
                SceneView.RepaintAll();
            }
            spriteDeformer.editorProps.mainToolBar = newMainTolbar;
          
            switch (spriteDeformer.editorProps.mainToolBar)
            {
                case MainToolBarInspector.EDIT_VERTICS:
                    inspectorWhenSelectPoints();
                    break;
                case MainToolBarInspector.STANDART:
                    inspectorMain();
                    break;
                case MainToolBarInspector.EDIT_BOUNDS:
                    inspectorBounds();
                    break;
                case MainToolBarInspector.EDIT_COLOR:
                    InspectorColor();
                    break;
                default: break;
            }
        }

        private void InspectorColor()
        {
            spriteDeformer.editorProps.paintColor = EditorGUILayout.ColorField("Color:", spriteDeformer.editorProps.paintColor);

        }
        private void sceneColor()
        {
            Undo.RecordObjects(new Object[] { spriteDeformer }, "Color");
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            if (Tools.current != Tool.View)
                Tools.current = Tool.None;
            SpritePoint clossetPoint = null;
            float minD = float.MaxValue;
            foreach (var p in spriteDeformer.points)
            {
                Vector2 ps = HandleUtility.WorldToGUIPoint(spriteDeformer.SpritePositionToGlobal(p.offset2d + p.spritePosition));
                float d = Vector2.Distance(Event.current.mousePosition, ps);
                if (d < minD)
                {
                    minD = d;
                    clossetPoint = p;
                }
            }
            /*if (clossetPoint != null && Event.current.type == EventType.mouseMove)
            {
                Handles.color = colorSets[ColorSet.LINE_TO_PAINT].standart;
                Handles.color = Color.black;
                Vector3 wPos = spriteDeformer.spritePositionToGlobal(clossetPoint.position + clossetPoint.offset2d);
                Handles.SphereCap(10000,wPos ,Quaternion.identity ,HandleUtility.GetHandleSize(wPos)*50f);
                HandleUtility.Repaint();
               
            } */
            if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) && clossetPoint != null && Event.current.button == 0 && !Event.current.alt)
            {
                if (clossetPoint.color != spriteDeformer.editorProps.paintColor)
                {
                    clossetPoint.color = spriteDeformer.editorProps.paintColor;
                    upDateMeshDate();
                    doItAfterMovePoints(new SpritePoint[] { clossetPoint });
                   
                   
                }
            }
        }
        public void sceneEditor()
        {
            Undo.RecordObjects(new Object[] { spriteDeformer }, "Move deformer");
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            if (Tools.current != Tool.View)
                Tools.current = Tool.None;

            
            if (Event.current.keyCode == KeyCode.Delete)
            {
                spriteDeformer.RemovePoints(selectedPoints.ToArray());
                selectedPoints.Clear();
                upDateMeshDate(true);
                Event.current.Use();
            }
            foreach (SpritePoint p in spriteDeformer.points)
            {
                Handles.color = colorSets[ColorSet.POINT].standart;
                Vector3 pos = spriteDeformer.SpritePositionToGlobal(p.spritePosition + p.offset2d);
                Handles.SphereCap(0, pos, Quaternion.identity, 0.1f * HandleUtility.GetHandleSize(pos));
            }
            foreach (Edge e in spriteDeformer.edges)
            {
                Handles.color = colorSets[ColorSet.EDGE].standart;
                Handles.DrawLine(spriteDeformer.SpritePositionToGlobal(e.point1.spritePosition + e.point1.offset2d), spriteDeformer.SpritePositionToGlobal(e.point2.spritePosition + e.point2.offset2d));
            }
            foreach (SpritePoint p in selectedPoints)
            {
                Vector3 pos = spriteDeformer.SpritePositionToGlobal(p.spritePosition + p.offset2d);
                Handles.color = colorSets[ColorSet.POINT].selected;
                Handles.SphereCap(0, pos, Quaternion.identity, 0.14f * HandleUtility.GetHandleSize(pos));
            }

            if (deformerTool == null) deformerTool = new DeformerTool(this);
            deformerTool.OnSceneGUI();
            if (Event.current.type == EventType.mouseDown && currentTool == CurrentTool.NONE && Tools.current != Tool.View && Event.current.button == 0)
            {
                if (GUIUtility.hotControl == 0 && !Event.current.alt)
                {
                    startSelectedRect = Event.current.mousePosition;
                    currentTool = CurrentTool.SELECT_RECT;
                }
            }
            if (currentTool == CurrentTool.SELECT_RECT)
            {
                Vector2 currentPos = Event.current.mousePosition;
                Handles.BeginGUI();
                Vector3 upLeft = (startSelectedRect);
                Vector3 downRight = (currentPos);
                Vector3 upRight = (new Vector2(currentPos.x, startSelectedRect.y));
                Vector3 downLeft = (new Vector2(startSelectedRect.x, currentPos.y));
                Vector3[] rectVerts = { upLeft, upRight, downRight, downLeft };

                Handles.color = colorSets[ColorSet.SELECTED_RECTANGLE].standart;
                Handles.DrawSolidRectangleWithOutline(rectVerts, colorSets[ColorSet.SELECTED_RECTANGLE].standart, colorSets[ColorSet.SELECTED_RECTANGLE].over);
                Handles.EndGUI();
                List<SpritePoint> inRecPoints = new List<SpritePoint>();
                foreach (SpritePoint p in spriteDeformer.points)
                {
                    Vector2 pScreenPos = HandleUtility.WorldToGUIPoint(spriteDeformer.SpritePositionToGlobal(p.spritePosition + p.offset2d));
                    Rect r = new Rect(startSelectedRect.x, startSelectedRect.y, currentPos.x - startSelectedRect.x, currentPos.y - startSelectedRect.y);
                    if (r.Contains(pScreenPos, true))
                    {
                        Vector3 pos = spriteDeformer.SpritePositionToGlobal(p.spritePosition + p.offset2d);
                        Handles.color = colorSets[ColorSet.POINT].over;
                        Handles.SphereCap(0, pos, Quaternion.identity, 0.15f * HandleUtility.GetHandleSize(pos));
                        inRecPoints.Add(p);
                    }
                }
                if (Event.current.rawType == EventType.MouseUp)
                {
                    if (!(Event.current.shift || Event.current.control)) 
                        if(!(startSelectedRect == Event.current.mousePosition))
                        selectedPoints.Clear();
                    if (startSelectedRect == Event.current.mousePosition)
                    {
                        SpritePoint clossestP = null;
                        float minD = float.MaxValue;
                        foreach (var item in spriteDeformer.points)
                        {
                            float d = Vector2.Distance(Event.current.mousePosition,
                                HandleUtility.WorldToGUIPoint(
                                spriteDeformer.SpritePositionToGlobal(item.spritePosition + item.offset2d)));
                            if (d < minD)
                            {
                                minD = d;
                                clossestP = item;
                            }
                        }
                        if (minD < 6f)
                        {
                            if (!(Event.current.shift || Event.current.control))
                            {
                                selectedPoints.Clear();   
                            }
                            inRecPoints.Add(clossestP);
                        }

                    }
                    foreach (SpritePoint p in inRecPoints)
                    {
                        if (Event.current.shift && Event.current.control)
                        {
                            if (!selectedPoints.Contains(p))
                                selectedPoints.Add(p);
                        }
                        else
                            if (Event.current.shift)
                            {
                                if (selectedPoints.Contains(p))
                                    selectedPoints.Remove(p);
                                else
                                    selectedPoints.Add(p);
                            }
                            else
                                if (Event.current.control)
                                {
                                    if (selectedPoints.Contains(p))
                                        selectedPoints.Remove(p);
                                }
                                else
                                {
                                    selectedPoints.Add(p);
                                }
                    }
                    if(!spriteDeformer.editorProps.savePivot)deformerTool.recalCulatePivot();
                    currentTool = CurrentTool.NONE;
                }
            }
            
            if (Event.current.rawType == EventType.MouseUp)
            {
                currentTool = CurrentTool.NONE;
            }
        }
        private void sceneBounds()
        {
            Undo.RecordObjects(new Object[] { spriteDeformer }, "Bounds");
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            if (Tools.current != Tool.View)
                Tools.current = Tool.None;
            Rect r = new Rect();
            Bounds b = spriteDeformer.bounds;

            r.size = b.size;
            r.center = b.center;
            Rect newR =
                RectHandle(spriteDeformer.transform, r,
                colorSets[ColorSet.BOUNDS_LINE].standart,
                colorSets[ColorSet.BOUNDS_DOT].standart);
            if (r != newR)
            {
                newR.width = Mathf.Abs(newR.width);
                newR.height = Mathf.Abs(newR.height);

                b.center = newR.center;
                b.size = new Vector3(newR.width, newR.height, b.size.z);
                if(!Application.isPlaying)spriteDeformer.bounds = b;                                              
                spriteDeformer.editorProps.boundsEditorMode = SpriteDeformerEditorSaver.BoundsEditorMode.USER;
            }
        }
        private void inspectorBounds()
        {
            string[] names = { "Encapsulate", "Crope", "User" };

            SpriteDeformerEditorSaver.BoundsEditorMode newBm =
           (SpriteDeformerEditorSaver.BoundsEditorMode)EditorGUILayout.Popup("Mode:", (int)spriteDeformer.editorProps.boundsEditorMode, names);
            if (newBm != spriteDeformer.editorProps.boundsEditorMode)
            {
                spriteDeformer.editorProps.boundsEditorMode = newBm;
                recalculeteBounds();
            }
            spriteDeformer.editorProps.boundsEditorMode = newBm;
        }
        private void recalculeteBounds()
        {
            if (Application.isPlaying) return; 
            Bounds b = spriteDeformer.bounds;
            switch (spriteDeformer.editorProps.boundsEditorMode)
            {
                case SpriteDeformerEditorSaver.BoundsEditorMode.USER: break;
                case SpriteDeformerEditorSaver.BoundsEditorMode.CROPE:
                    b.size = new Vector3(0, 0, 1);
                    foreach (var item in spriteDeformer.points)
                    {
                        foreach (var offset in item.offsets)
                        {
                            Vector3 v3 = spriteDeformer.SpritePositionToLocal((Vector3)item.spritePosition + offset);

                            b.Encapsulate((Vector3)v3); 
                        }
                    
                    }
                    break;
                case SpriteDeformerEditorSaver.BoundsEditorMode.Encapsulate:
                    foreach (var item in spriteDeformer.points)
                    {
                        foreach (var offset in item.offsets)
                        {
                            Vector3 v3 = spriteDeformer.SpritePositionToLocal((Vector3)item.spritePosition + offset);

                            b.Encapsulate((Vector3)v3);
                        }

                    }
                    break;
                default: break;
            }
            if(!Application.isPlaying)spriteDeformer.bounds = b;
        }
        protected virtual void inspectorMain()
        {
            bool newAutoFreezeInEditoe = EditorGUILayout.Toggle("Auto scale in editor", spriteDeformer.editorProps.autoFreezeScale);
            if (newAutoFreezeInEditoe != spriteDeformer.editorProps.autoFreezeScale && newAutoFreezeInEditoe)
            {
                spriteDeformer.FreezeScale();
                recalculeteBounds();
            }
            spriteDeformer.editorProps.autoFreezeScale = newAutoFreezeInEditoe;

            Sprite newSprite = (Sprite)EditorGUILayout.ObjectField("Sprite:", spriteDeformer.sprite, typeof(Sprite), true);
            if (newSprite != spriteDeformer.sprite)
            {
                if (spriteDeformer.points.Count == 0 && newSprite != null)
                {
                    spriteDeformer.sprite = newSprite;
                    spriteDeformer.SetRectanglePoints();
                    upDateMeshDate(true);
                }
                else
                {
                    spriteDeformer.sprite = newSprite;
                }
            }

            Vector2 newScale = EditorGUILayout.Vector2Field("Scale:", spriteDeformer.scale);
            if (newScale != spriteDeformer.scale)
            {
                spriteDeformer.scale = newScale;
                upDateMeshDate();
                recalculeteBounds();
            }
            spriteDeformer.scale = newScale;
            {
                string[] generateColliderNames = { "never", "editor", "editor and runtime" };
                int index = 0;
                if (spriteDeformer.generateColliderInRunTime) index = 2;
                if (spriteDeformer.editorProps.generateColliderInEditor && !spriteDeformer.generateColliderInRunTime) index = 1;
                if (!spriteDeformer.editorProps.generateColliderInEditor && !spriteDeformer.generateColliderInRunTime) index = 0;
                int newIndex = EditorGUILayout.Popup("Generate collider in: ", index, generateColliderNames);

                if (newIndex == 0) { spriteDeformer.editorProps.generateColliderInEditor = false; spriteDeformer.generateColliderInRunTime = false; }
                if (newIndex == 1) { spriteDeformer.editorProps.generateColliderInEditor = true; spriteDeformer.generateColliderInRunTime = false; }
                if (newIndex == 2) { spriteDeformer.editorProps.generateColliderInEditor = true; spriteDeformer.generateColliderInRunTime = true; }
                if (newIndex != index && spriteDeformer.editorProps.generateColliderInEditor == true) { upDateMeshDate(true); };
            }
            {
                if (spriteDeformer.meshRender != null)
                {
                    List<string> sortLaerNames = new List<string>(GetSortingLayerNames());
                    string currentLN = spriteDeformer.meshRender.sortingLayerName;
                    int index = sortLaerNames.IndexOf(currentLN);
                    if (index == -1) index = 0;
                    index = EditorGUILayout.Popup("Sorting layer:", index, sortLaerNames.ToArray());
                    if (index != -1)
                    {
                        spriteDeformer.meshRender.sortingLayerName = sortLaerNames[index];
                    }
                    else
                    {
                        spriteDeformer.meshRender.sortingLayerName = "";
                    }
                    spriteDeformer.meshRender.sortingOrder = EditorGUILayout.IntField("Sorting order:", spriteDeformer.meshRender.sortingOrder);
                }
            }
            spriteDeformer.maxFps = EditorGUILayout.FloatField("Maximum FPS :", spriteDeformer.maxFps);
        }
        public virtual void inspectorWhenSelectPoints()
        {
            EditorGUILayout.LabelField("Drag mouse to select;");
            EditorGUILayout.LabelField("Ctrl + Drag to subtract seletion;");
            EditorGUILayout.LabelField("Shift + Drag to invers section;");
            EditorGUILayout.LabelField("Ctrl + Shift + Drag to append selection;");
            spriteDeformer.editorProps.savePivot = EditorGUILayout.Toggle("Save pivot",spriteDeformer.editorProps.savePivot);
            if (selectedPoints.Count > 0)
            {
                if (GUILayout.Button("Reset offset"))
                {
                    foreach (SpritePoint p in selectedPoints)
                    {
                        p.offset = Vector2.zero;
                    }
                    doItAfterMovePoints(selectedPoints.ToArray());
                    upDateMeshDate();

                }
                if (GUILayout.Button("Remove points"))
                {
                    spriteDeformer.RemovePoints(selectedPoints.ToArray());
                    selectedPoints.Clear();
                    upDateMeshDate(true);
                }
            }
            if (selectedPoints.Count == 2 && !spriteDeformer.ContainsEdge(selectedPoints[0], selectedPoints[1]))
            {
                if (GUILayout.Button("Connect as edge"))
                {
                    spriteDeformer.CreateEdge(selectedPoints[0], selectedPoints[1]);
                    upDateMeshDate(true);
                }
            }

            if (selectedPoints.Count == 2 && spriteDeformer.GetEdgeWithTwoPoints(selectedPoints[0], selectedPoints[1]) != null)
            {
                if (GUILayout.Button("Remove edge"))
                {
                    spriteDeformer.RemoveEdge(spriteDeformer.GetEdgeWithTwoPoints(selectedPoints[0], selectedPoints[1]));
                    upDateMeshDate(true);
                }
            }
            if (selectedPoints.Count > 0)
            {
                selectedPointName = selectedPoints[0].name;
                foreach (var item in selectedPoints)
                {
                    if (item.name != selectedPointName)
                    {
                        selectedPointName = "";
                        break;
                    }
                }
                string newName = EditorGUILayout.TextField("Points\' names:", selectedPointName);
                if (newName != selectedPointName && newName != "")
                {
                    foreach (var item in selectedPoints)
                    {
                        item.name = newName; 
                    }
                }
            }
            if (selectedPoints.Count > 0)
            {
                float selectedZ = selectedPoints[0].offset.z;
                foreach (var item in selectedPoints)
                {
                    if (item.offset.z != selectedZ)
                    {
                        selectedZ = 0;
                    }
                }
                float newZ = EditorGUILayout.FloatField("Z:", selectedZ);
                if (newZ != selectedZ)
                {
                    foreach (var item in selectedPoints)
                    {
                        Vector3 v3 = item.offset;
                        v3.z = newZ;
                        item.offset = v3;
                    }
                    doItAfterMovePoints(selectedPoints.ToArray());
                    upDateMeshDate();
                }
                
            }
            if (selectedPoints.Count > 0)
            {
                Vector2 selColOff = selectedPoints[0].colliderOffset;
                foreach (var item in selectedPoints)
                {
                    if (item.colliderOffset != selColOff)
                    {
                        selColOff = Vector2.zero;
                    }
                }
                Vector2 newSelColOff = EditorGUILayout.Vector2Field("Collider offset:", selColOff);
                
                if(newSelColOff != selColOff)
                {
                    foreach (var item in selectedPoints)
                    {
                        item.colliderOffset = newSelColOff;  
                    }
                    if (spriteDeformer.editorProps.generateColliderInEditor)
                    {
                        spriteDeformer.GenerateCollider();  
                    }
                }
                
            }
        }
        string selectedPointName = "";
        public void upDateMeshDate(bool iRechengeTopology = false)
        {
            bool tr = spriteDeformer.triangulateWithOffsetPosition ? true : iRechengeTopology;
            spriteDeformer.dirty_offset = true;
            spriteDeformer.dirty_tris = tr;
            spriteDeformer.dirty_color = true;
            spriteDeformer.dirty_normals = true;
            spriteDeformer.UpdateMeshImmediate();
            recalculeteBounds();
            if (spriteDeformer.editorProps.generateColliderInEditor)
            {
                spriteDeformer.GenerateCollider();
            }

            EditorUtility.SetDirty(spriteDeformer);

        }
        private int selCorner = -1;
        public Rect RectHandle(Transform owner, Rect rect, Color lineColor, Color dotColor)
        {
            Rect resRect = rect;
            Vector2[] cornes = 
            {
               new Vector3(rect.x,rect.y), //corneUpLeft
               new Vector3(rect.xMax, rect.y),  // corneUpRight
                new Vector3(rect.x,rect.yMax),//corneDownLeft
                new Vector3(rect.xMax, rect.yMax) // corneDownRight
                
            };
            Vector2[] cornesS = new Vector2[4];
            for (int i = 0; i < 4; i++)
            {
                cornesS[i] = HandleUtility.WorldToGUIPoint(owner.TransformPoint(cornes[i]));
            }
            if (Event.current.type == EventType.mouseDown)
            {
                float minDis = float.MaxValue;
                Vector2 mPos = Event.current.mousePosition;
                int closestCorn = -1;
                for (int i = 0; i < cornesS.Length; i++)
                {
                    float d = Vector2.Distance(cornesS[i], mPos);
                    if (d < minDis)
                    {
                        minDis = d;
                        closestCorn = i;
                    }
                }
                if (minDis < 10)
                {
                    selCorner = closestCorn;
                }
                else selCorner = -1;
            }
            if (Event.current.type == EventType.mouseUp)
            {
                selCorner = -1;
            }
            if (Event.current.type == EventType.mouseDrag && selCorner != -1)
            {
                Plane3d plnr = new Plane3d(owner.transform.position, owner.transform.rotation);
                Vector3 wp = plnr.rayCast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition));
                Vector2 lm = owner.InverseTransformPoint(wp);
                if (selCorner == 0)
                {
                    resRect.xMin = lm.x;
                    resRect.yMin = lm.y;
                }
                if (selCorner == 1)
                {
                    resRect.xMax = lm.x;
                    resRect.yMin = lm.y;
                }
                if (selCorner == 2)
                {
                    resRect.xMin = lm.x;
                    resRect.yMax = lm.y;
                }
                if (selCorner == 3)
                {
                    resRect.xMax = lm.x;
                    resRect.yMax = lm.y;
                }
            }
            Vector3[] polyRect = 
            {
              owner.TransformPoint(resRect.x,resRect.y,0),
              owner.TransformPoint(resRect.xMax,resRect.y,0),
              owner.TransformPoint(resRect.xMax,resRect.yMax,0),
              owner.TransformPoint(resRect.x,resRect.yMax,0),
              owner.TransformPoint(resRect.x,resRect.y,0)
            };
            Handles.color = lineColor;
            Handles.DrawPolyLine(polyRect);
            Handles.color = dotColor;
            foreach (var item in polyRect)
            {
                Handles.DotCap(0, item, owner.rotation, HandleUtility.GetHandleSize(item) * 0.05f);
            }
            return resRect;
        }
        private Vector3 guiPositionToWorldPosition(Vector2 guiPos)
        {
            Ray r = HandleUtility.GUIPointToWorldRay(guiPos);
            Vector3 normal = spriteDeformer.transform.rotation * Vector3.back;
            float t = -Vector3.Dot(r.origin - spriteDeformer.transform.position, normal) / Vector3.Dot(r.direction, normal);
            return r.origin + r.direction * t;
        }
        private Vector3 guiPositionToLocalPosition(Vector2 guiPos)
        {
            return spriteDeformer.transform.InverseTransformPoint(guiPositionToWorldPosition(guiPos));
        }

    }
}

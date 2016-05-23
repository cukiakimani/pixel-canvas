using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
namespace Medvedya.SpriteDeformerTools
{
    class UVEditorWindow : EditorWindow, IHasCustomMenu
    {
        [System.NonSerialized]
        GUIStyle lockButtonStyle;

        [System.NonSerialized]
        bool locked = true;

        void ShowButton(Rect position)
        {
            if (lockButtonStyle == null)
                lockButtonStyle = "IN LockButton";
            locked = GUI.Toggle(position, locked, GUIContent.none, lockButtonStyle);
        }
        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Lock"), locked, () =>
            {
                locked = !locked;
            });
        }

        Vector2 offsetImage = new Vector2(0, 50);
        float snapDistance = 10;
        private bool inversGizmos = false;


        public SpriteDeformer spriteDeformer;

        enum UVtools { POINTS, DELETE, EDGE, INTERACTIVE_EDGE };
        UVtools uvTools;
        UVtools memoryUvTools;
        private string[] uvToolsNames = new string[] { "Points", "Remove", "Edge", "Interactive" };
        Vector2 scrollPosition;
        Vector2 spriteSize;
        float zoom = 1;
        Texture bavgroundTexture;

        void OnEnable()
        {
            bavgroundTexture = Resources.Load<Texture2D>("alpha_spriteDeformer");
        }
        void OnGUI()
        {
            this.wantsMouseMove = true;
            if (spriteDeformer == null || spriteDeformer.sprite == null) return;
            spriteSize = new Vector2(spriteDeformer.sprite.rect.width, spriteDeformer.sprite.rect.height);
            drawToolbar();
            drawImage();
        }

        float sliderZoom = 1;
        public void Update()
        {
            if (!locked && Selection.activeGameObject != null)
            {
                SpriteDeformer newSel = Selection.activeGameObject.GetComponent<SpriteDeformer>();
                if (newSel != null)
                {
                    if (newSel != spriteDeformer)
                    {
                        spriteDeformer = newSel;
                        resetParam();
                        this.titleContent.text = newSel.gameObject.name;
                        //this.title = newSel.gameObject.name;
                        Repaint();
                    }
                }
            }

        }

        void drawToolbar()
        {


            float newSliderZoom = sliderZoom;
            GUI.Label(new Rect(0, 20, 40, 20), "zoom:");
            newSliderZoom = GUI.HorizontalSlider(new Rect(40, 20, 200 - 40, 10), sliderZoom, 1f, 5f);
            if (sliderZoom != newSliderZoom)
            {
                //Vector2 curSize = zoom
                sliderZoom = newSliderZoom;
            }
            float oldZoom = zoom;

            zoom = Mathf.Min((this.position.width - offsetImage.x - offsetImage2.x) / spriteSize.x, (this.position.height - offsetImage.y - offsetImage2.y) / spriteSize.y);
            zoom *= sliderZoom;
            UVtools newUV = (UVtools)GUI.Toolbar(new Rect(130, 3, 290, 20), (int)uvTools, uvToolsNames);
            if (newUV != uvTools)
            {
                resetParam();
            }
            uvTools = newUV;
            if (zoom != oldZoom)
            {
                scrollPosition -= (spriteSize * oldZoom - spriteSize * zoom) / 2;
            }
            if (Event.current.keyCode == KeyCode.Space)
            {
                resetParam();
                Repaint();
                updateSpriteDeformer();
            }
            if (latestPoint != null && (uvTools == UVtools.EDGE || uvTools == UVtools.INTERACTIVE_EDGE))
            {

                if (GUI.Button(new Rect(220, 24, 190, 20), "Forgive the last point. (Space)"))
                {
                    resetParam();
                }
            }
            inversGizmos = GUI.Toggle(new Rect(0, 4, 100, 15), inversGizmos, "Invers gizmos");
            //zoom = Mathf.Clamp(zoom, (this.position.width - 50) / spriteSize.x, 10f);
        }
        void resetParam()
        {
            if (closestPoint != null && !spriteDeformer.points.Contains(closestPoint))
                closestPoint = null;
            clossesDiveder = null;
            dragPoint = null;
            latestPoint = null;
        }
        Vector2 offsetImage2 = new Vector2(0, 0);
        void drawImage()
        {

            Texture t = spriteDeformer.sprite.texture;

            Rect tr = spriteDeformer.sprite.textureRect;
            tr = spriteDeformer.sprite.rect;
            Rect r = new Rect(tr.x / t.width, tr.y / t.height, tr.width / t.width, tr.height / t.height);
            scrollPosition = GUI.BeginScrollView(
                new Rect(offsetImage.x, offsetImage.y, 
                this.position.width - offsetImage.x , 
                this.position.height - offsetImage.y), 
                scrollPosition, 
                new Rect(0, 0, tr.width * zoom, tr.height * zoom));
            Rect brect = new Rect(0, 0, 20,  spriteSize.y / spriteSize.x * 20f);
            GUI.DrawTextureWithTexCoords(new Rect(0, 0, tr.width * zoom, tr.height * zoom), bavgroundTexture, brect);

            GUI.DrawTextureWithTexCoords(new Rect(0, 0, tr.width * zoom, tr.height * zoom), t, r);

            Handles.BeginGUI();

            render();
            updateTools();
            Handles.EndGUI();
            GUI.EndScrollView();
        }
        void updateSpriteDeformer()
        {
            Undo.RecordObject(spriteDeformer, "Topology");
            spriteDeformer.SetAllDirty();

            spriteDeformer.UpdateMeshImmediate();
            EditorUtility.SetDirty(spriteDeformer);
        }
        void render()
        {
            foreach (SpritePoint p in spriteDeformer.points)
            {
                drawPoint(p);
            }
            foreach (Edge e in spriteDeformer.edges)
            {
                drawEdge(e);
            }
        }
        void updateTools()
        {
            Undo.RecordObject(spriteDeformer, "Topology");
            switch (uvTools)
            {
                case UVtools.POINTS: pointTool(); break;
                case UVtools.EDGE: edgeTool(); break;
                case UVtools.DELETE: deleteTool(); break;
                case UVtools.INTERACTIVE_EDGE: interactiveEdge(); break;
                default: break;


            }

            if (Event.current.type == EventType.KeyUp)
            {
                if (Event.current.keyCode == KeyCode.Delete || Event.current.keyCode == KeyCode.Backspace)
                {
                    if (closestPoint != null)
                    {
                        spriteDeformer.RemovePoint(closestPoint);
                        updateSpriteDeformer();
                        resetParam();
                        Repaint();
                    }
                    else if (clossesDiveder != null)
                    {
                        spriteDeformer.RemoveEdge(clossesDiveder.edge);
                        updateSpriteDeformer();
                        resetParam();
                        Repaint();
                    }

                }
            }
        }
        void interactiveEdge()
        {

            if (Event.current.type == EventType.MouseMove)
            {
                // if (latestPoint == null)
                {
                    closestPoint = null;
                    float clossestPointdist = snapDistance;
                    foreach (var point in spriteDeformer.points)
                    {
                        Vector2 pointScreenPos = getImagePos(point.spritePosition);
                        float d = Vector2.Distance(Event.current.mousePosition, pointScreenPos);
                        if (d < clossestPointdist)
                        {
                            clossestPointdist = d;
                            closestPoint = point;
                        }
                    }


                    float clossetDividerDist = float.MaxValue;
                    clossesDiveder = null;
                    if (closestPoint == null)
                    {
                        Vector2 mPos = getLocalPos(mousePosInImage);
                        //Debug.Log(mPos);
                        if (latestPoint != null)
                        {
                            List<Edge> wo = spriteDeformer.GetEdgesWithPoint(latestPoint);

                            clossesDiveder = spriteDeformer.GetClosestEdge(mPos, wo.ToArray());
                        }
                        else
                        {
                            clossesDiveder = spriteDeformer.GetClosestEdge(mPos);
                        }



                        if (clossesDiveder != null)
                        {


                            clossetDividerDist = Vector2.Distance(getImagePos(clossesDiveder.position), mousePosInImage);
                            if (clossetDividerDist > snapDistance)
                            {
                                clossesDiveder = null;
                            }
                            else closestPoint = null;

                        }

                    }

                }


            }
            List<SpritePoint> dontConnectPoint = new List<SpritePoint>();
            if (latestPoint != null)
            {
                List<Edge> edgsWithP = spriteDeformer.GetEdgesWithPoint(latestPoint);
                foreach (var item in edgsWithP)
                {
                    dontConnectPoint.Add(item.point1);
                    dontConnectPoint.Add(item.point2);
                }
            }

            if (Event.current.type == EventType.mouseDrag)
            {
                if (dragPoint != null)
                {
                    dragPoint.spritePosition = getLocalPos(Event.current.mousePosition);
                    updateSpriteDeformer();
                }
            }
            if (Event.current.rawType == EventType.mouseUp)
            {
                if (latestPoint != null)
                {

                }
            }
            if (closestPoint != null)
            {
                drawClosestPoint(closestPoint);
                if (latestPoint != null && !dontConnectPoint.Contains(closestPoint))
                {
                    drawNewEdge(latestPoint.spritePosition, closestPoint.spritePosition);
                }

            }
            else if (clossesDiveder != null)
            {
                drawNewPoint(clossesDiveder.position);
                if (latestPoint != null)
                    drawNewEdge(clossesDiveder.position, latestPoint.spritePosition);

            }
            else
            {
                drawNewPoint(getLocalPos(Event.current.mousePosition));
                if (latestPoint != null)
                    drawNewEdge(getLocalPos(Event.current.mousePosition), latestPoint.spritePosition);

            }

            if (Event.current.type == EventType.MouseDown)
            {
                if (closestPoint != null)
                {
                    if (latestPoint != null && !dontConnectPoint.Contains(closestPoint) && closestPoint != latestPoint)
                    {
                        spriteDeformer.CreateEdge(latestPoint, closestPoint);
                        updateSpriteDeformer();
                    }

                    dragPoint = closestPoint;
                    latestPoint = dragPoint;
                    //clossesDiveder = null;
                    // closestPoint = null;

                    //closestPoint = dragPoint;

                }
                else if (clossesDiveder != null)
                {
                    dragPoint = spriteDeformer.DivedeEdge(clossesDiveder, true);
                    if (latestPoint != null && closestPoint != latestPoint)
                    {
                        spriteDeformer.CreateEdge(latestPoint, dragPoint);
                        updateSpriteDeformer();
                    }

                    clossesDiveder = null;
                    latestPoint = dragPoint;

                }
                else
                {
                    SpritePoint newPoint = new SpritePoint(getLocalPos(Event.current.mousePosition));
                    spriteDeformer.AddPoint(newPoint, true);
                    if (latestPoint != null)
                    {
                        spriteDeformer.CreateEdge(latestPoint, newPoint);

                    }
                    dragPoint = newPoint;
                    latestPoint = dragPoint;
                    updateSpriteDeformer();
                    closestPoint = newPoint;
                    // Debug.Log("NewPoint");
                }
            }
            Repaint();
        }
        void deleteTool()
        {
            if (Event.current.type == EventType.MouseMove)
            {
                float minDis = snapDistance;
                closestPoint = null;
                foreach (SpritePoint p in spriteDeformer.points)
                {
                    if (p == latestPoint) continue;
                    if (spriteDeformer.ContainsEdge(latestPoint, p)) continue;

                    float cm = Vector2.Distance(getImagePos(p.spritePosition), Event.current.mousePosition);
                    if (cm < minDis)
                    {
                        minDis = cm;
                        closestPoint = p;
                    }
                }
                Repaint();
            }


            if (closestPoint != null)
            {
                List<Edge> edges = spriteDeformer.GetEdgesWithPoint(closestPoint);
                if (edges.Count == 0)
                {
                    drawDeletePoint(closestPoint);
                    if (Event.current.type == EventType.mouseUp)
                    {
                        spriteDeformer.RemovePoint(closestPoint);
                        updateSpriteDeformer();
                        closestPoint = null;
                        Repaint();

                    }
                }
                if (edges.Count == 2)
                {
                    drawDeletePoint(closestPoint);
                    SpritePoint anetherPoint1 = edges[0].point1 != closestPoint ? edges[0].point1 : edges[0].point2;
                    SpritePoint anetherPoint2 = edges[1].point1 != closestPoint ? edges[1].point1 : edges[1].point2;
                    drawDeleteEdge(edges[0]);
                    drawDeleteEdge(edges[1]);
                    if (!spriteDeformer.ContainsEdge(anetherPoint1, anetherPoint2))
                    {
                        drawNewEdge(anetherPoint1.spritePosition, anetherPoint2.spritePosition);
                    }
                    if (Event.current.type == EventType.mouseUp)
                    {
                        spriteDeformer.RemovePoint(closestPoint);
                        if (!spriteDeformer.ContainsEdge(anetherPoint1, anetherPoint2))
                            spriteDeformer.AddEdge(new Edge(anetherPoint1, anetherPoint2));
                        closestPoint = null;
                        Repaint();
                        updateSpriteDeformer();
                        return;
                    }
                }
                if (edges.Count == 1 || edges.Count > 2)
                {
                    foreach (Edge e in edges)
                    {
                        drawDeleteEdge(e);
                        drawDeletePoint(closestPoint);

                    }
                    if (Event.current.type == EventType.mouseUp)
                    {
                        foreach (Edge e in edges)
                        {
                            spriteDeformer.RemoveEdge(e);
                        }
                        spriteDeformer.RemovePoint(closestPoint);
                        updateSpriteDeformer();
                        closestPoint = null;
                        Repaint();
                    }
                }
            }
            if (closestPoint == null)
            {
                EdgeDivider ed = spriteDeformer.GetClosestEdge(getLocalPos(Event.current.mousePosition));

                if (Vector2.Distance(getImagePos(ed.position), Event.current.mousePosition) < snapDistance / 2)
                {
                    drawDeleteEdge(ed.edge);

                    if (Event.current.type == EventType.mouseUp)
                    {
                        spriteDeformer.RemoveEdge(ed.edge);
                        Repaint();
                    }
                }
            }
        }
        private SpritePoint latestPoint = null;
        void edgeTool()
        {

            if (Event.current.type == EventType.MouseMove)
            {
                float minDis = float.MaxValue;
                closestPoint = null;
                foreach (SpritePoint p in spriteDeformer.points)
                {
                    //if (p == latestPoint) continue;
                    if (spriteDeformer.ContainsEdge(latestPoint, p)) continue;

                    float cm = Vector2.Distance(getImagePos(p.spritePosition), Event.current.mousePosition);
                    if (cm < minDis)
                    {
                        minDis = cm;
                        closestPoint = p;
                    }
                }
                Repaint();
            }
            if (closestPoint != null)
            {
                drawDot(closestPoint.spritePosition, latestPoint == null ? Color.yellow : Color.blue);
                if (latestPoint == null)
                {
                    if (Event.current.type == EventType.mouseUp)
                    {
                        latestPoint = closestPoint;
                        Repaint();
                    }
                }
                if (latestPoint != null)
                {
                    if (latestPoint != closestPoint)
                    {
                        drawNewEdge(latestPoint.spritePosition, closestPoint.spritePosition);
                        drawLatestPoint(latestPoint);
                        if (Event.current.type == EventType.mouseUp)
                        {
                            if (latestPoint != closestPoint)
                            {
                                spriteDeformer.CreateEdge(latestPoint, closestPoint);
                            }
                            updateSpriteDeformer();

                            latestPoint = closestPoint;
                            Repaint();
                        }
                    }
                    else
                    {
                        if (Event.current.type == EventType.mouseUp)
                        {
                            Repaint();
                        }
                    }
                }
            }

        }
        private SpritePoint closestPoint = null;
        SpritePoint dragPoint = null;
        EdgeDivider clossesDiveder = null;
        Vector2 deltaDrag = Vector2.zero;
        void pointTool()
        {
            latestPoint = null;
            if (dragPoint != null)
            {
                dragPoint.spritePosition = getLocalPos(Event.current.mousePosition + deltaDrag);
                drawDragPoint(dragPoint);
                Repaint();
                if (Event.current.rawType == EventType.MouseUp)
                {
                    closestPoint = dragPoint;
                    dragPoint = null;
                    clossesDiveder = null;
                    Repaint();
                }
                if (Event.current.type == EventType.MouseDrag)
                    updateSpriteDeformer();
                return;
            }
            if (Event.current.type == EventType.MouseMove)
            {
                float minDis = snapDistance;
                closestPoint = null;
                foreach (SpritePoint p in spriteDeformer.points)
                {
                    float cm = Vector2.Distance(getImagePos(p.spritePosition), Event.current.mousePosition);
                    if (cm < minDis)
                    {
                        minDis = cm;
                        closestPoint = p;
                    }
                }
                clossesDiveder = null;
                if (closestPoint == null)
                {
                    Vector2 mPos = getLocalPos(mousePosInImage);
                    EdgeDivider ed = spriteDeformer.GetClosestEdge(mPos);

                    if (Vector2.Distance(getImagePos(ed.position), mousePosInImage) < snapDistance)
                    {
                        clossesDiveder = ed;
                    }
                }
                Repaint();
            }
            if (clossesDiveder != null)
            {
                drawNewPoint(clossesDiveder.position);
            }
            if (closestPoint != null)
            {
                drawClosestPoint(closestPoint);
            }
            if (closestPoint == null && clossesDiveder == null)
            {
                drawNewPoint(getLocalPos(Event.current.mousePosition));
            }

            if (Event.current.type == EventType.mouseDown)
            {
                if (closestPoint != null)
                {
                    dragPoint = closestPoint;
                    deltaDrag = getImagePos(dragPoint.spritePosition) - Event.current.mousePosition;
                }
                else
                {
                    if (clossesDiveder == null)
                    {
                        dragPoint = new SpritePoint(getLocalPos(Event.current.mousePosition));
                        deltaDrag = getImagePos(dragPoint.spritePosition) - Event.current.mousePosition;
                        spriteDeformer.AddPoint(dragPoint, true);
                        updateSpriteDeformer();
                    }
                    else
                    {
                        dragPoint = spriteDeformer.DivedeEdge(clossesDiveder, true);
                        closestPoint = dragPoint;
                        deltaDrag = getImagePos(dragPoint.spritePosition) - Event.current.mousePosition;
                        updateSpriteDeformer();
                    }
                }
            }

        }
        Vector2 getLocalPos(Vector2 p)
        {
            Vector2 v2 = p;
            //v2 -= offsetImage;
            //v2 += scrollPosition;
            v2 /= zoom;
            v2.x /= spriteSize.x;
            v2.y /= spriteSize.y;
            v2.y = 1 - v2.y;
            v2.x = Mathf.Clamp(v2.x, 0, 1);
            v2.y = Mathf.Clamp(v2.y, 0, 1);
            return v2;
        }
        Vector2 getImagePos(Vector2 p)
        {
            return (new Vector2(p.x * spriteSize.x, (1 - p.y) * spriteSize.y) * zoom);
        }
        Vector2 mousePosInImage
        {
            get
            {
                Vector2 r = Event.current.mousePosition;
                r = new Vector2(Mathf.Clamp(r.x, 0, spriteSize.x * zoom), Mathf.Clamp(r.y, 0, spriteSize.y * zoom));
                return r;
            }
        }
        void drawLatestPoint(SpritePoint p)
        {
            drawDot(p.spritePosition, Color.blue);
        }
        void drawDeletePoint(SpritePoint p)
        {
            drawDot(p.spritePosition, Color.red);
        }
        void drawNewPoint(Vector2 pos)
        {
            drawDot(pos, Color.red);
        }
        void drawClosestPoint(SpritePoint p)
        {
            drawDot(p.spritePosition, Color.yellow);
        }
        void drawPoint(SpritePoint p)
        {
            drawDot(p.spritePosition, Color.green);
        }

        void drawDragPoint(SpritePoint p)
        {
            drawDot(p.spritePosition, Color.yellow);
        }
        void drawEdge(Edge edge)
        {
            Handles.color = Color.green;
            if (inversGizmos)
            {
                inversHandlesColor();
            }
            Handles.DrawLine(getImagePos(edge.point1.spritePosition), getImagePos(edge.point2.spritePosition));
        }
        void drawNewEdge(Vector2 pos1, Vector2 pos2)
        {
            Handles.color = Color.blue;
            if (inversGizmos)
            {
                inversHandlesColor();
            }
            Handles.DrawLine(getImagePos(pos1), getImagePos(pos2));
        }
        void drawDeleteEdge(Edge edge)
        {
            Handles.color = Color.red;
            if (inversGizmos)
            {
                inversHandlesColor();
            }
            Handles.DrawLine(getImagePos(edge.point1.spritePosition), getImagePos(edge.point2.spritePosition));
        }


        void drawDot(Vector2 spritePosition, Color color, float size = 5, float z = -100)
        {
            Handles.color = color;
            if (inversGizmos)
            {
                inversHandlesColor();
            }
            Vector3 pos = getImagePos(spritePosition);
            pos.z = z;
            //Handles.SphereCap(0, pos, Quaternion.identity, size);
            //Handles.DotCap(1, pos, Quaternion.identity, size);
            Handles.DrawSolidDisc(pos, Vector3.forward, size);
        }
        void inversHandlesColor()
        {
            Color c = Handles.color;
            c.r = 1 - c.r;
            c.g = 1 - c.g;
            c.b = 1 - c.b;
            Handles.color = c;
        }
        void OnLostFocus()
        {
            resetParam();

        }
        void OnFocus()
        {
            resetParam();
        }


    }
}
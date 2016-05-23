using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Sprites;

namespace Medvedya.SpriteDeformerTools
{
    public enum MainToolBarInspector { STANDART, EDIT_VERTICS, EDIT_BOUNDS, EDIT_COLOR };

    public abstract class SpriteDeformer : MonoBehaviour, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Polygon of border points
        /// </summary>
        [System.Serializable]
        public class PolygonBorderPath
        {
            /// <summary>                  
            /// Indexes of points from SpriteDeformer.points
            /// </summary>
            public int[] points;
        }
        /// <summary>
        /// All border paths
        /// </summary>
        public List<PolygonBorderPath> borderPaths = new List<PolygonBorderPath>();
#if UNITY_EDITOR
        public SpriteDeformerEditorSaver editorProps = new SpriteDeformerEditorSaver();
#endif


        /// <summary>
        /// Current MeshRenderer
        /// </summary>
        public MeshRenderer meshRender
        {
            get
            {
                if (_meshRender == null) _meshRender = GetComponent<MeshRenderer>();
                return _meshRender;
            }
        }

        private MeshRenderer _meshRender;

        /// <summary>
        /// Current sprite, if you set it, the mesh will be builded when unity call Update. 
        /// </summary>
        public virtual Sprite sprite
        {
            get
            {
                return _sprite;
            }
            set
            {
                if (_sprite == value) return;
                onSpriteChange(_sprite, value);
                _sprite = value;
                _lastSprite = value;
                dirty_sprite = true;
                if (!Application.isPlaying)
                {
                    Update();
                }
            }
        }
        [SerializeField]
        private Sprite _sprite;
        private Sprite _lastSprite;

        private Rect lastSpriteRect;
        private Bounds lastSpriteBounds;


        /// <summary>
        /// List of all points.
        /// </summary>
#if UNITY_EDITOR
        public IList<SpritePoint> points { get { return _points.AsReadOnly(); } }
#else
        public List<SpritePoint> points { get{return _points;} }
#endif
        [SerializeField]
        private List<SpritePoint> _points = new List<SpritePoint>();

        /// <summary>
        /// Bounds of mesh, it will set immediate.
        /// </summary>
        public Bounds bounds
        {
            get { return _bounds; }
            set
            {
                //if (value == _bounds) return;
                {
                    _bounds = value;
                    if (mesh != null)
                        mesh.bounds = value;
                    //Debug.Log(mesh);
                }
            }
        }
        [SerializeField]
        private Bounds _bounds = new Bounds(Vector3.zero, new Vector3(1, 1, 1));
#if UNITY_EDITOR
        /// <summary>
        /// List of all edges.
        /// </summary>
        public IList<Edge> edges { get { return _edges.AsReadOnly(); } }
#else
        public List<Edge> edges { get{ return _edges;} }
#endif
        private List<Edge> _edges = new List<Edge>();
        [SerializeField]
        private Mesh mesh;

        protected Vector2 spriteUVposition;
        protected Vector2 spriteUVSize;
        private Vector2 spriteSizeInUnites;
        private Vector2 spritePivotInUnites;

        /// <summary>
        /// if it's true, next Update it will calculate sprite information for current generation of mesh.
        /// </summary>
        [System.NonSerialized]
        public bool dirty_sprite = false;

        /// <summary>
        /// If it's true, next Update it will calculate new vertexes for mesh. 
        /// </summary>
        [System.NonSerialized]
        public bool dirty_offset = false;
        /// <summary>
        /// If it's true, next Update will triangulete and calculate new borderPaths and new trises of mesh. 
        /// </summary>
        [System.NonSerialized]
        public bool dirty_tris = false;
        /// <summary>
        /// If it's true and generate collider is on, next FixedUpdate PolygonCollider 2D will change.
        /// </summary>
        [System.NonSerialized]
        public bool dirty_collider = false;
        /// <summary>
        /// If it's true, next Update it will calculate new UVs for mesh.
        /// </summary>
        [System.NonSerialized]
        public bool dirty_uv = false;
        /// <summary>
        /// If it's true, next Update it will calculate new colors for mesh.
        /// </summary>
        [System.NonSerialized]
        public bool dirty_color = false;
        /// <summary>
        /// If it's true, next Update it will calculate new normals for mesh.
        /// </summary>
        [System.NonSerialized]
        public bool dirty_normals = false;
        /// <summary>
        /// if it's true, triangulation will use offset from SpritePoint.
        /// </summary>
        public bool triangulateWithOffsetPosition
        {
            get { return _triangulateWithOffsetPosition; }
            set
            {
                if (value == _triangulateWithOffsetPosition) return;
                _triangulateWithOffsetPosition = value;
                dirty_tris = true;
                if (!Application.isPlaying)
                {
                    UpdateMeshImmediate();
                }
            }
        }
        [SerializeField]
        private bool _triangulateWithOffsetPosition = false;

        [SerializeField]
        protected Vector2[] mesh_uvs;
        [SerializeField]
        protected Vector3[] mesh_vertecs;
        [SerializeField]
        protected int[] mesh_triangles;
        [SerializeField]
        protected Vector3[] mesh_normals;
        [SerializeField]
        protected Color[] mesh_colors;

        /// <summary>
        /// Scale of sprite, if change it then dirty_points set as true and next Update will generate new vertices. 
        /// </summary>
        public Vector2 scale
        {
            get
            {
                return _scale;
            }
            set
            {
                if (value == _scale) return;
                _scale = value;
                dirty_offset = true;
                _lastScale = value;
            }
        }
        [SerializeField]
        private Vector2 _scale = new Vector2(1, 1);
        private Vector2 _lastScale;

        /// <summary>
        /// It set transform.LocalScale as Vector3 of "1,1,1" and calculate new SpriteDeformer.scale.
        /// </summary>
        public void FreezeScale()
        {
            Vector3 sc = transform.localScale;
            if (sc != new Vector3(1, 1, 1))
            {
                scale = Vector2.Scale(sc, scale);
                transform.localScale = new Vector3(1, 1, 1);
            }

        }
        /// <summary>
        ///Create new unique mesh. The old mesh form MeshFilter will not be destroyed.
        /// </summary>
        public void CreateNewMesh(bool destoryOldMesh = false)
        {
            mesh = new Mesh();
            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf.sharedMesh != null && destoryOldMesh) DestroyImmediate(mf.sharedMesh);
            if (!Application.isPlaying)
                mf.sharedMesh = mesh;
            else
                mf.mesh = mesh;
        }
        /// <summary>
        /// Clear list of points.
        /// </summary>
        public void ClearPoints()
        {
            while (points.Count > 0)
            {
                RemovePoint(points[0]);

            }

        }
        /// <summary>
        /// Clear list of edges.
        /// </summary>
        public void ClearEdges()
        {
            while (_edges.Count > 0)
            {
                _edges.Clear();
            }
            dirty_tris = true;
        }
        /// <summary>
        /// Remove all points and edges and create new topology as rectangle.
        /// </summary>
        public void SetRectanglePoints()
        {
            ClearEdges();
            ClearPoints();
            SpritePoint upLeft = new SpritePoint(0, 1);
            SpritePoint downLeft = new SpritePoint(0, 0);
            SpritePoint upRight = new SpritePoint(1, 1);
            SpritePoint downRight = new SpritePoint(1, 0);
            AddPoint(upLeft); AddPoint(downLeft); AddPoint(upRight); AddPoint(downRight);

            AddEdge(new Edge(upLeft, upRight));
            AddEdge(new Edge(upRight, downRight));
            AddEdge(new Edge(downRight, downLeft));
            AddEdge(new Edge(downLeft, upLeft));

        }
        /// <summary>
        /// Calculate new dependent sprite info for generation mesh.
        /// </summary>
        public void RecalculateSpriteInfo()
        {
            if (sprite == null) return;
            
            if (sprite.packed && sprite.packingMode == SpritePackingMode.Tight)
            {
                Debug.LogException(new System.Exception("Sprite packer -> TightPackerPolicy is not supported. Please use DefaultPackerPolicy"));
                return;
            }
            if (Application.isPlaying)
            {
                spriteUVposition = new Vector2(sprite.textureRect.x / sprite.texture.width, sprite.textureRect.y / sprite.texture.height);
                spriteUVposition -= new Vector2(sprite.textureRectOffset.x / sprite.texture.width, sprite.textureRectOffset.y / sprite.texture.height);
                //spriteVUSize = new Vector2(sprite.textureRect.width / sprite.texture.width, sprite.textureRect.height / sprite.texture.height);
                spriteUVSize = new Vector2(sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height);
            }
            else
            {
                spriteUVposition = new Vector2(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height);
                spriteUVSize = new Vector2(sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height);
            }
            
            spriteSizeInUnites = sprite.rect.size / sprite.pixelsPerUnit;
            spritePivotInUnites = -(sprite.pivot) / sprite.pixelsPerUnit;

            //it doesn't work
            //Vector2.Vector4 uv = UnityEngine.Sprites.DataUtility.GetOuterUV(sprite);
            //spriteVUSize = new Vector2(uv.w, uv.z);
            //spriteUVposition = new Vector2(uv.x, uv.y);
        }
        protected Triangulator triangulator;

        protected void tringulate()
        {
            if (triangulator == null)
                triangulator = new Triangulator(this);
            triangulator.useDeltaPosition = triangulateWithOffsetPosition;
            triangulator.trinagulate();
            borderPaths.Clear();
            foreach (var item in triangulator.bigLoops)
            {
                PolygonBorderPath newP = new PolygonBorderPath();
                borderPaths.Add(newP);
                List<int> pointList = new List<int>();
                foreach (var item2 in item.edgeNodes)
                {
                    pointList.Add(item2.index);
                }
                newP.points = pointList.ToArray();
            }
        }
        private static void NewArray<T>(ref T[] array,int newSize)
        {
            if (array == null)
            {
                array = new T[newSize];
                return;
            }
            if (array.Length != newSize)
            {
                System.Array.Resize<T>(ref array, newSize);
            }
        }
        protected virtual void generateMeshData(bool new_tringulate, bool new_uv, bool new_points, bool new_color, bool new_normals)
        {
            if (new_points || new_color || new_uv || new_normals)
            {
                if (new_normals) NewArray<Vector3>(ref mesh_normals, _points.Count);
                if (new_color) NewArray<Color>(ref mesh_colors,_points.Count);
                if (new_points) NewArray<Vector3>(ref mesh_vertecs, _points.Count);
                if (new_uv) NewArray<Vector2>(ref mesh_uvs, _points.Count);
                for (int i = 0; i < _points.Count; i++)
                {
                    SpritePoint cp = _points[i];
                    if (new_points)
                    {
                        Vector3 v3 = SpritePositionToLocal(getSpritePositionOfSpritePoint(cp));
                        v3.z = cp.offsets[0].z;
                        mesh_vertecs[i] = v3;
                    }
                    if (new_normals) mesh_normals[i] = Vector3.forward;
                    if (new_color) mesh_colors[i] = cp.color;
                    if (new_uv)
                    {
                        mesh_uvs[i] = Vector2.Scale(spriteUVSize, cp.spritePosition) + spriteUVposition;
                    }
                }
            }

            if (new_tringulate)
            {
                tringulate();
                mesh_triangles = triangulator.intTriangles;
            }
        }
        protected void setMeshDataToMesh(bool new_tringulate, bool new_uv, bool new_points, bool new_color, bool new_normals)
        {
            
            if (mesh == null) return;
            if (new_tringulate) mesh.Clear();
            if (new_tringulate || new_points)
            {
                mesh.MarkDynamic();
                mesh.vertices = mesh_vertecs;
            }
            if (new_tringulate || new_normals) mesh.normals = mesh_normals;
            if (new_tringulate || new_uv) mesh.uv = mesh_uvs;
            if (new_tringulate) mesh.triangles = mesh_triangles;
            if (new_tringulate || new_color) mesh.colors = mesh_colors;
            
        }

        /// <summary>
        /// Divide the edge.
        /// </summary>
        /// <param name="ed"></param>
        /// <param name="autoOffset"></param>
        /// <returns></returns>
        public SpritePoint DivedeEdge(EdgeDivider ed, bool autoOffset = false)
        {
            SpritePoint newPoint = new SpritePoint(ed.position);
            Edge e1 = new Edge(ed.edge.point1, newPoint);
            Edge e2 = new Edge(newPoint, ed.edge.point2);
            AddPoint(newPoint);
            AddEdge(e1);
            AddEdge(e2);
            if (autoOffset)
            {
                float d1 = Vector2.Distance(ed.edge.point1.spritePosition, ed.position);
                float d2 = Vector2.Distance(ed.edge.point2.spritePosition, ed.position);
                float ds = d1 + d2;
                float w1 = d1 / ds;
                Vector2 np = Vector2.Lerp(ed.edge.point1.spritePosition + ed.edge.point1.offset2d,
                    ed.edge.point2.spritePosition + ed.edge.point2.offset2d, w1);
                newPoint.offset2d = np - newPoint.spritePosition;
            }
            RemoveEdge(ed.edge);
            return newPoint;
        }

        /// <summary>
        /// Add edge to the topology and set true to dirty_tris.
        /// </summary>
        /// <param name="e"></param>
        public void AddEdge(Edge e)
        {
            _edges.Add(e);
            dirty_tris = true;
        }
        /// <summary>
        /// Get closet edge and get the point for splitting in "EdgeDivider".
        /// </summary>
        /// <param name="spritePos">sprite position</param>
        /// <param name="without">Edges wll not be used</param>
        /// <returns></returns>
        public EdgeDivider GetClosestEdge(Vector2 spritePos, Edge[] without = null)
        {
            EdgeDivider ed = new EdgeDivider();
            float minDis = float.MaxValue;
            List<Edge> wo = null;
            if (without != null)
            {
                wo = new List<Edge>(without);
            }
            foreach (Edge edge in edges)
            {
                if (without != null && wo.Contains(edge))
                {
                    continue;
                }
                Vector2 v = edge.GetClosest(spritePos);
                float d = Vector2.Distance(spritePos, v);
                if (d < minDis)
                {
                    minDis = d;
                    ed.edge = edge;
                    ed.position = v;
                }
            }
            return ed;
        }
        /// <summary>
        /// Get all edges what have this SpritePoint.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public List<Edge> GetEdgesWithPoint(SpritePoint p)
        {
            List<Edge> edgesp = new List<Edge>();
            foreach (Edge e in edges)
            {
                if (e.ContainsPoint(p))
                {
                    edgesp.Add(e);
                }
            }
            return edgesp;
        }
        /// <summary>
        ///  Get the edge which have this SpritePoints.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public Edge GetEdgeWithTwoPoints(SpritePoint p, SpritePoint p2)
        {
            foreach (Edge e in edges)
            {
                if (e.ContainsPoint(p) && e.ContainsPoint(p2))
                {
                    return e;
                }
            }
            return null;
        }
        /// <summary>
        ///  Get all edges what be connected this edge.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public List<Edge> GetEdgesWithEdge(Edge edge)
        {
            List<Edge> res = new List<Edge>();
            foreach (var item in edges)
            {
                if (item.ContainsPoint(edge.point1) || item.ContainsPoint(edge.point2))
                {
                    res.Add(item);
                }
            }
            return res;
        }

        public List<SpritePoint> GetСonnectedPoint(SpritePoint point, string pointName = "")
        {
            List<SpritePoint> res = new List<SpritePoint>();
            List<Edge> es = GetEdgesWithPoint(point);
            foreach (var item in es)
            {
                if (item.ContainsPoint(point))
                {
                    SpritePoint otherPoint = item.point1 == point ? item.point2 : item.point1;
                    if (pointName == "" || pointName == otherPoint.name)
                    {
                        res.Add(otherPoint);
                    }
                }
            }

            return res;
        }
        /// <summary>
        /// Is any edge with this points contains. 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public bool ContainsEdge(SpritePoint p1, SpritePoint p2)
        {
            foreach (Edge e in edges)
            {
                if (e.point1 == p1 && e.point2 == p2 || e.point1 == p2 && e.point2 == p1) return true;
            }
            return false;
        }

        [SerializeField]
        private List<EdgeSerialization> edgeSerializations;
        public void OnBeforeSerialize()
        {
            edgeSerializations = new List<EdgeSerialization>();
            for (int i = 0; i < edges.Count; i++)
            {
                edgeSerializations.Add(
                    new EdgeSerialization(
                        points.IndexOf(edges[i].point1),
                        points.IndexOf(edges[i].point2)
                        )
                    );
            }
#if UNITY_EDITOR
            editorProps.serelizableSelectedPoints = new int[editorProps.selectedPoints.Count];
            for (int i = 0; i < editorProps.selectedPoints.Count; i++)
            {
                editorProps.serelizableSelectedPoints[i] = points.IndexOf(editorProps.selectedPoints[i]);
            }
#endif

        }
        public virtual void OnAfterDeserialize()
        {
            _edges = new List<Edge>();
            for (int i = 0; i < edgeSerializations.Count; i++)
            {
                EdgeSerialization ce = edgeSerializations[i];
                _edges.Add(
                    new Edge(
                        points[ce.point1Index],
                        points[ce.point2index]
                         )
                        );
            }
#if UNITY_EDITOR
            editorProps.selectedPoints = new List<SpritePoint>();
            for (int i = 0; i < editorProps.serelizableSelectedPoints.Length; i++)
            {
                editorProps.selectedPoints.Add(points[editorProps.serelizableSelectedPoints[i]]);
            }
#endif
        }
        [SerializeField]
        private int lastID = 0;
        public void SetAllDirty(bool dirty_value = true)
        {
            dirty_tris = dirty_value;
            dirty_color = dirty_value;
            dirty_normals = dirty_value;
            dirty_offset = dirty_value;
            dirty_uv = dirty_value;
            dirty_sprite = dirty_value;
        }
        /// <summary>
        /// Add point to topology.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="autoOffset"></param>
        public virtual void AddPoint(SpritePoint point, bool autoOffset = false)
        {
            if (point.id != 0)
            {
                Debug.LogError("SpritePoint is not unique", this);
                return;
            }
            lastID++;
            point.id = lastID;
            _points.Add(point);
            SetAllDirty();
            if (autoOffset)
            {
                SpritePoint[] tr = GetTriangleAroundPoint(point);
                if (tr != null)
                {
                    Vector2 u1 = tr[0].spritePosition; // get the triangle UVs
                    Vector2 u2 = tr[1].spritePosition; // get the triangle UVs
                    Vector2 u3 = tr[2].spritePosition; // get the triangle UVs

                    Vector2 l1 = tr[0].spritePosition + (Vector2)tr[0].offset;
                    Vector2 l2 = tr[1].spritePosition + (Vector2)tr[1].offset;
                    Vector2 l3 = tr[2].spritePosition + (Vector2)tr[2].offset;

                    float a = Medvedya.GeometryMath.Triangle.area(u1, u2, u3); if (a == 0f) return;

                    float a1 = Medvedya.GeometryMath.Triangle.area(u2, u3, point.spritePosition) / a; if (a1 < 0) return;
                    float a2 = Medvedya.GeometryMath.Triangle.area(u3, u1, point.spritePosition) / a; if (a2 < 0) return;
                    float a3 = Medvedya.GeometryMath.Triangle.area(u1, u2, point.spritePosition) / a; if (a3 < 0) return;

                    Vector2 pInObj = a1 * l1 + a2 * l2 + a3 * l3;

                    point.offset = pInObj - point.spritePosition;
                }
            }
        }


        /// <summary>
        /// Get the triangle where the point is, it will work only after triangulation was done.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public SpritePoint[] GetTriangleAroundPoint(SpritePoint point)
        {
            for (int i = 0; i < mesh_triangles.Length; i += 3)
            {
                int tr0 = mesh_triangles[i];
                int tr1 = mesh_triangles[i + 1];
                int tr2 = mesh_triangles[i + 2];
                if (
                     Medvedya.GeometryMath.Triangle.isBelongOriantByClock(
                     points[tr0].spritePosition,
                     points[tr1].spritePosition,
                     points[tr2].spritePosition, point.spritePosition)
                     )
                {
                    return new SpritePoint[] { points[tr0], points[tr1], points[tr2] };
                }
            }
            return null;
        }
        /// <summary>
        /// Sprite position to transform-local position.
        /// </summary>
        /// <param name="spritePosition"></param>
        /// <returns></returns>
        public Vector2 SpritePositionToLocal(Vector2 spritePosition)
        {
            Vector2 p = Vector2.Scale(spritePosition, spriteSizeInUnites) + spritePivotInUnites;
            return Vector2.Scale(p, _scale);
        }
        /// <summary>
        ///  Sprite position to transform position.
        /// </summary>
        /// <param name="spritePosition"></param>
        /// <returns></returns>
        public Vector3 SpritePositionToGlobal(Vector2 spritePosition)
        {
            return transform.TransformPoint(SpritePositionToLocal(spritePosition));
        }
        /// <summary>
        /// Local - transform position to sprite position.
        /// </summary>
        /// <param name="localPosition"></param>
        /// <returns></returns>
        public Vector2 LocalPositionToSpritePosition(Vector2 localPosition)
        {
            Vector2 p = new Vector2(localPosition.x / spriteSizeInUnites.x / _scale.x, localPosition.y / spriteSizeInUnites.y / _scale.y)
                  -
                  new Vector2(spritePivotInUnites.x / spriteSizeInUnites.x, spritePivotInUnites.y / spriteSizeInUnites.y)
                ;
            return p;
        }
        /// <summary>
        /// Transform position to sprite position.
        /// </summary>
        /// <param name="globalPosition"></param>
        /// <returns></returns>
        public Vector2 GlobalPositionToSpritePosition(Vector3 globalPosition)
        {
            return LocalPositionToSpritePosition(transform.InverseTransformPoint(globalPosition));
        }

        /// <summary>
        /// Maximum fps if you use animation, if it's zero the mesh will change every frame.
        /// </summary>
        public float maxFps
        {
            get
            {
                if (minimumTimeToUpdate == 0) return 0;
                return 1f / minimumTimeToUpdate;
            }
            set
            {
                if (value == 0) minimumTimeToUpdate = 0;
                else
                    minimumTimeToUpdate = 1f / value;
            }
        }
        [SerializeField]
        private float minimumTimeToUpdate = 0;
        private float timeUpdate = 0;
        protected virtual void Update()
        {
            timeUpdate += Time.unscaledDeltaTime;

            if (meshRender != null && meshRender.isVisible && (timeUpdate >= minimumTimeToUpdate || !Application.isPlaying))
            {
                timeUpdate = 0;

                UpdateMeshImmediate();
            }
            if (!Application.isPlaying)
            {
                if (sprite != null && (lastSpriteRect != sprite.rect || lastSpriteBounds != sprite.bounds))
                {
                    lastSpriteRect = sprite.rect;
                    lastSpriteBounds = sprite.bounds;
                    dirty_sprite = true;
                    UpdateMeshImmediate();
                }

            }
        }
        protected virtual void onSpriteChange(Sprite lastSprite, Sprite currentSprite)
        {

        }

        /// <summary>
        /// Remove a point and all edges with this point and set true to dirty_tris, dirty_points, dirty_uv.
        /// </summary>
        /// <param name="p"></param>
        public virtual void RemovePoint(SpritePoint p)
        {
            SetAllDirty();
            List<Edge> delEdges = new List<Edge>();
            delEdges.AddRange(GetEdgesWithPoint(p));
            foreach (Edge e in delEdges)
            {
                RemoveEdge(e);
            }
            _points.Remove(p);

        }
        /// <summary>
        /// Remove points and all edges with this points.
        /// </summary>
        /// <param name="points"></param>
        public void RemovePoints(SpritePoint[] points)
        {
            foreach (SpritePoint p in points)
            {
                RemovePoint(p);
            }
        }
        /// <summary>
        /// Remove edge and set true to dirty_tris .
        /// </summary>
        /// <param name="e"></param>
        public void RemoveEdge(Edge e)
        {
            _edges.Remove(e);
            dirty_tris = true;
            dirty_offset = true;
        }
        /// <summary>
        /// Create new edge and add it to list of edges.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns>Created edge</returns>
        public Edge CreateEdge(SpritePoint p1, SpritePoint p2)
        {
            Edge newEdge = new Edge(p1, p2);
            _edges.Add(newEdge);
            dirty_tris = true;
            return newEdge;
        }

        /// <summary>
        /// Get EdgeDividers is cuting all edges by segmet.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private EdgeDivider[] getCutDivider(Vector2 v1, Vector2 v2)
        {
            List<EdgeDivider> eds = new List<EdgeDivider>();
            foreach (var item in edges)
            {
                Vector2 op = Vector2.zero;
                if (
                    Medvedya.GeometryMath.Line.intersection(v1, v2, item.point1.spritePosition, item.point2.spritePosition, out op)
                    )
                {
                    eds.Add(new EdgeDivider(op, item));
                }
            }
            return eds.ToArray();
        }
        private void cut(Vector2[] points)
        {
            for (int i = 0; i < points.Length - 1; i++)
            {
                Vector2 v1 = points[i];
                Vector2 v2 = points[i + 1];
                EdgeDivider[] eds = getCutDivider(v1, v2);
                List<SpritePoint> newPoints = new List<SpritePoint>();
                foreach (var item in eds)
                {
                    newPoints.Add(DivedeEdge(item));
                }
                newPoints.Sort(
                    (p1, p2)
                        =>
                        Vector2.Distance(v1, p1.spritePosition).CompareTo(Vector2.Distance(v1, p2.spritePosition))
                        );
                for (int j = 0; j < newPoints.Count - 1; j++)
                {
                    CreateEdge(newPoints[j], newPoints[j + 1]);
                }
            }

        }
        private void cut(Vector2 v1, Vector2 v2)
        {
            cut(new Vector2[] { v1, v2 });
        }

        protected virtual void OnEnable()
        {
            //meshRender.enabled = true;
        }
        protected virtual void OnDisable()
        {
            //meshRender.enabled = false;
        }
        protected virtual void Awake()
        {
            {
                int newSortedId = 0;
                int newSortedOrder = 0;
                bool setSorted = false;
                SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
                Sprite spriteToAdd = null;
                if (sr != null)
                {
                    spriteToAdd = sr.sprite;
                    newSortedId = sr.sortingLayerID;
                    newSortedOrder = sr.sortingOrder;
                    setSorted = true;
                    DestroyImmediate(sr);
                }
                if (gameObject.GetComponent<MeshRenderer>() == null)
                {
                    _meshRender = gameObject.AddComponent<MeshRenderer>();
                }
                if (gameObject.GetComponent<MeshFilter>() == null) gameObject.AddComponent<MeshFilter>();
                if (setSorted)
                {
                    meshRender.sortingLayerID = newSortedId;
                    meshRender.sortingOrder = newSortedOrder;
                }
                if (spriteToAdd != null)
                {
                    sprite = spriteToAdd;
                    SetRectanglePoints();
                }
            }
            if (!Application.isPlaying)
            {
                if (sprite != null)
                {
                    lastSpriteRect = sprite.rect;
                    lastSpriteBounds = sprite.bounds;
                }
                SpriteDeformer[] sds = gameObject.GetComponents<SpriteDeformer>();
                if (sds.Length > 1)
                {
                    foreach (var item in sds)
                    {
                        if (item != this)
                        {
                            this.sprite = item.sprite;
                            foreach (var atherPoint in item.points)
                            {
                                this.AddPoint(atherPoint);
                            }
                            foreach (var atherEdge in item.edges)
                            {
                                this.AddEdge(atherEdge);
                            }
                            DestroyImmediate(item);
                            dirty_tris = true;
                            dirty_offset = true;
                            break;
                        }
                    }
                }



            }


            if (_points.Count > 0 && _points[0].id == 0)
            {
                FixIDs();
            }

            dirty_sprite = true;
            _lastSprite = _sprite;
            _lastScale = _scale;
            UpdateMeshImmediate();
            CreateNewMesh();
            mesh.bounds = bounds;
            setMeshDataToMesh(true, true, true, true, true);
            if (Application.isPlaying)
            {
                if (!GetComponent<Renderer>().isVisible)
                {
                    this.enabled = false;
                }
            }

        }
        protected virtual void OnDestroy()
        {
            DestroyImmediate(mesh);
        }
        public Vector2 getOffsetPointPositionByGlobalPosition(SpritePoint point, Vector3 globalPosition)
        {
            Vector2 newPointPosition = GlobalPositionToSpritePosition(globalPosition);
            return newPointPosition - point.spritePosition;
        }

        /// <summary>
        /// Generate new data for mesh and set this data to mesh. It use dirty_* properties and will set all of these as false.
        /// </summary>
        public void UpdateMeshImmediate()
        {
            if (_lastScale != _scale)
            {
                dirty_offset = true;
                _lastScale = scale;
            }
            if (_lastSprite != _sprite)
            {
                dirty_sprite = true;
                onSpriteChange(_lastSprite, _sprite);
                _lastSprite = _sprite;
            }

            if (dirty_tris || dirty_offset || dirty_sprite || dirty_offset)
            {
                // Debug.Log("prite:" + dirty_sprite.ToString() + "    new trises:" + dirty_tris.ToString() + "    new UV:" + dirty_uv.ToString() + "    new Points:" + dirty_offset.ToString());
            }
            if (dirty_sprite)
            {
                RecalculateSpriteInfo();
                dirty_offset = true;
                dirty_uv = true;
            }
            if (dirty_tris || dirty_uv || dirty_offset || dirty_color || dirty_normals)
            {
                generateMeshData(dirty_tris, dirty_uv, dirty_offset, dirty_color, dirty_normals);
                setMeshDataToMesh(dirty_tris, dirty_uv, dirty_offset, dirty_color, dirty_normals);
                if (dirty_offset || dirty_uv || dirty_tris) dirty_collider = true;
            }
            SetAllDirty(false);
        }
        public virtual Vector2 getSpritePositionOfSpritePoint(SpritePoint point)
        {
            return point.spritePosition + (Vector2)point.offsets[0]; 
        }

        /*
        private Vector2 getConstarinPosition(SpritePoint point)
        {
            if (point.constarins!=null && point.constarins.Count > 0)
            {
                if (point.constarins.Count == 1)
                {
                    if (point.constarins[0].power != 0f)
                    {
                        return point.constarins[0].position;
                    }
                }
                else
                {
                    float allPower = 0;
                    for (int i = 0; i < point.constarins.Count; i++)
                    {
                        allPower += point.constarins[0].power;
                    }
                    if (allPower != 0)
                    {
                        Vector2 p = Vector2.zero;
                        for (int i = 0; i < point.constarins.Count; i++)
                        {
                            var cd = point.constarins[i];
                            p += cd.position * (cd.power / allPower);
                        }
                        return p;
                    }
                }
            }

            return point.spritePosition;
        }*/
        protected void FixedUpdate()
        {
            if (_generateColliderInRunTime && dirty_collider)
            {
                GenerateCollider();
            }
            dirty_collider = false;
        }
        protected PolygonCollider2D polygonCollider
        {
            get
            {
                if (_polygonCollider == null)
                    _polygonCollider = gameObject.GetComponent<PolygonCollider2D>();
                if (_polygonCollider == null)
                    _polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
                return _polygonCollider;
            }
        }
        private PolygonCollider2D _polygonCollider;
        /// <summary>
        /// if it's true collider will be change in run time.
        /// </summary>
        public bool generateColliderInRunTime
        {
            get { return _generateColliderInRunTime; }
            set
            {
                if (value != _generateColliderInRunTime)
                {
                    _generateColliderInRunTime = value;
                    if (value == true)
                    {
                        dirty_collider = true;
                    }
                }
            }
        }
        private Vector2[][] colliderRawPaths;

        [SerializeField]
        private bool _generateColliderInRunTime = false;
        /// <summary>
        /// Generate collider2d
        /// </summary>
        public void GenerateCollider()
        {
            
            NewArray<Vector2[]>(ref colliderRawPaths, borderPaths.Count);

            polygonCollider.pathCount = borderPaths.Count;
            for (int i = 0; i < borderPaths.Count; i++)
            {
                PolygonBorderPath colliderPath = borderPaths[i];
                NewArray<Vector2>(ref colliderRawPaths[i],colliderPath.points.Length);
                for (int j = 0; j < colliderPath.points.Length; j++)
                {
                    SpritePoint currentPoint = _points[colliderPath.points[j]];
                    colliderRawPaths[i][j] = SpritePositionToLocal(getSpritePositionOfSpritePoint(currentPoint) + currentPoint.colliderOffset);
                }
                polygonCollider.SetPath(i, colliderRawPaths[i]);
            }
        }
        protected void OnBecameVisible()
        {
            if (Application.isPlaying) this.enabled = true;
        }
        void OnBecameInvisible()
        {
            if (Application.isPlaying) this.enabled = false;
        }
        private void FixIDs()
        {
            foreach (var item in _points)
            {
                lastID++;
                item.id = lastID;
            }
        }


    }
}

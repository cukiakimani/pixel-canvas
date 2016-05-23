using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Medvedya.SpriteDeformerTools
{
    [ExecuteInEditMode]
    public class SpriteDeformerTargetPoints:SpriteDeformerWithMaterialPropertyBlock, ISerializationCallbackReceiver
    {
        public bool dirty = false;
        [System.Serializable]
        public class PointMover
        {
            public Transform transform;
            public SpritePoint point;
            public int pointIndex;
            public Vector3 lastPosition;
            public PointMover(Transform transform , SpritePoint point)
            {
                this.transform = transform;
                lastPosition = transform.position;
                this.point = point;
                
            }
        }
        public List<PointMover> pointMovers = new List<PointMover>();
        Transform tranformPointGroup
        {
            get {
                if (_tranformPointGroup == null)
                {
                    _tranformPointGroup = (new GameObject()).transform;
                    _tranformPointGroup.gameObject.name = "Animation points group";
                    _tranformPointGroup.parent = transform;
                    _tranformPointGroup.localPosition = Vector3.zero;
                    _tranformPointGroup.localRotation = Quaternion.identity;
                    _tranformPointGroup.localScale = new Vector3(1, 1, 1);
                }
                return _tranformPointGroup;
            }
        }
        [SerializeField]
        private Transform _tranformPointGroup;
        protected override void Awake()
        {
            base.Awake();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
        }
        protected override void Update()
        {
            if (!Application.isPlaying)
            {
                fixErrors();
            }
            dirty = false;
            foreach (PointMover pm in pointMovers)
            {
                if (pm.transform == null) continue;
                if (pm.transform.position != pm.lastPosition)
                {
                    dirty = true;
                    pm.lastPosition = pm.transform.position;
                    //Vector2 newPointPosition = globalPositionToSpritePosition(pm.lastPosition);
                    //pm.point.offset = newPointPosition - pm.point.position;
                    pm.point.offset = getOffsetPointPositionByGlobalPosition(pm.point, pm.lastPosition);
                    //Debug.Log(1);
                }
            }
            if (dirty)
            {
                //generateMeshDate(false, false);
                //updateMesh();
                dirty_offset = true;
            }
            base.Update();
        }
        
        public PointMover getAnimationPoint(SpritePoint p)
        {
            foreach (var mp in pointMovers)
            {
                if (mp.point == p) return mp;
            }
            return null;
        }
        public void fixErrors()
        {
            List<PointMover> delMovers = new List<PointMover>();
            foreach (PointMover pm in pointMovers)
            {
                if (pm.transform == null)
                {
                    delMovers.Add(pm);
                }
               
            }
            foreach (PointMover pm in delMovers)
            {
                pointMovers.Remove(pm);
            }
        }
        public void addMoverPoint(SpritePoint point , Transform target)
        {
            PointMover newPointMover = new PointMover(target,point);
            pointMovers.Add(newPointMover);
              
        }
        public override void RemovePoint(SpritePoint p)
        {
            PointMover pm = getAnimationPoint(p);
            if (pm != null)
            {
                pointMovers.Remove(pm);
            }
            base.RemovePoint(p);
        }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            foreach (PointMover pm in pointMovers)
            {
                pm.point = points[pm.pointIndex];
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            foreach (PointMover pm in pointMovers)
            {
                pm.pointIndex = points.IndexOf(pm.point);
            }
        }
    }
}

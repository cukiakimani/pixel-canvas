using UnityEngine;
using System.Collections;
using System;
namespace Medvedya.SpriteDeformerTools
{
    [ExecuteInEditMode]
    [AddComponentMenu("Sprite Deformer/Sprite deformer blend shape")]
    public class SpriteDeformerBlendShape : SpriteDeformerWithMaterialPropertyBlock
    {
        public int countOfShapes
        {
            get
            {

                return _countOfShapes;
            }
            set
            {
                if (value == _countOfShapes) return;
                Array.Resize<float>(ref _blendValues, value);
                _countOfShapes = value;
            }
        }

        [SerializeField]
        private int _countOfShapes = 1;
        [SerializeField]
        private float[] _blendValues = new float[1];

        public float[] blendValues { get { return _blendValues; } }

        public override void AddPoint(SpritePoint point, bool autoOffset = false)
        {
            point.countOfOfsset = countOfShapes;
            int index = 0;
            if (points.Count > 0)
            {
                index = points[0].index;
            }
            point.index = index;
            
            base.AddPoint(point, autoOffset);
        }
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
            base.Update();
        }
        public Vector2 getPointBlendOffset(SpritePoint point)
        {
            Vector2 newOffsetPos = point.offsets[0];
            for (int j = 0; j < countOfShapes; j++)
            {
                float value = _blendValues[j];
#if UNITY_EDITOR
                if (!Application.isPlaying && editorProps.mainToolBar == MainToolBarInspector.EDIT_VERTICS && points.Count > 0)
                    if (j + 1 == points[0].index)
                        value = 1;
                    else value = 0;
#endif
                newOffsetPos += Vector2.Lerp(point.offsets[0], point.offsets[j + 1], value) - (Vector2)point.offsets[0];
            }
            return newOffsetPos;
        }
        public void SetBlendShapeWeight(int index, float value)
        {
            if (_blendValues[index] != value)
            {
                _blendValues[index] = value;
                dirty_offset = true;
                dirty_collider = true;
            }
        }
        public override Vector2 getSpritePositionOfSpritePoint(SpritePoint point)
        {
            point.countOfOfsset = countOfShapes + 1;
            Vector2 v2 = point.spritePosition + getPointBlendOffset(point); 
            return v2;
        }
    }
}

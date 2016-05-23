using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Medvedya.SpriteDeformerTools
{

    [System.Serializable]
    public class SpriteDeformerEditorSaver
    {
        public enum BoundsEditorMode { Encapsulate = 0, CROPE = 1, USER = 2 ,NEED_SELECT = 3}
        [SerializeField]
        public BoundsEditorMode boundsEditorMode = BoundsEditorMode.NEED_SELECT;
        public Vector2 pivot;
        public float oriant = 0;
        public MainToolBarInspector mainToolBar;
        public List<SpritePoint> selectedPoints = new List<SpritePoint>();
        [SerializeField]
        public int[] serelizableSelectedPoints;
        public bool inversGizmos = false;
        public bool generateColliderInEditor = false;
        public Color paintColor = Color.white;
        public bool autoFreezeScale = false;
        public bool savePivot = false;
        public int selectShape = 0;
       
    }
}

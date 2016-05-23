using System;
using UnityEngine;
using System.Collections.Generic;

namespace Medvedya.SpriteDeformerTools
{
    /*
    public struct ConstrainData
    {
        public float power;
        public Vector2 position;
    }
    */
    /// <summary>
    /// Point data class
    /// </summary>
    [System.Serializable]
    public class SpritePoint
    {
        public int countOfOfsset
        {
            get { 
                return _countOfOfsset; 
            }
            set {
                if (value == _countOfOfsset) return;
                Array.Resize<Vector3>(ref offsets, value);
                if (value > countOfOfsset && value > 1)
                {
                    offsets[value - 1] = offsets[value - 2]; 
                }
                _countOfOfsset = value;
            }
        }
        [SerializeField]
        private int _countOfOfsset = 1;
        public int index
        {
            get {
                return _index;
            }
            set {
               
                _index = value;
            }
        }
        [SerializeField]
        private int _index = 0;
        /// <summary>
        /// Position in sprite where (0,0) is bottom left and (1,1) is top right;
        /// </summary>
        public Vector2 spritePosition;
        /// <summary>
        /// Color of the point
        /// </summary>
        public Color color;
        /// <summary>
        /// Normal of the point
        /// </summary>
        public Vector3 normal;
        /// <summary>
        /// Name of the point
        /// </summary>
        public string name = "point";
        /// <summary>
        /// (X,Y) offset relative to the point. 
        /// if you set (0,1) in mesh the vertex moves right on one width of sprite.
        /// (Z) offset in local transform. 
        /// </summary>
        public Vector3 offset {
            get { return offsets[_index]; }
            set { offsets[_index] = value; }
        }
        /// <summary>
        /// (X,Y) offset relative to the point. 
        /// if you set (0,1) in mesh the vertex moves right on one width of sprite.
        /// </summary>
        public Vector2 offset2d
        {
            get { return offsets[_index]; }
            set {
                Vector3 v3 = offsets[_index];
                v3.x = value.x;
                v3.y = value.y;
                offsets[_index] = v3;
            }
        }    
        public Vector3[] offsets = new Vector3[1];
        /// <summary>
        /// Collider offset.
        /// </summary>
        public Vector2 colliderOffset = Vector2.zero;
        /// <summary>
        /// ID of the point.
        /// </summary>
        public int id = 0;
        /*
        [NonSerialized]
        public List<ConstrainData> constarins = new List<ConstrainData>();
        */
        public SpritePoint(Vector2 position, Color? color = null)
        {
            this.spritePosition = position;
            this.color = color != null ? (Color)color : Color.white;
        }
        public SpritePoint(float x, float y)
        {
            this.spritePosition = new Vector2(x, y);
            color = Color.white;
        }
    }
}

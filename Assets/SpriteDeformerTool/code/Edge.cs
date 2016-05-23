using System;
using UnityEngine;
namespace Medvedya.SpriteDeformerTools
{
    [System.Serializable]
    public class EdgeSerialization
    {
        public int point1Index;
        public int point2index;
        public EdgeSerialization(int p1, int p2)
        {
            point1Index = p1;
            point2index = p2;
        }
    }
    public class EdgeDivider
    {
        public Edge edge;
        public Vector2 position;
        public EdgeDivider()
        { }
        public EdgeDivider(Vector2 position, Edge edge)
        {
            this.position = position;
            this.edge = edge;
        }

    }
    [System.Serializable]
    public class Edge
    {
        public SpritePoint point1;
        public SpritePoint point2;
        public Edge(SpritePoint p1, SpritePoint p2)
        {
            point1 = p1;
            point2 = p2;
        }
        /// <summary>
        /// Get clossest point on edge segment, it doesn't use offset from SpritePoint.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2 GetClosest(Vector2 point)
        {
            Vector2 p2 = point2.spritePosition - point1.spritePosition;
            float something = p2.x * p2.x + p2.y * p2.y;
            float u = Mathf.Clamp01(((point.x - point1.spritePosition.x) * p2.x + (point.y - point1.spritePosition.y) * p2.y) / something);
            return point1.spritePosition + u * p2;
        }
        public bool ContainsPoint(SpritePoint p)
        {
            return p == point1 || p == point2;
        }
    }
}

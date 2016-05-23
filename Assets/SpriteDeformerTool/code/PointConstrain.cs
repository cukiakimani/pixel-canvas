using UnityEngine;
using System.Collections.Generic;

namespace Medvedya.SpriteDeformerTools
{
    [ExecuteInEditMode]
    [AddComponentMenu("Sprite Deformer/Point Constrain")]

    public class PointConstrain : MonoBehaviour
    {

        public string pointsName;
        List<SpritePoint> points = new List<SpritePoint>();
        public Vector2 localOffset;
        public SpriteDeformer spriteDeformer;
        void Start()
        {
            if (spriteDeformer == null) return;
            FillPoints();
        }
        void FillPoints()
        {
            points.Clear();
            for (int i = 0; i < spriteDeformer.points.Count; i++)
            {
                SpritePoint sp = spriteDeformer.points[i];
                if (sp.name == pointsName)
                {
                    points.Add(sp);
                }
            }

        }
        void Update()
        {
            if (spriteDeformer == null) return;
            if (Application.isEditor && !Application.isPlaying)
            {
                FillPoints();
            }
            if (points.Count == 0) return;
            Vector2 rL = Vector2.zero;
            for (int i = 0; i < points.Count; i++)
            {
                SpritePoint p = points[i];
                rL += spriteDeformer.SpritePositionToLocal(p.spritePosition + p.offset2d);
            }
            rL /= (float)points.Count;
            rL += localOffset;
            Vector3 rG = spriteDeformer.transform.TransformPoint(rL);
            rG.z = transform.position.z;
            transform.position = rG;
        }
    }
}

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
namespace Medvedya.SpriteDeformerTools
{
    [CustomEditor(typeof(SpriteDeformerTargetPoints))]
    public class SpriteDeformerTargetPointsEditor : SpriteDeformerWithBaseOfMaterialEditor
    {
        SpriteDeformerTargetPoints spriteDeformerAnimation;
        public override void OnInspectorGUI()
        {
            spriteDeformerAnimation = (SpriteDeformerTargetPoints)target;
            base.InspectorSpriteDeformer();
            base.drawSelectMaterial();
            base.InspectorEditToolBar();
           // List<Point> canSelecteAsAnimate = new List<Point>();
            if (selectedPoints.Count == 1)
            {
                SpritePoint sPoint = selectedPoints[0];
                SpriteDeformerTargetPoints.PointMover targetMover = spriteDeformerAnimation.getAnimationPoint(sPoint);
                Transform targetObject = targetMover != null ? targetMover.transform : null;
                Transform newTarget = (Transform)EditorGUILayout.ObjectField("Target object: ", targetObject, typeof(Transform),true);
                if (newTarget != targetObject)
                {
                    
                    if (targetObject == null)
                    {
                        spriteDeformerAnimation.addMoverPoint(sPoint, newTarget);
                    }
                    if (newTarget == null && targetObject!=null)
                    {
                        spriteDeformerAnimation.pointMovers.Remove(targetMover);
                    }
                    if (targetObject != newTarget && targetObject != null)
                    {
                        targetMover.transform = newTarget;
                        
                    }
                    if (newTarget != null)
                    {
                        sPoint.offset = spriteDeformer.getOffsetPointPositionByGlobalPosition(sPoint, newTarget.position);
                        upDateMeshDate();
                    }
                }
            }



        }
         protected override void OnSceneGUI()
        {
            base.OnSceneGUI();
        }
        public override void doItAfterMovePoints(SpritePoint[] points)
        {
            foreach (var item in spriteDeformerAnimation.pointMovers)
            {
                item.transform.position = spriteDeformer.SpritePositionToGlobal(item.point.spritePosition + item.point.offset2d);
                Undo.RecordObject(item.transform.gameObject, "Move points");
                EditorUtility.SetDirty(item.transform);
            }
            base.doItAfterMovePoints(points);
        }

    }
}

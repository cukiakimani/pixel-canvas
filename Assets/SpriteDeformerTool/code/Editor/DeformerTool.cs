using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Medvedya.GeometryMath;
using UnityEditor;
namespace Medvedya.SpriteDeformerTools {
    public class DeformerTool
    {
        Vector2 pivot
        {
            get
            {
                return spriteDeformerEditor.spriteDeformer.editorProps.pivot;
            }
            set
            {
                spriteDeformerEditor.spriteDeformer.editorProps.pivot = new Vector2(value.x, value.y);
            }
        }
        SpriteDeformerEditor spriteDeformerEditor;
        float oriant
        {
            get
            {
                return spriteDeformerEditor.spriteDeformer.editorProps.oriant;

            }
            set
            {
                spriteDeformerEditor.spriteDeformer.editorProps.oriant = value;
            }
        }
        Vector2 oriantDir
        {
            get
            {
                return new Vector2(Mathf.Cos(oriant), Mathf.Sin(oriant));
            }
        }
        Vector2 oriantDirUp
        {
            get
            {
                return new Vector2(Mathf.Cos(oriant + Mathf.PI / 2f), Mathf.Sin(oriant + Mathf.PI / 2f));
            }
        }
        Vector3 pos2Dto3D(Vector2 pos2d)
        {
            return spriteDeformerEditor.spriteDeformer.transform.rotation * pos2d + spriteDeformerEditor.spriteDeformer.transform.position;
        }
        Vector2 pivotDeltaClick;
        public DeformerTool(SpriteDeformerEditor se)
        {
            spriteDeformerEditor = se;
            oriant = 0;
            pivotDeltaClick = Vector2.zero;
        }
        public void recalCulatePivot()
        {
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);

            foreach (SpritePoint p in spriteDeformerEditor.selectedPoints)
            {
                Vector2 pLocalPos = Vector2.Scale(
                    spriteDeformerEditor.spriteDeformer.SpritePositionToLocal(p.spritePosition + p.offset2d),
                     spriteDeformerEditor.spriteDeformer.transform.lossyScale)
                    ;
                if (pLocalPos.x < min.x) min.x = pLocalPos.x;
                if (pLocalPos.y < min.y) min.y = pLocalPos.y;
                if (pLocalPos.x > max.x) max.x = pLocalPos.x;
                if (pLocalPos.y > max.y) max.y = pLocalPos.y;
            }
            pivot = Vector2.Lerp(min, max, 0.5f);
            oriant = 0;

        }


        Vector2 pos2DtoScreen(Vector2 pos2d)
        {

            return HandleUtility.WorldToGUIPoint(pos2Dto3D(pos2d));
        }

        Vector2 mouse2Dpos
        {
            get
            {
                Plane3d p3d = new Plane3d(spriteDeformerEditor.spriteDeformer.transform.position, spriteDeformerEditor.spriteDeformer.transform.rotation);
                Vector3? worldPos = p3d.rayCast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition));
                if (worldPos == null) return Vector2.zero;
                Vector3 localPos = p3d.globalToLocal((Vector3)worldPos);
                return localPos;

                //if
            }
        }
        //Plane3d dPlane = new Plane3d();                 
        public void OnSceneGUI()
        {
            if (spriteDeformerEditor.selectedPoints.Count <= 0)
            {
                return;
            }
           // dPlane = new Plane3d(spriteDeformerEditor.spriteDeformer.transform.position, spriteDeformerEditor.spriteDeformer.transform.rotation);
            CurrentTool nextTool = CurrentTool.NONE;
            float newHandleSize = HandleUtility.GetHandleSize(pos2Dto3D(pivot));
            Vector2 newPivotHandle = pivot - oriantDirUp * newHandleSize / 3f;
            Vector2 newOriantHandle = pivot + oriantDir * newHandleSize * 1.5f;
            float newScaleStandartDistance = newHandleSize * 1.3f;
            Vector2 newScaleVerticalHandle = pivot + oriantDirUp * newScaleStandartDistance;
            Vector2 newScaleHorizontalHandle = pivot + oriantDir * newScaleStandartDistance;
            Vector2 newRotateHandle = pivot + oriantDirUp * newHandleSize * 1.5f;

            if (spriteDeformerEditor.currentTool == CurrentTool.NONE)
            {
                if (Vector3.Distance(Event.current.mousePosition, pos2DtoScreen(pivot)) < 7f)
                {
                    nextTool = CurrentTool.MOVE_POINTS;
                }
                else
                {
                    Vector2 l1Screen = pos2DtoScreen(pivot);
                    Vector2 l2Screen = pos2DtoScreen(pivot + oriantDir * newHandleSize);
                    Vector2 l2UpScreen = pos2DtoScreen(pivot + oriantDirUp * newHandleSize);
                    float d = Vector2.Distance(Line.ClosestPointOnSegment(l1Screen, l2Screen, Event.current.mousePosition), Event.current.mousePosition);
                    float dUP = Vector2.Distance(Line.ClosestPointOnSegment(l1Screen, l2UpScreen, Event.current.mousePosition), Event.current.mousePosition);
                    if (Mathf.Min(d, dUP) < 6f)
                    {
                        nextTool = d < dUP ? CurrentTool.MOVE_RIGHT : CurrentTool.MOVE_UP;
                    }
                    else if (Vector2.Distance(pos2DtoScreen(newPivotHandle), Event.current.mousePosition) < 6)
                    {
                        nextTool = CurrentTool.MOVE_PIVOT;
                    }
                    else if (Vector2.Distance(pos2DtoScreen(newScaleHorizontalHandle), Event.current.mousePosition) < 6)
                    {
                        nextTool = CurrentTool.SCALE_HORIZONTAL;
                        calculateLocalPositions();
                    }
                    else if (Vector2.Distance(pos2DtoScreen(newScaleVerticalHandle), Event.current.mousePosition) < 6)
                    {
                        nextTool = CurrentTool.SCALE_VERTICAL;
                        calculateLocalPositions();
                    }
                    else if (Vector2.Distance(pos2DtoScreen(newOriantHandle), Event.current.mousePosition) < 6)
                    {
                        nextTool = CurrentTool.ORIANT;
                    }
                    else if (Vector2.Distance(pos2DtoScreen(newRotateHandle), Event.current.mousePosition) < 6)
                    {
                        nextTool = CurrentTool.ROTATE;
                        calculateLocalPositions();
                    }
                }

                pivotDeltaClick = mouse2Dpos - pivot;
            }


            if (Event.current.type == EventType.mouseDown && nextTool != CurrentTool.NONE)
            {
                spriteDeformerEditor.currentTool = nextTool;
            }
            if (spriteDeformerEditor.currentTool == CurrentTool.MOVE_POINTS)
            {
                Vector2 newPos = mouse2Dpos - pivotDeltaClick;
                movePoints(pivot, newPos);
                pivot = newPos;
            }
            if (spriteDeformerEditor.currentTool == CurrentTool.MOVE_RIGHT || spriteDeformerEditor.currentTool == CurrentTool.MOVE_UP)
            {
                Vector2 cDir = spriteDeformerEditor.currentTool == CurrentTool.MOVE_RIGHT ? oriantDir : oriantDirUp;
                Vector2 newPivotPos = Line3d.closestPointInLine(mouse2Dpos - pivotDeltaClick, pivot, pivot + cDir);
                movePoints(pivot, newPivotPos);
                pivot = newPivotPos;
            }
            if (spriteDeformerEditor.currentTool == CurrentTool.MOVE_PIVOT)
            {
                pivot = mouse2Dpos - pivotDeltaClick;
            }
            if (spriteDeformerEditor.currentTool == CurrentTool.SCALE_HORIZONTAL
                ||
                spriteDeformerEditor.currentTool == CurrentTool.SCALE_VERTICAL)
            {

                Vector2 lineStart = pivot;
                Vector2 line = spriteDeformerEditor.currentTool == CurrentTool.SCALE_HORIZONTAL ? oriantDir : oriantDirUp;
                float t = Vector2.Dot(mouse2Dpos - lineStart, line);
                if (spriteDeformerEditor.currentTool == CurrentTool.SCALE_HORIZONTAL)
                {
                    newScaleHorizontalHandle = lineStart + line * t;
                    setPointsFromPointsInPivotLocal(new Vector2(t / newScaleStandartDistance, 1f));
                }
                else
                {
                    newScaleVerticalHandle = lineStart + line * t;
                    setPointsFromPointsInPivotLocal(new Vector2(1f, t / newScaleStandartDistance));
                }
            }
            if (spriteDeformerEditor.currentTool == CurrentTool.ORIANT)
            {
                Vector2 mlp = mouse2Dpos;
                oriant = Mathf.Atan2(mlp.y - pivot.y, mlp.x - pivot.x);
            }
            if (spriteDeformerEditor.currentTool == CurrentTool.ROTATE)
            {
                calculateLocalPositions();
                Vector2 mlp = mouse2Dpos;
                float a = Mathf.Atan2(mlp.y - pivot.y, mlp.x - pivot.x) - Mathf.PI / 2f;
                setPointsFromPointsInPivotLocal(new Vector2(1f, 1f), a - oriant);
                oriant = a;
            }
           

            Color overColor = spriteDeformerEditor.colorSets[ColorSet.DEFORMER_TOOL].over;
            Color standartColor = spriteDeformerEditor.colorSets[ColorSet.DEFORMER_TOOL].standart;

            Handles.color = nextTool == CurrentTool.MOVE_RIGHT ? overColor : standartColor;
            List<CurrentTool> candraw = new List<CurrentTool>() { CurrentTool.NONE, CurrentTool.ORIANT, CurrentTool.MOVE_PIVOT, CurrentTool.MOVE_RIGHT, CurrentTool.ROTATE };
            if (candraw.Contains(spriteDeformerEditor.currentTool))
                Handles.ArrowCap(15, pos2Dto3D(pivot), spriteDeformerEditor.spriteDeformer.transform.rotation * Quaternion.Euler(0 - oriant * Mathf.Rad2Deg, 90, 0), newHandleSize);

            Handles.color = nextTool == CurrentTool.MOVE_UP ? overColor : standartColor;
            candraw = new List<CurrentTool>() { CurrentTool.NONE, CurrentTool.ORIANT, CurrentTool.MOVE_PIVOT, CurrentTool.MOVE_UP, CurrentTool.ROTATE };
            if (candraw.Contains(spriteDeformerEditor.currentTool))
                Handles.ArrowCap(15, pos2Dto3D(pivot), spriteDeformerEditor.spriteDeformer.transform.rotation * Quaternion.Euler(-90 - oriant * Mathf.Rad2Deg, 90, 0), newHandleSize);

            Handles.color = nextTool == CurrentTool.MOVE_POINTS ? overColor : standartColor;
            Handles.CylinderCap(15, pos2Dto3D(pivot), spriteDeformerEditor.spriteDeformer.transform.rotation, newHandleSize / 5f);

            candraw = new List<CurrentTool>() { CurrentTool.NONE, CurrentTool.ORIANT, CurrentTool.MOVE_PIVOT, CurrentTool.MOVE_PIVOT };
            Handles.color = nextTool == CurrentTool.MOVE_PIVOT ? overColor : standartColor;
            if (candraw.Contains(spriteDeformerEditor.currentTool))
                Handles.SphereCap(0, pos2Dto3D(newPivotHandle), Quaternion.identity, newHandleSize / 10f);

            Handles.color = nextTool == CurrentTool.SCALE_HORIZONTAL ? overColor : standartColor;
            candraw = new List<CurrentTool>() { CurrentTool.NONE, CurrentTool.ORIANT, CurrentTool.MOVE_PIVOT, CurrentTool.SCALE_HORIZONTAL };
            if (candraw.Contains(spriteDeformerEditor.currentTool))
                Handles.CubeCap(0, pos2Dto3D(newScaleHorizontalHandle), spriteDeformerEditor.spriteDeformer.transform.rotation * Quaternion.Euler(0, 0, oriant * Mathf.Rad2Deg), newHandleSize / 7f);

            Handles.color = nextTool == CurrentTool.SCALE_VERTICAL ? overColor : standartColor;
            candraw = new List<CurrentTool>() { CurrentTool.NONE, CurrentTool.ORIANT, CurrentTool.MOVE_PIVOT, CurrentTool.SCALE_VERTICAL };
            if (candraw.Contains(spriteDeformerEditor.currentTool))
                Handles.CubeCap(0, pos2Dto3D(newScaleVerticalHandle), spriteDeformerEditor.spriteDeformer.transform.rotation * Quaternion.Euler(0, 0, oriant * Mathf.Rad2Deg), newHandleSize / 7f);

            Handles.color = nextTool == CurrentTool.ORIANT ? overColor : standartColor;
            candraw = new List<CurrentTool>() { CurrentTool.NONE, CurrentTool.ORIANT, CurrentTool.MOVE_PIVOT, CurrentTool.MOVE_PIVOT };
            if (candraw.Contains(spriteDeformerEditor.currentTool))
                Handles.ConeCap(0, pos2Dto3D(newOriantHandle), Quaternion.LookRotation(pos2Dto3D(newOriantHandle) - pos2Dto3D(pivot)), newHandleSize / 6f);

            Handles.color = nextTool == CurrentTool.ROTATE ? overColor : standartColor;
            candraw = new List<CurrentTool>() { CurrentTool.NONE, CurrentTool.ORIANT, CurrentTool.MOVE_PIVOT, CurrentTool.MOVE_PIVOT, CurrentTool.ROTATE };
            if (candraw.Contains(spriteDeformerEditor.currentTool))
                Handles.ConeCap(0, pos2Dto3D(newRotateHandle), Quaternion.LookRotation(pos2Dto3D(newOriantHandle) - pos2Dto3D(pivot)), newHandleSize / 6f);
        }
        Vector2[] pointsInPivotLocal;
        void calculateLocalPositions()
        {
            pointsInPivotLocal = new Vector2[spriteDeformerEditor.selectedPoints.Count];
            for (int i = 0; i < pointsInPivotLocal.Length; i++)
            {
                SpritePoint p = spriteDeformerEditor.selectedPoints[i];
                Vector2 lp = Vector2.Scale(spriteDeformerEditor.spriteDeformer.SpritePositionToLocal(p.spritePosition + p.offset2d),
                spriteDeformerEditor.spriteDeformer.transform.lossyScale) - pivot;
                float a = Mathf.Repeat(-oriant, Mathf.PI * 2);
                Vector2 newLP;
                newLP.x = Mathf.Cos(a) * lp.x - Mathf.Sin(a) * lp.y;
                newLP.y = Mathf.Sin(a) * lp.x + Mathf.Cos(a) * lp.y;
                pointsInPivotLocal[i] = newLP;
            }
        }
        void setPointsFromPointsInPivotLocal(Vector2 scale, float angle = 0)
        {
            for (int i = 0; i < pointsInPivotLocal.Length; i++)
            {
                SpritePoint p = spriteDeformerEditor.selectedPoints[i];
                Vector2 lp = Vector2.Scale(pointsInPivotLocal[i], scale);
                Vector2 newLP;
                newLP.x = Mathf.Cos(oriant + angle) * lp.x - Mathf.Sin(oriant + angle) * lp.y;
                newLP.y = Mathf.Sin(oriant + angle) * lp.x + Mathf.Cos(oriant + angle) * lp.y;
                newLP += pivot;
                Vector2 newLpLocal = new Vector2(
                        newLP.x / spriteDeformerEditor.spriteDeformer.transform.lossyScale.x,
                        newLP.y / spriteDeformerEditor.spriteDeformer.transform.lossyScale.y);
                p.offset2d = spriteDeformerEditor.spriteDeformer.LocalPositionToSpritePosition(newLpLocal) - p.spritePosition;
                spriteDeformerEditor.doItAfterMovePoints(spriteDeformerEditor.selectedPoints.ToArray());
            }
        }
        void movePoints(Vector2 oldPivot, Vector2 newPivot)
        {
            Vector2 s = spriteDeformerEditor.spriteDeformer.transform.lossyScale;
            Vector2 d =
                spriteDeformerEditor.spriteDeformer.LocalPositionToSpritePosition(oldPivot)
                -
                spriteDeformerEditor.spriteDeformer.LocalPositionToSpritePosition(newPivot);
            d = new Vector2(d.x / s.x, d.y / s.y);
            foreach (SpritePoint p in spriteDeformerEditor.selectedPoints)
            {
                p.offset -= (Vector3)d;
            }
            spriteDeformerEditor.doItAfterMovePoints(spriteDeformerEditor.selectedPoints.ToArray());
        }
    }
}

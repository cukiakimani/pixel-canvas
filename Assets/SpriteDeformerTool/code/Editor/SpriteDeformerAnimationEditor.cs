using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
namespace Medvedya.SpriteDeformerTools
{
    [CustomEditor(typeof(SpriteDeformerAnimation))]
    //[CanEditMultipleObjects]
    public class SpriteDeformerAnimationEditor : SpriteDeformerWithBaseOfMaterialEditor
    {
        SpriteDeformerAnimation spriteDeformerAnimation;
        public override void OnInspectorGUI()
        {

            spriteDeformerAnimation = (SpriteDeformerAnimation)target;
            base.InspectorSpriteDeformer();
            base.InspectorEditToolBar();




        }

        struct ColorOffset
        {
            public int a;
            public int r;
            public int g;
            public int b;
        }

        private ColorOffset[] colorOffset;
        enum KeyMode { COLOR_KEY, XY_KEY, Z_KEY };
        KeyMode keyMode = KeyMode.XY_KEY;
        bool isSpace = false;
        protected override void OnSceneGUI()
        {
            bool dontBase = false;
            if (spriteDeformer.editorProps.mainToolBar == MainToolBarInspector.EDIT_VERTICS)
            {
                {

                    Rect rArea = new Rect(0, 0, 500, 100);
                    Handles.BeginGUI();
                    GUILayout.BeginArea(rArea);
                    //if (GUILayout.Button("Reset Area"))
                    //    Debug.Log("test");
                    GUILayout.Box("Please choose key mode and then press space to add keys for selected points");
                    GUILayout.Box((keyMode == KeyMode.COLOR_KEY ? "->" : "") + "mode: Color key");
                    GUILayout.Box((keyMode == KeyMode.XY_KEY ? "->" : "") + "mode: X Y key");
                    GUILayout.Box((keyMode == KeyMode.Z_KEY ? "->" : "") + "mode: Z key");
                    GUILayout.EndArea();
                    Handles.EndGUI();
                }
                if (Event.current.type == EventType.mouseDown)
                {
                    if ((new Rect(0, 25, 90, 25).Contains(Event.current.mousePosition)))
                    {
                        keyMode = KeyMode.COLOR_KEY;
                        dontBase = true;

                    }
                    else if ((new Rect(0, 50, 90, 25).Contains(Event.current.mousePosition)))
                    {
                        keyMode = KeyMode.XY_KEY;
                        dontBase = true;
                    }
                    else if ((new Rect(0, 75, 90, 25).Contains(Event.current.mousePosition)))
                    {
                        keyMode = KeyMode.Z_KEY;
                        dontBase = true;
                    }
                }
            }
            if (!dontBase) base.OnSceneGUI();
            if (Event.current.keyCode == KeyCode.Space && Event.current.type == EventType.keyDown && !isSpace)
            {
                if (keyMode == KeyMode.COLOR_KEY)
                {
                    setColorKeyStart();
                }
                if (keyMode == KeyMode.XY_KEY)
                {
                    setPointKeyStart(true, true, false);
                }
                if (keyMode == KeyMode.Z_KEY)
                {
                    setPointKeyStart(false, false, true);
                }
                isSpace = true;
            }
            if (Event.current.keyCode == KeyCode.Space && Event.current.type == EventType.keyUp)
            {
                isSpace = false;
                if (keyMode == KeyMode.COLOR_KEY)
                {
                    setColorKeyEnd();
                }
                if (keyMode == KeyMode.XY_KEY)
                {
                    setPointKeyEnd(true, true, false);
                }
                if (keyMode == KeyMode.Z_KEY)
                {
                    setPointKeyEnd(false, false, true);
                }

            }

        }
        void setColorKeyStart()
        {
            colorOffset = new ColorOffset[selectedPoints.Count];
            for (int i = 0; i < colorOffset.Length; i++)
            {
                SpritePoint item = selectedPoints[i];
                Color32 c = item.color;
                if (c.a == 0)
                {
                    c.a += 1;
                    colorOffset[i].a = -1;
                }
                else
                {
                    c.a -= 1;
                    colorOffset[i].a = +1;
                }
                if (c.r == 0)
                {
                    c.r += 1;
                    colorOffset[i].r = -1;
                }
                else
                {
                    c.r -= 1;
                    colorOffset[i].r = +1;
                }

                if (c.g == 0)
                {
                    c.g += 1;
                    colorOffset[i].g = -1;
                }
                else
                {
                    c.g -= 1;
                    colorOffset[i].g = +1;
                }
                if (c.b == 0)
                {
                    c.b += 1;
                    colorOffset[i].b = -1;
                }
                else
                {
                    c.b -= 1;
                    colorOffset[i].b = +1;
                }
                item.color = c;
            }
            doItAfterMovePoints(selectedPoints.ToArray());
        }
        void setColorKeyEnd()
        {
            for (int i = 0; i < colorOffset.Length; i++)
            {
                Color32 c = selectedPoints[i].color;
                c.a = (byte)((int)c.a + colorOffset[i].a);
                c.r = (byte)((int)c.r + colorOffset[i].r);
                c.g = (byte)((int)c.g + colorOffset[i].g);
                c.b = (byte)((int)c.b + colorOffset[i].b);
                selectedPoints[i].color = c;
            }
            doItAfterMovePoints(selectedPoints.ToArray());
            Debug.Log("key End");

        }
        void setPointKeyStart(bool x, bool y, bool z)
        {

            foreach (var item in selectedPoints)
            {
                item.offset -= new Vector3(x ? 0.1f : 0, y ? 0.1f : 0, z ? 0.1f : 0);
            }
            doItAfterMovePoints(selectedPoints.ToArray());
            Debug.Log("Point start");
        }
        void setPointKeyEnd(bool x, bool y, bool z)
        {
            foreach (var item in selectedPoints)
            {

                item.offset += new Vector3(x ? 0.1f : 0, y ? 0.1f : 0, z ? 0.1f : 0);
            }
            doItAfterMovePoints(selectedPoints.ToArray());

            //AnimationUtility.
            Debug.Log("Point end");
        }
        public override void doItAfterMovePoints(SpritePoint[] points)
        {
            foreach (SpritePoint p in points)
            {
                List<SpritePoint> lps = new List<SpritePoint>();
                lps.AddRange(spriteDeformerAnimation.animationPoints);
                int index = lps.IndexOf(p);
                SpriteDeformerAnimation.PointInfo pi = new SpriteDeformerAnimation.PointInfo();
                pi.color = p.color;
                pi.position = p.spritePosition;
                pi.offset = p.offset;
                spriteDeformerAnimation.setValueByIndex(index, pi);
            }
            base.doItAfterMovePoints(points);
        }
        protected override void OnEnable()
        {


            base.OnEnable();
            if (spriteDeformer.editorProps.boundsEditorMode == SpriteDeformerEditorSaver.BoundsEditorMode.NEED_SELECT)
            {
                spriteDeformer.editorProps.boundsEditorMode = SpriteDeformerEditorSaver.BoundsEditorMode.Encapsulate;
            }
        }
    }
}

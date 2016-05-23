using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Medvedya.SpriteDeformerTools
{
    [CustomEditor(typeof(SpriteDeformerBlendShape))]
    public class SpriteDeformerBlendShapeEditor : SpriteDeformerWithBaseOfMaterialEditor
    {

        SpriteDeformerBlendShape spriteDeformerBlendShape;
        public override void OnInspectorGUI()
        {
            spriteDeformerBlendShape = (SpriteDeformerBlendShape)target;
            Undo.RecordObject(target, "Inspector");
            base.InspectorSpriteDeformer();
           
            base.InspectorEditToolBar();
            if (GUI.changed)
                EditorUtility.SetDirty(target);



            spriteDeformerBlendShape.dirty_offset = true; 

        }
        private int enterCount = -1;
        public override void inspectorWhenSelectPoints()
        {
            if (enterCount == -1)
            {
                enterCount = spriteDeformerBlendShape.countOfShapes;
            }
            enterCount = EditorGUILayout.IntField("Count of shapes:", enterCount);
            if (enterCount < 0) enterCount = 0;
            if (enterCount != spriteDeformerBlendShape.countOfShapes)
            {
                if(GUILayout.Button("Set count"))
                spriteDeformerBlendShape.countOfShapes = enterCount;
            
            }

            string[] strings = new string[spriteDeformerBlendShape.countOfShapes + 1];
            strings[0] = "Base";
            for (int i = 1; i < spriteDeformerBlendShape.countOfShapes + 1; i++)
            {
                strings[i] = (i).ToString(); 
            }
            int selIndex = GUILayout.Toolbar
                (
                spriteDeformerBlendShape.points[0].index, 
                strings
                );
            if (selIndex < 0) selIndex = 0;
            if (selIndex > spriteDeformerBlendShape.countOfShapes) selIndex = spriteDeformerBlendShape.countOfShapes;
            foreach (var point in spriteDeformerBlendShape.points)
            {
                point.index = selIndex; 
            }
            if (selIndex != 0 && spriteDeformerBlendShape.editorProps.selectedPoints.Count > 0)
            {
                if (GUILayout.Button("To base position"))
                    foreach (var item in spriteDeformerBlendShape.editorProps.selectedPoints)
                    {
                        item.offsets[selIndex] = item.offsets[0]; 
                    }
            }
            
            base.inspectorWhenSelectPoints();

        }
        protected override void inspectorMain()
        {
            EditorGUILayout.LabelField("Weight:");
            bool isChangeWeight = false;
            for (int i = 0; i < spriteDeformerBlendShape.countOfShapes; i++)
            {
                float newValue = EditorGUILayout.Slider(spriteDeformerBlendShape.blendValues[i], 0, 1);
                if (spriteDeformerBlendShape.blendValues[i] != newValue)
                {
                    spriteDeformerBlendShape.SetBlendShapeWeight(i, newValue);
                    isChangeWeight = true;
                }
                
            }
            if (isChangeWeight && (spriteDeformerBlendShape.generateColliderInRunTime || spriteDeformerBlendShape.editorProps.generateColliderInEditor))
            {
                spriteDeformerBlendShape.GenerateCollider();
            }
            base.inspectorMain();
        }
        protected override void OnEnable()
        {

            base.OnEnable();
            if (spriteDeformer.editorProps.boundsEditorMode == SpriteDeformerEditorSaver.BoundsEditorMode.NEED_SELECT)
            {
                spriteDeformer.editorProps.boundsEditorMode = SpriteDeformerEditorSaver.BoundsEditorMode.CROPE;
            }

        }
        protected override void OnSceneGUI()
        {

            base.OnSceneGUI();
           
        }

    }
}

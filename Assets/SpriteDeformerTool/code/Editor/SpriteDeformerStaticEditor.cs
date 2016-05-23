using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Medvedya.SpriteDeformerTools
{
    [CustomEditor(typeof(SpriteDeformerStatic))]
    public class SpriteDeformerStaticEditor : SpriteDeformerWithBaseOfMaterialEditor
    {
        
        public override void OnInspectorGUI()
        {

            Undo.RecordObject(target, "Inspector");
            base.InspectorSpriteDeformer();
            spriteDeformer.triangulateWithOffsetPosition 
                = 
                EditorGUILayout.Toggle("full triangulating", spriteDeformer.triangulateWithOffsetPosition);
            base.InspectorEditToolBar();
            if (GUI.changed)
				EditorUtility.SetDirty(target);

        }
        protected override void OnEnable()
        {
            
            base.OnEnable();
            if (spriteDeformer.editorProps.boundsEditorMode == SpriteDeformerEditorSaver.BoundsEditorMode.NEED_SELECT)
            {
                spriteDeformer.editorProps.boundsEditorMode = SpriteDeformerEditorSaver.BoundsEditorMode.CROPE;
            }
 
        }
        protected override void inspectorMain()
        {
            base.inspectorMain();

        }
        protected override void OnSceneGUI()
        {
            
            base.OnSceneGUI();
            if (Event.current.type == EventType.MouseUp)
            {
                //EditorUtility.SetDirty(target);
            }
        }
            
    }
}

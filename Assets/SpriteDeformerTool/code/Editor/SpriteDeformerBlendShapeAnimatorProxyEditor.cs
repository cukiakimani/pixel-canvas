using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Medvedya.SpriteDeformerTools
{
    //[CustomEditor(typeof(SpriteDeformerBlendShapeAnimatorProxy))]
    public class SpriteDeformerBlendShapeAnimatorProxyEditor : Editor
    {
        SpriteDeformerBlendShapeAnimatorProxy proxy; 
        public override void OnInspectorGUI()
        {
            proxy = (SpriteDeformerBlendShapeAnimatorProxy)this.target;
            if (proxy.spriteDeformerBlendShape == null)
            {
                EditorGUILayout.LabelField("Sprite deformer blendShape is not found");
                return;
            }
            float count = proxy.spriteDeformerBlendShape.countOfShapes;
            if (count > 20) count = 20;
            for (int i = 0; i < count; i++)
            {
                float value =  EditorGUILayout.Slider(proxy.getValueByIndex(i),0,1);
                proxy.setValueByIndex(i, value);
                
            }
            if (GUI.changed)
            {
                EditorUtility.SetDirty(proxy);
                EditorUtility.SetDirty(proxy.spriteDeformerBlendShape);
            }

            //base.OnInspectorGUI();
        }
    }
}

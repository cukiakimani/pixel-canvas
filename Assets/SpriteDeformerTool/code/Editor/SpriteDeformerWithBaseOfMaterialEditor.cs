using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Medvedya.SpriteDeformerTools
{

    public class SpriteDeformerWithBaseOfMaterialEditor :SpriteDeformerEditor
    {
        SpriteDeformerWithMaterialPropertyBlock spriteDeformerWithM;
        public void drawSelectMaterial()
        {
            //if (Application.isPlaying) return;
            spriteDeformerWithM = (SpriteDeformerWithMaterialPropertyBlock)target;

                spriteDeformerWithM.material 
                = 
                 (Material)EditorGUILayout.ObjectField("Material:", spriteDeformerWithM.material, typeof(Material),false);

            
        }
        protected override void inspectorMain()
        {
            drawSelectMaterial();
            base.inspectorMain();
        }
        protected override void OnEnable()
        {

            base.OnEnable();

        }
    }

}

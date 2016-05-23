/*
using UnityEngine;
using System.Collections;
namespace Medvedya.SpriteDeformerTools
{
    public abstract class SpriteDeformerWithBaseOfMaterial1:SpriteDeformer
    {

        [System.NonSerialized]
        private Material currentMaterial;
        public Material referenceMaterial 
        {
            
            get {
                return _referenceMaterial;
            }
            set
            {
                if (value == referenceMaterial) return;
                currentMaterial = null;
                if (sprite != null)
                {
                    if(_referenceMaterial!=null)
                    {
                        BaseOfMaterials.IDestory(_referenceMaterial, sprite.texture);
                    }
                    if (value != null)
                    {
                        currentMaterial = BaseOfMaterials.GetMaterial(value, sprite.texture);
                    }
                    
                }
                _referenceMaterial = value;
                ApplyCurrentMaterial();
            }
            
        }
        [SerializeField]
        private Material _referenceMaterial;

        protected override void Update()
        {
            if (!Application.isPlaying)
            {
                if (needLoadMaterialEditorFix)
                {
                    if (sprite != null && referenceMaterial != null)
                    {
                        Debug.Log("code  ");
                        currentMaterial = BaseOfMaterials.GetMaterial(referenceMaterial, sprite.texture);
                        ApplyCurrentMaterial();
                       
                    }
                    needLoadMaterialEditorFix =  false;
                }
            }
            base.Update();
        }
        void ApplyCurrentMaterial()
        {
            meshRender.material = currentMaterial;
        }
        private bool needLoadMaterialEditorFix = false;
        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();

            needLoadMaterialEditorFix = true;

        }
        
        protected override void Awake()
        {
            base.Awake();
            if (currentMaterial == null && sprite != null && referenceMaterial != null)
            {
                Debug.Log("A  ");
                currentMaterial = BaseOfMaterials.GetMaterial(referenceMaterial, sprite.texture);
            }
            ApplyCurrentMaterial();

            needLoadMaterialEditorFix = false; 


        }
        
        protected override void onSpriteChange(Sprite lastSprite, Sprite currentSprite)
        {

            if (lastSprite!=null && currentSprite!=null && lastSprite.texture == currentSprite.texture) return;
            if (lastSprite != null && referenceMaterial != null)
            {

                currentMaterial = null;
                ApplyCurrentMaterial();
                BaseOfMaterials.IDestory(referenceMaterial, lastSprite.texture);
                
            }
            if (currentSprite != null && referenceMaterial != null)
            {
                Debug.Log("c  ");
                currentMaterial = BaseOfMaterials.GetMaterial(referenceMaterial, currentSprite.texture);
                ApplyCurrentMaterial();
                 

            }
            base.onSpriteChange(lastSprite, currentSprite);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (sprite != null && referenceMaterial != null)
            {
                Debug.Log("d " + Application.isPlaying);
                
                BaseOfMaterials.IDestory(referenceMaterial, sprite.texture);
            }
            
        }
    }
}
*/
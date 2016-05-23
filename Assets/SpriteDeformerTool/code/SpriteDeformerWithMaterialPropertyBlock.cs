
using UnityEngine;
using System.Collections;
namespace Medvedya.SpriteDeformerTools
{
    public abstract class SpriteDeformerWithMaterialPropertyBlock : SpriteDeformer
    {
        private static Material defaultMaterial
        {
            get
            {
                if (_defaultMaterial == null)
                {
                    _defaultMaterial = Resources.Load<Material>("SpriteDeformerStandartMaterial");
                }
                return _defaultMaterial;
            }
        }
        private static Material _defaultMaterial = null;

        private const string mainTextureKeyword = "_MainTex";
        private MaterialPropertyBlock propertyBlock
        {
            get
            {
                _propertyBlock = new MaterialPropertyBlock();
                meshRender.SetPropertyBlock(_propertyBlock);
                return _propertyBlock;
            }
        }
        MaterialPropertyBlock _propertyBlock;

        public Material material
        {
            get
            {
                return meshRender.sharedMaterial;
            }
            set
            {
                meshRender.sharedMaterial = value;
            }
        }
        [SerializeField]
        private Material _referenceMaterial;

        protected override void Update()
        {
            base.Update();
        }
        protected override void Awake()
        {
           
            base.Awake();
            if (_referenceMaterial != null)
            {
                material = _referenceMaterial;
                _referenceMaterial = null;
            }
            if (material == null)
            {
                material = defaultMaterial;
            }
            SetSprite(sprite);
        }
        private void SetSprite(Sprite _sprite)
        {
            if (_sprite == null)
            {
                propertyBlock.SetTexture(mainTextureKeyword, Texture2D.whiteTexture);
            }
            else
            {
                propertyBlock.SetTexture(mainTextureKeyword, _sprite.texture);
            }
            meshRender.SetPropertyBlock(_propertyBlock);
        }
        protected override void onSpriteChange(Sprite lastSprite, Sprite currentSprite)
        {
            base.onSpriteChange(lastSprite, currentSprite);
            if (lastSprite != null && currentSprite != null && lastSprite.texture == currentSprite.texture) return;
            SetSprite(currentSprite);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
       

    }
}

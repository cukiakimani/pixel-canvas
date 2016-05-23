/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Medvedya.SpriteDeformerTools
{
    public class BaseOfMaterials
    {
        private static Dictionary<Material, MaterialBaseElement> materialList = new Dictionary<Material, MaterialBaseElement>();

        public class MaterialBaseElement
        {
            public Material refMaterial;
            public Dictionary<Texture, TextureBaseElement> materialsByTexture = new Dictionary<Texture, TextureBaseElement>();
            
            public Material GetNewMaterialByTexture(Texture texture)
            {
                TextureBaseElement textureBaseElement = null;
                if (!materialsByTexture.TryGetValue(texture, out textureBaseElement))
                {
                    textureBaseElement = new TextureBaseElement();
                    textureBaseElement.material = (Material)Material.Instantiate(refMaterial);
                    textureBaseElement.material.mainTexture = texture;
                    textureBaseElement.material.name = refMaterial.name + "_" + texture.name + textureBaseElement.material.GetInstanceID().ToString();
                    materialsByTexture[texture] = textureBaseElement;

                }
                textureBaseElement.CountOfObjects++;
                return textureBaseElement.material;
            }
            
        }
        public class TextureBaseElement
        {
            public int CountOfObjects = 0;
            public Material material;

        }
        internal static Material GetMaterial(Material referenceMaterial, Texture texture)
        {

            Material m = null;
            MaterialBaseElement mb = null;
            if (materialList.TryGetValue(referenceMaterial, out mb))
            {
                m = mb.GetNewMaterialByTexture(texture);
                if (m == null)
                {
                    Debug.LogWarning(referenceMaterial);
                }
            }
            else
            {
                mb = new MaterialBaseElement();
                materialList[referenceMaterial] = mb;
                mb.refMaterial = referenceMaterial;
                m = mb.GetNewMaterialByTexture(texture);
            }
            Debug.Log(GetDebug());

            return m;
        }
        static string GetDebug()
        {
            string s = materialList.Keys.Count + " ";
            foreach (var item in materialList.Keys)
            {
                s += item.name + " ";
                foreach (var item2 in materialList[item].materialsByTexture.Keys)
                {
                    s += " " + item2.name + " " + materialList[item].materialsByTexture[item2].CountOfObjects;
                }
                s += "\n";

            }
            return s;
        }
        public static void IDestory(Material refMaterial, Texture texture)
        {

            if (materialList.Count == 0) return;
            //if (!materialList.ContainsKey(refMaterial)) return;
            MaterialBaseElement mbe = materialList[refMaterial];
            TextureBaseElement tbe = mbe.materialsByTexture[texture];
        

            tbe.CountOfObjects--;
            if (tbe.CountOfObjects == 0)
            {
                mbe.materialsByTexture.Remove(texture);
                Object.DestroyImmediate(tbe.material);

            }

            if (mbe.materialsByTexture.Count == 0)
            {
                materialList.Remove(refMaterial);
            }
            Debug.Log("destr      " + GetDebug());


        }
        public static Material[] GetAllMaterialsByReferenceMaterial(Material referensMaterial)
        {
            List<Material> materials = new List<Material>();
            return materials.ToArray();
            if (materialList.ContainsKey(referensMaterial))
            {
                MaterialBaseElement mbe = materialList[referensMaterial];
                foreach (TextureBaseElement tb in mbe.materialsByTexture.Values)
                {
                    materials.Add(tb.material);
                }
            }
            return materials.ToArray();
        }

    }
}
*/
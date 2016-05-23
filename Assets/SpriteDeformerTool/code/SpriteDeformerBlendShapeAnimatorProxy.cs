using UnityEngine;
using System.Collections;
namespace Medvedya.SpriteDeformerTools
{
    [ExecuteInEditMode]
    [AddComponentMenu("Sprite Deformer/Sprite deformer blend shape Animator proxy")]
    public class SpriteDeformerBlendShapeAnimatorProxy : MonoBehaviour
    {

        [Range(0, 1)]
        public float value_00 = 0, value_01 = 0, value_02 = 0, value_03 = 0, value_04 = 0, value_05 = 0, value_06 = 0, value_07 = 0, value_08 = 0, value_09 = 0, value_10;
        [Range(0, 1)]
        public float value_11 = 0, value_12 = 0, value_13 = 0, value_14 = 0, value_15 = 0, value_16 = 0, value_17 = 0, value_18 = 0, value_19 = 0, value_20 = 0;
        // Use this for initialization
        public SpriteDeformerBlendShape spriteDeformerBlendShape;
        void Start()
        {
            spriteDeformerBlendShape = GetComponent<SpriteDeformerBlendShape>();
        }

        // Update is called once per frame
        void Update()
        {
            if (spriteDeformerBlendShape == null)
            {
                spriteDeformerBlendShape = GetComponent<SpriteDeformerBlendShape>();
                if (spriteDeformerBlendShape == null)
                return;
            }
            int count = spriteDeformerBlendShape.countOfShapes;
            if (count > 20) count = 20;
            for (int i = 0; i < count; i++)
            {
                spriteDeformerBlendShape.SetBlendShapeWeight(i, getValueByIndex(i));  
            }
            for (int i = count; i < 20; i++)
            {
                setValueByIndex(i, 0);
            }
        }
        public void setValueByIndex(int index, float value)
        {
            switch (index)
            {
                case 0: value_00 = value; break;
                case 1: value_01 = value; break;
                case 2: value_02 = value; break;
                case 3: value_03 = value; break;
                case 4: value_04 = value; break;
                case 5: value_05 = value; break;
                case 6: value_06 = value; break;
                case 7: value_07 = value; break;
                case 8: value_08 = value; break;
                case 9: value_09 = value; break;
                case 10: value_10 = value; break;
                case 11: value_11 = value; break;
                case 12: value_12 = value; break;
                case 13: value_13 = value; break;
                case 14: value_14 = value; break;
                case 15: value_15 = value; break;
                case 16: value_16 = value; break;
                case 17: value_17 = value; break;
                case 18: value_18 = value; break;
                case 19: value_19 = value; break;
                case 20: value_20 = value; break;
                default:
                    break;
            }
        }
        public float getValueByIndex(int index)
        {
            switch (index)
            {
                case 0: return value_00;
                case 1: return value_01;
                case 2: return value_02;
                case 3: return value_03;
                case 4: return value_04;
                case 5: return value_05;
                case 6: return value_06;
                case 7: return value_07;
                case 8: return value_08;
                case 9: return value_09;
                case 10: return value_10;
                case 11: return value_11;
                case 12: return value_12;
                case 13: return value_13;
                case 14: return value_14;
                case 15: return value_15;
                case 16: return value_16;
                case 17: return value_17;
                case 18: return value_18;
                case 19: return value_19;
                case 20: return value_20;

                default:
                    break;
            }

            return 0;
        }
    }
}

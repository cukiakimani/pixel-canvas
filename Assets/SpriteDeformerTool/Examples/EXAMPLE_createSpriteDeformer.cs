using UnityEngine;
using System.Collections;
using Medvedya.SpriteDeformerTools;

public class EXAMPLE_createSpriteDeformer : MonoBehaviour
{

    public Sprite sprite;
    public Material material;
    SpriteDeformerStatic mySprite;
    private SpritePoint centerPoint;
    void Start () {
        mySprite = gameObject.AddComponent<SpriteDeformerStatic>();
        mySprite.sprite = sprite;
        mySprite.material = material;
        mySprite.SetRectanglePoints();

        centerPoint = new SpritePoint(0.5f, 0.5f);
        mySprite.AddPoint(centerPoint);
       
        Bounds b = mySprite.bounds;
        foreach (var item in mySprite.points)
        {
            b.Encapsulate((Vector3)mySprite.SpritePositionToLocal(item.spritePosition));
        }
        mySprite.bounds = b;
        mySprite.UpdateMeshImmediate();
	}
    
    void Update()
    {
        centerPoint.offset2d = 
            new Vector2(Mathf.Cos(Time.time) * 0.3f, 
                Mathf.Sin(Time.time) * 0.3f);

        mySprite.dirty_offset = true;
        
        float t = Mathf.PingPong(Time.time,1);
        centerPoint.color = Color.Lerp(Color.blue, Color.red,t);
        mySprite.dirty_color = true;
    }
}

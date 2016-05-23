using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Medvedya.SpriteDeformerTools;

public class EXAMPLE_monster : MonoBehaviour
{

    SpriteDeformerAnimation spriteDeformerAnimation;
    List<SpritePoint> footPoints = new List<SpritePoint>();
    public Vector2 hitOffset = new Vector2(0, -0.1f);
    void Start()
    {
        spriteDeformerAnimation = GetComponent<SpriteDeformerAnimation>();
        foreach (var point in spriteDeformerAnimation.points)
        {
            if (point.name == "foot")
            {
                footPoints.Add(point);
                spriteDeformerAnimation.notAnimatedPoints.Add(point);
            }
        }
    }
    void Update()
    {
        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        for (int i = 0; i < footPoints.Count; i++)
        {
            var point = footPoints[i];
            Vector2 footGlobalPos = spriteDeformerAnimation.SpritePositionToGlobal(point.spritePosition);
            Vector2 orign = new Vector2(footGlobalPos.x, transform.position.y);
            RaycastHit2D hit = Physics2D.Raycast(orign, -Vector2.up);
            if (hit.collider != null)
            {
                Vector2 newOffset = spriteDeformerAnimation.GlobalPositionToSpritePosition(hit.point + hitOffset) - point.spritePosition;
                point.offset2d = newOffset;
                spriteDeformerAnimation.dirty_offset = true;
            }
        }
    }
}

using UnityEngine;
using System.Collections;
namespace Medvedya.CameraUtilities
{

    public class DragCamera2D : MonoBehaviour
    {
        Vector2 lastPos;
        bool drag = false;
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                drag = true;
                lastPos = this.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            }
            if (drag)
            {
                Vector2 curPos = this.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
                this.GetComponent<Camera>().transform.position += (Vector3)(lastPos - curPos);
            }
            if (Input.GetMouseButtonUp(0))
            {
                drag = false;
            }
        }
        public void reset()
        {
            lastPos = this.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour {

    public GameObject self;
    private CustomTransform selfTransform;

    Vector3 dist;
    //float posX;
    float posY;
    float posZ;

    // Use this for initialization
    void Start () {

        selfTransform = self.GetComponent<CustomTransform>();

    }

    private void OnMouseDown()
    {
        dist = Camera.main.WorldToScreenPoint(selfTransform.position);
        //posX = Input.mousePosition.x - dist.x;
        posY = Input.mousePosition.y - dist.y;
        posZ = Input.mousePosition.z - dist.z;
   
    }

    private void OnMouseDrag()
    {
        Vector3 curPos = new Vector3(dist.x, Input.mousePosition.y - posY, Input.mousePosition.z - posZ );
        Vector3 worldPos = Camera.main.WorldToScreenPoint(curPos);
        selfTransform.position = worldPos;
    }
}

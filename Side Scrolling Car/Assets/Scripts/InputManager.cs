using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TouchInfo
{
    public int horiontal;
    public int vertical;
    public Vector2 touchPoint;
    public string touchedObjectTag;
}

public class InputManager : Singleton<InputManager> {

    public TouchInfo touchInfo;

    bool firstTouchDown = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        TouchCheck();
        CalculateTouchPosition();
	}

    void TouchCheck()
    {
        if(Input.GetMouseButtonDown(0))
        {
            firstTouchDown = true;
            Debug.Log("TOuching");
        }
        if (Input.GetMouseButtonUp(0))
        {
            firstTouchDown = false;
        }
    }

    void CalculateTouchPosition()
    {
        touchInfo.touchPoint = Vector2.zero;
        touchInfo.horiontal = 0;    touchInfo.vertical = 0;
        touchInfo.touchedObjectTag = null;

        if (firstTouchDown)
        {
            Vector3 mousePoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 30));

            RaycastHit2D raycast = Physics2D.Raycast(mousePoint, worldPoint - mousePoint);
            Debug.DrawRay(mousePoint, worldPoint - mousePoint);

            //If a hit is detected, store the hit values in a structure, which will be used by the car
            if (raycast)
            {
                touchInfo.touchedObjectTag = raycast.collider.tag;
                touchInfo.touchPoint = raycast.point;
                touchInfo.horiontal = raycast.point.x > Camera.main.transform.position.x ? 1 : -1;
                touchInfo.vertical = raycast.point.y > Camera.main.transform.position.y ? 1 : -1;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : Singleton<EnvironmentManager> {

    //List of all the Foreground and Background elements
    public GameObject Road;
    public bool enableScrolling = false;

    private float scrolledUV = 0;
    private float scrollSpeed = 0;
	// Use this for initialization
	void Start () {
        //Bind the UpdateScrollSpeed function to the event in LevelManager
        LevelManager.Instance.SpeedChangedEvent += UpdateScrollSpeed;
	}
	
	// Update is called once per frame
	void Update () {
        Scroll();	
	}
    
    void Scroll()
    {
        if (enableScrolling)
        {
            //Shader.SetGlobalFloat("_ScrollSpeed", speed);
            scrolledUV += scrollSpeed * 0.05f * Time.deltaTime;
            //Debug.Log("ScrolledUV " + scrolledUV);
            Shader.SetGlobalFloat("_ScrolledUV", scrolledUV);
            return;
        }
        Shader.SetGlobalFloat("_ScrolledUV", 0);

    }

    public Bounds? GetRoadBounds()
    {
        if (Road)
        {
            Bounds b = Road.GetComponent<Collider2D>().bounds;
            return b;
        }
        return null;
    }

    void UpdateScrollSpeed(float speed)
    {
        scrollSpeed = speed;
    }

}

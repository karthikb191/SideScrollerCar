using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : Singleton<EnvironmentManager> {

    //List of all the Foreground and Background elements
    public GameObject[] EnvironmentObjects;
    public bool enableScrolling = false;

	// Use this for initialization
	void Start () {
        //Bind the UpdateScrollSpeed function to the event in LevelManager
        LevelManager.Instance.SpeedChangedEvent += UpdateScrollSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void UpdateScrollSpeed(float speed)
    {
        if (enableScrolling)
        {
            Shader.SetGlobalFloat("_ScrollSpeed", speed);
            return;
        }
        Shader.SetGlobalFloat("_ScrollSpeed", 0);
        
    }

}

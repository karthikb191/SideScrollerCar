using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

/// <summary>
/// Base Singleton Class that acts as a template for all the singletons
/// Ensures correct value of instance is preserved. Duplicates are deleted
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    bool destroy = false;
    //Singleton ensures first occurance of Instance is always preserved
    public Singleton(){
        if (instance == null)
            instance = (T)this;
        else
            destroy = true;
    }

    private static T instance;
    public static T Instance {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (destroy)
            Destroy(this.gameObject);
    }
}


public class LevelManager : Singleton<LevelManager> {

    //Acts as a single point of access to all the updates related to the speed of the car
    public Action<float> SpeedChangedEvent;

    public float deltaTime;
	// Use this for initialization
	void Start () {
        //SpeedChangedEvent = new Action<float>();
        Debug.Log("Instance is: " + Instance.gameObject.name);
	}
	
	// Update is called once per frame
	void Update () {
        deltaTime = Time.deltaTime > 0.0333f ? 0.0333f : Time.deltaTime;	
	}
}

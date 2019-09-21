using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : Singleton<Car> {

    public float speed = 0;
    
    private float previousSpeed;
    private GameObject body;
    Vector3 bodyInitialPosition;

	// Use this for initialization
	void Start () {
        Debug.Log("Instance is: " + Instance.gameObject.name);

        body = transform.GetChild(0).Find("Body").gameObject;
        bodyInitialPosition = body.transform.position;

        //Set the initial speed for all the components
        LevelManager.Instance.SpeedChangedEvent.Invoke(speed);
        previousSpeed = speed;
	}
	
	// Update is called once per frame
	void Update () {
        CheckForSpeedChange();
        BounceTheCar();
	}

    //Check for any changes in the speed. If the speed of the car changes, the event on level manager is triggered
    void CheckForSpeedChange()
    {
        if (previousSpeed != speed)
        {
            LevelManager.Instance.SpeedChangedEvent.Invoke(speed);
            previousSpeed = speed;
        }
    }
    float oldPerlinNoise;
    void BounceTheCar()
    {
        //Root must always be back to the default position.
        Vector3 rootForce = (bodyInitialPosition - body.transform.position) * 3000 * Time.deltaTime * Time.deltaTime;

        //float x = Random.Range(0.0f, 1.0f); float y = Random.Range(0.0f, 1.0f);
        float x = Mathf.Sin(Time.time) + Random.Range(0.0f, 0.2f);
        float y = Mathf.Sin(Time.time) + Random.Range(0.0f, 0.3f);
        //float x1 = Random.Range(0.0f, 1.0f) * 2 - 1;
        //x += Time.deltaTime * 0.1f;    y += Time.deltaTime * 0.1f;
        //float perlinNoise = Mathf.PerlinNoise(x, y) * 3 - 1;
        //float bodyPerlinNoise = Mathf.Lerp(oldNoiseValue, perlinNoise, 0.5f);
        float bodyPerlinNoise = Mathf.PerlinNoise(x, y) * 3.0f - 1;
        bodyPerlinNoise = Mathf.Lerp(oldPerlinNoise, bodyPerlinNoise, 0.2f);
        
        //float bodyPerlinNoise = Random.Range(0.0f, 1.0f) * 2 - 1;
        //bodyPerlinNoise = Mathf.Lerp(oldPerlinNoise, bodyPerlinNoise, 0.1f);
        //oldPerlinNoise = bodyPerlinNoise;

        Debug.Log(bodyPerlinNoise);
        Vector3 oldPosition = body.transform.position;

        Vector3 movementDelta = new Vector3(0, bodyPerlinNoise * speed * 1.5f * Time.deltaTime, 0);

        Vector3 newPosition = body.transform.position + movementDelta + rootForce;

        Vector3 newRotation = new Vector3(0, 0, (bodyPerlinNoise) * speed);
        newRotation = Vector3.Lerp(Vector3.zero, newRotation, bodyPerlinNoise / 1.5f);

        body.transform.position = Vector3.Lerp(oldPosition, newPosition, 0.2f);
        body.transform.rotation = Quaternion.Euler(Vector3.Lerp(body.transform.rotation.eulerAngles, newRotation, 0.15f));

        oldPerlinNoise = bodyPerlinNoise;
        
    }
}

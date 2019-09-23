using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Car : Singleton<Car> {

    public float speed = 0;
    [HideInInspector]
    public bool controlIssued = false;

    #region private fields
    private float previousSpeed;
    private GameObject body;
    private Vector3 initialCarPosition;
    private Vector3 bodyInitialPosition;
    private ParticleSystem ps;
    private float multiplier = 0;
    private TouchInfo touchInfo;
    private bool flipped = false;
    private Animator animator;

    private Bounds roadBounds;

    private Vector3 rootPositionOfCar;
    private Vector3 bottomLaneInitialPosition = new Vector3(-6, -3, 0);
    private Vector3 topLaneInitialPosition = new Vector3(4.5f, -1, 0);
    
    #endregion
    // Use this for initialization
    void Start () {
        Debug.Log("Instance is: " + Instance.gameObject.name);

        body = transform.GetChild(0).Find("Body").gameObject;
        ps = transform.GetChild(0).GetComponentInChildren<ParticleSystem>();

        roadBounds = EnvironmentManager.Instance.GetRoadBounds().GetValueOrDefault();
        initialCarPosition = gameObject.transform.position;
        bodyInitialPosition = body.transform.localPosition;
        rootPositionOfCar = bottomLaneInitialPosition;
        touchInfo = InputManager.Instance.touchInfo;
        animator = GetComponent<Animator>();

        LevelManager.Instance.SpeedChangedEvent += ChangeParticleProperties;

        //Set the initial speed for all the components
        LevelManager.Instance.SpeedChangedEvent.Invoke(speed);
        previousSpeed = speed;
	}
	
	// Update is called once per frame
	void Update () {
        if (!controlIssued)
            return;

        Move();
        CheckForPreviousSpeed();
        //BounceTheCar();
	}
    private void LateUpdate()
    {
        if (!controlIssued)
            return;
        
        SetRootPosition();
        BounceTheCar();
    }

    void SetRootPosition()
    {
        if (!controlIssued)
            return;
        //Move the car to left or right of screen depending on speed
        float moveLength = 10.0f;
        float moveScale = Mathf.Clamp(Mathf.Abs(speed) - 30, 0, 20) / 20;

        //Move the root
        gameObject.transform.position = rootPositionOfCar + new Vector3(Mathf.Sign(speed) * moveScale * moveLength, 0, 0);
        
    }

    void ChangeParticleProperties(float s)
    {
        var emission = ps.emission;
        emission.rate = Mathf.Clamp(Mathf.Abs( s / 2), 0, 15);

        var vel = ps.velocityOverLifetime;
        vel.x = new ParticleSystem.MinMaxCurve(-s / 2 - 5, -s / 2);
        vel.y = new ParticleSystem.MinMaxCurve(-s / 4, s / 2 + 1);
    }

    //moves the root of the car
    void Move()
    {
        Debug.DrawLine(Vector3.zero, roadBounds.max);
        Debug.DrawLine(Vector3.zero, roadBounds.min);
        float rateOfSpeedChange = 2.0f;
        multiplier = InputManager.Instance.touchInfo.horiontal;
        float newSpeed = 0;

        //Arrange the alignment
        if((speed < 0 && !flipped) || (speed > 0 && flipped))
        {
            if (speed > 0)
                flipped = false;
            else
                flipped = true;

            StartCoroutine(Flip(flipped));
        }
        
        if (touchInfo.touchedObjectTag == null)
        {
            //If the input is released, 
            newSpeed = 0;
        }
        if(touchInfo.touchedObjectTag == "road")
        {
            //Debug.Log("multiplier " + multiplier);
            //Speed increases or decreases at a constant rate for now
            newSpeed = 50.0f * multiplier;
        }
        if(touchInfo.touchedObjectTag == "fg")
        {

        }
        if(touchInfo.touchedObjectTag == "bg")
        {
            
        }
        //Debug.Log(newSpeed);
        speed = Mathf.SmoothStep(speed, newSpeed, rateOfSpeedChange * Time.deltaTime);
        speed = Mathf.Clamp(speed, -50.0f, 50.0f);
        if (Mathf.Abs(speed) < 0.01f)
            speed = 0;

        //Debug.Log("Speed: " + speed);
    }

    IEnumerator Flip(bool flip)
    {
        Debug.Log("Flip : " + flip);

        float norm = flip ? 0 : 1;
        //animator.speed
        if(flip)
            animator.Play("Flip");
        else
            animator.Play("ReverseFlip");

        //Get the length of flip animation clip
        yield return new WaitForFixedUpdate();
        float time = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

        //float time = animator.GetCurrentAnimatorStateInfo(0).length;
        Debug.Log("Length: " + time);
        yield return new WaitForSeconds(time/2);

        if (flip)
            rootPositionOfCar = topLaneInitialPosition;
        else
            rootPositionOfCar = bottomLaneInitialPosition;
        
        yield return new WaitForSeconds(time / 2);
        
    }

    //Check for any changes in the speed. If the speed of the car changes, the event on level manager is triggered
    void CheckForPreviousSpeed()
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
        Vector3 rootForce = (bodyInitialPosition - body.transform.localPosition) * 3000 * LevelManager.Instance.deltaTime * LevelManager.Instance.deltaTime;

        //float x = Random.Range(0.0f, 1.0f); float y = Random.Range(0.0f, 1.0f);
        float x = Mathf.Sin(Time.time) + UnityEngine.Random.Range(0.0f, 0.2f);
        float y = Mathf.Sin(Time.time) + UnityEngine.Random.Range(0.0f, 0.3f);
        //float x1 = Random.Range(0.0f, 1.0f) * 2 - 1;
        //x += LevelManager.Instance.deltaTime * 0.1f;    y += LevelManager.Instance.deltaTime * 0.1f;
        //float perlinNoise = Mathf.PerlinNoise(x, y) * 3 - 1;
        //float bodyPerlinNoise = Mathf.Lerp(oldNoiseValue, perlinNoise, 0.5f);
        float bodyPerlinNoise = Mathf.PerlinNoise(x, y) * 3.0f - 1;
        bodyPerlinNoise = Mathf.Lerp(oldPerlinNoise, bodyPerlinNoise, 0.2f);
        
        //float bodyPerlinNoise = Random.Range(0.0f, 1.0f) * 2 - 1;
        //bodyPerlinNoise = Mathf.Lerp(oldPerlinNoise, bodyPerlinNoise, 0.1f);
        //oldPerlinNoise = bodyPerlinNoise;

        //Debug.Log(bodyPerlinNoise);
        Vector3 oldPosition = body.transform.localPosition;

        Vector3 movementDelta = new Vector3(0, bodyPerlinNoise * speed * LevelManager.Instance.deltaTime, 0);

        Vector3 newPosition = body.transform.localPosition + movementDelta + rootForce;

        float sign = flipped ? -1 : 1;
        Vector3 newRotation = new Vector3(0, 0, sign * (bodyPerlinNoise) * speed);
        newRotation = Vector3.Lerp(Vector3.zero, newRotation, bodyPerlinNoise / 1.5f);

        body.transform.localPosition = Vector3.Lerp(oldPosition, newPosition, 0.2f);
        body.transform.localRotation = Quaternion.Euler(Vector3.Lerp(body.transform.localRotation.eulerAngles, newRotation, 0.15f));

        oldPerlinNoise = bodyPerlinNoise;
        
    }

    void IssueControl()
    {
        controlIssued = true;
        EnvironmentManager.Instance.enableScrolling = true;
    }
}

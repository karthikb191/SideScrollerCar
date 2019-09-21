using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour {

    public GameObject hingeToFollow;

    Vector3 oldForce;
    Car car;
    private void Start()
    {
        car = Car.Instance;
    }
    // Update is called once per frame
    void Update () {
        RotateWheel();
        SpringOnHinge();
	}
    
    void SpringOnHinge()
    {
        //float distanceSquare = Vector3.Distance(gameObject.transform.position, hingeToFollow.transform.position);
        Vector3 newForce = (hingeToFollow.transform.position - gameObject.transform.position) * 1500f;
        Vector3 force = Vector3.Lerp(oldForce, newForce, 0.25f);
        gameObject.transform.position += force * Time.deltaTime * Time.deltaTime; 
    }

    void RotateWheel()
    {
        Vector3 eulerAngles = gameObject.transform.rotation.eulerAngles;
        float oldZ = eulerAngles.z;
        gameObject.transform.rotation = Quaternion.Euler(eulerAngles.x, 
                                                            eulerAngles.y, 
                                                            eulerAngles.z + car.speed * -30 * Time.deltaTime);
    }
    
}

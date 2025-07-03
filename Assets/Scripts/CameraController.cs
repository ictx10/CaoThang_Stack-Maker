using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private Transform target; 
    [SerializeField] private Vector3 offset;
    [SerializeField] private float _speed; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.x = 0;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _speed);
        transform.position = smoothedPosition;
    }
}

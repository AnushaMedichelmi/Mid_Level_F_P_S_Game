using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    public float playerSpeed;
    bool isGrounded;
    public float jumpForce;
    Rigidbody rb;
    CapsuleCollider capsule;
    Quaternion playerRotation;
    Quaternion camRotation;
    public Camera cam;
    public float rotationSpeed;
    public float minX=-90f;
    public float maxX = 90f;

    void Start()
    {
        rb= GetComponent<Rigidbody>();
        capsule= GetComponent<CapsuleCollider>();
        //cam= GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    private void FixedUpdate()
    {
        float InputX = Input.GetAxis("Horizontal");
        float InputZ = Input.GetAxis("Vertical");

        transform.position += new Vector3(InputX * playerSpeed, 0f, InputZ * playerSpeed);

        if(Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce); 
        }


        float mouseX = Input.GetAxis("Mouse X")*rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y")*rotationSpeed;

        playerRotation=Quaternion.Euler(0f, mouseY, 0f)*playerRotation;
        camRotation= ClampRotationOfPlayer(Quaternion.Euler(-mouseX,0f, 0f)*camRotation);

        this.transform.localRotation = playerRotation;
        cam.transform.localRotation = camRotation;
        
    }

    public bool IsGrounded()
    {
        RaycastHit hit;
        if(Physics.SphereCast(transform.position,capsule.radius,Vector3.down,out hit,((capsule.height/2)-(capsule.radius))+0.1f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    Quaternion ClampRotationOfPlayer(Quaternion n)
    {
        n.w = 1f;
        n.x /= n.w;
        n.y /= n.w;
        n.z /= n.w;

        float angleX=2.0f*Mathf.Rad2Deg*Mathf.Atan(n.x);
        angleX = Mathf.Clamp(angleX,minX,maxX); //To restrict the value we use clamp (min and max(-90f and 90f))
        n.x=Mathf.Tan(Mathf.Deg2Rad*angleX*0.5f);
        return n;
    }

   
}

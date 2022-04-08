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
    public float minX = -90f;
    public float maxX = 90f;

    float inputX, inputZ;

    public Animator animator;
    int ammo = 0;
    int medical = 0;
    int maxAmmo = 100;
    int maxMedical = 100;
    int reloadAmmo = 0;
    int maxReloadAmmo = 100;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        //cam= GetComponent<Camera>();
        // animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetBool("IsAiming", !animator.GetBool("IsAiming"));
        }

        if (Input.GetMouseButtonDown(0) && animator.GetBool("IsTrigger"))
        {
            if (ammo > 0)
            {
                //animator.SetBool("IsFiring", !animator.GetBool("IsFiring"));
                animator.SetTrigger("IsTrigger");
                ammo = Mathf.Clamp(ammo - 10, 0, maxAmmo);
            }
            else
            {
                //Trigger the sound for empty bullets
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            animator.SetTrigger("IsReload");
            int amountAmmoNeeded = maxReloadAmmo - reloadAmmo;
            int ammoAvailable = amountAmmoNeeded < ammo ? amountAmmoNeeded : ammo;
            ammo -= ammoAvailable;
            reloadAmmo += ammoAvailable;
            Debug.Log("Ammo left :"+ammo);
            Debug.Log("Reloaded ammo:"+reloadAmmo);
         }
        if (Mathf.Abs(inputX) > 0 || Mathf.Abs(inputZ) > 0)
        {

            if (!animator.GetBool("IsWalking"))
                animator.SetBool("IsWalking", true);
        }
        else if (animator.GetBool("IsWalking"))
        {
            animator.SetBool("IsWalking", false);
        }
    }

    private void FixedUpdate()
    {
        float InputX = Input.GetAxis("Horizontal");
        float InputZ = Input.GetAxis("Vertical");

        transform.position += new Vector3(InputX * playerSpeed, 0f, InputZ * playerSpeed);

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce);
        }


        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        playerRotation = Quaternion.Euler(0f, mouseY, 0f) * playerRotation;
        camRotation = ClampRotationOfPlayer(Quaternion.Euler(-mouseX, 0f, 0f) * camRotation);

        this.transform.localRotation = playerRotation;
        cam.transform.localRotation = camRotation;

    }

    public bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, capsule.radius, Vector3.down, out hit, ((capsule.height / 2) - (capsule.radius)) + 0.1f))
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

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(n.x);
        angleX = Mathf.Clamp(angleX, minX, maxX); //To restrict the value we use clamp (min and max(-90f and 90f))
        n.x = Mathf.Tan(Mathf.Deg2Rad * angleX * 0.5f);
        return n;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ammo" && ammo < maxAmmo)
        {


            Debug.Log("collected box");
            // ammo += 10;
            ammo = Mathf.Clamp(ammo + 10, 0, maxAmmo);
            Debug.Log("ammo:"+ammo);
            collision.gameObject.SetActive(false);

        }
        if (collision.gameObject.tag == "Medical" && medical < maxMedical)
        {

            Debug.Log("collected box");
            //medical += 10;
            medical=Mathf.Clamp(medical + 10, 0, maxMedical);
            collision.gameObject.SetActive(false);
        }
        else if(collision.gameObject.tag =="Lava")
        {
            //Need to trigger dead sound when medical is 0
            medical = Mathf.Clamp(medical - 10,0, maxMedical);
            Debug.Log(medical);
        }
    }

}

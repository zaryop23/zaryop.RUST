using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float shiftSpeed = 15f;
    [SerializeField] float jumpForce = 7f;
    [SerializeField] GameObject pistol, rifle, miniGun;
    [SerializeField] Image pistolUI, rifleUI, miniGunUI, cusror;
    [SerializeField] AudioSource characterSounds;
    [SerializeField] AudioClip jumpSFX;

    Rigidbody rb;
    Animator anim;

    int health;

    

    Vector3 direction;

    float currentSpeed;
    bool isGrounded = true;
    float stamina = 5f;
    bool isPistol, isRifle, isMiniGun;
    Weapons weapons = Weapons.None;

    public enum Weapons
    {
        None,
        Pistol,
        Rifle,
        MiniGun
    }


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        currentSpeed = moveSpeed;

        health = 100;
    }

    void FixedUpdate()
    {
        rb.MovePosition(transform.position + direction * currentSpeed * Time.deltaTime);
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        direction = new Vector3(moveHorizontal, 0f, moveVertical);
        direction = transform.TransformDirection(direction);

        if (Input.GetKeyDown(KeyCode.Alpha1) && isPistol)
        {
            ChooseWeapon(Weapons.Pistol);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && isRifle)
        {
            ChooseWeapon(Weapons.Rifle);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && isMiniGun)
        {
            ChooseWeapon(Weapons.MiniGun);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ChooseWeapon(Weapons.None);
        }

        if(stamina > 5f)
        {
            stamina = 5f;
        }
        else if (stamina < 0)
        {
            stamina = 0;
        }

        if (direction.x != 0 || direction.z != 0)
        {
            anim.SetBool("Run", true);

            if(!characterSounds.isPlaying && isGrounded)
            {
                characterSounds.Play();
            }
        }
        else if (direction.x == 0 && direction.z == 0)
        {
            anim.SetBool("Run", false);
            characterSounds.Stop();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
            isGrounded = false;

            anim.SetBool("Jump", true);

            characterSounds.Stop();
            AudioSource.PlayClipAtPoint(jumpSFX, transform.position);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {

            stamina -= Time.deltaTime;
            currentSpeed = shiftSpeed;  
        }
        else if (!Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = moveSpeed;
            stamina += Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
        anim.SetBool("Jump", false);
    }

    
    
    public void ChooseWeapon(Weapons weapons)
    {
        anim.SetBool("Pistol", weapons == Weapons.Pistol);
        anim.SetBool("Assault", weapons == Weapons.Rifle);
        anim.SetBool("MiniGun", weapons == Weapons.MiniGun);
        anim.SetBool("NoWeapon", weapons == Weapons.None);
        pistol.SetActive(weapons == Weapons.Pistol);
        rifle.SetActive(weapons == Weapons.Rifle);
        miniGun.SetActive(weapons == Weapons.MiniGun);

        if(weapons != Weapons.None)
        {
            cusror.enabled = true;
        }
        else
        {
            cusror.enabled = false;
        }
    }

    

    void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "pistol":
                if (!isPistol)
                {
                    isPistol = true;
                    pistolUI.color = Color.white;
                    ChooseWeapon(Weapons.Pistol);
                }
                break;
            case "rifle":
                if (!isRifle)
                {
                    isRifle = true;
                    rifleUI.color = Color.white;
                    ChooseWeapon(Weapons.Rifle);
                }
                break;
            case "minigun":
                if (!isMiniGun)
                {
                    isMiniGun = true;
                    miniGunUI.color = Color.white;
                    ChooseWeapon(Weapons.MiniGun);
                }
                break;
            default:
                break;
        }

           Destroy(other.gameObject);
    }

    public void ChangeHealth(int count)
    {
        health -= count;
        if (health <= 0)
        {

            anim.SetBool("Die", true);
            ChooseWeapon(Weapons.None);
            enabled = false;        

        }
    
    }
}

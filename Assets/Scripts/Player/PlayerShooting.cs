﻿using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
public class PlayerShooting : MonoBehaviour
{
    public int damagePerShot = 20;
    public float timeBetweenBullets = 0.15f;
    public float range = 500f;
    public float inputThreshold = 0.1f;
    public XRNode inputSource;
    public InputHelpers.Button inputButton;

    float timer;
    Ray shootRay = new Ray();
    RaycastHit shootHit;
    int shootableMask;
    ParticleSystem gunParticles;
    LineRenderer gunLine;
    AudioSource gunAudio;
    Light gunLight;
    float effectsDisplayTime = 0.2f;
    public AudioSource balloonpop;

    Animator anim;
    GameObject player;
    PlayerHealth playerHealth;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        shootableMask = LayerMask.GetMask ("Shootable");
        gunParticles = GetComponent<ParticleSystem> ();
        gunLine = GetComponent <LineRenderer> ();
        gunAudio = GetComponent<AudioSource> ();
        gunLight = GetComponent<Light> ();
        gunAudio.volume =AudioMenuSFX.SFXs; 
       // balloonpop.volume= AudioMenuSFX.SFXs;
    }


    void Update ()
    {
        timer += Time.deltaTime;
        gunAudio.volume = AudioMenuSFX.SFXs;
        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), inputButton, out bool isPressed, inputThreshold);
		if(/*Input.GetButton("Fire1")*/ isPressed && timer >= timeBetweenBullets && Time.timeScale != 0 && !PlayerHealth.isDead &&!MenuPaused.gamepaused)
        {
            Shoot ();
        }

        if(timer >= timeBetweenBullets * effectsDisplayTime)
        {
            DisableEffects ();
        }

        if (MenuPaused.gamepaused)
        {
            gunParticles.Stop();
            DisableEffects();
            gunLine.enabled = false;
            gunLine.enabled = false;
        }

    }


    public void DisableEffects ()
    {
        gunLine.enabled = false;
        gunLight.enabled = false;
    }


    void Shoot ()
    {
        timer = 0f;

        gunAudio.Play ();

        gunParticles.Stop();

        gunParticles.Stop ();
        gunParticles.Play ();

        gunLine.enabled = true;
        gunLine.SetPosition (0, transform.position);

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;



        RaycastHit theHit;
        // Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out theHit);
        bool hit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out theHit, 500);

        if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
        {
            gunLine.SetPosition(1, shootHit.point);
        }
        else
        {
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
            if (theHit.collider.tag == "Balloon")
            {
                Destroy(theHit.collider.gameObject);
                ScoreManager.Score(10);

            }
            if (theHit.collider.tag == "BlueBalloon") 
            { 
                Destroy(theHit.collider.gameObject);            
                playerHealth.RecuperarVida(50);
                ScoreManager.Score(50);                
            }
            if (theHit.collider.tag == "YellowBalloon")
            {
                Destroy(theHit.collider.gameObject);
                playerHealth.TakeDamage(20);
                ScoreManager.Score(-20);
            }

            
        }

    }
}

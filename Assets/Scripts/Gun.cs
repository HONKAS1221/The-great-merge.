using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GunObjScriptable gunData; // Reference to the GunObjScriptable scriptable object
    public AudioClip gunShotC;// Reference to the gun shot sound clip
    private bool isSetup; 


    private CharacterInput controls;

    public float damage = 10f;
    public float range = 100f;

    public Camera fpsCam;

    void Awake()
    {
        controls = new CharacterInput();
    }

    void OnEnable()
    {
        controls.Enable();
        isSetup = false; // Reset the setup flag when the gun is enabled
    }

    void OnDisable()
    {
        controls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSetup) 
        { 
            gunData.Setup(gameObject, this); // Call the Setup method of the GunObjScriptable to set up the gun with the current game object and this script
            isSetup = true;
        }

        if (gunData.Tick(controls.Player.Shoot.IsPressed())) // Call the Tick method of the GunObjScriptable to handle shooting logic
        {
            SoundFX_Manager.instance.PlaySoundFXClip(gunShotC, transform, 1f); // Play the gun shot sound effect
        }
    }
}   

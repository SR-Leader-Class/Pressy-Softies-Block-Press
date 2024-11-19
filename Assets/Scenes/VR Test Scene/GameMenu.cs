using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;


public class GameMenu : MonoBehaviour
{
    
    public Transform head;
    public float spawnMenuDis = 2f;
    public GameObject gameMenu;
    public InputActionProperty showButton;

    bool pauseMenu;

    [Header("Audio Mixer")]

    public AudioMixer audioMixerMusic;
    public AudioMixer audioMixerEffect;

    void Start()
    {
        
    }

    
    void Update()
    {
        if(showButton.action.WasPressedThisFrame())
        {
            gameMenu.SetActive(!gameMenu.activeSelf);

            gameMenu.transform.position = head.position + new Vector3(head.forward.x, 0, head.forward.z).normalized * spawnMenuDis;

            pauseMenu = !pauseMenu;

            if(pauseMenu == true)
                Time.timeScale = 0f;
            else
                Time.timeScale = 1f;
        }
            
        gameMenu.transform.LookAt(new Vector3(head.position.x, gameMenu.transform.position.y, head.position.z));
        gameMenu.transform.forward *= -1;
        /*
        leftHand.transform.LookAt(leftArm.transform.position);
        rightHand.transform.LookAt(rightArm.transform.position);

        if(leftActive.action.ReadValue<float>() > 0.1f)
            Debug.Log("Left active button pressed");

        if(rightActive.action.ReadValue<float>() > 0.1f)
            Debug.Log("Right active button pressed");

        if(leftCancel.action.ReadValue<float>() > 0.1f)
            Debug.Log("Left cancel button pressed");

        if(rightCancel.action.ReadValue<float>() > 0.1f)
            Debug.Log("Right cancel button pressed");
        */

    }

    public void SetMusicVolume(float volume)
    {
        audioMixerMusic.SetFloat("volume_2", volume);
    }

    public void SetEffectVolume(float volume)
    {
        audioMixerEffect.SetFloat("volume", volume);
    }
}

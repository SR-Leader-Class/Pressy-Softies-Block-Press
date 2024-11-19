using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.XR;
using System;

public class GameController : MonoBehaviour
{

    static public bool LeftActivePress; // active
    static public bool RightActivePress;
    static public bool LeftCancelPress; // pick
    static public bool RightCancelPress;

    static public int DifficultyNum = 1;
    static public int KilledEnemyNum;
    static public bool CorrectDifficulty;

    //[Header("Key Input")]
    public InputActionProperty leftActive;
    public InputActionProperty rightActive;

    public InputActionProperty leftCancel;
    public InputActionProperty rightCancel;

    public Text[] pressButttonValuesText;

    //[Header("Check Press Intensity")]
    public float pressLowLimit = 0.02f;
    public float[] pressIntensity = new float[3];
    public Text[] pressIntensityText;
    public int pressIntensityNum;
    private float tempPressIntensity, priTempPressIntensity;
    private bool pressIntensityLock = true, isCheckIntensity;

    [Header("Test Check Press Intensity")]
    private float maxIntensity, averageIntensity;
    
    [Header("In Game Check Press Intensity")]
    public float[] inGamePressIntensity;
    private float inGameMaxIntensity, inGameAverageIntensity;

    //[Header("Check Difficulty")]
    public Difficulty DifficultySet = Difficulty.Close;
    public enum Difficulty
    {
        Close, Far, Move
    };
    public GameObject playerHead;
    public GameObject playerBlock;
    public Text difficultyHint;
    public float ctrlCloseDis, ctrlFarDis;
    private float disPlayerHeadHand;

    //[Header("Killed Enemy Num")]
    public Text killedEnemyNumText;

    //[Header("Countdown 20 Min Timer")]
    public float totalTime = 1200f; // 20 minutes in seconds
    public float currentTime;
    public Text timerText;
    private bool timeUp = false;
    
    void Start()
    {
        currentTime = totalTime;
    }

    
    void Update()
    {
        
        Value_Information();
        Check_Press_Intensity(); // Check Press Intensity
        Check_Difficulty();
        if(inGamePressIntensity.Length != 0)
            Countdown_Timer();
        Count_Final_Score();

        killedEnemyNumText.text = "Killed : " + KilledEnemyNum;
        
        // --------------------------------------------------------------- Key Input

        if(leftActive.action.ReadValue<float>() > averageIntensity || Input.GetKey(KeyCode.Y))
        {
            //Debug.Log("Left active button pressed");
            LeftActivePress = true;
        }
        else
            LeftActivePress = false; 

        if(rightActive.action.ReadValue<float>() > averageIntensity || Input.GetKey(KeyCode.O))
        {
            Debug.Log("Right active button pressed");
            RightActivePress = true;
        }
        else
            RightActivePress = false;

        if(leftCancel.action.ReadValue<float>() > averageIntensity || Input.GetKey(KeyCode.U))
        {
            //Debug.Log("Left cancel button pressed");
            LeftCancelPress = true;
        }
        else
            LeftCancelPress = false;    

        if(rightCancel.action.ReadValue<float>() > averageIntensity || Input.GetKey(KeyCode.I))
        {
            Debug.Log("Right cancel button pressed");
            RightCancelPress = true;
        }
        else
            RightCancelPress = false;
    }

    // --------------------------------------------------------------- Check Press Intensity

    void Check_Press_Intensity()
    {
        if(rightActive.action.ReadValue<float>() >= pressLowLimit && pressIntensityLock == true) // Pressed
        {
            pressIntensityLock = false;
            isCheckIntensity = true;
        }

        if(isCheckIntensity == true) // Check Max Value
        {
            tempPressIntensity = rightActive.action.ReadValue<float>();

            if(tempPressIntensity > priTempPressIntensity)
                priTempPressIntensity = tempPressIntensity;
        }

        if(rightActive.action.ReadValue<float>() < pressLowLimit && pressIntensityLock == false) // Release
        {
            if(pressIntensityNum < 3)
                pressIntensity[pressIntensityNum] = priTempPressIntensity;
            else
            {
                Array.Resize(ref inGamePressIntensity, inGamePressIntensity.Length + 1);
                inGamePressIntensity[pressIntensityNum - 3] = priTempPressIntensity;
            }
                
            pressIntensityNum ++;
            priTempPressIntensity = 0;
            pressIntensityLock = true;
        }

        maxIntensity = Check_Max_Intensity(pressIntensity);
        averageIntensity = (pressIntensity[0] + pressIntensity[1] + pressIntensity[2]) / 3;
    }

    float Check_Max_Intensity(float[] values)
    {
        float maxValue = 0;
        foreach(float value in values)
        {
            if(maxValue < value)
                maxValue = value;
        }
        return maxValue;
    }

    void Count_Final_Score()
    {
        inGameMaxIntensity = Check_Max_Intensity(inGamePressIntensity);
        float totalInGameIntensity = 0f;
        for(int i = 0; i < inGamePressIntensity.Length; i ++)
            totalInGameIntensity += inGamePressIntensity[i];
        inGameAverageIntensity = totalInGameIntensity / inGamePressIntensity.Length;
    }

    // --------------------------------------------------------------- Difficulty

    void Check_Difficulty()
    {
        disPlayerHeadHand = Vector3.Distance(playerHead.transform.position, playerBlock.transform.position);

        if(DifficultySet == Difficulty.Close && disPlayerHeadHand <= ctrlCloseDis)
        {
            CorrectDifficulty = true;
            difficultyHint.text = "Close : Good";
            DifficultyNum = 1;
        }
        else if(DifficultySet == Difficulty.Close)
        {
            CorrectDifficulty = false;
            difficultyHint.text = "Close : Too Far!";
        }

        if(DifficultySet == Difficulty.Far && disPlayerHeadHand >= ctrlFarDis)
        {
            CorrectDifficulty = true;
            difficultyHint.text = "Far : Good";
            DifficultyNum = 2;
        }
        else if(DifficultySet == Difficulty.Far)
        {
            CorrectDifficulty = false;
            difficultyHint.text = "Far : Too Close!";
        }
            
        if(DifficultySet == Difficulty.Move)
        {
            CorrectDifficulty = true;
            difficultyHint.text = "Move";
            DifficultyNum = 3;
        }
    }

    public void Set_Difficulty(string difficulty)
    {
        if(difficulty == "Close")
            DifficultySet = Difficulty.Close; 
        if(difficulty == "Far")
            DifficultySet = Difficulty.Far; 
        if(difficulty == "Move")
            DifficultySet = Difficulty.Move; 
    }

    // --------------------------------------------------------------- Timer

    void Countdown_Timer()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            // Format time to display as mm:ss
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            timerText.text = "Time Up"; // Time Up
            timeUp = true;
        }
    }

    // --------------------------------------------------------------- Value Information

    void Value_Information()
    {
        pressButttonValuesText[0].text = "LA : " + leftActive.action.ReadValue<float>();
        pressButttonValuesText[1].text = "RA : " + rightActive.action.ReadValue<float>();
        pressButttonValuesText[2].text = "LC : " + leftCancel.action.ReadValue<float>();
        pressButttonValuesText[3].text = "RC : " + rightCancel.action.ReadValue<float>();

        pressIntensityText[0].text = "A : "   + pressIntensity[0];
        pressIntensityText[1].text = "B : "   + pressIntensity[1];
        pressIntensityText[2].text = "C : "   + pressIntensity[2];
        pressIntensityText[3].text = "Max : " + maxIntensity;
        pressIntensityText[4].text = "Ave : " + averageIntensity;
        pressIntensityText[5].text = "Max : " + inGameMaxIntensity;
        pressIntensityText[6].text = "Ave : " + inGameAverageIntensity;
    }

    // -----------------------------------------------------------------------------

#if UNITY_EDITOR

[System.Serializable]
    [CustomEditor(typeof(GameController))]
    public class GameControllerEditor : Editor
    {
        private string[] tabs = { "Key Input", "Check Intensity", "Difficulty", "TextUI" };
        private int currentTab = 0;

        override public void OnInspectorGUI()
        {
            serializedObject.Update();
            GameController myScript = target as GameController;

            //Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/WeaponPickeable_CustomEditor") as Texture2D;
            //GUILayout.Label(myTexture);

            EditorGUILayout.BeginVertical();
            //currentTab = GUILayout.Toolbar(currentTab, tabs);
            currentTab = GUILayout.SelectionGrid(currentTab, tabs, 3);
            EditorGUILayout.Space(10f);
            EditorGUILayout.EndVertical();
            #region variables

            if (currentTab >= 0 || currentTab < tabs.Length)
            {
                switch (tabs[currentTab])
                {
                    case "Key Input":
                        EditorGUILayout.LabelField("\nKey Input\n", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftActive"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightActive"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftCancel"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightCancel"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("pressButttonValuesText"));
                        break;
                    case "Check Intensity":
                        EditorGUILayout.LabelField("\nIntensity\n", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("pressLowLimit"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("pressIntensity"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("inGamePressIntensity"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("pressIntensityText")); 
                        break;
                    case "Difficulty":
                        EditorGUILayout.LabelField("\nDifficulty\n", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("DifficultySet"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerHead"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerBlock")); 
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("difficultyHint"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("ctrlCloseDis"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("ctrlFarDis"));
                        break;
                    case "TextUI":
                        EditorGUILayout.LabelField("\nTextUI\n", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("killedEnemyNumText"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("totalTime"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentTime"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("timerText")); 
                        break;
                }
            }

            #endregion

            serializedObject.ApplyModifiedProperties();

        }
    }
#endif
}

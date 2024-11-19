using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.AI;

public class CharacterMovementPSXL : MonoBehaviour
{

    // -------------------------------------------------------

    public Mode ModeSet = Mode.Enemy;
    public enum Mode
    {
        Enemy, Health
    };

    [Space]
    //[Header("Enemy")]
    public Difficulty DifficultySet = Difficulty.Close;
    public enum Difficulty
    {
        Close, Far, Move
    };

    //public GameObject[] allDriftPoints;
    public EnemySpawnerPSXL enemySpawnerPSXL;
    public BoxCollider boxCollider;
    public float followPointSwitchTime = 1f, speed = 0.5f, healthValue;

    public GameObject followPoint;
    private NavMeshAgent agent;    
    private bool followPointLock = true, deadLock = true;

    [Space]
    //[Header("Health")]
    public CharacterMovementPSXL characterMovement;

    
    void Start()
    {
        healthValue = 1;
        followPointLock = true;
        deadLock = true;
        //allDriftPoints = FindObjectsOfType<GameObject>()
        //.Where(go => go.name == "Picked Hint Text")
        //.ToArray();

        agent = GetComponent<NavMeshAgent>();

        if(agent != null)
        {
            float randomSpeed = Random.Range(0.2f, speed);
            agent.speed = randomSpeed;
        }
    }

    
    void Update()
    {
        Set_Difficulty();

        if(ModeSet == Mode.Enemy)
        {
            if(followPointLock == true)
                StartCoroutine(set_follow_point());

            if(DifficultySet == Difficulty.Close || DifficultySet == Difficulty.Far)
            {
                agent.SetDestination(this.transform.position);
            }

            if(DifficultySet == Difficulty.Move)
            {
                agent.SetDestination(followPoint.transform.position);
            }
        }
    }

    void Set_Difficulty()
    {
        if(GameController.DifficultyNum == 1)
            DifficultySet = Difficulty.Close;
        if(GameController.DifficultyNum == 2)
            DifficultySet = Difficulty.Far;
        if(GameController.DifficultyNum == 3)
            DifficultySet = Difficulty.Move;
    }

    public void Damage(float _damageValue)
    {
        healthValue -= _damageValue;

        if(healthValue <= 0 && deadLock == true)
        {   
            deadLock = false;
            GameController.KilledEnemyNum ++;
            enemySpawnerPSXL.Spawn_Enemy();
            Destroy(gameObject, 0.1f);
        }
    }

    // ------------------------------------------------------------------------- Health

    public void Take_Damage(float _damageValue)
    {
        characterMovement.Damage(_damageValue);
    }

    // ------------------------------------------------------------------------- Follow Point

    IEnumerator set_follow_point()
    {
        followPointLock = false;
        //followPoint = GetRandomPointInBox();
        yield return new WaitForSeconds(followPointSwitchTime);
        followPointLock = true;
    }

    Vector3 GetRandomPointInBox()
    {
        Vector3 size = boxCollider.size;
        Vector3 center = boxCollider.center;

        Vector3 worldCenter = boxCollider.transform.TransformPoint(center);
        Vector3 halfExtents = size * 0.5f;

        float randomX = Random.Range(-halfExtents.x, halfExtents.x);
        float randomY = Random.Range(-halfExtents.y, halfExtents.y);
        float randomZ = Random.Range(-halfExtents.z, halfExtents.z);

        Vector3 localRandomPosition = new Vector3(randomX, randomY, randomZ);
        Vector3 worldRandomPosition = boxCollider.transform.TransformPoint(localRandomPosition);

        return worldRandomPosition;
    }

    // -------------------------------------------------------------------------

#if UNITY_EDITOR

[System.Serializable]
    [CustomEditor(typeof(CharacterMovementPSXL))]
    public class CharacterMovementEditorPSXL : Editor
    {
        private string[] tabs = { "Mode Switch" };
        private int currentTab = 0;

        override public void OnInspectorGUI()
        {
            serializedObject.Update();
            CharacterMovementPSXL myScript = target as CharacterMovementPSXL;

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
                    case "Mode Switch":
                        if (myScript.ModeSet == Mode.Enemy)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("ModeSet"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("DifficultySet"));
                            //EditorGUILayout.PropertyField(serializedObject.FindProperty("allDriftPoints"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("enemySpawnerPSXL"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("boxCollider"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("followPointSwitchTime"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("followPoint"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("healthValue"));
                        }
                        if (myScript.ModeSet == Mode.Health)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("ModeSet"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("characterMovement"));
                        }
                        break;
                }
            }

            #endregion

            serializedObject.ApplyModifiedProperties();

        }
    }
#endif
}

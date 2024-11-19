using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeaponPSXL : MonoBehaviour
{

    //public float duration, magnitude;
    
    public GameObject shooterAngle;
    public GameObject shooterAngle2;
    public GameObject aimTargetPoint;
    public GameObject[] allAimTarget;
    //public LayerMask shootLayer;

    private Vector3 aimPoint;

    public float shootRange, damage, ctrlDisAimTarget, shootBreakTime;
    public bool isAutoAim;
    private float disAimTarget;
    private bool shootLock = true, isUILayer;

    [Header("Cannon")]
    public GameObject cannonBase, cannonBarrel;
    public float turnSpeed;
    public Transform cannonRotation;

    [Header("Basic")]
    public Animator animator;
    public CamersShake camersShake;

    void Start()
    {
        
    }

    void Update()
    {
        Shoot();
        Auto_Aim();
        Cannon_Angle();
    }

    void Shoot() // Auto Aim
    {
        RaycastHit hit;
        RaycastHit hit2;
        if(Physics.Raycast(shooterAngle.transform.position, shooterAngle.transform.forward, out hit, shootRange))
        {
            aimPoint = hit.point;

            if(hit.transform.gameObject.layer == 5)
                isUILayer = true;
            else
                isUILayer = false;

            if(Physics.Raycast(shooterAngle2.transform.position, shooterAngle2.transform.forward, out hit2, shootRange) // Shoot
            && GameController.RightActivePress == true && GameController.CorrectDifficulty == true && shootLock == true)
            {   
                //Debug.Log(hit2.transform.name);
                StartCoroutine(shoot_lock());
                //StartCoroutine(camersShake.Shake(duration, magnitude));
                //Debug.Log("Shoot" + GameController.RightActivePress + "; " + GameController.CorrectDifficulty);
                CharacterMovementPSXL target = hit2.transform.GetComponent<CharacterMovementPSXL>();
                if(target != null)
                    target.Take_Damage(damage);
            }
        }
    }

    void Auto_Aim()
    {
        allAimTarget = FindObjectsOfType<GameObject>()
                     .Where(obj => obj.name == "Enemy Health")
                     .ToArray();

        foreach(GameObject aimTarget in allAimTarget)
        {
            if(allAimTarget != null)
                disAimTarget = Vector3.Distance(aimTarget.transform.position, aimPoint);

            if(disAimTarget <= ctrlDisAimTarget && allAimTarget != null && isUILayer == false) // Is Auto Aim
            {
                isAutoAim = true;
                aimTargetPoint.SetActive(true);
                aimTargetPoint.transform.position = aimTarget.transform.position;
            }
            else if(isUILayer == false) // ------------------------------------------------------ No Auto Aim
            {
                isAutoAim = false;
                aimTargetPoint.SetActive(true);
                aimTargetPoint.transform.position = aimPoint;
            }
            else
            {
                aimTargetPoint.SetActive(false);
            }
        }
    }

    IEnumerator shoot_lock()
    {
        shootLock = false;
        animator.Play("Cannon Fire");
        yield return new WaitForSeconds(shootBreakTime);
        animator.Play("Cannon Unfire");
        shootLock = true;
    }

    void Cannon_Angle()
    {
        var targetRotation = Quaternion.LookRotation(aimTargetPoint.transform.position - cannonRotation.position);
        cannonRotation.rotation = Quaternion.Slerp(cannonRotation.rotation, targetRotation, turnSpeed * Time.deltaTime);

        Vector3 eulerRotationY = new Vector3(0, cannonRotation.eulerAngles.y, 0);
        cannonBase.transform.rotation = Quaternion.Euler(eulerRotationY);

        Vector3 eulerRotationX = new Vector3(cannonRotation.eulerAngles.x, cannonBarrel.transform.eulerAngles.y, 0);
        cannonBarrel.transform.rotation = Quaternion.Euler(eulerRotationX);

        shooterAngle2.transform.rotation = Quaternion.Slerp(shooterAngle2.transform.rotation, targetRotation, turnSpeed * 10f * Time.deltaTime);
    }
}

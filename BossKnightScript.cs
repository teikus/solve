 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossKnightScript : MonoBehaviour

{

   [Header("Параметры босса")]
   [SerializeField] float speed;
   [SerializeField] GameObject player;
   [SerializeField] Animator animator;
   CharacterController ch_controller;
   [SerializeField] bool goMoveToAttack=true;
   Vector3 directionToTarget;
   Quaternion quat;
   [SerializeField] float coldownCyclone=5;
   [SerializeField] float coldownRush=5;
   [SerializeField] float coldownShockWave=5;
   [SerializeField] float durationCyclone = 5;
   [SerializeField] float spdCyclone=10;   
   ParticleSystem.MainModule effectSwordMain;
   [SerializeField] ParticleSystem effectSword;

    #region шоквейв
   [Header("Параметры шоквейва")]
   [SerializeField] int countWave =5;
   [SerializeField] GameObject prefabShockWave;
   [SerializeField] Transform startShockWavePosition;
   [SerializeField] float damageShockWave = 25;
   [SerializeField] float velocitiShockWave=25;
   int tepmCountWave;
   GameObject curentProjectile;
   ProjectileBoss tempprojectileScript;

   #endregion
   #region Раш
   [Header("Параметры Раш")]
   public float myltiplySpeedRush = 10;
   #endregion
   public GameObject colliderCyclone;
   public GameObject collideAttack;
 
   public bool attack=true;
    private void Start() {
       ch_controller = GetComponent<CharacterController>();
       effectSwordMain = effectSword.GetComponent<ParticleSystem>().main;
       tepmCountWave = countWave;
       AIBoss();
    }
    private void Update() {
      if(goMoveToAttack && attack) MoveToAttack();
      if(animator.GetBool("Cyclone")) Move(spdCyclone);
      if(animator.GetBool("Rush")) RuhsMove();
    }


  int a;
  int prevA=211212;
   void AIBoss()
   {
      goMoveToAttack = true;
      a = Random.Range(0,3);
      if(a==prevA){
         AIBoss();
         return;
      }
      prevA = a;
      switch (a)
      {
          case 0:
          StartCoroutine(StartCyclone());
          break;
          case 1: 
          StartCoroutine(StartShockWave());
          break;
          case 2: 
          StartCoroutine(StartRush());
          break;
      }
   }

Vector3 relative;
float angle;
   void MoveToAttack(){
            //получаем угол между двумя векторам, проверям находится ли персонаж за спиной
           relative = transform.InverseTransformPoint (player.transform.position);
           angle= Mathf.Atan2 (relative.x, relative.z) * Mathf.Rad2Deg ;
          //
          Move();
          if(Vector3.Magnitude(transform.position-player.transform.position)<5 && Mathf.Abs(angle)<60) {
            animator.SetTrigger("Attack");
            attack=false;
           return;
           }
    }
   IEnumerator StartCyclone(){
      print("StartCyclone");
      yield return new WaitForSeconds(coldownCyclone); 
      goMoveToAttack=false; 
      Cyclone();
   }
   IEnumerator StartShockWave(){
      print("StartShockWave");
      yield return new WaitForSeconds(coldownShockWave);  
      goMoveToAttack=false;
      ShockWave();
   }

   

   void Cyclone(){
      StartCoroutine(StopCyclone());
      animator.SetBool("Cyclone",true);
      effectSwordMain.loop = true;
      effectSword.Play();
   }
   
  IEnumerator StopCyclone(){
      print("StopCyclone");
     yield return new WaitForSeconds(durationCyclone);
      animator.SetBool("Cyclone",false); 
      colliderCyclone.SetActive(false);
      effectSwordMain.loop = false;
      AIBoss();
   }

    public void ShockWave(){
     
       if(tepmCountWave==countWave){
          tepmCountWave--;
           directionToTarget = (player.transform.position-transform.position).normalized;
            directionToTarget.y=0;
            transform.rotation  = Quaternion.LookRotation(directionToTarget);
           animator.SetTrigger("ShockWave");
          return;
       }
      if(tepmCountWave>=0){
         tepmCountWave--;
         directionToTarget = (player.transform.position-transform.position).normalized;
         directionToTarget.y=0;
         transform.rotation  = Quaternion.LookRotation(directionToTarget);
         curentProjectile = Instantiate(prefabShockWave, startShockWavePosition);
         curentProjectile.transform.parent = null; //переносим его в иерархии 
         tempprojectileScript = curentProjectile.GetComponent<ProjectileBoss>();
         tempprojectileScript.velocity = velocitiShockWave;
         tempprojectileScript.direction = directionToTarget;
         tempprojectileScript.damage=damageShockWave;
         CameraShake.Shake(1, 0.5f);
         if(tepmCountWave!=0) {
            animator.SetTrigger("ShockWave");
         }
         else{
         tepmCountWave = countWave;
         AIBoss();
         }
      }
         
   }

   void Move(){
           RotateToPlayer();
           ch_controller.Move(fov*Time.deltaTime*speed);
   }
   float tick=0.2f;
   float tempTick=0.2f;
   void Move(float speedCyclone){
           RotateToPlayer();
           ch_controller.Move(fov*Time.deltaTime*speedCyclone);
           tempTick-=Time.deltaTime;
           if(tempTick<0){
              tempTick=tick;
              if(colliderCyclone.activeSelf) colliderCyclone.SetActive(false);
              else colliderCyclone.SetActive(true);
           }

   }
    Vector3 fov;
   void RotateToPlayer(){
           directionToTarget = (player.transform.position-transform.position).normalized;
           directionToTarget.y=0;
           directionToTarget.Normalize();
           quat = Quaternion.LookRotation(directionToTarget);
           transform.rotation = Quaternion.RotateTowards(transform.rotation, quat, 6*Time.deltaTime*100);
           fov = transform.TransformDirection(Vector3.forward);
           fov.y=-9.8f;
   }
    IEnumerator StartRush(){
       print("StartRush");
       yield return new WaitForSeconds(coldownRush);  
        goMoveToAttack=false;
        RedyRush();
    }

   void RedyRush(){
       animator.SetTrigger("RedyRush");  
       StartCoroutine(Rush());
   }
   IEnumerator Rush(){
      print("Rush");
      yield return new WaitForSeconds(1);  
      collideAttack.SetActive(true);
      animator.SetBool("Rush",true);  
      StartCoroutine(StopRush());
   }
   void RuhsMove(){
       if(Vector3.Distance(player.transform.position,transform.position)>10)  RotateToPlayer();
       ch_controller.Move(fov*Time.deltaTime*speed*myltiplySpeedRush);
   }
    IEnumerator StopRush(){
       print("StopRush");
       yield return new WaitForSeconds(10); 
       animator.SetBool("Rush",false);  
       collideAttack.SetActive(false);
       AIBoss();
    }

}

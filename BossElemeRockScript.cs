using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossElemeRockScript : MonoBehaviour
{
    [SerializeField] float speed=2f;
    float startSpeed;
    CharacterController ch_controller;
    [SerializeField] GameObject player;
    [SerializeField] Animator animator;

    [SerializeField] GameObject wallSkillPref;
    [SerializeField] float coldowWallSkill=10;
    public Transform startPositionWallSkill;

    [SerializeField] float coldowAuraSkill=2;
    [SerializeField] ScriptContollerAuraBoss auraBoss;

    [SerializeField] float coldownRockAttack = 5;

    [SerializeField] float coldownSlow = 5;
    [SerializeField] GameObject preffSlowSkill;

    private void Start() {
        startSpeed=speed;
        ch_controller = GetComponent<CharacterController>();
        AIBoss();
    }
    private void Update() {
        Move();
    }


    //region движение обьекта
     void Move(){
           RotateToPlayer();
           ch_controller.Move(vectorMove*Time.deltaTime*startSpeed);
           if(startSpeed==0) animator.SetBool("Run",false);
           else animator.SetBool("Run",true);
    }
    Vector3 vectorMove;
    Vector3 directionToTarget;
    Quaternion quat;
    void RotateToPlayer(){
           directionToTarget = (player.transform.position-transform.position).normalized;
           directionToTarget.y=0;
           directionToTarget.Normalize();
           quat = Quaternion.LookRotation(directionToTarget);
           transform.rotation = Quaternion.RotateTowards(transform.rotation, quat, 6*Time.deltaTime*100);
           vectorMove = transform.TransformDirection(Vector3.forward);
           vectorMove.y=-9.8f;
   }
   //endregion
    int a;
  public  void AIBoss()
   {
     a = Random.Range(0,4);
      switch (a)
      {
          case 0:
           StartCoroutine(StartWallSkill());
          break;
          case 1:
                if(Vector3.Distance(player.transform.position,transform.position)>20)StartCoroutine(StartAura());
                else {
                    AIBoss();
                    return;
                }
          break;
          case 2:
          StartCoroutine(StartAttackRock());
          break;
          case 3:
             StartCoroutine(StartSlow());
          break;
      }
   }

   GameObject containerGmObj; 
   IEnumerator StartWallSkill(){
       print("StartWallSkill");
       yield return new WaitForSeconds(coldowWallSkill);  
       CameraShake.Shake(1, 0.5f);
       animator.SetBool("CastWall",true);
       startSpeed=0;
       containerGmObj = Instantiate(wallSkillPref,startPositionWallSkill.position,Quaternion.identity);
       containerGmObj.GetComponent<BossElemWallSkill>().player=player;
       AIBoss();
       StartCoroutine(setSpeed(2.5f));
   }

   //остановка босса
    IEnumerator setSpeed(float time){
        yield return new WaitForSeconds(time);  
        startSpeed=speed;
        animator.SetBool("CastWall",false);
    }

    // увеличение ауры
    IEnumerator StartAura(){
        print("StartAura");
        yield return new WaitForSeconds(coldowAuraSkill);  
        auraBoss.StartAuraSkill();
        AIBoss();
    }

    //способность атаки камнями
    IEnumerator StartAttackRock(){
        print("StartAttackRock");
        yield return new WaitForSeconds(coldownRockAttack);  
        animator.SetTrigger("CastAttackRock");
        startSpeed=0;
        StartCoroutine(setSpeed(2.5f));
        AIBoss();
    }
     //способность замедления
    IEnumerator StartSlow(){
        print("StartSlow");
        yield return new WaitForSeconds(coldownSlow);  
        Vector3 tempPos = player.transform.position;
        tempPos.y=0;
        Instantiate(preffSlowSkill,tempPos,Quaternion.identity);
        AIBoss();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour
{   
    bool isgo;
    public bool isGo{
        get
        {
            return isgo;
        }
 
        set
        {
            isgo = value;
            if(value){
                arrow.SetActive(false);
                StartCoroutine(Destroy());
                audioSource.Play();
                StartCoroutine(ExplosiveSound());
            }
        }
    }
    public Vector2 direction;
    [SerializeField] GameObject particalExplos;
    [SerializeField] GameObject arrow;
    AudioSource audioSource;
    [SerializeField] AudioClip explosiveSound;
    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }
    private void Update() {
        if(isGo){
            transform.Translate(direction*Time.deltaTime,Space.World);
        }else {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Vector2.Angle(Vector2.up, position - transform.position); 
        transform.eulerAngles = new Vector3(0f, 0f, transform.position.x < position.x ? -angle : angle);
        }
    }

    IEnumerator Destroy(){
          yield return new WaitForSeconds(.2f);
          Instantiate(particalExplos,transform.position,Quaternion.identity);
          Destroy(gameObject);
    }
    IEnumerator ExplosiveSound(){
         yield return new WaitForSeconds(.15f);
         audioSource.clip=explosiveSound;
         audioSource.pitch=1;
         audioSource.Play();

    }
}

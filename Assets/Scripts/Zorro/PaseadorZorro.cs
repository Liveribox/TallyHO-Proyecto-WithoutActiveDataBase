using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaseadorZorro : MonoBehaviour
{
    public bool dejarDeCrearPaseadores = false;

    private MoverFichaZorro moverFichaZorro;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }


    private void OnTriggerEnter(Collider other) {
        /*if(other.gameObject.name == "Pato(Clone)"){
            dejarDeCrearPaseadores = true;

            moverFichaZorro = GameObject.Find("Zorro(Clone)").GetComponent<MoverFichaZorro>();


            if(moverFichaZorro != null){
                Debug.Log(dejarDeCrearPaseadores);
                moverFichaZorro.setDetenerPaseador(dejarDeCrearPaseadores);
            }
            else{
                Debug.Log("Me cago en todoooo");
            }
        }*/
    }

}

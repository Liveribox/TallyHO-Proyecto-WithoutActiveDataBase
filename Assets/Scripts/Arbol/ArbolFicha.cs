using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArbolFicha : MonoBehaviour
{
    private GameManager gameManager;

    //Añade sonido a las fichas
    public AudioClip sonidoVuelta;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown(){

        if(gameManager.getPoderClickear() == true){

            if(Mathf.Approximately(transform.eulerAngles.z , 180f)){

                transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,0f);
                gameManager.QuitarFichasVuelta(1);
                audioSource.PlayOneShot(sonidoVuelta,1f);
                gameManager.CambiarTurno();
                
            }

        }
        
        
    }
}

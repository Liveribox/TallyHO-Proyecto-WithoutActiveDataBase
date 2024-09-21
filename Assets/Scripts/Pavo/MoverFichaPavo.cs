using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverFichaPavo : MonoBehaviour
{
    // Tamaño del tablero y tamaño de las casillas
    private int x = 7;
    private int y = 7;

    // Tamaño y separación entre las casillas
    private float separacionEntreCasillas = 1f;

    // Posiciones de la ficha en el tablero
    private int posX;
    private int posY;

    //Posiciones del paseador
    private int posXPaseador;
    private int posYPaseador;

    //Objeto paseador
    public GameObject paseador;

    //Spawneadores de los paseadores
    private GameObject spawnPaseadorArriba;
    private GameObject spawnPaseadorIzquierda;
    private GameObject spawnPaseadorDerecha;
    private GameObject spawnPaseadorAbajo;

    //Coloca los spawneadores en la direccion que tu le pidas
    private Vector3 posicionCasillasArriba;
    private Vector3 posicionCasillasIzquierda;
    private Vector3 posicionCasillasDerecha;
    private Vector3 posicionCasillasAbajo;

    //Listas para los paseadores
    private List<GameObject> listaPaseadoresArriba = new List<GameObject>();
    private List<GameObject> listaPaseadoresAbajo = new List<GameObject>();
    private List<GameObject> listaPaseadoresIzquierda = new List<GameObject>();
    private List<GameObject> listaPaseadoresDerecha = new List<GameObject>();


    //Llamar a la camara main
    private Camera camaraMain;
    private bool clickIzquierdoActivado = false;

    //Comprobar si estan tocando un paseador
    private bool tocarPaseador = false;

    //No permitir turno movimiento
    public bool poderMoverFichaPavo = true;

    public int turnoAnterior = 0;

    private GameManager gameManager;

    //Añade sonido a las fichas
    public AudioClip sonidoVuelta;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        camaraMain = Camera.main;
        CalcularPosicion();

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if((turnoAnterior + 1) == gameManager.turnosRegistrados){
            poderMoverFichaPavo = false;
        }
        else{
            poderMoverFichaPavo = true;
        }


        if (clickIzquierdoActivado){
            
            Ray ray = camaraMain.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Paseador"))
                {
                    tocarPaseador = true;
                    CalcularPosicionPaseador(hit.collider.gameObject);
                    // El ratón está tocando un paseador
                    Debug.Log("El ratón está tocando un paseador");
                    Debug.Log("Psoicion del paseador: " + "Casilla X: " + posXPaseador + " Casilla Y: " + posYPaseador);
                }
                else if(!hit.collider.gameObject.CompareTag("Paseador")){
                    tocarPaseador = false;
                    Debug.Log("No estas tocando un paseador");
                }
            }
        }
    }

    private void OnMouseDown()
    {
        if(gameManager.getPoderClickear() == true){
            
            clickIzquierdoActivado = true;

            if(Mathf.Approximately(transform.eulerAngles.z , 180f)){
                transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,0f);
                gameManager.QuitarFichasVuelta(1);
                audioSource.PlayOneShot(sonidoVuelta,1f);
                gameManager.CambiarTurno();
            }
            else if(!poderMoverFichaPavo){
                Debug.Log("No puedes mover una ficha neutral dos veces seguidas");
            }
            else{
                LimitesPaseador();
            }

        }
        
    }

    private void OnMouseUp() {
        clickIzquierdoActivado = false;
        
        ActualizarPosicionFicha();

        BorrarPaseadores();
        
    }
    
    private void CalcularPosicion()
    {
        // Obtenemos la posición actual del objeto
        Vector3 posicion = transform.position;

        // Calcular las coordenadas de la casilla sobre la posición de la ficha
        posX = Mathf.FloorToInt(posicion.x / separacionEntreCasillas);
        posY = Mathf.FloorToInt(posicion.z / separacionEntreCasillas);

        // Ajustar las coordenadas 
        posX = Mathf.Clamp(posX, 0, x - 1);
        posY = Mathf.Clamp(posY, 0, y - 1);
    }

    private void CalcularPosicionPaseador(GameObject paseadorr)
    {
        // Obtenemos la posición actual del objeto
        Vector3 posicionPaseador = paseadorr.transform.position;

        // Calcular las coordenadas de la casilla sobre la posición de la ficha paseador
        posXPaseador = Mathf.FloorToInt(posicionPaseador.x / separacionEntreCasillas);
        posYPaseador = Mathf.FloorToInt(posicionPaseador.z / separacionEntreCasillas);
   
    }

    

    private void LimitesPaseador(){
        
        // Bloqueo para no salir del tablero hacia arriba
        if (posX >= 0 && posX <= 6 && posY >= 0 && posY <= 6) {
            bool colisionConClonArriba = false;

            for (int y = posY - 1; y >= 0; y--) {
                posicionCasillasArriba = new Vector3(posX * separacionEntreCasillas, 0, y * separacionEntreCasillas);
                spawnPaseadorArriba = Instantiate(paseador, posicionCasillasArriba, Quaternion.identity);

                listaPaseadoresArriba.Add(spawnPaseadorArriba);

                Collider[] colliders = Physics.OverlapSphere(posicionCasillasArriba, 0.5f);

                foreach (Collider collider in colliders) {

                    if(collider.gameObject.CompareTag("Cazador") || collider.gameObject.CompareTag("Pino") || collider.gameObject.CompareTag("Leñador") || collider.gameObject.CompareTag("Zorro") || collider.gameObject.CompareTag("Oso") || collider.gameObject.CompareTag("Pavo") || collider.gameObject.CompareTag("Pato") || collider.gameObject.CompareTag("Arbol")){
                        Destroy(listaPaseadoresArriba[listaPaseadoresArriba.Count - 1]);
                        colisionConClonArriba = true;
                        break;
                    }
                }

                if(colisionConClonArriba){
                    break;
                }

            }
        } 
        else {
            Debug.Log("Bloqueo para no ir hacia arriba");
        }

        
        //Bloqueo para no salir del tablero hacia abajo
        if(posX >= 0 && posX <= 6 && posY >= 0 && posY <= 6){
            bool colisionConClonAbajo = false;

            for (int y = posY + 1; y <= 6; y++){
                posicionCasillasAbajo = new Vector3(posX * separacionEntreCasillas, 0, y * separacionEntreCasillas);
                spawnPaseadorAbajo = Instantiate(paseador, posicionCasillasAbajo, Quaternion.identity);
                listaPaseadoresAbajo.Add(spawnPaseadorAbajo);

                Collider[] colliders = Physics.OverlapSphere(posicionCasillasAbajo, 0.5f);

                foreach (Collider collider in colliders) {

                    if(collider.gameObject.CompareTag("Cazador") || collider.gameObject.CompareTag("Pino") || collider.gameObject.CompareTag("Leñador") || collider.gameObject.CompareTag("Zorro") || collider.gameObject.CompareTag("Oso") || collider.gameObject.CompareTag("Pavo") || collider.gameObject.CompareTag("Pato") || collider.gameObject.CompareTag("Arbol")){
                        Destroy(listaPaseadoresAbajo[listaPaseadoresAbajo.Count - 1]);
                        colisionConClonAbajo = true;
                        break;
                    }
                }

                if(colisionConClonAbajo){
                    break;
                }
            }

        }
        else{
            Debug.Log("Bloqueo para no ir hacia abajo");
        }

        
        //Bloqueo para no salir del tablero hacia la izquierda
        if(posX >= 0 && posX <= 6 && posY >= 0 && posY <= 6){
            bool colisionConClonIzquierda = false;

            for (int x = posX + 1; x <= 6; x++){
                posicionCasillasIzquierda = new Vector3(x * separacionEntreCasillas, 0, posY * separacionEntreCasillas);
                spawnPaseadorIzquierda = Instantiate(paseador, posicionCasillasIzquierda, Quaternion.identity);
                listaPaseadoresIzquierda.Add(spawnPaseadorIzquierda);

                Collider[] colliders = Physics.OverlapSphere(posicionCasillasIzquierda, 0.5f);

                foreach (Collider collider in colliders) {

                    if(collider.gameObject.CompareTag("Cazador") || collider.gameObject.CompareTag("Pino") || collider.gameObject.CompareTag("Leñador") || collider.gameObject.CompareTag("Zorro") || collider.gameObject.CompareTag("Oso") || collider.gameObject.CompareTag("Pavo") || collider.gameObject.CompareTag("Pato") || collider.gameObject.CompareTag("Arbol")){
                        Destroy(listaPaseadoresIzquierda[listaPaseadoresIzquierda.Count - 1]);
                        colisionConClonIzquierda = true;
                        break;
                    }
                }

                if(colisionConClonIzquierda){
                    break;
                }
            }
        }
        else{
            Debug.Log("Bloqueo para no ir hacia la izquierda");
        }

        //Bloqueo para no salir del tablero hacia la derecha
        if(posX >= 0 && posX <= 6 && posY >= 0 && posY <= 6){
            bool colisionConClonDerecha = false;

            for (int x = posX - 1; x >= 0; x--){
                posicionCasillasDerecha = new Vector3(x * separacionEntreCasillas, 0, posY * separacionEntreCasillas);
                spawnPaseadorDerecha = Instantiate(paseador, posicionCasillasDerecha, Quaternion.identity);
                listaPaseadoresDerecha.Add(spawnPaseadorDerecha);

                Collider[] colliders = Physics.OverlapSphere(posicionCasillasDerecha, 0.5f);

                foreach (Collider collider in colliders) {

                    if(collider.gameObject.CompareTag("Cazador") || collider.gameObject.CompareTag("Pino") || collider.gameObject.CompareTag("Leñador") || collider.gameObject.CompareTag("Zorro") || collider.gameObject.CompareTag("Oso") || collider.gameObject.CompareTag("Pavo") || collider.gameObject.CompareTag("Pato") || collider.gameObject.CompareTag("Arbol")){
                        Destroy(listaPaseadoresDerecha[listaPaseadoresDerecha.Count - 1]);
                        colisionConClonDerecha = true;
                        break;
                    }
                }

                if(colisionConClonDerecha){
                    break;
                }
            }

            
        }
        else{
            Debug.Log("Bloqueo para no ir hacia la derecha");
        }
    }

    private void BorrarPaseadores(){
        for(int i = 0; i < listaPaseadoresArriba.Count; i++){
            Destroy(listaPaseadoresArriba[i]);
        }

        for(int i = 0; i < listaPaseadoresAbajo.Count; i++){
            Destroy(listaPaseadoresAbajo[i]);
        }

        for(int i = 0; i < listaPaseadoresIzquierda.Count; i++){
            Destroy(listaPaseadoresIzquierda[i]);
        }

        for(int i = 0; i < listaPaseadoresDerecha.Count; i++){
            Destroy(listaPaseadoresDerecha[i]);
        }
    }

    private void ActualizarPosicionFicha(){
        if(tocarPaseador){
            posX = posXPaseador;
            posY = posYPaseador;

            transform.position = new Vector3(posX,0f,posY);

            turnoAnterior = gameManager.turnosRegistrados;

            gameManager.DescontarTurno(1);

            gameManager.CambiarTurno();
        }
        else{
            Debug.Log("No se movio ficha al final");
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MoverFichaLenyador : MonoBehaviour
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
    private int posXPaseador = -1;
    private int posYPaseador = -1;

    //Posicion annterior
    public int posXAnterior;
    public int posYAnterior;

    //Objeto paseador
    public GameObject paseador;

    //Objeto paseador salida
    public GameObject paseadorSalida;

    //Objeto limitador
    public GameObject limitador;

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

    //Spawneadores de los paseadores salida
    private GameObject spawnPaseadorSalidaArriba;
    private GameObject spawnPaseadorSalidaIzquierda;
    private GameObject spawnPaseadorSalidaDerecha;
    private GameObject spawnPaseadorSalidaAbajo;

    //Colocas los spawneadores de los spawneadores de salida
    private Vector3 posicionSalidaArriba;
    private Vector3 posicionSalidaIzquierda;
    private Vector3 posicionSalidaDerecha;
    private Vector3 posicionSalidaAbajo;

    //Llamar a la camara main
    private Camera camaraMain;
    private bool clickIzquierdoActivado = false;

    //Comprobar si estan tocando un paseador
    private bool tocarPaseador = false;

    //No permitir deshacer movimiento
    public bool poderMoverFichaLeñador = true;

    public int turnoAnterior = 0;

    private GameManager gameManager;

    private GameObject baseDatosJuego;

    //Añadir sonido eliminar ficha
    public AudioClip sonidoEliminar;
    public AudioClip sonidoVuelta;
    private AudioSource audioSource;

    void Start()
    {
        camaraMain = Camera.main;
        CalcularPosicion();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        baseDatosJuego = GameObject.Find("BaseDatosJuego");

        audioSource = GetComponent<AudioSource>();
    }

    void Update(){

        if((turnoAnterior + 2) == gameManager.turnosRegistrados){
            poderMoverFichaLeñador = false;
        }
        else{
            poderMoverFichaLeñador = true;
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
            else{
                PoderMoverFicha();
                ActivarSalida();
            }

        }
        
        
    }

    private void OnMouseUp() {
        clickIzquierdoActivado = false;
        BorrarPaseadores();
        ActualizarPosicionFicha();
        RescatarFichaLeñador();

    }
    
    private void CalcularPosicion()
    {
        // Obtenemos la posición actual del objeto
        Vector3 posicion = transform.position;

        // Calculamos las coordenadas de la casilla en base a la posición del objeto
        posX = Mathf.FloorToInt(posicion.x / separacionEntreCasillas);
        posY = Mathf.FloorToInt(posicion.z / separacionEntreCasillas);

        // Ajustamos las coordenadas al rango del tablero
        posX = Mathf.Clamp(posX, 0, x - 1);
        posY = Mathf.Clamp(posY, 0, y - 1);
    }

    private void CalcularPosicionPaseador(GameObject paseadorr)
    {
        // Obtenemos la posición actual del objeto
        Vector3 posicionPaseador = paseadorr.transform.position;

        // Calculamos las coordenadas de la casilla en base a la posición del objeto
        posXPaseador = Mathf.FloorToInt(posicionPaseador.x / separacionEntreCasillas);
        posYPaseador = Mathf.FloorToInt(posicionPaseador.z / separacionEntreCasillas);
   
    }

    private void LimitesPaseador(){
        //Bloqueo para no salir del tablero hacia arriba
        if(posX >= 0 && posX <= 6 && (posY-1) >= 0 && (posY-1) <= 6){
            posicionCasillasArriba = new Vector3(posX * separacionEntreCasillas, 0f, (posY-1) * separacionEntreCasillas);
            Collider[] colliders = Physics.OverlapSphere(posicionCasillasArriba, 0.5f);

            if(colliders.Length == 0){

                if(posX == posXAnterior && (posY-1) == posYAnterior && !poderMoverFichaLeñador){
                    spawnPaseadorArriba = Instantiate(limitador, posicionCasillasArriba, Quaternion.identity);
                }
                else{
                    spawnPaseadorArriba = Instantiate(paseador, posicionCasillasArriba, Quaternion.identity);
                }

                
            }
            else{
                foreach (Collider collider in colliders) {

                    if(Mathf.Approximately(collider.gameObject.transform.eulerAngles.z,180f)){
                        Debug.Log("No crea un paseador hacia arriba");
                    }
                    else if(collider.gameObject.CompareTag("Pino")){
                        spawnPaseadorArriba = Instantiate(paseador, posicionCasillasArriba, Quaternion.identity);
                    }
                    else if(collider.gameObject.CompareTag("Arbol")){
                        spawnPaseadorArriba = Instantiate(paseador, posicionCasillasArriba, Quaternion.identity);
                    }
                       
                }
            }

            
        }
        else{
            Debug.Log("Bloqueo para no ir hacia arriba");
        }

        //Bloqueo para no salir del tablero hacia abajo
        if(posX >= 0 && posX <= 6 && (posY+1) >= 0 && (posY+1) <= 6){
            posicionCasillasAbajo = new Vector3(posX * separacionEntreCasillas, 0f, (posY+1) * separacionEntreCasillas);
            
            Collider[] colliders = Physics.OverlapSphere(posicionCasillasAbajo, 0.5f);

            if(colliders.Length == 0){

                if(posX == posXAnterior && (posY+1) == posYAnterior && !poderMoverFichaLeñador){
                    spawnPaseadorAbajo = Instantiate(limitador, posicionCasillasAbajo, Quaternion.identity);
                }
                else{
                    spawnPaseadorAbajo = Instantiate(paseador, posicionCasillasAbajo, Quaternion.identity);
                }

            }
            else{

                foreach (Collider collider in colliders) {

                    if(Mathf.Approximately(collider.gameObject.transform.eulerAngles.z,180f)){
                        Debug.Log("No crea un paseador hacia abajo");
                    }
                    else if(collider.gameObject.CompareTag("Pino")){
                        spawnPaseadorAbajo = Instantiate(paseador, posicionCasillasAbajo, Quaternion.identity);
                    }
                    else if(collider.gameObject.CompareTag("Arbol")){
                        spawnPaseadorAbajo = Instantiate(paseador, posicionCasillasAbajo, Quaternion.identity);
                    }
                       
                }
            }

            
        }
        else{
            Debug.Log("Bloqueo para no ir hacia abajo");
        }

        //Bloqueo para no salir del tablero hacia la izquierda
        if((posX+1) >= 0 && (posX+1) <= 6 && posY >= 0 && posY <= 6){
            posicionCasillasIzquierda = new Vector3((posX+1) * separacionEntreCasillas, 0f, posY * separacionEntreCasillas);
            
            Collider[] colliders = Physics.OverlapSphere(posicionCasillasIzquierda, 0.5f);

            if(colliders.Length == 0){

                if((posX+1) == posXAnterior && posY == posYAnterior && !poderMoverFichaLeñador){
                    spawnPaseadorIzquierda = Instantiate(limitador, posicionCasillasIzquierda, Quaternion.identity);
                }
                else{
                    spawnPaseadorIzquierda = Instantiate(paseador, posicionCasillasIzquierda, Quaternion.identity);
                }
 
            }
            else{

                foreach (Collider collider in colliders) {

                    if(Mathf.Approximately(collider.gameObject.transform.eulerAngles.z,180f)){
                        Debug.Log("No crea un paseador hacia la izquierda");
                    }
                    else if(collider.gameObject.CompareTag("Pino")){
                        spawnPaseadorIzquierda = Instantiate(paseador, posicionCasillasIzquierda, Quaternion.identity);
                    }
                    else if(collider.gameObject.CompareTag("Arbol")){
                        spawnPaseadorIzquierda = Instantiate(paseador, posicionCasillasIzquierda, Quaternion.identity);
                    }
                       
                }
            }

            
        }
        else{
            Debug.Log("Bloqueo para no ir hacia la izquierda");
        }

        //Bloqueo para no salir del tablero hacia la derecha
        if((posX-1) >= 0 && (posX-1) <= 6 && posY >= 0 && posY <= 6){
            posicionCasillasDerecha = new Vector3((posX-1) * separacionEntreCasillas, 0f, posY * separacionEntreCasillas);
            

            Collider[] colliders = Physics.OverlapSphere(posicionCasillasDerecha, 0.5f);

            if(colliders.Length == 0){

                if((posX-1) == posXAnterior && posY == posYAnterior && !poderMoverFichaLeñador){
                    spawnPaseadorDerecha = Instantiate(limitador, posicionCasillasDerecha, Quaternion.identity);
                }
                else{
                    spawnPaseadorDerecha = Instantiate(paseador, posicionCasillasDerecha, Quaternion.identity);
                }

            }
            else{

                foreach (Collider collider in colliders) {

                    if(Mathf.Approximately(collider.gameObject.transform.eulerAngles.z,180f)){
                        Debug.Log("No crea un paseador hacia la derecha");
                    }
                    else if(collider.gameObject.CompareTag("Pino")){
                        spawnPaseadorDerecha = Instantiate(paseador, posicionCasillasDerecha, Quaternion.identity);
                    }
                    else if(collider.gameObject.CompareTag("Arbol")){
                        spawnPaseadorDerecha = Instantiate(paseador, posicionCasillasDerecha, Quaternion.identity);
                    }
                       
                }
            }

        }
        else{
            Debug.Log("Bloqueo para no ir hacia la derecha");
        }
    }

    void OnTriggerEnter(Collider other) {

        if(gameManager.getRondaActual() == 1){
            if(other.gameObject.CompareTag("Pino")){
                gameManager.SumarPuntosJugador2(2);
                gameManager.SumarFichasObtenidasJugador2(1);
                audioSource.PlayOneShot(sonidoEliminar,1f);
                baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().QuitarDisponibilidadFichaPasada(other.gameObject);
            }
            else if(other.gameObject.CompareTag("Arbol")){
                gameManager.SumarPuntosJugador2(2);
                gameManager.SumarFichasObtenidasJugador2(1);
                audioSource.PlayOneShot(sonidoEliminar,1f);
                baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().QuitarDisponibilidadFichaPasada(other.gameObject);
            }
        }
        else if(gameManager.getRondaActual() == 2){
            if(other.gameObject.CompareTag("Pino")){
                gameManager.SumarPuntosJugador1(2);
                gameManager.SumarFichasObtenidasJugador1(1);
                audioSource.PlayOneShot(sonidoEliminar,1f);
                baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().QuitarDisponibilidadFichaPasada2(other.gameObject);
            }
            else if(other.gameObject.CompareTag("Arbol")){
                gameManager.SumarPuntosJugador1(2);
                gameManager.SumarFichasObtenidasJugador1(1);
                audioSource.PlayOneShot(sonidoEliminar,1f);
                baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().QuitarDisponibilidadFichaPasada2(other.gameObject);
            }
        }

            
    }

    void PoderMoverFicha(){

        if(gameManager.getRondaActual() == 1){

            if(gameManager.turnoActual.ToString() == "Humano" && gameManager.jugadorActual.ToString() == "J2"){

                LimitesPaseador();
            
            }
            else if(gameManager.turnoActual.ToString() == "Animal" && gameManager.jugadorActual.ToString() == "J1"){
                Debug.Log("No es el turno de mover esta ficha");
            }

        }
        else if(gameManager.getRondaActual() == 2){

            if(gameManager.turnoActual.ToString() == "Humano" && gameManager.jugadorActual.ToString() == "J1"){

                LimitesPaseador();
            
            }
            else if(gameManager.turnoActual.ToString() == "Animal" && gameManager.jugadorActual.ToString() == "J2"){
                Debug.Log("No es el turno de mover esta ficha");
            }
        }

        
    }

    private void ActualizarPosicionFicha(){
        if(tocarPaseador){

            posXAnterior = posX;
            posYAnterior = posY;

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

    private void BorrarPaseadores(){
        Destroy(spawnPaseadorArriba);
        Destroy(spawnPaseadorIzquierda);
        Destroy(spawnPaseadorDerecha);
        Destroy(spawnPaseadorAbajo);

        Destroy(spawnPaseadorSalidaArriba);
        Destroy(spawnPaseadorSalidaAbajo);
        Destroy(spawnPaseadorSalidaIzquierda);
        Destroy(spawnPaseadorSalidaDerecha);
    }

    private void ActivarSalida(){

        if(gameManager.getRondaActual() == 1){
            if(gameManager.turnoActual.ToString() == "Humano" && gameManager.jugadorActual.ToString() == "J2"){

                if(posX == 3 && posY == 0 && gameManager.getTurnosAlternadosActivados()){
                    posicionSalidaArriba = new Vector3(3 * separacionEntreCasillas, 0f , -1 * separacionEntreCasillas);
                    spawnPaseadorSalidaArriba = Instantiate(paseadorSalida, posicionSalidaArriba, Quaternion.identity);
                }
                else if(posX == 3 && posY == 6 && gameManager.getTurnosAlternadosActivados()){
                    posicionSalidaAbajo = new Vector3(3 * separacionEntreCasillas , 0f , 7 * separacionEntreCasillas);
                    spawnPaseadorSalidaAbajo = Instantiate(paseadorSalida, posicionSalidaAbajo, Quaternion.identity);
                }
                else if(posX == 6 && posY == 3 && gameManager.getTurnosAlternadosActivados()){
                    posicionSalidaIzquierda = new Vector3(7 * separacionEntreCasillas , 0f , 3 * separacionEntreCasillas);
                    spawnPaseadorSalidaIzquierda = Instantiate(paseadorSalida, posicionSalidaIzquierda, Quaternion.identity);
                }
                else if(posX == 0 && posY == 3 && gameManager.getTurnosAlternadosActivados()){
                    posicionSalidaDerecha = new Vector3(-1 * separacionEntreCasillas , 0f , 3 * separacionEntreCasillas);
                    spawnPaseadorSalidaDerecha = Instantiate(paseadorSalida, posicionSalidaDerecha, Quaternion.identity);
                }

            }
            else if(gameManager.turnoActual.ToString() == "Animal" && gameManager.jugadorActual.ToString() == "J1"){
                Debug.Log("La salida solo se activa si estas en tu turno");
            }
        }
        else if(gameManager.getRondaActual() == 2){

            if(gameManager.turnoActual.ToString() == "Humano" && gameManager.jugadorActual.ToString() == "J1"){

                if(posX == 3 && posY == 0 && gameManager.getTurnosAlternadosActivados()){
                    posicionSalidaArriba = new Vector3(3 * separacionEntreCasillas, 0f , -1 * separacionEntreCasillas);
                    spawnPaseadorSalidaArriba = Instantiate(paseadorSalida, posicionSalidaArriba, Quaternion.identity);
                }
                else if(posX == 3 && posY == 6 && gameManager.getTurnosAlternadosActivados()){
                    posicionSalidaAbajo = new Vector3(3 * separacionEntreCasillas , 0f , 7 * separacionEntreCasillas);
                    spawnPaseadorSalidaAbajo = Instantiate(paseadorSalida, posicionSalidaAbajo, Quaternion.identity);
                }
                else if(posX == 6 && posY == 3 && gameManager.getTurnosAlternadosActivados()){
                    posicionSalidaIzquierda = new Vector3(7 * separacionEntreCasillas , 0f , 3 * separacionEntreCasillas);
                    spawnPaseadorSalidaIzquierda = Instantiate(paseadorSalida, posicionSalidaIzquierda, Quaternion.identity);
                }
                else if(posX == 0 && posY == 3 && gameManager.getTurnosAlternadosActivados()){
                    posicionSalidaDerecha = new Vector3(-1 * separacionEntreCasillas , 0f , 3 * separacionEntreCasillas);
                    spawnPaseadorSalidaDerecha = Instantiate(paseadorSalida, posicionSalidaDerecha, Quaternion.identity);
                }

            }
            else if(gameManager.turnoActual.ToString() == "Animal" && gameManager.jugadorActual.ToString() == "J2"){
                Debug.Log("La salida solo se activa si estas en tu turno");
            }
        }


        
    }

    private void RescatarFichaLeñador(){

        if(gameManager.getRondaActual() == 1){

            if(posX == 3 && posY == -1){
                gameManager.EliminarFichaJugador2(1);
                gameManager.SumarPuntosJugador2(5);
                gameManager.SumarFichasObtenidasJugador2(1);
                baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().QuitarDisponibilidadFichaPasada(gameObject);
            }
            else if(posX == 3 && posY == 7){
                gameManager.EliminarFichaJugador2(1);
                gameManager.SumarPuntosJugador2(5);
                gameManager.SumarFichasObtenidasJugador2(1);
                baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().QuitarDisponibilidadFichaPasada(gameObject);
            }
            else if(posX == 7 && posY == 3){
                gameManager.EliminarFichaJugador2(1);
                gameManager.SumarPuntosJugador2(5);
                gameManager.SumarFichasObtenidasJugador2(1);
                baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().QuitarDisponibilidadFichaPasada(gameObject);
            }
            else if(posX == -1 && posY == 3){
                gameManager.EliminarFichaJugador2(1);
                gameManager.SumarPuntosJugador2(5);
                gameManager.SumarFichasObtenidasJugador2(1);
                baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().QuitarDisponibilidadFichaPasada(gameObject);
            }

        }
        else if(gameManager.getRondaActual() == 2){

            if(posX == 3 && posY == -1){
                gameManager.EliminarFichaJugador1(1);
                gameManager.SumarPuntosJugador1(5);
                gameManager.SumarFichasObtenidasJugador1(1);
                baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().QuitarDisponibilidadFichaPasada2(gameObject);
            }
            else if(posX == 3 && posY == 7){
                gameManager.EliminarFichaJugador1(1);
                gameManager.SumarPuntosJugador1(5);
                gameManager.SumarFichasObtenidasJugador1(1);
                baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().QuitarDisponibilidadFichaPasada2(gameObject);
            }
            else if(posX == 7 && posY == 3){
                gameManager.EliminarFichaJugador1(1);
                gameManager.SumarPuntosJugador1(5);
                gameManager.SumarFichasObtenidasJugador1(1);
                baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().QuitarDisponibilidadFichaPasada2(gameObject);
            }
            else if(posX == -1 && posY == 3){
                gameManager.EliminarFichaJugador1(1);
                gameManager.SumarPuntosJugador1(5);
                gameManager.SumarFichasObtenidasJugador1(1);
                baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().QuitarDisponibilidadFichaPasada2(gameObject);
            }

        }

    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverFichaCazador : MonoBehaviour
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
    public int posXAnterior = -1;
    public int posYAnterior = -1;

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

    //Spawneadores de los limitadores
    private GameObject spawnLimitadorArriba;
    private GameObject spawnLimitadorIzquierda;
    private GameObject spawnLimitadorDerecha;
    private GameObject spawnLimitadorAbajo;

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

    //No permitir deshacer movimiento
    public bool poderMoverFichaCazador = true;

    public int turnoAnterior = 0;

    private GameManager gameManager;

    private GameObject baseDatosJuego;

    //Añadir sonido eliminar ficha
    public AudioClip sonidoEliminar;
    public AudioClip sonidoVuelta;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        camaraMain = Camera.main;
        CalcularPosicion();

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        baseDatosJuego = GameObject.Find("BaseDatosJuego");

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if((turnoAnterior + 2) == gameManager.turnosRegistrados){
            poderMoverFichaCazador = false;
        }
        else{
            poderMoverFichaCazador = true;
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
        
        ActualizarPosicionFicha();
        BorrarPaseadores();
        RescatarFichaCazador();

        //tocarPaseador = false;
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
        
        // Bloqueo para no salir del tablero hacia arriba
        if (posX >= 0 && posX <= 6 && posY >= 0 && posY <= 6) {
            bool colisionConClonArriba = false;

            for (int y = posY - 1; y >= 0; y--) {
                posicionCasillasArriba = new Vector3(posX * separacionEntreCasillas, 0, y * separacionEntreCasillas);
                spawnPaseadorArriba = Instantiate(paseador, posicionCasillasArriba, Quaternion.identity);
                listaPaseadoresArriba.Add(spawnPaseadorArriba);

                if(y == posYAnterior && !poderMoverFichaCazador){
                    Destroy(listaPaseadoresArriba[listaPaseadoresArriba.Count - 1]);
                    spawnLimitadorArriba = Instantiate(limitador, posicionCasillasArriba, Quaternion.identity);
                    break;
                }

                Collider[] colliders = Physics.OverlapSphere(posicionCasillasArriba, 0.5f);

                foreach (Collider collider in colliders) {

                    if(Mathf.Approximately(collider.gameObject.transform.eulerAngles.z,180f)){
                        colisionConClonArriba = true;
                        Destroy(listaPaseadoresArriba[listaPaseadoresArriba.Count - 1]);
                        break;
                    }
                    else if(collider.gameObject.CompareTag("Pato") || collider.gameObject.CompareTag("Pavo") || collider.gameObject.CompareTag("Oso") || collider.gameObject.CompareTag("Zorro")){
                        if(transform.localRotation == Quaternion.Euler(0,270f,0)){
                            colisionConClonArriba = true;
                            Destroy(spawnPaseadorSalidaArriba); 
                            break;
                        }
                        else{
                            colisionConClonArriba = true;
                            Destroy(listaPaseadoresArriba[listaPaseadoresArriba.Count - 1]);
                            Destroy(spawnPaseadorSalidaArriba);                            
                            break;
                        }
                    }
                    else if(collider.gameObject.CompareTag("Pino") || collider.gameObject.CompareTag("Cazador") || collider.gameObject.CompareTag("Leñador") || collider.gameObject.CompareTag("Arbol")){
                        colisionConClonArriba = true;
                        Destroy(listaPaseadoresArriba[listaPaseadoresArriba.Count - 1]);
                        Destroy(spawnPaseadorSalidaArriba);
                        break;
                    }

                    if(gameManager.getTurnosAlternadosActivados()){

                        if(collider.transform.position.x == 3 && collider.transform.position.z == 0){
                            posicionSalidaArriba = new Vector3(3 * separacionEntreCasillas , 0f , -1 * separacionEntreCasillas);
                            spawnPaseadorSalidaArriba = Instantiate(paseadorSalida, posicionSalidaArriba, Quaternion.identity);
                        }

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

                if(y == posYAnterior && !poderMoverFichaCazador){
                    Destroy(listaPaseadoresAbajo[listaPaseadoresAbajo.Count - 1]);
                    spawnLimitadorAbajo = Instantiate(limitador, posicionCasillasAbajo, Quaternion.identity);
                    break;
                }

                Collider[] colliders = Physics.OverlapSphere(posicionCasillasAbajo, 0.5f);

                foreach (Collider collider in colliders) {

                    if(Mathf.Approximately(collider.gameObject.transform.eulerAngles.z,180f)){
                        colisionConClonAbajo = true;
                        Destroy(listaPaseadoresAbajo[listaPaseadoresAbajo.Count - 1]);
                        break;
                    }
                    else if(collider.gameObject.CompareTag("Pato") || collider.gameObject.CompareTag("Pavo") || collider.gameObject.CompareTag("Oso") || collider.gameObject.CompareTag("Zorro")){
                        if(transform.localRotation == Quaternion.Euler(0,90f,0)){
                            colisionConClonAbajo = true;
                            Destroy(spawnPaseadorSalidaAbajo);
                            break;
                        }
                        else{
                            colisionConClonAbajo = true;
                            Destroy(listaPaseadoresAbajo[listaPaseadoresAbajo.Count - 1]);
                            Destroy(spawnPaseadorSalidaAbajo);
                            break;
                        }
                    }
                    else if(collider.gameObject.CompareTag("Pino") || collider.gameObject.CompareTag("Cazador") || collider.gameObject.CompareTag("Leñador") || collider.gameObject.CompareTag("Arbol")){
                        colisionConClonAbajo = true;
                        Destroy(listaPaseadoresAbajo[listaPaseadoresAbajo.Count - 1]);
                        Destroy(spawnPaseadorSalidaAbajo);
                        break;
                    }

                    if(gameManager.getTurnosAlternadosActivados()){

                        if(collider.transform.position.x == 3 && collider.transform.position.z == 6){
                            posicionSalidaAbajo = new Vector3(3 * separacionEntreCasillas , 0f , 7 * separacionEntreCasillas);
                            spawnPaseadorSalidaAbajo = Instantiate(paseadorSalida, posicionSalidaAbajo, Quaternion.identity);
                        }
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

                if(x == posXAnterior && !poderMoverFichaCazador){
                    Destroy(listaPaseadoresIzquierda[listaPaseadoresIzquierda.Count - 1]);
                    spawnLimitadorIzquierda = Instantiate(limitador, posicionCasillasIzquierda, Quaternion.identity);
                    break;
                }

                Collider[] colliders = Physics.OverlapSphere(posicionCasillasIzquierda, 0.5f);

                foreach (Collider collider in colliders) {

                    if(Mathf.Approximately(collider.gameObject.transform.eulerAngles.z,180f)){
                        colisionConClonIzquierda = true;
                        Destroy(listaPaseadoresIzquierda[listaPaseadoresIzquierda.Count - 1]);
                        break;
                    }
                    else if(collider.gameObject.CompareTag("Pato") || collider.gameObject.CompareTag("Pavo") || collider.gameObject.CompareTag("Oso") || collider.gameObject.CompareTag("Zorro")){
                        if(transform.localRotation == Quaternion.Euler(0,180f,0)){
                            colisionConClonIzquierda = true;
                            Destroy(spawnPaseadorSalidaIzquierda);
                            break;
                        }
                        else{
                            colisionConClonIzquierda = true;
                            Destroy(listaPaseadoresIzquierda[listaPaseadoresIzquierda.Count - 1]);
                            Destroy(spawnPaseadorSalidaIzquierda);
                            break;
                        }
                    }
                    else if(collider.gameObject.CompareTag("Pino") || collider.gameObject.CompareTag("Cazador") || collider.gameObject.CompareTag("Leñador") || collider.gameObject.CompareTag("Arbol")){
                        colisionConClonIzquierda = true;
                        Destroy(listaPaseadoresIzquierda[listaPaseadoresIzquierda.Count - 1]);
                        Destroy(spawnPaseadorSalidaIzquierda);
                        break;
                    }

                    if(gameManager.getTurnosAlternadosActivados()){

                        if(collider.transform.position.x == 6 && collider.transform.position.z == 3){
                            posicionSalidaIzquierda = new Vector3(7 * separacionEntreCasillas , 0f , 3 * separacionEntreCasillas);
                            spawnPaseadorSalidaIzquierda = Instantiate(paseadorSalida, posicionSalidaIzquierda, Quaternion.identity);
                        }

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

                if(x == posXAnterior && !poderMoverFichaCazador){
                    Destroy(listaPaseadoresDerecha[listaPaseadoresDerecha.Count - 1]);
                    spawnLimitadorDerecha = Instantiate(limitador, posicionCasillasDerecha, Quaternion.identity);
                    break;
                }

                Collider[] colliders = Physics.OverlapSphere(posicionCasillasDerecha, 0.5f);

                foreach (Collider collider in colliders) {


                    if(Mathf.Approximately(collider.gameObject.transform.eulerAngles.z,180f)){
                        colisionConClonDerecha = true;
                        Destroy(listaPaseadoresDerecha[listaPaseadoresDerecha.Count - 1]);
                        break;
                    }
                    else if(collider.gameObject.CompareTag("Pato") || collider.gameObject.CompareTag("Pavo") || collider.gameObject.CompareTag("Oso") || collider.gameObject.CompareTag("Zorro")){
                        if(transform.localRotation == Quaternion.Euler(0,0,0)){
                            colisionConClonDerecha = true;
                            Destroy(spawnPaseadorSalidaDerecha);
                            break;
                        }
                        else{
                            colisionConClonDerecha = true;
                            Destroy(listaPaseadoresDerecha[listaPaseadoresDerecha.Count - 1]);
                            Destroy(spawnPaseadorSalidaDerecha);
                            break;
                        }
                    }
                    else if(collider.gameObject.CompareTag("Pino") || collider.gameObject.CompareTag("Cazador") || collider.gameObject.CompareTag("Leñador") || collider.gameObject.CompareTag("Arbol")){
                        colisionConClonDerecha = true;
                        Destroy(listaPaseadoresDerecha[listaPaseadoresDerecha.Count - 1]);
                        Destroy(spawnPaseadorSalidaDerecha);
                        break;
                    }

                    if(gameManager.getTurnosAlternadosActivados()){

                        if(collider.transform.position.x == 0 && collider.transform.position.z == 3){
                            posicionSalidaDerecha = new Vector3(-1 * separacionEntreCasillas , 0f , 3 * separacionEntreCasillas);
                            spawnPaseadorSalidaDerecha = Instantiate(paseadorSalida, posicionSalidaDerecha, Quaternion.identity);
                        }
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

        Destroy(spawnLimitadorArriba);
        Destroy(spawnLimitadorIzquierda);
        Destroy(spawnLimitadorDerecha);
        Destroy(spawnLimitadorAbajo);

        Destroy(spawnPaseadorSalidaArriba);
        Destroy(spawnPaseadorSalidaAbajo);
        Destroy(spawnPaseadorSalidaIzquierda);
        Destroy(spawnPaseadorSalidaDerecha);
    }

    private void ActualizarPosicionFicha(){
        if(tocarPaseador){
            posXAnterior = posX;
            posYAnterior = posY;

            posX = posXPaseador;
            posY = posYPaseador;

            transform.position = new Vector3(posX,0f,posY);

            gameManager.DescontarTurno(1);

            turnoAnterior = gameManager.turnosRegistrados;

            gameManager.CambiarTurno(); 
        }
        else{
            Debug.Log("No se movio ficha al final");
        }
    }

    private void OnTriggerEnter(Collider other) {

        if(gameManager.getRondaActual() == 1){

            if (other.gameObject.CompareTag("Pato")){
                gameManager.SumarPuntosJugador2(2);
                gameManager.SumarFichasObtenidasJugador2(1);
                audioSource.PlayOneShot(sonidoEliminar,1f);
                baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().QuitarDisponibilidadFichaPasada(other.gameObject);
            }
            else if (other.gameObject.CompareTag("Pavo")){
                gameManager.SumarPuntosJugador2(3);
                gameManager.SumarFichasObtenidasJugador2(1);
                audioSource.PlayOneShot(sonidoEliminar,1f);
                baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().QuitarDisponibilidadFichaPasada(other.gameObject);
            }
            else if (other.gameObject.CompareTag("Zorro")){
                gameManager.SumarPuntosJugador2(5);
                gameManager.SumarFichasObtenidasJugador2(1);
                gameManager.EliminarFichaJugador1(1);
                audioSource.PlayOneShot(sonidoEliminar,1f);
                baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().QuitarDisponibilidadFichaPasada(other.gameObject);
            }
            else if (other.gameObject.CompareTag("Oso")){
                audioSource.PlayOneShot(sonidoEliminar,1f);
            }
        }
        else if(gameManager.getRondaActual() == 2){

            if (other.gameObject.CompareTag("Pato")){
                gameManager.SumarPuntosJugador1(2);
                gameManager.SumarFichasObtenidasJugador1(1);
                audioSource.PlayOneShot(sonidoEliminar,1f);
                baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().QuitarDisponibilidadFichaPasada2(other.gameObject);
            }
            else if (other.gameObject.CompareTag("Pavo")){
                gameManager.SumarPuntosJugador1(3);
                gameManager.SumarFichasObtenidasJugador1(1);
                audioSource.PlayOneShot(sonidoEliminar,1f);
                baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().QuitarDisponibilidadFichaPasada2(other.gameObject);
            }
            else if (other.gameObject.CompareTag("Zorro")){
                gameManager.SumarPuntosJugador1(5);
                gameManager.SumarFichasObtenidasJugador1(1);
                gameManager.EliminarFichaJugador2(1);
                audioSource.PlayOneShot(sonidoEliminar,1f);
                baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().QuitarDisponibilidadFichaPasada2(other.gameObject);
            }
            else if (other.gameObject.CompareTag("Oso")){
                audioSource.PlayOneShot(sonidoEliminar,1f);
            }
        }
        
    }

    void PoderMoverFicha(){

        if(gameManager.getRondaActual() == 1){
            if(gameManager.turnoActual.ToString() == "Humano" && gameManager.jugadorActual.ToString() == "J2"){
                clickIzquierdoActivado = true;

                gameManager.morirCazador = false;

                LimitesPaseador();
                
            }
            else if(gameManager.turnoActual.ToString() == "Animal" && gameManager.jugadorActual.ToString() == "J1"){
                return;
            }
        }
        else if(gameManager.getRondaActual() == 2){

            if(gameManager.turnoActual.ToString() == "Humano" && gameManager.jugadorActual.ToString() == "J1"){
                clickIzquierdoActivado = true;

                gameManager.morirCazador = false;

                LimitesPaseador();
                
            }
            else if(gameManager.turnoActual.ToString() == "Animal" && gameManager.jugadorActual.ToString() == "J2"){
                return;
            }
        }

        
    }

    //Si estas en la casilla de una de las salidas se activara un paseador para poder salir
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

    private void RescatarFichaCazador(){

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

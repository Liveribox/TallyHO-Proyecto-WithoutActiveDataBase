using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;

public class MontarTablero2 : MonoBehaviour
{

    MongoClient client = new MongoClient("");
    IMongoDatabase database;
    IMongoCollection<BsonDocument> collectionPartidas;

    //Fichas del juego
    public List<GameObject> fichas = new List<GameObject>();

    public GameObject fichaTapadora;

    //Tamaño de las fichas y separacion entre ellas
    private int casillasX = 7;
    private int casillasY = 7;
    private float separacionEntreCasillas = 1f;

    //Posicion de las fichas y rotacion aleatorias
    private Vector3 posicionCasillas;

    private Quaternion rotacionFichas;

    //Contadores de las fichas
    private int contadorCazador = 0; 
    private int contadorOso = 0;

    private int contadorLeñador = 0;

    private int contadorPino = 0;

    private int contadorArbol = 0;

    private int contadorPavo = 0;
    private int contadorPato = 0;
    private int contadorZorro = 0;

    private GameManager gameManager;

    private string nombreActual = "";
    private int contadorActual = 0;

    private GameObject baseDatosJuego;

    private string partidaPasada = "";

    // Start is called before the first frame update
    async void Start()
    {

        database = client.GetDatabase("TallyHODB!");
        collectionPartidas = database.GetCollection<BsonDocument>("Partidas");

        partidaPasada = PlayerPrefs.GetString("partidaPasado");


        var filtroPartida = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
        var datosPartida = await collectionPartidas.Find(filtroPartida).FirstOrDefaultAsync();

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        baseDatosJuego = GameObject.Find("BaseDatosJuego");

        if(datosPartida["montarTablero2PrimeraVez"] == true){

            Debug.Log("Montar tablero 2 por primera vez");
            
            gameManager.CambiarRolesTurno();
            MontarTableroo();
            baseDatosJuego.GetComponent<AccesoBaseDatosJuego>().GuardarPartida();
        }
        else if(datosPartida["montarTablero2PrimeraVez"] == true){
            Debug.Log("Ya se ha montado deja de molestar");
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        DecirGanador();
    }

    void MontarTableroo(){
        for (int x = 0; x < casillasX; x++){
            for (int y = 0; y < casillasY; y++){

                // No spawea ninguna ficha en esa casilla
                if(x == 3 && y == 3){ 
                    continue;
                }

                //Con ella repartes fichas aleatorias
                int fichaRandom = Random.Range(0, fichas.Count-1);

                // Rotacion aleatorias y sus opciones
                int fichaRotacionRandom = Random.Range(0,4);

                if (fichaRotacionRandom == 0){
                    rotacionFichas = Quaternion.Euler(180f,0f,0f);
                }
                else if(fichaRotacionRandom == 1){
                    rotacionFichas = Quaternion.Euler(180f,90f,0f);
                }
                else if(fichaRotacionRandom == 2){
                    rotacionFichas = Quaternion.Euler(180f,180f,0f);
                }
                else if(fichaRotacionRandom == 3){
                    rotacionFichas = Quaternion.Euler(180f,270f,0f);
                }
                else{
                    Debug.Log("Rotacion fallida en ficha: " + x + " - " + y);
                }

                
                //Repartir fichas
                if (fichas[fichaRandom].CompareTag("Cazador") && contadorCazador < 8)
                {
                    contadorActual = contadorCazador++;
                    nombreActual = "Cazador";
                }
                else if (fichas[fichaRandom].CompareTag("Leñador") && contadorLeñador < 2)
                {
                    contadorActual = contadorLeñador++;
                    nombreActual = "Leñador";
                }
                else if (fichas[fichaRandom].CompareTag("Zorro")  && contadorZorro < 6)
                {
                    contadorActual = contadorZorro++;
                    nombreActual = "Zorro";
                }
                else if (fichas[fichaRandom].CompareTag("Oso") && contadorOso < 2)
                {
                    contadorActual = contadorOso++;
                    nombreActual = "Oso";
                }
                else if (fichas[fichaRandom].CompareTag("Pato") && contadorPato < 7)
                {
                    contadorActual = contadorPato++;
                    nombreActual = "Pato";
                }
                else if (fichas[fichaRandom].CompareTag("Pavo") && contadorPavo < 8)
                {
                    contadorActual = contadorPavo++;
                    nombreActual = "Pavo";
                }
                else if (fichas[fichaRandom].CompareTag("Pino") && contadorPino < 7)
                {
                    contadorActual = contadorPino++;
                    nombreActual = "Pino";
                }
                else if(fichas[fichaRandom].CompareTag("Arbol") && contadorArbol < 8)
                {
                    contadorActual = contadorArbol++;
                    nombreActual = "Arbol";
                }
                else
                {
                    
                    if (fichas[fichaRandom].name == "Cazador" && contadorCazador >= 8)
                    {
                        
                        if(contadorZorro < 6){
                            contadorActual = contadorZorro++;
                            nombreActual = "Zorro";

                            fichas[fichaRandom] = fichas[5];

                        }
                        else if (contadorLeñador < 2){
                            contadorActual = contadorLeñador++;
                            nombreActual = "Leñador";
                            fichas[fichaRandom] = fichas[1];

                        }
                        else if(contadorOso < 2){
                            contadorActual = contadorOso++;
                            nombreActual = "Oso";
                            fichas[fichaRandom] = fichas[2];

                        }
                        else if(contadorArbol < 8){
                            contadorActual = contadorArbol++;
                            nombreActual = "Arbol";
                            fichas[fichaRandom] = fichas[7];

                        }
                        else if(contadorPato < 7){
                            contadorActual = contadorPato++;
                            nombreActual = "Pato";
                            fichas[fichaRandom] = fichas[3];

                        }
                        else if(contadorPino < 7){
                            contadorActual = contadorPino++;
                            nombreActual = "Pino";
                            fichas[fichaRandom] = fichas[6];

                        }
                        else if(contadorPavo < 8){
                            contadorActual = contadorPavo++;
                            nombreActual = "Pavo";
                            fichas[fichaRandom] = fichas[4];

                        }
                        
                        

                    }
                    else if (fichas[fichaRandom].name == "Leñador" && contadorLeñador >= 2)
                    {

                        if(contadorPino < 7){
                            contadorActual = contadorPino++;
                            nombreActual = "Pino";
                            fichas[fichaRandom] = fichas[6];

                        } 
                        else if(contadorArbol < 8){
                            contadorActual = contadorArbol++;
                            nombreActual = "Arbol";
                            fichas[fichaRandom] = fichas[7];

                        }
                        else if(contadorZorro < 6){
                            contadorActual = contadorZorro++;
                            nombreActual = "Zorro";

                            fichas[fichaRandom] = fichas[5];

                        }
                        else if(contadorOso < 2){
                            contadorActual = contadorOso++;
                            nombreActual = "Oso";
                            fichas[fichaRandom] = fichas[2];

                        }
                        else if(contadorPato < 7){
                            contadorActual = contadorPato++;
                            nombreActual = "Pato";
                            fichas[fichaRandom] = fichas[3];

                        }
                        else if(contadorPavo < 8){
                            contadorActual = contadorPavo++;
                            nombreActual = "Pavo";
                            fichas[fichaRandom] = fichas[4];

                        }
                        else if (contadorCazador < 8){
                            contadorActual = contadorCazador++;
                            nombreActual = "Cazador";
                            fichas[fichaRandom] = fichas[0];

                        }
                        
                         
                    }
                    else if (fichas[fichaRandom].name == "Oso" && contadorOso >= 2)
                    {

                        if (contadorLeñador < 2){
                            contadorActual = contadorLeñador++;
                            nombreActual = "Leñador";
                            fichas[fichaRandom] = fichas[1];

                        }
                        else if(contadorPino < 7){
                            contadorActual = contadorPino++;
                            nombreActual = "Pino";
                            fichas[fichaRandom] = fichas[6];

                        }
                        else if(contadorPavo < 8){
                            contadorActual = contadorPavo++;
                            nombreActual = "Pavo";
                            fichas[fichaRandom] = fichas[4];

                        }
                        else if(contadorZorro < 6){
                            contadorActual = contadorZorro++;
                            nombreActual = "Zorro";

                            fichas[fichaRandom] = fichas[5];

                        }
                        else if(contadorCazador < 8){
                            contadorActual = contadorCazador++;
                            nombreActual = "Cazador";
                            fichas[fichaRandom] = fichas[0];

                        }
                        else if(contadorPato < 7){
                            contadorActual = contadorPato++;
                            nombreActual = "Pato";
                            fichas[fichaRandom] = fichas[3];

                        }
                        else if(contadorArbol < 8){
                            contadorActual = contadorArbol++;
                            nombreActual = "Arbol";
                            fichas[fichaRandom] = fichas[7];

                        }
                        
                        
                        
                    }
                    else if (fichas[fichaRandom].name == "Pato" && contadorPato >= 7)
                    {

                        if(contadorPavo < 8){
                            contadorActual = contadorPavo++;
                            nombreActual = "Pavo";
                            fichas[fichaRandom] = fichas[4];

                        }
                        else if (contadorLeñador < 2){
                            contadorActual = contadorLeñador++;
                            nombreActual = "Leñador";
                            fichas[fichaRandom] = fichas[1];

                        }
                        else if(contadorZorro < 6){
                            contadorActual = contadorZorro++;
                            nombreActual = "Zorro";
                            fichas[fichaRandom] = fichas[5];

                        }
                        else if(contadorArbol < 8){
                            contadorActual = contadorArbol++;
                            nombreActual = "Arbol";
                            fichas[fichaRandom] = fichas[7];

                        }
                        else if(contadorOso < 2){
                            contadorActual = contadorOso++;
                            nombreActual = "Oso";
                            fichas[fichaRandom] = fichas[2];

                        }
                        else if(contadorPino < 7){
                            contadorActual = contadorPino++;
                            nombreActual = "Pino";
                            fichas[fichaRandom] = fichas[6];

                        }
                        else if(contadorCazador < 8){
                            contadorActual = contadorCazador++;
                            nombreActual = "Cazador";
                            fichas[fichaRandom] = fichas[0];

                        }
 
                    }
                    else if (fichas[fichaRandom].name == "Pavo" && contadorPavo >= 8)
                    {

                        if (contadorLeñador < 2){
                            contadorActual = contadorLeñador++;
                            nombreActual = "Leñador";
                            fichas[fichaRandom] = fichas[1];

                        }
                        else if(contadorZorro < 6){
                            contadorActual = contadorZorro++;
                            nombreActual = "Zorro";
                            fichas[fichaRandom] = fichas[5];

                        }
                        else if(contadorOso < 2){
                            contadorActual = contadorOso++;
                            nombreActual = "Oso";
                            fichas[fichaRandom] = fichas[2];

                        }
                        else if(contadorPino < 7){
                            contadorActual = contadorPino++;
                            nombreActual = "Pino";
                            fichas[fichaRandom] = fichas[6];

                        }
                        else if(contadorArbol < 8){
                            contadorActual = contadorArbol++;
                            nombreActual = "Arbol";
                            fichas[fichaRandom] = fichas[7];

                        }
                        else if(contadorPato < 7){
                            contadorActual = contadorPato++;
                            nombreActual = "Pato";
                            fichas[fichaRandom] = fichas[3];

                        }
                        else if(contadorCazador < 8){
                            contadorActual = contadorCazador++;
                            nombreActual = "Cazador";
                            fichas[fichaRandom] = fichas[0];

                        }
                        
                    }
                    else if (fichas[fichaRandom].name == "Zorro" && contadorZorro >= 6)
                    {   
                        if(contadorCazador < 8){
                            contadorActual = contadorCazador++;
                            nombreActual = "Cazador";
                            fichas[fichaRandom] = fichas[0];

                        }
                        else if (contadorLeñador < 2){
                            contadorActual = contadorLeñador++;
                            nombreActual = "Leñador";
                            fichas[fichaRandom] = fichas[1];

                        }
                        else if(contadorOso < 2){
                            contadorActual = contadorOso++;
                            nombreActual = "Oso";
                            fichas[fichaRandom] = fichas[2];

                        }
                        else if(contadorPato < 7){
                            contadorActual = contadorPato++;
                            nombreActual = "Pato";
                            fichas[fichaRandom] = fichas[3];

                           
                        }
                        else if(contadorArbol < 8){
                            contadorActual = contadorArbol++;
                            nombreActual = "Arbol";
                            fichas[fichaRandom] = fichas[7];

                        }
                        else if(contadorPino < 7){
                            contadorActual = contadorPino++;
                            nombreActual = "Pino";
                            fichas[fichaRandom] = fichas[6];

                           
                        }
                        else if(contadorPavo < 8){
                            contadorActual = contadorPavo++;
                            nombreActual = "Pavo";
                            fichas[fichaRandom] = fichas[4];

                            
                        }
                        
                        
                        
                    }
                    else if (fichas[fichaRandom].name == "Pino" && contadorPino >= 7)
                    {
                        

                        if (contadorLeñador < 2){
                            contadorActual = contadorLeñador++;
                            nombreActual = "Leñador";
                            fichas[fichaRandom] = fichas[1];

                            
                        }
                        else if(contadorArbol < 8){
                            contadorActual = contadorArbol++;
                            nombreActual = "Arbol";
                            fichas[fichaRandom] = fichas[7];

                        }
                        else if(contadorZorro < 6){
                            contadorActual = contadorZorro++;
                            nombreActual = "Zorro";
                            fichas[fichaRandom] = fichas[5];

                           
                        }
                        else if(contadorOso < 2){
                            contadorActual = contadorOso++;
                            nombreActual = "Oso";
                            fichas[fichaRandom] = fichas[2];

                           
                        }
                        else if(contadorPato < 7){
                            contadorActual = contadorPato++;
                            nombreActual = "Pato";
                            fichas[fichaRandom] = fichas[3];

                           
                        }
                        else if(contadorPavo < 8){
                            contadorActual = contadorPavo++;
                            nombreActual = "Pavo";
                            fichas[fichaRandom] = fichas[4];

                           
                        }
                        else if(contadorCazador < 8){
                            contadorActual = contadorCazador++;
                            nombreActual = "Cazador";
                            fichas[fichaRandom] = fichas[0];
                        }
                        
                        

                        
                    }
                    else if(fichas[fichaRandom].name == "Arbol" && contadorArbol >= 8)
                    {
                        if (contadorLeñador < 2){
                            contadorActual = contadorLeñador++;
                            nombreActual = "Leñador";
                            fichas[fichaRandom] = fichas[1];
                        }
                        else if(contadorOso < 2){
                            contadorActual = contadorOso++;
                            nombreActual = "Oso";
                            fichas[fichaRandom] = fichas[2]; 
                        }
                        else if(contadorCazador < 8){
                            contadorActual = contadorCazador++;
                            nombreActual = "Cazador";
                            fichas[fichaRandom] = fichas[0];
                        }
                        else if(contadorPato < 7){
                            contadorActual = contadorPato++;
                            nombreActual = "Pato";
                            fichas[fichaRandom] = fichas[3];
                        }
                        else if(contadorPavo < 8){
                            contadorActual = contadorPavo++;
                            nombreActual = "Pavo";
                            fichas[fichaRandom] = fichas[4];
                        }
                        else if(contadorPino < 7){
                            contadorActual = contadorPino++;
                            nombreActual = "Pino";
                            fichas[fichaRandom] = fichas[6]; 
                        }
                        else if(contadorZorro < 6){
                            contadorActual = contadorZorro++;
                            nombreActual = "Zorro";
                            fichas[fichaRandom] = fichas[5]; 
                        }
                    }
                    
                }

                PonerFicha(x, y, fichaRandom, rotacionFichas,nombreActual,contadorActual);
                
            }
        }

        Debug.Log("Total cazadores: " + contadorCazador);
    }

    void DecirGanador(){
        if(gameManager.getNumeroFichasJugador1() <= 0 || gameManager.getNumeroFichasJugador2() <= 0 || gameManager.getTurnosJugador1Numero() <= 0 && gameManager.getTurnosJugador2Numero() <= 0){
            
            gameManager.ActivarYDesactivarBotones(true,false,true);

            StartCoroutine(EsperarUnMomento());

            if(gameManager.getPuntosJugador1() > gameManager.getPuntosJugador2()){
                gameManager.fondoGanador.SetActive(true);
                gameManager.textoGanador.text = gameManager.getJugador1() + " Gana";
            }
            else if(gameManager.getPuntosJugador2() > gameManager.getPuntosJugador1()){
                gameManager.fondoGanador.SetActive(true);
                gameManager.textoGanador.text = gameManager.getJugador2() + " Gana";
            }
            else if(gameManager.getPuntosJugador1() == gameManager.getPuntosJugador2()){

                if(gameManager.getFichasObtenidasJugador1() > gameManager.getFichasObtenidasJugador2()){
                    gameManager.fondoGanador.SetActive(true);
                    gameManager.textoGanador.text = gameManager.getJugador1() + " Gana";
                }
                else if(gameManager.getFichasObtenidasJugador2() > gameManager.getFichasObtenidasJugador1()){
                    gameManager.fondoGanador.SetActive(true);
                    gameManager.textoGanador.text = gameManager.getJugador2() + " Gana";
                }
                else if(gameManager.getFichasObtenidasJugador1() == gameManager.getFichasObtenidasJugador2()){
                    gameManager.fondoGanador.SetActive(true);
                    gameManager.textoGanador.text = "Empate";
                }

            }

            
            
        }
    }

    void PonerFicha(int x,int y,int fichaRandom,Quaternion rotacionFichas,string nombreActual, int contadorActual){
        posicionCasillas = new Vector3(x * separacionEntreCasillas, 0f, y * separacionEntreCasillas);
        GameObject spawnFichas = Instantiate(fichas[fichaRandom], posicionCasillas, rotacionFichas);
        spawnFichas.transform.SetParent(transform);
        contadorActual++;
        spawnFichas.name = nombreActual+contadorActual;
    }

    IEnumerator EsperarUnMomento()
    {
        transform.position = new Vector3(40f,0,0);
        
        yield return new WaitForSeconds(0.5f);

        gameObject.SetActive(false);
        
    }
}

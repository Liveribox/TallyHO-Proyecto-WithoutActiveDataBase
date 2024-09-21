using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
using TMPro;

public class AccesoBaseDatosJuego : MonoBehaviour
{

    MongoClient client = new MongoClient("");
    IMongoDatabase database;
    IMongoCollection<BsonDocument> collectionPartidas;

    public List<GameObject> fichas = new List<GameObject>();
    private string partidaPasada = "";

    private GameManager gameManager;

    public GameObject tablero;

    public GameObject tablero2;

    public GameObject guardarOCargar;

    public TextMeshProUGUI guardarOCargarTexto;

    private int contCazador = 0;
    private int contLeñador = 0;
    private int contOso = 0;
    private int contPato = 0;
    private int contPavo = 0;
    private int contZorro = 0;
    private int contPino = 0;
    private int contArbol = 0;

    private List<string> fichasEliminadas = new List<string>();

    private List<string> fichasEliminadas2 = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        tablero.SetActive(false);
        tablero2.SetActive(false);

        database = client.GetDatabase("TallyHODB!");
        collectionPartidas = database.GetCollection<BsonDocument>("Partidas");

        partidaPasada = PlayerPrefs.GetString("partidaPasado");

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        ObtenerPartida(partidaPasada);

        ObtenerDatosTablero(partidaPasada);
        
    }

    private async void ObtenerDatosTablero(string nombrePartida){

        var filtroPartida = Builders<BsonDocument>.Filter.Eq("partida", nombrePartida);
        var datosPartida = await collectionPartidas.Find(filtroPartida).FirstOrDefaultAsync();

        if(datosPartida["rondaActual"] == 1 && datosPartida["montarTablero1PrimeraVez"] == true){

            tablero.GetComponent<MontarTablero>().MontarTableroo();

            GuardarPartida();

        }
        else if(datosPartida["rondaActual"] == 1 && datosPartida["montarTablero1PrimeraVez"] == false){

            tablero.SetActive(true);

            gameManager.ActivarYDesactivarBotones(false,false,false);

            CargarTablero1();
            CargarFichas1();
        }
        else if(datosPartida["rondaActual"] == 2){ //Cargar datos de tablero 2
            tablero2.SetActive(true);

            gameManager.ActivarYDesactivarBotones(false,false,false);

            CargarTablero2();
            CargarFichas2();

            Debug.Log("CargarTablero 2 y Patida 2 ya era hora ahora me toca ami");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S)){
            GuardarPartida();
        } 
    }

    public async void ObtenerPartida(string nombrePartida)
    {

        var filtroPartida = Builders<BsonDocument>.Filter.Eq("partida", nombrePartida);
        var datosPartida = await collectionPartidas.Find(filtroPartida).FirstOrDefaultAsync();

        if (datosPartida != null)
        {
            string jugador1 = datosPartida["jugador 1"].ToString();
            string jugador2 = datosPartida["jugador 2"].ToString();
            int fichasSinRotar = datosPartida["fichassinrotar"].ToInt32();
            int numeroFichasJugador1 = datosPartida["numeroFichasJugador1"].ToInt32();
            int numeroFichasJugador2 = datosPartida["numeroFichasJugador2"].ToInt32();
            int fichasObtenidasJugador1 = datosPartida["fichasObtenidasJugador1"].ToInt32();
            int fichasObtenidasJugador2 = datosPartida["fichasObtenidasJugador2"].ToInt32();
            int puntosJugador1 = datosPartida["puntosJugador1"].ToInt32();
            int puntosJugador2 = datosPartida["puntosJugador2"].ToInt32();
            int rondaActual = datosPartida["rondaActual"].ToInt32();
            int turnosJugador1Numero = datosPartida["turnosRestantesJugador1"].ToInt32();
            int turnosJugador2Numero = datosPartida["turnosRestantesJugador2"].ToInt32();
            string turnoActual = datosPartida["turnoActual"].ToString();
            string jugadorActual = datosPartida["jugadorActual"].ToString();
            int turnosRegistrados = datosPartida["turnosRegistrados"].ToInt32();

            gameManager.RecibirInformacionUsuario(jugador1,jugador2,fichasSinRotar,numeroFichasJugador1,numeroFichasJugador2,fichasObtenidasJugador1,fichasObtenidasJugador2,puntosJugador1,puntosJugador2,rondaActual,turnosJugador1Numero,turnosJugador2Numero,turnoActual,jugadorActual,turnosRegistrados);
        }
        else
        {
            Debug.Log("Usuario no encontrado.");
        }
    }

    public async void GuardarPartida(){
        gameManager.setPoderClickear(false);

        gameManager.ActivarYDesactivarBotones(false,false,false);

        var filtroPartidaActualizar = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);

        var fichasSinRotar = Builders<BsonDocument>.Update.Set("fichassinrotar", gameManager.getFichasVueltaNumero());
        var numeroFichasJugador1 = Builders<BsonDocument>.Update.Set("numeroFichasJugador1", gameManager.getNumeroFichasJugador1());
        var numeroFichasJugador2 = Builders<BsonDocument>.Update.Set("numeroFichasJugador2", gameManager.getNumeroFichasJugador2());
        var fichasObtenidasJugador1 = Builders<BsonDocument>.Update.Set("fichasObtenidasJugador1", gameManager.getFichasObtenidasJugador1());
        var fichasObtenidasJugador2 = Builders<BsonDocument>.Update.Set("fichasObtenidasJugador2", gameManager.getFichasObtenidasJugador2());
        var puntosJugador1 = Builders<BsonDocument>.Update.Set("puntosJugador1", gameManager.getPuntosJugador1());
        var puntosJugador2 = Builders<BsonDocument>.Update.Set("puntosJugador2", gameManager.getPuntosJugador2());
        var rondaActual = Builders<BsonDocument>.Update.Set("rondaActual", gameManager.getRondaActual());
        var turnosJugador1Numero = Builders<BsonDocument>.Update.Set("turnosRestantesJugador1", gameManager.getTurnosJugador1Numero());
        var turnosJugador2Numero = Builders<BsonDocument>.Update.Set("turnosRestantesJugador2", gameManager.getTurnosJugador2Numero());
        var turnoActual = Builders<BsonDocument>.Update.Set("turnoActual", gameManager.getTurnoActual());
        var jugadorActual = Builders<BsonDocument>.Update.Set("jugadorActual", gameManager.getJugadorActual());
        var turnosRegistrados = Builders<BsonDocument>.Update.Set("turnosRegistrados", gameManager.getTurnosRegistrados());

        await collectionPartidas.UpdateOneAsync(filtroPartidaActualizar, fichasSinRotar);
        await collectionPartidas.UpdateOneAsync(filtroPartidaActualizar, numeroFichasJugador1);
        await collectionPartidas.UpdateOneAsync(filtroPartidaActualizar, numeroFichasJugador2);
        await collectionPartidas.UpdateOneAsync(filtroPartidaActualizar, fichasObtenidasJugador1);
        await collectionPartidas.UpdateOneAsync(filtroPartidaActualizar, fichasObtenidasJugador2);
        await collectionPartidas.UpdateOneAsync(filtroPartidaActualizar, puntosJugador1);
        await collectionPartidas.UpdateOneAsync(filtroPartidaActualizar, puntosJugador2);
        await collectionPartidas.UpdateOneAsync(filtroPartidaActualizar, rondaActual);
        await collectionPartidas.UpdateOneAsync(filtroPartidaActualizar, turnosJugador1Numero);
        await collectionPartidas.UpdateOneAsync(filtroPartidaActualizar, turnosJugador2Numero);
        await collectionPartidas.UpdateOneAsync(filtroPartidaActualizar, turnoActual);
        await collectionPartidas.UpdateOneAsync(filtroPartidaActualizar, jugadorActual);
        await collectionPartidas.UpdateOneAsync(filtroPartidaActualizar, turnosRegistrados);

        var datosPartida = await collectionPartidas.Find(filtroPartidaActualizar).FirstOrDefaultAsync();

        if(datosPartida["rondaActual"] == 1){
            tablero.SetActive(true);
            tablero2.SetActive(false);

            GuardarTablero1();
            GuardarFichas1();
        }
        else if(datosPartida["rondaActual"] == 2){

            if(tablero != null){
                tablero.SetActive(false);    
            }

            
            tablero2.SetActive(true);

            GuardarTablero2();
            GuardarFichas2();
        } 

        
    }

    public async void GuardarTablero1(){

        var posicion = tablero.transform.position;
        var rotacion = tablero.transform.rotation.eulerAngles;
        var escala = tablero.transform.localScale;

        // Crear documentos BSON para cada componente
        var estado = new BsonDocument
        { 
            { 
                "nombre", tablero.name
            },
            { "posicion", new BsonDocument
                {
                    { "x", posicion.x },
                    { "y", posicion.y },
                    { "z", posicion.z }
                }
            },
            { "rotacion", new BsonDocument
                {
                    { "x", rotacion.x },
                    { "y", rotacion.y },
                    { "z", rotacion.z }
                }
            },
            { "escala", new BsonDocument
                {
                    { "x", escala.x },
                    { "y", escala.y },
                    { "z", escala.z }
                }
            }
                
        };

        // Identificador único del documento que deseas actualizar (puedes utilizar algún identificador único)
        var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);

        // Actualizar el documento en MongoDB
        var update = Builders<BsonDocument>.Update.Set("tablero1",estado);
        var result = await collectionPartidas.UpdateOneAsync(filtro, update);
        
        // Comprueba si el tablero se actualizo bien
        if (result.ModifiedCount == 1)
        {
            Debug.Log("Tablero actualizado exitosamente.");
        }
        else
        {
            Debug.Log("No se pudo actualizar el documento.");
        }
    }

    public async void CargarTablero1(){

        var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
        var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

        var posicion = datosPartida["tablero1"]["posicion"].ToBsonDocument();
        var rotacion = datosPartida["tablero1"]["rotacion"].ToBsonDocument();
        var escala = datosPartida["tablero1"]["escala"].ToBsonDocument();

        //Obtener posicion
        float PosX = (float)posicion["x"].ToDouble();
        float PosY = (float)posicion["y"].ToDouble();
        float PosZ = (float)posicion["z"].ToDouble();

        //Obtener rotacion
        float RotX = (float)rotacion["x"].ToDouble();
        float RotY = (float)rotacion["y"].ToDouble();
        float RotZ = (float)rotacion["z"].ToDouble();

        //Obtener escala
        float EscX = (float)escala["x"].ToDouble();
        float EscY = (float)escala["y"].ToDouble();
        float EscZ = (float)escala["z"].ToDouble();

        tablero.transform.position = new Vector3(PosX, PosY, PosZ);
        tablero.transform.rotation = Quaternion.Euler(RotX,RotY,RotZ);
        tablero.transform.localScale = new Vector3(EscX,EscY,EscZ);

    }

    public async void GuardarFichas1(){

        Debug.Log("Guardando datos fichas espere...");

        guardarOCargarTexto.text = "Guardando...";

        guardarOCargar.SetActive(true);
        
        Transform tableroPadre = tablero.transform;


        // Recorrer cada hijo del objeto padre utilizando foreach
        foreach (Transform ficha in tableroPadre)
        {
            string nombreFichaSinNumero;
            char ultimoCaracter;

            var posicion = ficha.transform.position;
            var rotacion = ficha.transform.rotation.eulerAngles;
            var escala = ficha.transform.localScale;

            if(ficha.name.Contains("Cazador")){

                nombreFichaSinNumero = ficha.name.Remove(ficha.name.Length - 1 );
                ultimoCaracter = ficha.name[ficha.name.Length - 1];

                MoverFichaCazador fichaCazador = ficha.GetComponent<MoverFichaCazador>();

                contCazador += 1;

                var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
                var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

                var estado = new BsonDocument();

                if(!datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter) && datosPartida["montarTablero1PrimeraVez"] == true){
                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaCazador.poderMoverFichaCazador
                        },
                        {
                            "turnoAnterior", fichaCazador.turnoAnterior 
                        },
                        {
                            "posXAnterior" , fichaCazador.posXAnterior 
                        },
                        {
                            "posYAnterior" , fichaCazador.posYAnterior
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }

                    };

                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
                else if(datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter)){
                    
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaCazador.poderMoverFichaCazador
                        },
                        {
                            "turnoAnterior", fichaCazador.turnoAnterior 
                        },
                        {
                            "posXAnterior" , fichaCazador.posXAnterior 
                        },
                        {
                            "posYAnterior" , fichaCazador.posYAnterior
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }

                    };

                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
            }
            else if(ficha.name.Contains("Leñador")){

                nombreFichaSinNumero = ficha.name.Remove(ficha.name.Length - 1 );
                ultimoCaracter = ficha.name[ficha.name.Length - 1];

                MoverFichaLenyador fichaLeñador = ficha.GetComponent<MoverFichaLenyador>();

                contLeñador += 1;

                var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
                var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

                var estado = new BsonDocument();

                if(!datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter) && datosPartida["montarTablero1PrimeraVez"] == true){

                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaLeñador.poderMoverFichaLeñador
                        },
                        {
                            "turnoAnterior", fichaLeñador.turnoAnterior
                        },
                        {
                            "posXAnterior" , fichaLeñador.posXAnterior 
                        },
                        {
                            "posYAnterior" , fichaLeñador.posYAnterior
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
                else if(datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter)){

                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaLeñador.poderMoverFichaLeñador
                        },
                        {
                            "turnoAnterior", fichaLeñador.turnoAnterior
                        },
                        {
                            "posXAnterior" , fichaLeñador.posXAnterior 
                        },
                        {
                            "posYAnterior" , fichaLeñador.posYAnterior
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
                
            }
            else if(ficha.name.Contains("Oso")){

                nombreFichaSinNumero = ficha.name.Remove(ficha.name.Length - 1 );
                ultimoCaracter = ficha.name[ficha.name.Length - 1];

                MoverFichaOso fichaOso = ficha.GetComponent<MoverFichaOso>();

                contOso += 1;

                var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
                var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

                var estado = new BsonDocument();

                if(!datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter) && datosPartida["montarTablero1PrimeraVez"] == true){
                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaOso.poderMoverFichaOso
                        },
                        {
                            "turnoAnterior", fichaOso.turnoAnterior
                        },
                        {
                            "posXAnterior" , fichaOso.posXAnterior 
                        },
                        {
                            "posYAnterior" , fichaOso.posYAnterior
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    // Actualizar el documento en MongoDB
                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
                else if(datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter)){

                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaOso.poderMoverFichaOso
                        },
                        {
                            "turnoAnterior", fichaOso.turnoAnterior
                        },
                        {
                            "posXAnterior" , fichaOso.posXAnterior 
                        },
                        {
                            "posYAnterior" , fichaOso.posYAnterior
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    // Actualizar el documento en MongoDB
                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
    
            }
            else if(ficha.name.Contains("Pato")){

                nombreFichaSinNumero = ficha.name.Remove(ficha.name.Length - 1 );
                ultimoCaracter = ficha.name[ficha.name.Length - 1];

                MoverFichaPato fichaPato = ficha.GetComponent<MoverFichaPato>();

                contPato += 1;

                var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
                var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

                var estado = new BsonDocument();

                if(!datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter) && datosPartida["montarTablero1PrimeraVez"] == true){

                    Debug.Log("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter);
                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaPato.poderMoverFichaPato
                        },
                        {
                            "turnoAnterior", fichaPato.turnoAnterior 
                        },
                        {
                            "fichaDisponible", fichaPato.fichaDisponiblePato
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);       
                }
                else if(datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter)){

                    Debug.Log("Actualizando ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter);

                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaPato.poderMoverFichaPato
                        },
                        {
                            "turnoAnterior", fichaPato.turnoAnterior 
                        },
                        {
                            "fichaDisponible", fichaPato.fichaDisponiblePato
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
                
 
            }
            else if(ficha.name.Contains("Pavo")){

                nombreFichaSinNumero = ficha.name.Remove(ficha.name.Length - 1 );
                ultimoCaracter = ficha.name[ficha.name.Length - 1];

                MoverFichaPavo fichaPavo = ficha.GetComponent<MoverFichaPavo>();

                contPavo += 1;

                var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
                var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

                var estado = new BsonDocument();

                if(!datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter) && datosPartida["montarTablero1PrimeraVez"] == true){
                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaPavo.poderMoverFichaPavo
                        },
                        {
                            "turnoAnterior", fichaPavo.turnoAnterior 
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    // Actualizar el documento en MongoDB
                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
                else if(datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter)){

                    Debug.Log("Actualizando ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter);

                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaPavo.poderMoverFichaPavo
                        },
                        {
                            "turnoAnterior", fichaPavo.turnoAnterior 
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }


                
            }
            else if(ficha.name.Contains("Zorro")){

                nombreFichaSinNumero = ficha.name.Remove(ficha.name.Length - 1 );
                ultimoCaracter = ficha.name[ficha.name.Length - 1];

                MoverFichaZorro fichaZorro = ficha.GetComponent<MoverFichaZorro>();

                contZorro += 1;

                var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
                var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

                var estado = new BsonDocument();

                if(!datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter) && datosPartida["montarTablero1PrimeraVez"] == true){
                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaZorro.poderMoverFichaZorro
                        },
                        {
                            "turnoAnterior", fichaZorro.turnoAnterior 
                        },
                        {
                            "posXAnterior" , fichaZorro.posXAnterior 
                        },
                        {
                            "posYAnterior" , fichaZorro.posYAnterior
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }

                    };

                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
                else if(datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter)){

                    Debug.Log("Actualizando ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter);

                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaZorro.poderMoverFichaZorro
                        },
                        {
                            "turnoAnterior", fichaZorro.turnoAnterior 
                        },
                        {
                            "posXAnterior" , fichaZorro.posXAnterior 
                        },
                        {
                            "posYAnterior" , fichaZorro.posYAnterior
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }

                    }; 

                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);             
                }

            }
            else if(ficha.name.Contains("Pino")){

                nombreFichaSinNumero = ficha.name.Remove(ficha.name.Length - 1 );
                ultimoCaracter = ficha.name[ficha.name.Length - 1];

                contPino += 1;

                var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
                var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

                var estado = new BsonDocument();

                if(!datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter) && datosPartida["montarTablero1PrimeraVez"] == true){
                    // Crear documentos BSON para cada componente

                    Debug.Log("nombreSinNumero: " + nombreFichaSinNumero);
                    Debug.Log("ultimoCaracter: " + ultimoCaracter);
                    Debug.Log("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter);

                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    // Actualizar el documento en MongoDB
                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
                else if(datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter)){

                    Debug.Log("Actualizando ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter);

                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    // Actualizar el documento en MongoDB
                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);

                }
                
            }
            else if(ficha.name.Contains("Arbol")){

                nombreFichaSinNumero = ficha.name.Remove(ficha.name.Length - 1 );
                ultimoCaracter = ficha.name[ficha.name.Length - 1];

                contArbol += 1;

                var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
                var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

                var estado = new BsonDocument();

                if(!datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter) && datosPartida["montarTablero1PrimeraVez"] == true){
                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    // Actualizar el documento en MongoDB
                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
                else if(datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter)){

                    Debug.Log("Actualizando ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter);

                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    // Actualizar el documento en MongoDB
                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);

                }
            }
            
        }

        contZorro = 0;
        contCazador = 0;
        contOso = 0;
        contPato = 0;
        contPavo = 0;
        contLeñador = 0;
        contPino = 0;
        contArbol = 0;

        if(fichasEliminadas.Count <= 0){
            Debug.Log("Nada que borrar XDD");
        }
        else{

            var filtroBorrarFicha = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);

            for (int i = 0; i < fichasEliminadas.Count; i++)
            {
                Debug.Log("Eliminando ficha en la base de datos: " + fichasEliminadas[i]);
                var eliminarFicha = Builders<BsonDocument>.Update.Unset(fichasEliminadas[i]);
                await collectionPartidas.UpdateOneAsync(filtroBorrarFicha, eliminarFicha);
            }

        }
        
        //Aqui el valor de montarTablero1PrimeraVez se volvera false para indicar de que ya se montó una vez 
        var filtroMontarPartida = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);

        var cambiarMontarTablero1PrimeraVez = Builders<BsonDocument>.Update.Set("montarTablero1PrimeraVez",false);
        var resultado = await collectionPartidas.UpdateOneAsync(filtroMontarPartida, cambiarMontarTablero1PrimeraVez);

        guardarOCargar.SetActive(false);

        gameManager.setPoderClickear(true);

        gameManager.ActivarYDesactivarBotones(true,true,true);

        Debug.Log("Guardando datos ficha correctamente");
  
    }

    public async void CargarFichas1(){

        Debug.Log("Cargando fihas espere...");

        guardarOCargar.SetActive(true);

        var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
        var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

        for (int contCazador = 1; contCazador <= 8; contCazador++) {

            if(!datosPartida.Contains("fichaCazadorTablero1 " + contCazador)){
                continue;
            }
            else{
                var posicion = datosPartida["fichaCazadorTablero1 " + contCazador]["posicion"].ToBsonDocument();
                var rotacion = datosPartida["fichaCazadorTablero1 " + contCazador]["rotacion"].ToBsonDocument();
                var escala = datosPartida["fichaCazadorTablero1 " + contCazador]["escala"].ToBsonDocument();
                bool poderMoverFicha = datosPartida["fichaCazadorTablero1 " + contCazador]["poderMoverFicha"].ToBoolean();
                int turnoAnteriorCazador = datosPartida["fichaCazadorTablero1 " + contCazador]["turnoAnterior"].ToInt32();
                int posXAnterior = datosPartida["fichaCazadorTablero1 " + contCazador]["posXAnterior"].ToInt32();
                int posYAnterior = datosPartida["fichaCazadorTablero1 " + contCazador]["posYAnterior"].ToInt32();
                
                //Obtener posicion
                float PosX = (float)posicion["x"].ToDouble();
                float PosY = (float)posicion["y"].ToDouble();
                float PosZ = (float)posicion["z"].ToDouble();

                //Obtener rotacion
                float RotX = (float)rotacion["x"].ToDouble();
                float RotY = (float)rotacion["y"].ToDouble();
                float RotZ = (float)rotacion["z"].ToDouble();

                //Obtener escala
                float EscX = (float)escala["x"].ToDouble();
                float EscY = (float)escala["y"].ToDouble();
                float EscZ = (float)escala["z"].ToDouble();

                Quaternion rotacionn = Quaternion.Euler(RotX,RotY,RotZ);
                
                Vector3 posicionCasillas = new Vector3(PosX * 1f, 0f, PosZ * 1f);
                GameObject spawnFichas = Instantiate(fichas[0], posicionCasillas, rotacionn);
                spawnFichas.transform.SetParent(tablero.transform);
                spawnFichas.name = "Cazador"+contCazador;

                Debug.Log("PoderMoverFichaCazador: " + poderMoverFicha);

                spawnFichas.GetComponent<MoverFichaCazador>().poderMoverFichaCazador = poderMoverFicha;
                spawnFichas.GetComponent<MoverFichaCazador>().turnoAnterior = turnoAnteriorCazador;
                spawnFichas.GetComponent<MoverFichaCazador>().posXAnterior = posXAnterior;
                spawnFichas.GetComponent<MoverFichaCazador>().posYAnterior = posYAnterior;
            }

        }

        for (int contLeñador = 1; contLeñador <= 2; contLeñador++) {

            if(!datosPartida.Contains("fichaLeñadorTablero1 " + contLeñador)){
                continue;
            }
            else{

                var posicion = datosPartida["fichaLeñadorTablero1 " + contLeñador]["posicion"].ToBsonDocument();
                var rotacion = datosPartida["fichaLeñadorTablero1 " + contLeñador]["rotacion"].ToBsonDocument();
                var escala = datosPartida["fichaLeñadorTablero1 " + contLeñador]["escala"].ToBsonDocument();
                bool poderMoverFicha = datosPartida["fichaLeñadorTablero1 " + contLeñador]["poderMoverFicha"].ToBoolean();
                int turnoAnteriorLeñador = datosPartida["fichaLeñadorTablero1 " + contLeñador]["turnoAnterior"].ToInt32();
                int posXAnterior = datosPartida["fichaLeñadorTablero1 " + contLeñador]["posXAnterior"].ToInt32();
                int posYAnterior = datosPartida["fichaLeñadorTablero1 " + contLeñador]["posYAnterior"].ToInt32();

                //Obtener posicion
                float PosX = (float)posicion["x"].ToDouble();
                float PosY = (float)posicion["y"].ToDouble();
                float PosZ = (float)posicion["z"].ToDouble();

                //Obtener rotacion
                float RotX = (float)rotacion["x"].ToDouble();
                float RotY = (float)rotacion["y"].ToDouble();
                float RotZ = (float)rotacion["z"].ToDouble();

                //Obtener escala
                float EscX = (float)escala["x"].ToDouble();
                float EscY = (float)escala["y"].ToDouble();
                float EscZ = (float)escala["z"].ToDouble();

                Quaternion rotacionn = Quaternion.Euler(RotX,RotY,RotZ);
                
                Vector3 posicionCasillas = new Vector3(PosX * 1f, 0f, PosZ * 1f);
                GameObject spawnFichas = Instantiate(fichas[1], posicionCasillas, rotacionn);
                spawnFichas.transform.SetParent(tablero.transform);
                spawnFichas.name = "Leñador"+contLeñador;

                spawnFichas.GetComponent<MoverFichaLenyador>().poderMoverFichaLeñador = poderMoverFicha;
                spawnFichas.GetComponent<MoverFichaLenyador>().turnoAnterior = turnoAnteriorLeñador;
                spawnFichas.GetComponent<MoverFichaLenyador>().posXAnterior = posXAnterior;
                spawnFichas.GetComponent<MoverFichaLenyador>().posYAnterior = posYAnterior;

            }

        }

        for (int contOso = 1; contOso <= 2; contOso++) {

            if(!datosPartida.Contains("fichaOsoTablero1 " + contOso)){
                continue;
            }
            else{

                var posicion = datosPartida["fichaOsoTablero1 " + contOso]["posicion"].ToBsonDocument();
                var rotacion = datosPartida["fichaOsoTablero1 " + contOso]["rotacion"].ToBsonDocument();
                var escala = datosPartida["fichaOsoTablero1 " + contOso]["escala"].ToBsonDocument();
                bool poderMoverFicha = datosPartida["fichaOsoTablero1 " + contOso]["poderMoverFicha"].ToBoolean();
                int turnoAnteriorOso = datosPartida["fichaOsoTablero1 " + contOso]["turnoAnterior"].ToInt32();
                int posXAnterior = datosPartida["fichaOsoTablero1 " + contOso]["posXAnterior"].ToInt32();
                int posYAnterior = datosPartida["fichaOsoTablero1 " + contOso]["posYAnterior"].ToInt32();

                //Obtener posicion
                float PosX = (float)posicion["x"].ToDouble();
                float PosY = (float)posicion["y"].ToDouble();
                float PosZ = (float)posicion["z"].ToDouble();

                //Obtener rotacion
                float RotX = (float)rotacion["x"].ToDouble();
                float RotY = (float)rotacion["y"].ToDouble();
                float RotZ = (float)rotacion["z"].ToDouble();

                //Obtener escala
                float EscX = (float)escala["x"].ToDouble();
                float EscY = (float)escala["y"].ToDouble();
                float EscZ = (float)escala["z"].ToDouble();

                Quaternion rotacionn = Quaternion.Euler(RotX,RotY,RotZ);
                
                Vector3 posicionCasillas = new Vector3(PosX * 1f, 0f, PosZ * 1f);
                GameObject spawnFichas = Instantiate(fichas[2], posicionCasillas, rotacionn);
                spawnFichas.transform.SetParent(tablero.transform);
                spawnFichas.name = "Oso"+contOso;

                spawnFichas.GetComponent<MoverFichaOso>().poderMoverFichaOso = poderMoverFicha;
                spawnFichas.GetComponent<MoverFichaOso>().turnoAnterior = turnoAnteriorOso;
                spawnFichas.GetComponent<MoverFichaOso>().posXAnterior = posXAnterior;
                spawnFichas.GetComponent<MoverFichaOso>().posYAnterior = posYAnterior;
            }
            
        }

        for (int contPato = 1; contPato <= 7; contPato++) {

            if(!datosPartida.Contains("fichaPatoTablero1 " + contPato)){
                continue;
            }
            else{

                var posicion = datosPartida["fichaPatoTablero1 " + contPato]["posicion"].ToBsonDocument();
                var rotacion = datosPartida["fichaPatoTablero1 " + contPato]["rotacion"].ToBsonDocument();
                var escala = datosPartida["fichaPatoTablero1 " + contPato]["escala"].ToBsonDocument();
                bool poderMoverFicha = datosPartida["fichaPatoTablero1 " + contPato]["poderMoverFicha"].ToBoolean();
                int turnoAnteriorPato = datosPartida["fichaPatoTablero1 " + contPato]["turnoAnterior"].ToInt32();
                bool fichaDisponiblePato = datosPartida["fichaPatoTablero1 " + contPato]["fichaDisponible"].ToBoolean();

                //Obtener posicion
                float PosX = (float)posicion["x"].ToDouble();
                float PosY = (float)posicion["y"].ToDouble();
                float PosZ = (float)posicion["z"].ToDouble();

                //Obtener rotacion
                float RotX = (float)rotacion["x"].ToDouble();
                float RotY = (float)rotacion["y"].ToDouble();
                float RotZ = (float)rotacion["z"].ToDouble();

                //Obtener escala
                float EscX = (float)escala["x"].ToDouble();
                float EscY = (float)escala["y"].ToDouble();
                float EscZ = (float)escala["z"].ToDouble();

                Quaternion rotacionn = Quaternion.Euler(RotX,RotY,RotZ);
                
                Vector3 posicionCasillas = new Vector3(PosX * 1f, 0f, PosZ * 1f);
                GameObject spawnFichas = Instantiate(fichas[3], posicionCasillas, rotacionn);
                spawnFichas.transform.SetParent(tablero.transform);
                spawnFichas.name = "Pato"+contPato;

                spawnFichas.GetComponent<MoverFichaPato>().poderMoverFichaPato = poderMoverFicha;
                spawnFichas.GetComponent<MoverFichaPato>().turnoAnterior = turnoAnteriorPato;
                spawnFichas.GetComponent<MoverFichaPato>().fichaDisponiblePato = fichaDisponiblePato;

            }
        }

        for (int contPavo = 1; contPavo <= 8; contPavo++) {

            if(!datosPartida.Contains("fichaPavoTablero1 " + contPavo)){
                continue;
            }
            else{
                var posicion = datosPartida["fichaPavoTablero1 " + contPavo]["posicion"].ToBsonDocument();
                var rotacion = datosPartida["fichaPavoTablero1 " + contPavo]["rotacion"].ToBsonDocument();
                var escala = datosPartida["fichaPavoTablero1 " + contPavo]["escala"].ToBsonDocument();
                bool poderMoverFicha = datosPartida["fichaPavoTablero1 " + contPavo]["poderMoverFicha"].ToBoolean();
                int turnoAnteriorPavo = datosPartida["fichaPavoTablero1 " + contPavo]["turnoAnterior"].ToInt32();

                //Obtener posicion
                float PosX = (float)posicion["x"].ToDouble();
                float PosY = (float)posicion["y"].ToDouble();
                float PosZ = (float)posicion["z"].ToDouble();

                //Obtener rotacion
                float RotX = (float)rotacion["x"].ToDouble();
                float RotY = (float)rotacion["y"].ToDouble();
                float RotZ = (float)rotacion["z"].ToDouble();

                //Obtener escala
                float EscX = (float)escala["x"].ToDouble();
                float EscY = (float)escala["y"].ToDouble();
                float EscZ = (float)escala["z"].ToDouble();

                Quaternion rotacionn = Quaternion.Euler(RotX,RotY,RotZ);
                
                Vector3 posicionCasillas = new Vector3(PosX * 1f, 0f, PosZ * 1f);
                GameObject spawnFichas = Instantiate(fichas[4], posicionCasillas, rotacionn);
                spawnFichas.transform.SetParent(tablero.transform);
                spawnFichas.name = "Pavo"+contPavo;

                spawnFichas.GetComponent<MoverFichaPavo>().poderMoverFichaPavo = poderMoverFicha;
                spawnFichas.GetComponent<MoverFichaPavo>().turnoAnterior = turnoAnteriorPavo;
            }

        }

        for (int contZorro = 1; contZorro <= 6; contZorro++) {

            if(!datosPartida.Contains("fichaZorroTablero1 " + contZorro)){
                continue;
            }
            else{
                var posicion = datosPartida["fichaZorroTablero1 " + contZorro]["posicion"].ToBsonDocument();
                var rotacion = datosPartida["fichaZorroTablero1 " + contZorro]["rotacion"].ToBsonDocument();
                var escala = datosPartida["fichaZorroTablero1 " + contZorro]["escala"].ToBsonDocument();
                bool poderMoverFicha = datosPartida["fichaZorroTablero1 " + contZorro]["poderMoverFicha"].ToBoolean();
                int turnoAnteriorZorro = datosPartida["fichaZorroTablero1 " + contZorro]["turnoAnterior"].ToInt32();
                int posXAnterior = datosPartida["fichaZorroTablero1 " + contZorro]["posXAnterior"].ToInt32();
                int posYAnterior = datosPartida["fichaZorroTablero1 " + contZorro]["posYAnterior"].ToInt32();

                //Obtener posicion
                float PosX = (float)posicion["x"].ToDouble();
                float PosY = (float)posicion["y"].ToDouble();
                float PosZ = (float)posicion["z"].ToDouble();

                //Obtener rotacion
                float RotX = (float)rotacion["x"].ToDouble();
                float RotY = (float)rotacion["y"].ToDouble();
                float RotZ = (float)rotacion["z"].ToDouble();

                //Obtener escala
                float EscX = (float)escala["x"].ToDouble();
                float EscY = (float)escala["y"].ToDouble();
                float EscZ = (float)escala["z"].ToDouble();

                Quaternion rotacionn = Quaternion.Euler(RotX,RotY,RotZ);
                
                Vector3 posicionCasillas = new Vector3(PosX * 1f, 0f, PosZ * 1f);
                GameObject spawnFichas = Instantiate(fichas[5], posicionCasillas, rotacionn);
                spawnFichas.transform.SetParent(tablero.transform);
                spawnFichas.name = "Zorro"+contZorro;

                spawnFichas.GetComponent<MoverFichaZorro>().poderMoverFichaZorro = poderMoverFicha;
                spawnFichas.GetComponent<MoverFichaZorro>().turnoAnterior = turnoAnteriorZorro;
                spawnFichas.GetComponent<MoverFichaZorro>().posXAnterior = posXAnterior;
                spawnFichas.GetComponent<MoverFichaZorro>().posYAnterior = posYAnterior;
            }
        }

        for (int contPino = 1; contPino <= 7; contPino++) {

            if(!datosPartida.Contains("fichaPinoTablero1 " + contPino)){
                continue;
            }
            else{

                var posicion = datosPartida["fichaPinoTablero1 " + contPino]["posicion"].ToBsonDocument();
                var rotacion = datosPartida["fichaPinoTablero1 " + contPino]["rotacion"].ToBsonDocument();
                var escala = datosPartida["fichaPinoTablero1 " + contPino]["escala"].ToBsonDocument();

                //Obtener posicion
                float PosX = (float)posicion["x"].ToDouble();
                float PosY = (float)posicion["y"].ToDouble();
                float PosZ = (float)posicion["z"].ToDouble();

                //Obtener rotacion
                float RotX = (float)rotacion["x"].ToDouble();
                float RotY = (float)rotacion["y"].ToDouble();
                float RotZ = (float)rotacion["z"].ToDouble();

                //Obtener escala
                float EscX = (float)escala["x"].ToDouble();
                float EscY = (float)escala["y"].ToDouble();
                float EscZ = (float)escala["z"].ToDouble();

                Quaternion rotacionn = Quaternion.Euler(RotX,RotY,RotZ);
                
                Vector3 posicionCasillas = new Vector3(PosX * 1f, 0f, PosZ * 1f);
                GameObject spawnFichas = Instantiate(fichas[6], posicionCasillas, rotacionn);
                spawnFichas.transform.SetParent(tablero.transform);
                spawnFichas.name = "Pino"+contPino;

            }
        }

        for (int contArbol = 1; contArbol <= 8; contArbol++) {

            if(!datosPartida.Contains("fichaArbolTablero1 " + contArbol)){
                continue;
            }
            else{

                var posicion = datosPartida["fichaArbolTablero1 " + contArbol]["posicion"].ToBsonDocument();
                var rotacion = datosPartida["fichaArbolTablero1 " + contArbol]["rotacion"].ToBsonDocument();
                var escala = datosPartida["fichaArbolTablero1 " + contArbol]["escala"].ToBsonDocument();

                //Obtener posicion
                float PosX = (float)posicion["x"].ToDouble();
                float PosY = (float)posicion["y"].ToDouble();
                float PosZ = (float)posicion["z"].ToDouble();

                //Obtener rotacion
                float RotX = (float)rotacion["x"].ToDouble();
                float RotY = (float)rotacion["y"].ToDouble();
                float RotZ = (float)rotacion["z"].ToDouble();

                //Obtener escala
                float EscX = (float)escala["x"].ToDouble();
                float EscY = (float)escala["y"].ToDouble();
                float EscZ = (float)escala["z"].ToDouble();

                Quaternion rotacionn = Quaternion.Euler(RotX,RotY,RotZ);
                
                Vector3 posicionCasillas = new Vector3(PosX * 1f, 0f, PosZ * 1f);
                GameObject spawnFichas = Instantiate(fichas[7], posicionCasillas, rotacionn);
                spawnFichas.transform.SetParent(tablero.transform);
                spawnFichas.name = "Arbol"+contArbol;

            }
        }

        Debug.Log("Fihas cargadas con exito...");

        guardarOCargar.SetActive(false);

        gameManager.ActivarYDesactivarBotones(true,true,true);
    }

    public void QuitarDisponibilidadFichaPasada(GameObject fichaPasada){

        string nombreFicha = fichaPasada.name;

        string nombreFichaSinNumero = nombreFicha.Remove(nombreFicha.Length - 1 );
        char ultimoCaracter = nombreFicha[nombreFicha.Length - 1];

        Debug.Log("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter);

        //Guarda la ficha eliminada sino se guarda la parida no habrán cambios
        fichasEliminadas.Add("ficha" + nombreFichaSinNumero + "Tablero1 " + ultimoCaracter);

        Destroy(fichaPasada);
    }

    public async void GuardarTablero2(){
        

        var posicion = tablero2.transform.position;
        var rotacion = tablero2.transform.rotation.eulerAngles;
        var escala = tablero2.transform.localScale;

        // Crear documentos BSON para cada componente
        var estado = new BsonDocument
        { 
            { 
                "nombre", tablero2.name
            },
            { "posicion", new BsonDocument
                {
                    { "x", posicion.x },
                    { "y", posicion.y },
                    { "z", posicion.z }
                }
            },
            { "rotacion", new BsonDocument
                {
                    { "x", rotacion.x },
                    { "y", rotacion.y },
                    { "z", rotacion.z }
                }
            },
            { "escala", new BsonDocument
                {
                    { "x", escala.x },
                    { "y", escala.y },
                    { "z", escala.z }
                }
            }
                
        };

        // Identificador único del documento que deseas actualizar (puedes utilizar algún identificador único)
        var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);

        // Actualizar el documento en MongoDB
        var update = Builders<BsonDocument>.Update.Set("tablero2",estado);
        var result = await collectionPartidas.UpdateOneAsync(filtro, update);
        
        // Comprueba si el tablero 2 se actualizo bien
        if (result.ModifiedCount == 1)
        {
            Debug.Log("Tablero actualizado exitosamente.");
        }
        else
        {
            Debug.Log("No se pudo actualizar el documento.");
        }
    }

    public async void GuardarFichas2(){

        Debug.Log("Guardando datos fichas 2 espere...");

        guardarOCargarTexto.text = "Guardando...";

        guardarOCargar.SetActive(true);
        
        Transform tableroPadre = tablero2.transform;


        // Recorrer cada hijo del objeto padre utilizando foreach
        foreach (Transform ficha in tableroPadre)
        {
            string nombreFichaSinNumero;
            char ultimoCaracter;

            var posicion = ficha.transform.position;
            var rotacion = ficha.transform.rotation.eulerAngles;
            var escala = ficha.transform.localScale;

            if(ficha.name.Contains("Cazador")){

                nombreFichaSinNumero = ficha.name.Remove(ficha.name.Length - 1 );
                ultimoCaracter = ficha.name[ficha.name.Length - 1];

                MoverFichaCazador fichaCazador = ficha.GetComponent<MoverFichaCazador>();

                contCazador += 1;

                var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
                var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

                var estado = new BsonDocument();

                if(!datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter) && datosPartida["montarTablero2PrimeraVez"] == true){
                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaCazador.poderMoverFichaCazador
                        },
                        {
                            "turnoAnterior", fichaCazador.turnoAnterior 
                        },
                        {
                            "posXAnterior" , fichaCazador.posXAnterior 
                        },
                        {
                            "posYAnterior" , fichaCazador.posYAnterior
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }

                    };

                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
                else if(datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter) ){
                    
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaCazador.poderMoverFichaCazador
                        },
                        {
                            "turnoAnterior", fichaCazador.turnoAnterior 
                        },
                        {
                            "posXAnterior" , fichaCazador.posXAnterior 
                        },
                        {
                            "posYAnterior" , fichaCazador.posYAnterior
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }

                    };

                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
            }
            else if(ficha.name.Contains("Leñador")){

                nombreFichaSinNumero = ficha.name.Remove(ficha.name.Length - 1 );
                ultimoCaracter = ficha.name[ficha.name.Length - 1];

                MoverFichaLenyador fichaLeñador = ficha.GetComponent<MoverFichaLenyador>();

                contLeñador += 1;

                var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
                var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

                var estado = new BsonDocument();

                if(!datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter) && datosPartida["montarTablero2PrimeraVez"] == true){

                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaLeñador.poderMoverFichaLeñador
                        },
                        {
                            "turnoAnterior", fichaLeñador.turnoAnterior
                        },
                        {
                            "posXAnterior" , fichaLeñador.posXAnterior 
                        },
                        {
                            "posYAnterior" , fichaLeñador.posYAnterior
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
                else if(datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter)){

                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaLeñador.poderMoverFichaLeñador
                        },
                        {
                            "turnoAnterior", fichaLeñador.turnoAnterior
                        },
                        {
                            "posXAnterior" , fichaLeñador.posXAnterior 
                        },
                        {
                            "posYAnterior" , fichaLeñador.posYAnterior
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
                
            }
            else if(ficha.name.Contains("Oso")){

                nombreFichaSinNumero = ficha.name.Remove(ficha.name.Length - 1 );
                ultimoCaracter = ficha.name[ficha.name.Length - 1];

                MoverFichaOso fichaOso = ficha.GetComponent<MoverFichaOso>();

                contOso += 1;

                var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
                var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

                var estado = new BsonDocument();

                if(!datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter) && datosPartida["montarTablero2PrimeraVez"] == true){
                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaOso.poderMoverFichaOso
                        },
                        {
                            "turnoAnterior", fichaOso.turnoAnterior
                        },
                        {
                            "posXAnterior" , fichaOso.posXAnterior 
                        },
                        {
                            "posYAnterior" , fichaOso.posYAnterior
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    // Actualizar el documento en MongoDB
                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
                else if(datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter)){

                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaOso.poderMoverFichaOso
                        },
                        {
                            "turnoAnterior", fichaOso.turnoAnterior
                        },
                        {
                            "posXAnterior" , fichaOso.posXAnterior 
                        },
                        {
                            "posYAnterior" , fichaOso.posYAnterior
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    // Actualizar el documento en MongoDB
                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
    
            }
            else if(ficha.name.Contains("Pato")){

                nombreFichaSinNumero = ficha.name.Remove(ficha.name.Length - 1 );
                ultimoCaracter = ficha.name[ficha.name.Length - 1];

                MoverFichaPato fichaPato = ficha.GetComponent<MoverFichaPato>();

                contPato += 1;

                var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
                var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

                var estado = new BsonDocument();

                if(!datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter) && datosPartida["montarTablero2PrimeraVez"] == true){

                    Debug.Log("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter);
                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaPato.poderMoverFichaPato
                        },
                        {
                            "turnoAnterior", fichaPato.turnoAnterior 
                        },
                        {
                            "fichaDisponible", fichaPato.fichaDisponiblePato
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);       
                }
                else if(datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter)){

                    Debug.Log("Actualizando ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter);

                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaPato.poderMoverFichaPato
                        },
                        {
                            "turnoAnterior", fichaPato.turnoAnterior 
                        },
                        {
                            "fichaDisponible", fichaPato.fichaDisponiblePato
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
                
 
            }
            else if(ficha.name.Contains("Pavo")){

                nombreFichaSinNumero = ficha.name.Remove(ficha.name.Length - 1 );
                ultimoCaracter = ficha.name[ficha.name.Length - 1];

                MoverFichaPavo fichaPavo = ficha.GetComponent<MoverFichaPavo>();

                contPavo += 1;

                var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
                var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

                var estado = new BsonDocument();

                if(!datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter) && datosPartida["montarTablero2PrimeraVez"] == true){
                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaPavo.poderMoverFichaPavo
                        },
                        {
                            "turnoAnterior", fichaPavo.turnoAnterior 
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    // Actualizar el documento en MongoDB
                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
                else if(datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter)){

                    Debug.Log("Actualizando ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter);

                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaPavo.poderMoverFichaPavo
                        },
                        {
                            "turnoAnterior", fichaPavo.turnoAnterior 
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }


                
            }
            else if(ficha.name.Contains("Zorro")){

                nombreFichaSinNumero = ficha.name.Remove(ficha.name.Length - 1 );
                ultimoCaracter = ficha.name[ficha.name.Length - 1];

                MoverFichaZorro fichaZorro = ficha.GetComponent<MoverFichaZorro>();

                contZorro += 1;

                var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
                var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

                var estado = new BsonDocument();

                if(!datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter) && datosPartida["montarTablero2PrimeraVez"] == true){
                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaZorro.poderMoverFichaZorro
                        },
                        {
                            "turnoAnterior", fichaZorro.turnoAnterior 
                        },
                        {
                            "posXAnterior" , fichaZorro.posXAnterior 
                        },
                        {
                            "posYAnterior" , fichaZorro.posYAnterior
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }

                    };

                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
                else if(datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter)){

                    Debug.Log("Actualizando ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter);

                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        {
                            "poderMoverFicha", fichaZorro.poderMoverFichaZorro
                        },
                        {
                            "turnoAnterior", fichaZorro.turnoAnterior 
                        },
                        {
                            "posXAnterior" , fichaZorro.posXAnterior 
                        },
                        {
                            "posYAnterior" , fichaZorro.posYAnterior
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }

                    }; 

                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);             
                }

            }
            else if(ficha.name.Contains("Pino")){

                nombreFichaSinNumero = ficha.name.Remove(ficha.name.Length - 1 );
                ultimoCaracter = ficha.name[ficha.name.Length - 1];

                contPino += 1;

                var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
                var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

                var estado = new BsonDocument();

                if(!datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter) && datosPartida["montarTablero2PrimeraVez"] == true){
                    // Crear documentos BSON para cada componente

                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    // Actualizar el documento en MongoDB
                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
                else if(datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter)){

                    Debug.Log("Actualizando ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter);

                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    // Actualizar el documento en MongoDB
                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);

                }
                
            }
            else if(ficha.name.Contains("Arbol")){

                nombreFichaSinNumero = ficha.name.Remove(ficha.name.Length - 1 );
                ultimoCaracter = ficha.name[ficha.name.Length - 1];

                contArbol += 1;

                var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
                var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

                var estado = new BsonDocument();

                if(!datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter) && datosPartida["montarTablero2PrimeraVez"] == true){
                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    // Actualizar el documento en MongoDB
                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);
                }
                else if(datosPartida.Contains("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter)){

                    Debug.Log("Actualizando ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter);

                    // Crear documentos BSON para cada componente
                    estado = new BsonDocument
                    { 
                        { 
                            "nombre", ficha.name
                        },
                        { "posicion", new BsonDocument
                            {
                                { "x", posicion.x },
                                { "y", posicion.y },
                                { "z", posicion.z }
                            }
                        },
                        { "rotacion", new BsonDocument
                            {
                                { "x", rotacion.x },
                                { "y", rotacion.y },
                                { "z", rotacion.z }
                            }
                        },
                        { "escala", new BsonDocument
                            {
                                { "x", escala.x },
                                { "y", escala.y },
                                { "z", escala.z }
                            }
                        }
                            
                    };

                    // Actualizar el documento en MongoDB
                    var update = Builders<BsonDocument>.Update.Set("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter ,estado);
                    var result = await collectionPartidas.UpdateOneAsync(filtro, update);

                }
            }
            
        }

        contZorro = 0;
        contCazador = 0;
        contOso = 0;
        contPato = 0;
        contPavo = 0;
        contLeñador = 0;
        contPino = 0;
        contArbol = 0;

        if(fichasEliminadas2.Count <= 0){
            Debug.Log("Nada que borrar XDD");
        }
        else{

            var filtroBorrarFicha2 = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);

            for (int i = 0; i < fichasEliminadas2.Count; i++)
            {
                Debug.Log("Eliminando ficha en la base de datos: " + fichasEliminadas2[i]);
                var eliminarFicha2 = Builders<BsonDocument>.Update.Unset(fichasEliminadas2[i]);
                await collectionPartidas.UpdateOneAsync(filtroBorrarFicha2, eliminarFicha2);
            }

        }

        //Aqui el valor de montarTablero2PrimeraVez se volvera false para indicar de que ya se montó una vez 
        var filtroMontarPartida = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);

        var cambiarMontarTablero2PrimeraVez = Builders<BsonDocument>.Update.Set("montarTablero2PrimeraVez",false);
        await collectionPartidas.UpdateOneAsync(filtroMontarPartida, cambiarMontarTablero2PrimeraVez);

        guardarOCargar.SetActive(false);

        gameManager.setPoderClickear(true);

        gameManager.ActivarYDesactivarBotones(true,true,true);

        Debug.Log("Guardando datos ficha correctamente");
    }

    public async void CargarTablero2(){

        var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
        var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

        var posicion = datosPartida["tablero2"]["posicion"].ToBsonDocument();
        var rotacion = datosPartida["tablero2"]["rotacion"].ToBsonDocument();
        var escala = datosPartida["tablero2"]["escala"].ToBsonDocument();

        //Obtener posicion
        float PosX = (float)posicion["x"].ToDouble();
        float PosY = (float)posicion["y"].ToDouble();
        float PosZ = (float)posicion["z"].ToDouble();

        //Obtener rotacion
        float RotX = (float)rotacion["x"].ToDouble();
        float RotY = (float)rotacion["y"].ToDouble();
        float RotZ = (float)rotacion["z"].ToDouble();

        //Obtener escala
        float EscX = (float)escala["x"].ToDouble();
        float EscY = (float)escala["y"].ToDouble();
        float EscZ = (float)escala["z"].ToDouble();

        tablero2.transform.position = new Vector3(PosX, PosY, PosZ);
        tablero2.transform.rotation = Quaternion.Euler(RotX,RotY,RotZ);
        tablero2.transform.localScale = new Vector3(EscX,EscY,EscZ);

    }

    public async void CargarFichas2(){

        Debug.Log("Cargando fihas 2 espere...");

        guardarOCargar.SetActive(true);

        var filtro = Builders<BsonDocument>.Filter.Eq("partida", partidaPasada);
        var datosPartida = await collectionPartidas.Find(filtro).FirstOrDefaultAsync();

        for (int contCazador = 1; contCazador <= 8; contCazador++) {

            if(!datosPartida.Contains("fichaCazadorTablero2 " + contCazador)){
                continue;
            }
            else{
                var posicion = datosPartida["fichaCazadorTablero2 " + contCazador]["posicion"].ToBsonDocument();
                var rotacion = datosPartida["fichaCazadorTablero2 " + contCazador]["rotacion"].ToBsonDocument();
                var escala = datosPartida["fichaCazadorTablero2 " + contCazador]["escala"].ToBsonDocument();
                bool poderMoverFicha = datosPartida["fichaCazadorTablero2 " + contCazador]["poderMoverFicha"].ToBoolean();
                int turnoAnteriorCazador = datosPartida["fichaCazadorTablero2 " + contCazador]["turnoAnterior"].ToInt32();
                int posXAnterior = datosPartida["fichaCazadorTablero2 " + contCazador]["posXAnterior"].ToInt32();
                int posYAnterior = datosPartida["fichaCazadorTablero2 " + contCazador]["posYAnterior"].ToInt32();
                
                //Obtener posicion
                float PosX = (float)posicion["x"].ToDouble();
                float PosY = (float)posicion["y"].ToDouble();
                float PosZ = (float)posicion["z"].ToDouble();

                //Obtener rotacion
                float RotX = (float)rotacion["x"].ToDouble();
                float RotY = (float)rotacion["y"].ToDouble();
                float RotZ = (float)rotacion["z"].ToDouble();

                //Obtener escala
                float EscX = (float)escala["x"].ToDouble();
                float EscY = (float)escala["y"].ToDouble();
                float EscZ = (float)escala["z"].ToDouble();

                Quaternion rotacionn = Quaternion.Euler(RotX,RotY,RotZ);
                
                Vector3 posicionCasillas = new Vector3(PosX * 1f, 0f, PosZ * 1f);
                GameObject spawnFichas = Instantiate(fichas[0], posicionCasillas, rotacionn);
                spawnFichas.transform.SetParent(tablero2.transform);
                spawnFichas.name = "Cazador"+contCazador;

                Debug.Log("PoderMoverFichaCazador: " + poderMoverFicha);

                spawnFichas.GetComponent<MoverFichaCazador>().poderMoverFichaCazador = poderMoverFicha;
                spawnFichas.GetComponent<MoverFichaCazador>().turnoAnterior = turnoAnteriorCazador;
                spawnFichas.GetComponent<MoverFichaCazador>().posXAnterior = posXAnterior;
                spawnFichas.GetComponent<MoverFichaCazador>().posYAnterior = posYAnterior;
            }

        }

        for (int contLeñador = 1; contLeñador <= 2; contLeñador++) {

            if(!datosPartida.Contains("fichaLeñadorTablero2 " + contLeñador)){
                continue;
            }
            else{

                var posicion = datosPartida["fichaLeñadorTablero2 " + contLeñador]["posicion"].ToBsonDocument();
                var rotacion = datosPartida["fichaLeñadorTablero2 " + contLeñador]["rotacion"].ToBsonDocument();
                var escala = datosPartida["fichaLeñadorTablero2 " + contLeñador]["escala"].ToBsonDocument();
                bool poderMoverFicha = datosPartida["fichaLeñadorTablero2 " + contLeñador]["poderMoverFicha"].ToBoolean();
                int turnoAnteriorLeñador = datosPartida["fichaLeñadorTablero2 " + contLeñador]["turnoAnterior"].ToInt32();
                int posXAnterior = datosPartida["fichaLeñadorTablero2 " + contLeñador]["posXAnterior"].ToInt32();
                int posYAnterior = datosPartida["fichaLeñadorTablero2 " + contLeñador]["posYAnterior"].ToInt32();

                //Obtener posicion
                float PosX = (float)posicion["x"].ToDouble();
                float PosY = (float)posicion["y"].ToDouble();
                float PosZ = (float)posicion["z"].ToDouble();

                //Obtener rotacion
                float RotX = (float)rotacion["x"].ToDouble();
                float RotY = (float)rotacion["y"].ToDouble();
                float RotZ = (float)rotacion["z"].ToDouble();

                //Obtener escala
                float EscX = (float)escala["x"].ToDouble();
                float EscY = (float)escala["y"].ToDouble();
                float EscZ = (float)escala["z"].ToDouble();

                Quaternion rotacionn = Quaternion.Euler(RotX,RotY,RotZ);
                
                Vector3 posicionCasillas = new Vector3(PosX * 1f, 0f, PosZ * 1f);
                GameObject spawnFichas = Instantiate(fichas[1], posicionCasillas, rotacionn);
                spawnFichas.transform.SetParent(tablero2.transform);
                spawnFichas.name = "Leñador"+contLeñador;

                spawnFichas.GetComponent<MoverFichaLenyador>().poderMoverFichaLeñador = poderMoverFicha;
                spawnFichas.GetComponent<MoverFichaLenyador>().turnoAnterior = turnoAnteriorLeñador;
                spawnFichas.GetComponent<MoverFichaLenyador>().posXAnterior = posXAnterior;
                spawnFichas.GetComponent<MoverFichaLenyador>().posYAnterior = posYAnterior;

            }

        }

        for (int contOso = 1; contOso <= 2; contOso++) {

            if(!datosPartida.Contains("fichaOsoTablero2 " + contOso)){
                continue;
            }
            else{

                var posicion = datosPartida["fichaOsoTablero2 " + contOso]["posicion"].ToBsonDocument();
                var rotacion = datosPartida["fichaOsoTablero2 " + contOso]["rotacion"].ToBsonDocument();
                var escala = datosPartida["fichaOsoTablero2 " + contOso]["escala"].ToBsonDocument();
                bool poderMoverFicha = datosPartida["fichaOsoTablero2 " + contOso]["poderMoverFicha"].ToBoolean();
                int turnoAnteriorOso = datosPartida["fichaOsoTablero2 " + contOso]["turnoAnterior"].ToInt32();
                int posXAnterior = datosPartida["fichaOsoTablero2 " + contOso]["posXAnterior"].ToInt32();
                int posYAnterior = datosPartida["fichaOsoTablero2 " + contOso]["posYAnterior"].ToInt32();

                //Obtener posicion
                float PosX = (float)posicion["x"].ToDouble();
                float PosY = (float)posicion["y"].ToDouble();
                float PosZ = (float)posicion["z"].ToDouble();

                //Obtener rotacion
                float RotX = (float)rotacion["x"].ToDouble();
                float RotY = (float)rotacion["y"].ToDouble();
                float RotZ = (float)rotacion["z"].ToDouble();

                //Obtener escala
                float EscX = (float)escala["x"].ToDouble();
                float EscY = (float)escala["y"].ToDouble();
                float EscZ = (float)escala["z"].ToDouble();

                Quaternion rotacionn = Quaternion.Euler(RotX,RotY,RotZ);
                
                Vector3 posicionCasillas = new Vector3(PosX * 1f, 0f, PosZ * 1f);
                GameObject spawnFichas = Instantiate(fichas[2], posicionCasillas, rotacionn);
                spawnFichas.transform.SetParent(tablero2.transform);
                spawnFichas.name = "Oso"+contOso;

                spawnFichas.GetComponent<MoverFichaOso>().poderMoverFichaOso = poderMoverFicha;
                spawnFichas.GetComponent<MoverFichaOso>().turnoAnterior = turnoAnteriorOso;
                spawnFichas.GetComponent<MoverFichaOso>().posXAnterior = posXAnterior;
                spawnFichas.GetComponent<MoverFichaOso>().posYAnterior = posYAnterior;
            }
            
        }

        for (int contPato = 1; contPato <= 7; contPato++) {

            if(!datosPartida.Contains("fichaPatoTablero2 " + contPato)){
                continue;
            }
            else{

                var posicion = datosPartida["fichaPatoTablero2 " + contPato]["posicion"].ToBsonDocument();
                var rotacion = datosPartida["fichaPatoTablero2 " + contPato]["rotacion"].ToBsonDocument();
                var escala = datosPartida["fichaPatoTablero2 " + contPato]["escala"].ToBsonDocument();
                bool poderMoverFicha = datosPartida["fichaPatoTablero2 " + contPato]["poderMoverFicha"].ToBoolean();
                int turnoAnteriorPato = datosPartida["fichaPatoTablero2 " + contPato]["turnoAnterior"].ToInt32();
                bool fichaDisponiblePato = datosPartida["fichaPatoTablero2 " + contPato]["fichaDisponible"].ToBoolean();

                //Obtener posicion
                float PosX = (float)posicion["x"].ToDouble();
                float PosY = (float)posicion["y"].ToDouble();
                float PosZ = (float)posicion["z"].ToDouble();

                //Obtener rotacion
                float RotX = (float)rotacion["x"].ToDouble();
                float RotY = (float)rotacion["y"].ToDouble();
                float RotZ = (float)rotacion["z"].ToDouble();

                //Obtener escala
                float EscX = (float)escala["x"].ToDouble();
                float EscY = (float)escala["y"].ToDouble();
                float EscZ = (float)escala["z"].ToDouble();

                Quaternion rotacionn = Quaternion.Euler(RotX,RotY,RotZ);
                
                Vector3 posicionCasillas = new Vector3(PosX * 1f, 0f, PosZ * 1f);
                GameObject spawnFichas = Instantiate(fichas[3], posicionCasillas, rotacionn);
                spawnFichas.transform.SetParent(tablero2.transform);
                spawnFichas.name = "Pato"+contPato;

                spawnFichas.GetComponent<MoverFichaPato>().poderMoverFichaPato = poderMoverFicha;
                spawnFichas.GetComponent<MoverFichaPato>().turnoAnterior = turnoAnteriorPato;
                spawnFichas.GetComponent<MoverFichaPato>().fichaDisponiblePato = fichaDisponiblePato;

            }
        }

        for (int contPavo = 1; contPavo <= 8; contPavo++) {

            if(!datosPartida.Contains("fichaPavoTablero2 " + contPavo)){
                continue;
            }
            else{
                var posicion = datosPartida["fichaPavoTablero2 " + contPavo]["posicion"].ToBsonDocument();
                var rotacion = datosPartida["fichaPavoTablero2 " + contPavo]["rotacion"].ToBsonDocument();
                var escala = datosPartida["fichaPavoTablero2 " + contPavo]["escala"].ToBsonDocument();
                bool poderMoverFicha = datosPartida["fichaPavoTablero2 " + contPavo]["poderMoverFicha"].ToBoolean();
                int turnoAnteriorPavo = datosPartida["fichaPavoTablero2 " + contPavo]["turnoAnterior"].ToInt32();

                //Obtener posicion
                float PosX = (float)posicion["x"].ToDouble();
                float PosY = (float)posicion["y"].ToDouble();
                float PosZ = (float)posicion["z"].ToDouble();

                //Obtener rotacion
                float RotX = (float)rotacion["x"].ToDouble();
                float RotY = (float)rotacion["y"].ToDouble();
                float RotZ = (float)rotacion["z"].ToDouble();

                //Obtener escala
                float EscX = (float)escala["x"].ToDouble();
                float EscY = (float)escala["y"].ToDouble();
                float EscZ = (float)escala["z"].ToDouble();

                Quaternion rotacionn = Quaternion.Euler(RotX,RotY,RotZ);
                
                Vector3 posicionCasillas = new Vector3(PosX * 1f, 0f, PosZ * 1f);
                GameObject spawnFichas = Instantiate(fichas[4], posicionCasillas, rotacionn);
                spawnFichas.transform.SetParent(tablero2.transform);
                spawnFichas.name = "Pavo"+contPavo;

                spawnFichas.GetComponent<MoverFichaPavo>().poderMoverFichaPavo = poderMoverFicha;
                spawnFichas.GetComponent<MoverFichaPavo>().turnoAnterior = turnoAnteriorPavo;
            }

        }

        for (int contZorro = 1; contZorro <= 6; contZorro++) {

            if(!datosPartida.Contains("fichaZorroTablero2 " + contZorro)){
                continue;
            }
            else{
                var posicion = datosPartida["fichaZorroTablero2 " + contZorro]["posicion"].ToBsonDocument();
                var rotacion = datosPartida["fichaZorroTablero2 " + contZorro]["rotacion"].ToBsonDocument();
                var escala = datosPartida["fichaZorroTablero2 " + contZorro]["escala"].ToBsonDocument();
                bool poderMoverFicha = datosPartida["fichaZorroTablero2 " + contZorro]["poderMoverFicha"].ToBoolean();
                int turnoAnteriorZorro = datosPartida["fichaZorroTablero2 " + contZorro]["turnoAnterior"].ToInt32();
                int posXAnterior = datosPartida["fichaZorroTablero2 " + contZorro]["posXAnterior"].ToInt32();
                int posYAnterior = datosPartida["fichaZorroTablero2 " + contZorro]["posYAnterior"].ToInt32();

                //Obtener posicion
                float PosX = (float)posicion["x"].ToDouble();
                float PosY = (float)posicion["y"].ToDouble();
                float PosZ = (float)posicion["z"].ToDouble();

                //Obtener rotacion
                float RotX = (float)rotacion["x"].ToDouble();
                float RotY = (float)rotacion["y"].ToDouble();
                float RotZ = (float)rotacion["z"].ToDouble();

                //Obtener escala
                float EscX = (float)escala["x"].ToDouble();
                float EscY = (float)escala["y"].ToDouble();
                float EscZ = (float)escala["z"].ToDouble();

                Quaternion rotacionn = Quaternion.Euler(RotX,RotY,RotZ);
                
                Vector3 posicionCasillas = new Vector3(PosX * 1f, 0f, PosZ * 1f);
                GameObject spawnFichas = Instantiate(fichas[5], posicionCasillas, rotacionn);
                spawnFichas.transform.SetParent(tablero2.transform);
                spawnFichas.name = "Zorro"+contZorro;

                spawnFichas.GetComponent<MoverFichaZorro>().poderMoverFichaZorro = poderMoverFicha;
                spawnFichas.GetComponent<MoverFichaZorro>().turnoAnterior = turnoAnteriorZorro;
                spawnFichas.GetComponent<MoverFichaZorro>().posXAnterior = posXAnterior;
                spawnFichas.GetComponent<MoverFichaZorro>().posYAnterior = posYAnterior;
            }
        }

        for (int contPino = 1; contPino <= 7; contPino++) {

            if(!datosPartida.Contains("fichaPinoTablero2 " + contPino)){
                continue;
            }
            else{

                var posicion = datosPartida["fichaPinoTablero2 " + contPino]["posicion"].ToBsonDocument();
                var rotacion = datosPartida["fichaPinoTablero2 " + contPino]["rotacion"].ToBsonDocument();
                var escala = datosPartida["fichaPinoTablero2 " + contPino]["escala"].ToBsonDocument();

                //Obtener posicion
                float PosX = (float)posicion["x"].ToDouble();
                float PosY = (float)posicion["y"].ToDouble();
                float PosZ = (float)posicion["z"].ToDouble();

                //Obtener rotacion
                float RotX = (float)rotacion["x"].ToDouble();
                float RotY = (float)rotacion["y"].ToDouble();
                float RotZ = (float)rotacion["z"].ToDouble();

                //Obtener escala
                float EscX = (float)escala["x"].ToDouble();
                float EscY = (float)escala["y"].ToDouble();
                float EscZ = (float)escala["z"].ToDouble();

                Quaternion rotacionn = Quaternion.Euler(RotX,RotY,RotZ);
                
                Vector3 posicionCasillas = new Vector3(PosX * 1f, 0f, PosZ * 1f);
                GameObject spawnFichas = Instantiate(fichas[6], posicionCasillas, rotacionn);
                spawnFichas.transform.SetParent(tablero2.transform);
                spawnFichas.name = "Pino"+contPino;

            }
        }

        for (int contArbol = 1; contArbol <= 8; contArbol++) {

            if(!datosPartida.Contains("fichaArbolTablero2 " + contArbol)){
                continue;
            }
            else{

                var posicion = datosPartida["fichaArbolTablero2 " + contArbol]["posicion"].ToBsonDocument();
                var rotacion = datosPartida["fichaArbolTablero2 " + contArbol]["rotacion"].ToBsonDocument();
                var escala = datosPartida["fichaArbolTablero2 " + contArbol]["escala"].ToBsonDocument();

                //Obtener posicion
                float PosX = (float)posicion["x"].ToDouble();
                float PosY = (float)posicion["y"].ToDouble();
                float PosZ = (float)posicion["z"].ToDouble();

                //Obtener rotacion
                float RotX = (float)rotacion["x"].ToDouble();
                float RotY = (float)rotacion["y"].ToDouble();
                float RotZ = (float)rotacion["z"].ToDouble();

                //Obtener escala
                float EscX = (float)escala["x"].ToDouble();
                float EscY = (float)escala["y"].ToDouble();
                float EscZ = (float)escala["z"].ToDouble();

                Quaternion rotacionn = Quaternion.Euler(RotX,RotY,RotZ);
                
                Vector3 posicionCasillas = new Vector3(PosX * 1f, 0f, PosZ * 1f);
                GameObject spawnFichas = Instantiate(fichas[7], posicionCasillas, rotacionn);
                spawnFichas.transform.SetParent(tablero2.transform);
                spawnFichas.name = "Arbol"+contArbol;

            }
        }

        Debug.Log("Fichas 2 cargadas con exito...");

        guardarOCargar.SetActive(false);

        gameManager.ActivarYDesactivarBotones(true,true,true);
    }

    public void QuitarDisponibilidadFichaPasada2(GameObject fichaPasada){

        string nombreFicha = fichaPasada.name;

        string nombreFichaSinNumero = nombreFicha.Remove(nombreFicha.Length - 1 );
        char ultimoCaracter = nombreFicha[nombreFicha.Length - 1];

        Debug.Log("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter);

        //Guarda la ficha eliminada sino se guarda la parida no habrán cambios
        fichasEliminadas2.Add("ficha" + nombreFichaSinNumero + "Tablero2 " + ultimoCaracter);

        Destroy(fichaPasada);
    }
}
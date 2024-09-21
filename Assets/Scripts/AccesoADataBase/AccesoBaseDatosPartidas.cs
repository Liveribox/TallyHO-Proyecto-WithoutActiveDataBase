using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;


public class AccesoBaseDatosPartidas : MonoBehaviour
{

    MongoClient client = new MongoClient("");
    IMongoDatabase database;
    IMongoCollection<BsonDocument> collectionUsuarios;
    IMongoCollection<BsonDocument> collectionPartidas;

    public TMP_InputField partidaInput;

    public TMP_InputField jugador1Input;

    public TMP_InputField jugador2Input;

    public TextMeshProUGUI textoMensajePartida;
    public TextMeshProUGUI textoMensajePartida2;

    public RectTransform scrollViewContent;
    public GameObject botonCrear;

    string usuarioPasado = "";

    private GameObject Canvas;
    private GameObject Canvas2;

    public Sprite imagenBotonSprite;

    // Start is called before the first frame update
    void Start()
    {
        database = client.GetDatabase("TallyHODB!");
        collectionUsuarios = database.GetCollection<BsonDocument>("Usuarios");
        collectionPartidas = database.GetCollection<BsonDocument>("Partidas");

        usuarioPasado = PlayerPrefs.GetString("usuarioPasado");

        Canvas = GameObject.Find("Canvas");
        Canvas.SetActive(true);

        Canvas2 = GameObject.Find("Canvas2");
        Canvas2.SetActive(false);

        EnseñarPartidas(usuarioPasado);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void CrearPartida(){

        if(partidaInput.text.Trim() == "" || jugador1Input.text.Trim() == "" || jugador2Input.text.Trim() == ""){
            textoMensajePartida2.text = "Campos incompletos";
        }
        else{

            var filtroPartidaExistente = Builders<BsonDocument>.Filter.Eq("partida", partidaInput.text);
            var partida = await collectionPartidas.Find(filtroPartidaExistente).FirstOrDefaultAsync();

            if(partida != null){
                textoMensajePartida2.text = "Un usuario ya creó una partida con ese nombre";
            }
            else if(partida == null){

                if(jugador1Input.text.Trim() == jugador2Input.text.Trim()){
                    textoMensajePartida2.text = "Los jugadores no pueden tener el mismo nombre";
                }
                else{

                    var filtroUsuario = Builders<BsonDocument>.Filter.Eq("usuario", usuarioPasado);
                    var usuario = await collectionUsuarios.Find(filtroUsuario).FirstOrDefaultAsync();

                    ObjectId idUsuario = usuario["_id"].AsObjectId;

                    var datosCrearPartida = new BsonDocument { {"partida",partidaInput.text.Trim()},{"CreadorID", idUsuario.ToString()},{"Creador",usuarioPasado},{"jugador 1", jugador1Input.text.Trim()},{"jugador 2",jugador2Input.text.Trim()},{"fichassinrotar", 48},{"turnosRestantesJugador1", 5},{"turnosRestantesJugador2", 5},{"numeroFichasJugador1",8},{"numeroFichasJugador2",10},{"puntosJugador1",0},{"puntosJugador2",0},{"fichasObtenidasJugador1",0},{"fichasObtenidasJugador2",0},{"rondaActual", 1},{"turnoActual","Animal"},{"jugadorActual","J1"},{"turnosRegistrados",0},{"montarTablero1PrimeraVez",true},{"montarTablero2PrimeraVez",true} };
                    await collectionPartidas.InsertOneAsync(datosCrearPartida);

                    
                    var filtroPartidaNueva = Builders<BsonDocument>.Filter.Eq("partida", partidaInput.text.Trim());
                    var partidaNueva = await collectionPartidas.Find(filtroPartidaNueva).FirstOrDefaultAsync();

                    Debug.Log("Partida nueva creada: " + partidaNueva["partida"]);

                    CrearBoton(partidaNueva["partida"].ToString());


                    if(partidaNueva == null){
                        textoMensajePartida.text = "Error al entrar en partida";
                    }
                    else if(partidaNueva != null){

                        textoMensajePartida2.text = "Partida creada con exito";

                        PlayerPrefs.SetString("partidaPasado",partidaNueva["partida"].ToString());

                        SceneManager.LoadScene(2);

                    }
                }
            }
        }
        
    }

    public async void UnirsePartida(string buttonName){

            var filtroUnirsePartida = Builders<BsonDocument>.Filter.Eq("partida", buttonName);
            var partidaUnirse = await collectionPartidas.Find(filtroUnirsePartida).FirstOrDefaultAsync();

            if(partidaUnirse == null){
                textoMensajePartida.text = "Error al entrar en partida";
            }
            else if(partidaUnirse != null){

                PlayerPrefs.SetString("partidaPasado",partidaUnirse["partida"].ToString());

                SceneManager.LoadScene(2);

            }
        
    }

    public async void EnseñarPartidas(string nombreUsuario){
        var colecciones = database.ListCollectionNames().ToList();

        foreach (var coleccion in colecciones)
        {
            var colecionn = database.GetCollection<BsonDocument>(coleccion);
            var filtroUsuario = Builders<BsonDocument>.Filter.Eq("Creador", nombreUsuario);
            var buscarTodasLasPartidas = await colecionn.FindAsync(filtroUsuario);
            var documentos = await buscarTodasLasPartidas.ToListAsync();

            foreach (var documento in documentos)
            {
                if (documento.Contains("partida"))
                {
                    var partida = documento["partida"];

                    CrearBoton(partida.ToString());

                }
            }
        }
    }


    private void CrearBoton(string nombrePartida){

        GameObject botonObjeto = Instantiate(botonCrear, scrollViewContent);

        RectTransform rectTransform = botonObjeto.GetComponent<RectTransform>();

        Image imagenBoton = botonObjeto.GetComponent<Image>();

        imagenBoton.sprite = imagenBotonSprite;

        float ancho = 350f;
        float alto = 60f;
        rectTransform.sizeDelta = new Vector2(ancho, alto);

        
        botonObjeto.transform.localScale = Vector3.one;
        
        TextMeshProUGUI buttonText = botonObjeto.GetComponentInChildren<TextMeshProUGUI>();

        buttonText.text = nombrePartida;

        Button newButton = botonObjeto.GetComponent<Button>();
        
        newButton.onClick.AddListener(() => UnirsePartida(buttonText.text));

    }

    public void VolverALogin(){
        SceneManager.LoadScene(0);
    }

    public void ActivarCrearPartida(){
        Canvas.SetActive(false);
        Canvas2.SetActive(true);  
    }

    public void VolverAPartidas(){
        Canvas.SetActive(true);
        Canvas2.SetActive(false);
    }
}

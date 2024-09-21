using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    //Para impedir que el cazador muera si intenta eliminar a un oso
    public bool morirCazador = false;

    public string turnoActual = "";
    public string jugadorActual = "";

    //Texto que muestra el truno
    public TextMeshProUGUI turnosTexto;

    private string jugador1 = "";
    private string jugador2 = "";

    //Texto y variables de las fichas sin rotar
    public TextMeshProUGUI fichasVuelta;
    private int fichasVueltaNumero = 48;

    //Texto que te muestra advertencias
    public TextMeshProUGUI advertenciasTexto;
    public bool advertenciaActiva = false;

    //Texto que muestra los turnos una vez todas las fichas estén rotadas
    public TextMeshProUGUI turnosJugador1;
    public TextMeshProUGUI turnosJugador2;

    private int turnosJugador1Numero = 5;
    private int turnosJugador2Numero = 5;
    private bool turnosAlternadosActivados = false;

    //Fichas humanos y animales
    public TextMeshProUGUI numeroTextoFichasJugador1;
    public TextMeshProUGUI numeroTextoFichasJugador2; 
    private int numeroFichasJugador1 = 8;
    private int numeroFichasJugador2 = 10;

    //Puntos jugadores
    public TextMeshProUGUI puntosJugador1Texto;
    public TextMeshProUGUI puntosJugador2Texto;

    private int puntosJugador1 = 0;
    private int puntosJugador2 = 0;

    // Fichas jugadores obtenidas
    public TextMeshProUGUI fichasObtenidasJugador1Texto;
    public TextMeshProUGUI fichasObtenidasJugador2Texto;

    private int fichasObtenidasJugador1 = 0;
    private int fichasObtenidasJugador2 = 0;

    //Texto para saber en que rondas estás
    public TextMeshProUGUI rondasTexto;
    private int rondaActual = 1;

    //Lista de turnos
    //public List<string> turnosRegistrados = new List<string>();
    public int turnosRegistrados = 0;
    public string ultimoTurnoRegistrado = "";

    //Texto e imagen ganador
    public TextMeshProUGUI textoGanador;
    public GameObject fondoGanador;

    //Musica de fondo
    public AudioSource musica;
    public AudioSource musica2;

    //Slider para controlar volumen
    public Slider volumenMusica;
    
    //Partida pasada
    private string partidaPasada = "";

    //Poder clickear
    private bool poderClickear = true;

    public GameObject botonVolver;

    public GameObject botonGuardar;

    public GameObject botonSalir;

    // Start is called before the first frame update
    void Start()
    {

        textoGanador.text = "";
        fondoGanador.SetActive(false);
        
        
    }

    
    // Update is called once per frame
    void Update()
    {
        if(rondaActual == 1){
            musica.volume = volumenMusica.value;
            musica2.volume = 0f;
        }
        else if(rondaActual == 2){
            musica.volume = 0f;
            musica2.volume = volumenMusica.value;
        }

        if(fichasVueltaNumero <= 0){
            
            musica.pitch = 1.3f;
            musica2.pitch = 1.3f;

            turnosAlternadosActivados = true;
            turnosJugador1.text = jugador1 + ": " + turnosJugador1Numero.ToString();
            turnosJugador2.text = jugador2 + ": " + turnosJugador2Numero.ToString();
        }
        else{
            turnosJugador1.text = "";
            turnosJugador2.text = "";
        }

    }

    public void RecibirInformacionUsuario(string jugador1,string jugador2,int fichasSinRotar,int numeroFichasJugador1 , int numeroFichasJugador2 , int fichasObtenidasJugador1, int fichasObtenidasJugador2,int puntosJugador1,int puntosJugador2,int rondaActual,int turnosJugador1Numero,int turnosJugador2Numero,string turnoActual,string jugadorActual,int turnosRegistrados)
    {
        this.jugador1 = jugador1;
        this.jugador2 = jugador2;
        this.fichasVueltaNumero = fichasSinRotar;
        this.numeroFichasJugador1 = numeroFichasJugador1;
        this.numeroFichasJugador2 = numeroFichasJugador2;
        this.fichasObtenidasJugador1 = fichasObtenidasJugador1;
        this.fichasObtenidasJugador2 = fichasObtenidasJugador2;
        this.puntosJugador1 = puntosJugador1;
        this.puntosJugador2 = puntosJugador2;
        this.rondaActual = rondaActual;
        this.turnosJugador1Numero = turnosJugador1Numero;
        this.turnosJugador2Numero = turnosJugador2Numero;
        this.turnoActual = turnoActual;
        this.jugadorActual = jugadorActual;

        fichasVuelta.text = "Fichas sin rotar: " + fichasVueltaNumero.ToString();

        this.numeroTextoFichasJugador1.text = jugador1 + ": " + this.numeroFichasJugador1.ToString();
        this.numeroTextoFichasJugador2.text = jugador2 + ": " + this.numeroFichasJugador2.ToString();

        
        this.fichasObtenidasJugador1Texto.text = jugador1 + ": " + this.fichasObtenidasJugador1.ToString();
        this.fichasObtenidasJugador2Texto.text = jugador2 + ": " + this.fichasObtenidasJugador2.ToString();

        this.puntosJugador1Texto.text = jugador1 + ": " + this.puntosJugador1.ToString();
        this.puntosJugador2Texto.text = jugador2 + ": " + this.puntosJugador2.ToString();

        this.rondasTexto.text = "Ronda " + rondaActual.ToString() + "/2";

        this.turnosJugador1.text = jugador1 + ": " + turnosJugador1Numero.ToString();
        this.turnosJugador2.text = jugador2 + ": " + turnosJugador2Numero.ToString();

        this.turnosRegistrados = turnosRegistrados;

        RestaurarTurno();

    }


    public void RestaurarTurno(){

        if(rondaActual == 1){
            if(turnoActual == "Animal" && jugadorActual == "J1"){
                turnosTexto.text = "Turno " + jugador1 + " (Animal)";
            }
            else if(turnoActual == "Humano" && jugadorActual == "J2"){
                turnosTexto.text = "Turno " + jugador2 + " (Humano)";
            }
        }
        else if(rondaActual == 2){
            if(turnoActual == "Animal" && jugadorActual == "J2"){
                turnosTexto.text = "Turno " + jugador2 + " (Animal)";
            }
            else if(turnoActual == "Humano" && jugadorActual == "J1"){
                turnosTexto.text = "Turno " + jugador1 + " (Humano)";
            }
        }

    }

    //Cambia el turno del animal y del humano , tambien el texto de los turnos
    public void CambiarTurno(){

        
        if(rondaActual == 1){

            if(turnoActual == "Animal" && jugadorActual == "J1"){
                turnoActual = "Humano";
                jugadorActual = "J2";
                turnosTexto.text = "Turno " + jugador2 + " (Humano)";
            }
            else{
                turnoActual = "Animal";
                jugadorActual = "J1";
                turnosTexto.text = "Turno " + jugador1 + " (Animal)";
            }
        }
        else if(rondaActual == 2){

            if(turnoActual == "Animal" && jugadorActual == "J2"){
                turnoActual = "Humano";
                jugadorActual = "J1";
                turnosTexto.text = "Turno " + jugador1 + " (Humano)";
            }
            else{
                turnoActual = "Animal";
                jugadorActual = "J2";
                turnosTexto.text = "Turno " + jugador2 + " (Animal)";
            }
        }

        
        turnosRegistrados += 1;

        //turnosRegistrados.Add(turnoActual.ToString());

        Debug.Log("Turnos registrados: " + turnosRegistrados);
    }

    public void QuitarFichasVuelta(int valorFicha){
        fichasVueltaNumero -= valorFicha;
        fichasVuelta.text = "Fichas sin rotar: " + fichasVueltaNumero.ToString();
    }

    public void DescontarTurno(int turnoUsado){
        if(turnosAlternadosActivados){

            if(rondaActual == 1){
                if(turnoActual == "Animal" && jugadorActual == "J1"){
                    turnosJugador1Numero -= turnoUsado;
                    turnosJugador1.text = "Turnos " + jugador1 + ": " + turnosJugador1Numero.ToString();
                }
                else if(turnoActual == "Humano" && jugadorActual == "J2"){
                    turnosJugador2Numero -= turnoUsado;
                    turnosJugador2.text = "Turnos " + jugador2 + ": " + turnosJugador2Numero.ToString();
                }
            }
            else if(rondaActual == 2){
                if(turnoActual == "Animal" && jugadorActual == "J2"){
                    turnosJugador2Numero -= turnoUsado;
                    turnosJugador2.text = "Turnos " + jugador2 + ": " + turnosJugador2Numero.ToString();
                }
                else if(turnoActual == "Humano" && jugadorActual == "J1"){
                    turnosJugador1Numero -= turnoUsado;
                    turnosJugador1.text = "Turnos " + jugador1 + ": " + turnosJugador1Numero.ToString();
                }
            }

        }
        else{
            Debug.Log("Aun no es el momento de hacer esto");
        }
        
    }

    public void EliminarFichaJugador1(int numeroFichasJugador1){
        this.numeroFichasJugador1 -= numeroFichasJugador1;
        numeroTextoFichasJugador1.text = jugador1 + this.numeroFichasJugador1.ToString();
        
    }

    public void EliminarFichaJugador2(int numeroFichasJugador2){
        this.numeroFichasJugador2 -= numeroFichasJugador2;
        numeroTextoFichasJugador2.text = jugador2 + ": " + this.numeroFichasJugador2.ToString();
    }

    public void SumarPuntosJugador1(int puntosJugador1){
        this.puntosJugador1 += puntosJugador1;
        puntosJugador1Texto.text = jugador1 + ": " + this.puntosJugador1.ToString();
    }


    public void SumarPuntosJugador2(int puntosJugador2){
        this.puntosJugador2 += puntosJugador2;
        puntosJugador2Texto.text = jugador2 + ": " +this.puntosJugador2.ToString();
    }

    public void SumarFichasObtenidasJugador1(int fichasObtenidasJugador1){
        this.fichasObtenidasJugador1 += fichasObtenidasJugador1;
        fichasObtenidasJugador1Texto.text = jugador1 + ": " + this.fichasObtenidasJugador1.ToString();
    }

    public void SumarFichasObtenidasJugador2(int fichasObtenidasJugador2){
        this.fichasObtenidasJugador2 += fichasObtenidasJugador2;
        fichasObtenidasJugador2Texto.text = jugador2 + ": " + this.fichasObtenidasJugador2.ToString();
    }
    
    public void CambiarRolesTurno(){

        if(turnoActual == "Animal" && jugadorActual == "J1"){
            turnoActual = "Humano";
            jugadorActual = "J1";
            turnosTexto.text = "Turno " + jugador1 + " (Humano)";
        }
        else if(turnoActual == "Humano" && jugadorActual == "J2"){
            turnoActual = "Animal";
            jugadorActual = "J2";
            turnosTexto.text = "Turno " + jugador2 + " (Animal)";
        }

    }

    public void ActivarYDesactivarBotones(bool botonVolverBool , bool botonGuardarBool, bool botonSalirBool){
        botonVolver.SetActive(botonVolverBool);
        botonGuardar.SetActive(botonGuardarBool);
        botonSalir.SetActive(botonSalirBool);
    }

    

    //Getters y setters
    public int getNumeroFichasJugador1() {
        return numeroFichasJugador1;
    }

    public void setNumeroFichasJugador1(int numeroFichasJugador1) {
        this.numeroFichasJugador1 = numeroFichasJugador1;
    }

    public int getNumeroFichasJugador2() {
        return numeroFichasJugador2;
    }

    public void setNumeroFichasJugador2(int numeroFichasJugador2) {
        this.numeroFichasJugador2 = numeroFichasJugador2;
    }

    public int getFichasObtenidasJugador1(){
        return fichasObtenidasJugador1;
    }

    public void setFichasObtenidasJugador1(int fichasObtenidasJugador1){
        this.fichasObtenidasJugador1 = fichasObtenidasJugador1;
    }

    public int getFichasObtenidasJugador2(){
        return fichasObtenidasJugador2;
    }

    public void setFichasObtenidasJugador2(int fichasObtenidasJugador2){
        this.fichasObtenidasJugador2 = fichasObtenidasJugador2;
    }

    public int getTurnosJugador1Numero(){
        return turnosJugador1Numero;
    }

    public void setTurnosJugador1Numero(int turnosJugador1Numero) {
        this.turnosJugador1Numero = turnosJugador1Numero;
    }

    public int getTurnosJugador2Numero(){
        return turnosJugador2Numero;
    }

    public void setTurnosJugador2Numero(int turnosJugador2Numero) {
        this.turnosJugador2Numero = turnosJugador2Numero;
    }

    public bool getTurnosAlternadosActivados(){
        return turnosAlternadosActivados;
    }

    public void setTurnosAlternadosActivados(bool turnosAlternadosActivados) {
        this.turnosAlternadosActivados = turnosAlternadosActivados;
    }

    public int getFichasVueltaNumero(){
        return fichasVueltaNumero;
    }

    public void setFichasVueltaNumero(int fichasVueltaNumero){
        this.fichasVueltaNumero = fichasVueltaNumero;
    }

    public int getRondaActual(){
        return rondaActual;
    }

    public void setRondaActual(int rondaActual){
        this.rondaActual = rondaActual;
    }

    public int getPuntosJugador1(){
        return puntosJugador1;
    }

    public void setPuntosJugador1(int puntosJugador1){
        this.puntosJugador1 = puntosJugador1;
    }

    public int getPuntosJugador2(){
        return puntosJugador2;
    }

    public void setPuntosJugador2(int puntosJugador2){
        this.puntosJugador2 = puntosJugador2;
    }

    public string getJugador1(){
        return jugador1;
    }

    public void setJugador1(string jugador1){
        this.jugador1 = jugador1;
    }

    public string getJugador2(){
        return jugador2;
    }

    public void setJugador2(string jugador2){
        this.jugador2 = jugador2;
    }

    public string getTurnoActual(){
        return turnoActual;
    }

    public void setTurnoActual(string turnoActual){
        this.turnoActual = turnoActual;
    }

    public string getJugadorActual(){
        return jugadorActual;
    }

    public void setJugadorActual(string jugadorActual){
        this.jugadorActual = jugadorActual;
    }

    public int getTurnosRegistrados(){
        return turnosRegistrados;
    }

    public void setTurnosRegistrados(int turnosRegistrados){
        this.turnosRegistrados = turnosRegistrados;
    }

    public bool getPoderClickear(){
        return poderClickear;
    }

    public void setPoderClickear(bool poderClickear){
        this.poderClickear = poderClickear;
    }

}



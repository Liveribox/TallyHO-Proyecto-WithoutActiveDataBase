using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;


public class AccesoBaseDatosUsuarioLogin : MonoBehaviour
{
    
    MongoClient client = new MongoClient("");
    IMongoDatabase database;
    IMongoCollection<BsonDocument> collectionUsuarios;

    IMongoCollection<BsonDocument> collectionPartidas;

    public TMP_InputField usuarioInput;
    public TMP_InputField contraseñaInput;

    public Toggle mostrarContraseñaToggle;


    public TextMeshProUGUI usuarioExisteMensaje;

    // Start is called before the first frame update
    void Start()
    {
        database = client.GetDatabase("TallyHODB!");
        collectionUsuarios = database.GetCollection<BsonDocument>("Usuarios");
        collectionPartidas = database.GetCollection<BsonDocument>("Partidas");

        mostrarContraseñaToggle.isOn = false;

        usuarioExisteMensaje.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void GuardarUsuario(){

        try
        {
            var filtro = Builders<BsonDocument>.Filter.Eq("usuario", usuarioInput.text);
            var usuario = await collectionUsuarios.Find(filtro).FirstOrDefaultAsync();

            //Comprobar si los campos estan completos
            if(usuarioInput.text.Trim() == ""){
                Debug.Log("El campo usuario está vacio");
            }
            else if(contraseñaInput.text.Trim() == ""){
                Debug.Log("El campo contraseña está vacio");
            }
            else if(usuario != null && usuario["usuario"] == usuarioInput.text.Trim()){
                usuarioExisteMensaje.text = "Ese usuario ya existe";
            }
            else{
                var crearUsuario = new BsonDocument { {"usuario", usuarioInput.text.Trim() },{"contraseña",contraseñaInput.text.Trim()} };
                await collectionUsuarios.InsertOneAsync(crearUsuario);

                usuarioExisteMensaje.text = "Usuario " + usuarioInput.text + " creado con exito";
            }    
        }
        catch (Exception ex)
        {
            usuarioExisteMensaje.text = "Error al crear el usuario " + ex.Message;
        }

        
    }

    public async void Verificar(){

        try{

            var filtro = Builders<BsonDocument>.Filter.Eq("usuario", usuarioInput.text);
            var usuario = await collectionUsuarios.Find(filtro).FirstOrDefaultAsync();

            if (usuario != null && usuario["usuario"] == usuarioInput.text.Trim() && usuario["contraseña"] == contraseñaInput.text.Trim())
            {

                usuarioExisteMensaje.text = "Verificacion completada: " + usuario["usuario"].ToString();

                PlayerPrefs.SetString("usuarioPasado",usuario["usuario"].ToString());

                SceneManager.LoadScene(1);
            }
            else{
                usuarioExisteMensaje.text = "Usuario o contraseña incorrecto";
            }
            
        } catch(Exception ex){
            usuarioExisteMensaje.text = "Error al verificar el usuario " + ex.Message;
        }
        
        
    }

    public async void EliminarUsuario(){

        try
        {
            var filtro = Builders<BsonDocument>.Filter.Eq("usuario", usuarioInput.text.Trim());
            var usuario = await collectionUsuarios.Find(filtro).FirstOrDefaultAsync();

            if(usuario != null && usuario["usuario"] == usuarioInput.text.Trim() && usuario["contraseña"] == contraseñaInput.text.Trim()){
                usuarioExisteMensaje.text = "Eliminando usuario";

                var filtroPartidas = Builders<BsonDocument>.Filter.Eq("Creador", usuarioInput.text.Trim());
                var partidas = await collectionPartidas.Find(filtroPartidas).ToListAsync();

                
                foreach (var partida in partidas)
                {
                    if (partida.Contains("Creador"))
                    {
                        await collectionPartidas.DeleteOneAsync(nombreCreador => nombreCreador["Creador"]==usuario["usuario"]);
                    }
                }
                
                await collectionUsuarios.DeleteOneAsync(nombreUsuario => nombreUsuario["usuario"]==usuarioInput.text.Trim());

                usuarioExisteMensaje.text = "Usuario eliminado";
            }
            else{
                usuarioExisteMensaje.text = "Usuario o contraseña incorrecto";
            }        
        }
        catch (Exception ex)
        {
            usuarioExisteMensaje.text = "Error al eliminar el usuario " + ex.Message;
        }

        
    }

    
    public void MostrarContraseña(){

        if(mostrarContraseñaToggle.isOn){
            contraseñaInput.contentType = TMP_InputField.ContentType.Custom;
            contraseñaInput.lineType = TMP_InputField.LineType.SingleLine;
            contraseñaInput.inputType = TMP_InputField.InputType.Standard;
            contraseñaInput.ActivateInputField();
        }
        else{
            contraseñaInput.contentType = TMP_InputField.ContentType.Custom;
            contraseñaInput.lineType = TMP_InputField.LineType.SingleLine;
            contraseñaInput.inputType = TMP_InputField.InputType.Password;
            contraseñaInput.ActivateInputField();
        }

    }

}
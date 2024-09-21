using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaseadorLenyador : MonoBehaviour
{
    public bool avanzar = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.name == "Hierva(Clone)"){
            Destroy(gameObject);
        }
        else if(other.gameObject.name == "Cazador(Clone)" || other.gameObject.name == "Oso(Clone)" || other.gameObject.name == "Zorro(Clone)" || other.gameObject.name == "Pato(Clone)" || other.gameObject.name == "Pavo(Clone)" || other.gameObject.name == "Leñador(Clone)"){
            Destroy(gameObject);
        }
    }


}

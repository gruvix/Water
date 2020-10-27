using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalsaUnwaver : MonoBehaviour
{
    private GameObject Floaters;
    private GameObject Bote;
    // Start is called before the first frame update
    void Start()
    {
        Floaters = GameObject.Find("Floaters");
        Bote = GameObject.Find("Bote");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        VerificadorDeBote();
    }

    public void VerificadorDeBote(){
        bool desatado=true;
        Collider2D[] colisiones = new Collider2D[10];
        //Si es parte del bote verifica estar tocando el bote. Si no esta pasa a ser floater
        if(gameObject.transform.parent.name == "Bote"){
            int cantidad = gameObject.GetComponent<Collider2D>().GetContacts(colisiones);
            foreach(Collider2D col in colisiones){
                //Debug.Log(col);
                if(col == null){
                    break;
                }
                else if(col.tag!="Player" && col.gameObject.transform.parent.name == "Bote"){
                    desatado=false;
                    break;
                }
            }
            if (desatado){
                gameObject.transform.SetParent(Floaters.transform);
                gameObject.GetComponent<FixedJoint2D>().connectedBody = null;
                gameObject.GetComponent<FixedJoint2D>().enabled = false;
                Floaters.GetComponentInParent<SetMaterial>().Resolve();
            }
        }
    }

    public void BoteColiderManager() {
        int i=0;
        Transform thischild;
        bool isBoat=false;
        
        if(gameObject.transform.parent.name == "Bote"){
            isBoat=true;
        }
        // esto deberia recorrer todos los objetos del floater y del bote y asignas las colisiones segun correpondan
        // Setea las solisiones con el bote
        do{
            thischild = Floaters.transform.GetChild(i);
            if(thischild != null){
                Physics2D.IgnoreCollision(thischild.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), isBoat);
                i++;
            }
        }while(thischild != null || i > 50);
        
        // Setea las solisiones con los floaters
        i=0;
        do{
            thischild = Bote.transform.GetChild(i);
            if(thischild != null){
                Physics2D.IgnoreCollision(thischild.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), !isBoat);
                i++;
            }
        }while(thischild != null || i > 50);
    }
}

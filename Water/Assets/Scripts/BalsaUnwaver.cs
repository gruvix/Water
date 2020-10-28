using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FixedJoint2D))]
[RequireComponent(typeof(Rigidbody2D))]
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

}

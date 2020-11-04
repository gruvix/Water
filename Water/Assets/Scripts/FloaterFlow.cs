using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FloaterFlow : MonoBehaviour
{
    List<Object> prefabList = new List<Object>();

    private GameObject Floaters;

    public Vector2 SpawnPoint = new Vector2(-3.69f,0.51f);

    [Range(1, 50)]
    public int MaxFloater=10;

    private int count_time=0;
    
    // Start is called before the first frame update
    private void Start()
    {
        Floaters = GameObject.Find("Floaters");
        // Levanta todos los prefab de la carpeta Resources/Floaters
        foreach(Object i in  Resources.LoadAll("Floaters", typeof(GameObject))){
            prefabList.Add(i);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        int prefabIndex;
        if(count_time>=1000){
            count_time=0;
            if(gameObject.transform.childCount < MaxFloater){
                prefabIndex = UnityEngine.Random.Range(0,prefabList.Count);
                GameObject NewFloater = Instantiate(prefabList[prefabIndex]) as GameObject;
                NewFloater.transform.position = SpawnPoint;
                NewFloater.transform.parent = Floaters.transform;
                NewFloater.layer = 9;
            }
        }
        else{
            count_time++;
        }
    }
    
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player" || col.tag == "Gema" )
        {
            Debug.Log("Game Over");
        }

        if (col.tag != "Ghost")
        {
            Destroy(col.gameObject);
        }
    }

}

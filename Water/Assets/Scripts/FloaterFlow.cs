﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FloaterFlow : NetworkBehaviour
{
    List<Object> prefabList = new List<Object>();

    private GameObject Floaters;
    private GameObject Bote;

    public Vector2 SpawnPoint = new Vector2(-3.69f,0.51f);

    [Range(1, 50)]
    public int MaxFloater=10;

    private int count_time=0;

    public Transform spawnpointNucleo;

	// Start is called before the first frame update
	public void Start()
	{

        Floaters = GameObject.Find("Floaters");
        Bote = GameObject.Find("Bote");
        // Levanta todos los prefab de la carpeta Resources/Floaters
        foreach (Object i in  Resources.LoadAll("Floaters", typeof(GameObject))){
            prefabList.Add(i);
        }

        //Spawn del bote inicial
        GameObject nucleo = Instantiate(Resources.Load("Other/nucleo") as GameObject, spawnpointNucleo.position, Quaternion.identity, Bote.transform);
        nucleo.name = "Nucleo";
        NetworkServer.Spawn(nucleo);

        int j = -4;
        while (j < 4)
        {
            GameObject boteFloater = Instantiate(Resources.Load("Floaters/Crate1") as GameObject, spawnpointNucleo.position + new Vector3(j, -1, 0), Quaternion.identity);
            boteFloater.GetComponent<Floater>().Nucleo = nucleo;
            boteFloater.GetComponent<Floater>().daddy = Bote.transform;
            NetworkServer.Spawn(boteFloater);
            j++;
        }
    }



    // Update is called once per frame
    private void Update()
    {
        int prefabIndex;
        if(count_time>=2000){
            count_time=0;
            if(gameObject.transform.childCount < MaxFloater){
                prefabIndex = Random.Range(0,prefabList.Count);
                GameObject newFloater = Instantiate(prefabList[prefabIndex]) as GameObject;
                newFloater.transform.position = SpawnPoint;
                newFloater.transform.parent = Floaters.transform;
                newFloater.GetComponent<Floater>().daddy = Floaters.transform;
                newFloater.layer = 9;
                NetworkServer.Spawn(newFloater);
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

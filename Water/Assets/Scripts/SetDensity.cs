using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDensity : MonoBehaviour
{
    [Range(0.01f, 2f)]
    public float Densidad=1;
	// Start is called before the first frame update
	private void Start()
	{
		if(GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic)
		{
			gameObject.GetComponent<Collider2D>().density = Densidad;
		}
	}

	public void Set()
    {
        gameObject.GetComponent<Collider2D>().density = Densidad;
    }

}

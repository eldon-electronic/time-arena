using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicController : MonoBehaviour
{

    private float speed = 1;
    //public GameObject crystal;
    Vector3 temp;
    public Material sandMat;
    
    // Start is called before the first frame update
    void Start()
    {
        
        //sandMat.SetVector("Sand_Direction", new Vector2(0f,0.2f));
        //sandMat.SetVector("Noise_Direction",new Vector2(0f,0.2f));

        
    }

    // Update is called once per frame
    void Update()
    { 
        if(Input.GetKey(KeyCode.E)){

           /* temp = crystal.transform.localScale;
            temp.x += 1f * speed * Time.deltaTime;
            temp.y += 1f * speed * Time.deltaTime;
            temp.z += 1f * speed * Time.deltaTime;
            crystal.transform.localScale = temp;*/

            sandMat.SetVector("Sand_Direction", new Vector2(0f,3f));
            sandMat.SetVector("Noise_Direction",new Vector2(0f,3f));
            sandMat.SetVector("Voronoi_Direction",new Vector2(0f,3f));


        }
       
        else if(Input.GetKey(KeyCode.Q)){

            /*temp = crystal.transform.localScale;
            temp.x -= 1f * speed * Time.deltaTime;
            temp.y -= 1f * speed * Time.deltaTime;
            temp.z -= 1f * speed * Time.deltaTime;
            crystal.transform.localScale = temp;*/

            sandMat.SetVector("Sand_Direction", new Vector2(0f, -3f));
            sandMat.SetVector("Noise_Direction",new Vector2(0f, -3f));
            sandMat.SetVector("Voronoi_Direction",new Vector2(0f,-3f));
        }
        else{

            sandMat.SetVector("Sand_Direction", new Vector2(0f,0.2f));
            sandMat.SetVector("Noise_Direction",new Vector2(0f,0.2f));
            sandMat.SetVector("Voronoi_Direction",new Vector2(0f,0.2f));

        }
     

       

        
    }

}

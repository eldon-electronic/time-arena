using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalDevice : MonoBehaviour
{
    public GameObject CompassPointer;
    public GameObject ButtonArrow;
    public GameObject OffLine;
    public GameObject DeviceButton;
    public Material ButtonForwardMat;
    public Material ButtonBackwardMat;
    public Material ButtonOffMat;
    public Material ButtonArrowForwardMat;
    public Material ButtonArrowBackwardMat;
    public Material OffLineMat;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
           
    }
    public void ChangePointerPosition(float position)
    {
         Vector3 eulerRotation = CompassPointer.transform.rotation.eulerAngles;
         CompassPointer.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, position);
        
    }

    public void SetButtonMaterial(Material buttonMat){

        DeviceButton.GetComponent<Renderer>().material = buttonMat;

        if(buttonMat == ButtonForwardMat){

            ButtonArrow.GetComponent<Renderer>().material = ButtonArrowForwardMat;
            Vector3 eulerRotation = ButtonArrow.transform.rotation.eulerAngles;
            ButtonArrow.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 180f);
            OffLine.SetActive(false);

        }
        else if(buttonMat == ButtonBackwardMat){

            ButtonArrow.GetComponent<Renderer>().material = ButtonArrowBackwardMat;
            Vector3 eulerRotation = ButtonArrow.transform.rotation.eulerAngles;
            ButtonArrow.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0f);
            OffLine.SetActive(false);


        }
        else if(buttonMat == ButtonOffMat){

            ButtonArrow.GetComponent<Renderer>().material = OffLineMat;
            ButtonArrow.SetActive(false);
            OffLine.SetActive(true);
            

        }
        
    }


   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparency : MonoBehaviour
{
    // Start is called before the first frame update
 
    private Material[] _mat;
    private bool _isSet = false;
    private float _counter = 0.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("FaceOut") || other.gameObject.tag.Equals("CenterBlock")) { 
            _mat = other.gameObject.GetComponent<Renderer>().materials;
            Debug.Log(_mat[1].name) ;

            if (_mat[1].GetInt("_IsCollided")==0) {
                _mat[1].SetFloat("_Alpha", 1.0f);
                _mat[1].SetInt("_IsCollided", 1);
            }

            //Debug.Log(_mat[1].GetFloat("_Alpha"));
            //_isSet = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("FaceOut") || other.gameObject.tag.Equals("CenterBlock"))
        {
            _mat = other.gameObject.GetComponent<Renderer>().materials;
            Debug.Log(_mat[1].GetFloat("_IsCollided"));

            if (_mat[1].GetInt("_IsCollided") == 1)
            {
                _mat[1].SetFloat("_Alpha", 0.0f);
                _mat[1].SetInt("_IsCollided", 0);
            }
            Debug.Log(_mat[1].GetFloat("_Alpha"));

        }
    }

    void ReduceAlpha(Material mat) {

        
      

    }
}

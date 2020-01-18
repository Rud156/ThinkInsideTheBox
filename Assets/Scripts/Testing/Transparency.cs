using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparency : MonoBehaviour
{
    // Start is called before the first frame update
 
    private Material _mat;
    private bool _isSet = false;
    private float _counter = 0.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (_isSet && _counter <=1.0f) {

            ReduceAlpha(_mat);
            _counter += Time.deltaTime;

        }*/
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("FaceOut")) {

            _mat = other.gameObject.GetComponent<Renderer>().material;
            Debug.Log(_mat.GetFloat("_Alpha")) ;
            _mat.SetFloat("_Alpha",1.0f);
            _mat.SetInt("_IsCollided",1);
            Debug.Log(_mat.GetFloat("_Alpha"));
            //_isSet = true;
        }
    }

    void ReduceAlpha(Material mat) {

        
        mat.SetFloat("_Alpha",_counter);

    }
}

using UnityEngine;
using Utils;

namespace Testing
{
    public class Transparency : MonoBehaviour
    {
        // Start is called before the first frame update

        private Material[] _mat;
        private bool _isSet = false;
        private float _counter = 0.0f;

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(TagManager.FaceOut) || other.CompareTag(TagManager.WaterHole))
            {
                _mat = other.gameObject.GetComponent<Renderer>().materials;
                if (_mat.Length < 2)
                {
                    return;
                }

                if (_mat[1].GetInt("_IsCollided") == 0)
                {
                    _mat[1].SetFloat("_Alpha", 1.0f);
                    _mat[1].SetInt("_IsCollided", 1);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag.Equals("FaceOut") || other.gameObject.tag.Equals("CenterBlock"))
            {
                _mat = other.gameObject.GetComponent<Renderer>().materials;
                if (_mat.Length < 2)
                {
                    return;
                }

                if (_mat[1].GetInt("_IsCollided") == 1)
                {
                    _mat[1].SetFloat("_Alpha", 0.0f);
                    _mat[1].SetInt("_IsCollided", 0);
                }
            }
        }
    }
}
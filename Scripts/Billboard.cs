using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomMapper
{
    public class Billboard : MonoBehaviour
    {

        private Camera _camera;

        private Camera MainCam
        {
            get
            {
                if(_camera == null)
                {
                    _camera = Camera.main;
                }
                return _camera;
            }
        }

        // Update is called once per frame
        void Update()
        {
            transform.forward = (transform.position - MainCam.transform.position).normalized;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

namespace RoomMapper
{
    public class EyeTarget : MonoBehaviour
    {
        public float RequiredTime;
        public Image ProgressBarImage;
        public SphereCollider Collider;

        private float _sinceStart = 0;
        private bool _done = false;

        private Camera _camera;

        public bool IsDone
        {
            get { return _done; }
        }

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

        private float CompletionPercentage {
            get { return Mathf.Clamp01(_sinceStart / RequiredTime); }
        }

        private bool IsBeingViewed()
        {
            if (MLEyes.FixationConfidence < 0.7f)
            {
                return false;
            }
            var fixPoint = MLEyes.FixationPoint;
            var headPoint = MainCam.transform.position;
            var dir = (fixPoint - headPoint).normalized;
            var ray = new Ray(headPoint, dir);
            RaycastHit rh;
            return Collider.Raycast(ray, out rh, 10);
        }

        // Update is called once per frame
        void Update()
        {
            if(_done || !IsBeingViewed())
            {
                return;
            }

            _sinceStart += Time.deltaTime;
            ProgressBarImage.fillAmount = CompletionPercentage;
            if(_sinceStart > RequiredTime)
            {
                _done = true;
                gameObject.SetActive(false);
            }
        }
    }
}
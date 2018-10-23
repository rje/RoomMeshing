using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.SceneManagement;

namespace RoomMapper
{
    public class EyeTargetManager : MonoBehaviour
    {
        public Locator MainLocator;
        public string MainScene;

        public TargetGenerator SphericalGenerator;

        // Use this for initialization
        void Start()
        {
            if (!MLEyes.IsStarted)
            {
                MLEyes.Start();
            }
            if(!MLInput.IsStarted)
            {
                MLInput.Start();
            }
            MLInput.OnControllerButtonDown += OnButtonDown;
            SphericalGenerator.InitializeGenerator(MainLocator);
        }

        private void OnButtonDown(byte arg1, MLInputControllerButton button)
        {
            if(button == MLInputControllerButton.Bumper)
            {
                SphericalGenerator.Abort();
            }
        }

        private void OnDestroy()
        {
            if (MLEyes.IsStarted)
            {
                MLEyes.Stop();
            }
            if(MLInput.IsStarted)
            {
                MLInput.Stop();
            }
        }

        void ProceedToMainScene()
        {
            SceneManager.LoadSceneAsync(MainScene);
        }

        void Update()
        {
            if(SphericalGenerator.IsDone())
            {
                ProceedToMainScene();
                return;
            }
        }
    }
}
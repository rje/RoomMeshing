using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.SceneManagement;

namespace RoomMeshing
{
    public class EyeTargetManager : MonoBehaviour
    {
        public enum GeneratorType
        {
            Spherical,
            ConfidenceBased
        }

        public Locator MainLocator;
        public string MainScene;
        public GeneratorType TypeToUse;

        public TargetGenerator SphericalGenerator;
        public TargetGenerator ConfidenceBasedGenerator;

        public TargetGenerator ActiveGenerator
        {
            get
            {
                switch(TypeToUse)
                {
                    case GeneratorType.Spherical:
                        return SphericalGenerator;
                    default:
                    case GeneratorType.ConfidenceBased:
                        return ConfidenceBasedGenerator;
                }
            }
        }

        // Use this for initialization
        void Start()
        {
            var mapper = FindObjectOfType<MLSpatialMapper>();
            mapper.RefreshAllMeshes();
            if (!MLEyes.IsStarted)
            {
                MLEyes.Start();
            }
            if(!MLInput.IsStarted)
            {
                MLInput.Start();
            }
            MLInput.OnControllerButtonDown += OnButtonDown;
            var gen = ActiveGenerator;
            DisableOtherGenerators(gen);
            gen.InitializeGenerator(MainLocator);
        }

        void DisableOtherGenerators(TargetGenerator active)
        {
            SphericalGenerator.enabled = SphericalGenerator == active;
            ConfidenceBasedGenerator.enabled = ConfidenceBasedGenerator == active;
        }

        private void OnButtonDown(byte arg1, MLInputControllerButton button)
        {
            if(button == MLInputControllerButton.Bumper)
            {
                ActiveGenerator.Abort();
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
            if(ActiveGenerator.IsDone())
            {
                ProceedToMainScene();
                return;
            }
        }
    }
}
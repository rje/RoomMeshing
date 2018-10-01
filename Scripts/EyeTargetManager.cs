using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.SceneManagement;

namespace RoomMapper
{
    public class EyeTargetManager : MonoBehaviour
    {
        public GameObject TargetPrefab;
        public Locator MainLocator;
        public int Radius = 2;
        public int TargetsPerSlice = 4;
        public int NumSlices = 3;

        public string MainScene;

        private List<EyeTarget> _targets;
        private bool _isDone;

        // Use this for initialization
        void Start()
        {
            _isDone = false;
            if (!MLEyes.IsStarted)
            {
                MLEyes.Start();
            }
            if(!MLInput.IsStarted)
            {
                MLInput.Start();
            }
            MLInput.OnControllerButtonDown += OnButtonDown;
            SpawnTargets();
        }

        private void OnButtonDown(byte arg1, MLInputControllerButton button)
        {
            if(button == MLInputControllerButton.Bumper)
            {
                _isDone = true;
                ProceedToMainScene();
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

        void SpawnTargets()
        {
            var anglePerSlice = 180.0f / (NumSlices + 1);
            var anglePerTarget = 360.0f / TargetsPerSlice;
            var origin = Camera.main.transform.position;
            _targets = new List<EyeTarget>();
            for (var slice = 0; slice < NumSlices; slice++)
            {
                var vec = Vector3.up;
                var sliceRot = Quaternion.AngleAxis(anglePerSlice * (slice + 1), Vector3.forward);
                vec = sliceRot * vec;
                var targetRot = Quaternion.AngleAxis(anglePerTarget, Vector3.up);
                for (var targetCount = 0; targetCount < TargetsPerSlice; targetCount++)
                {
                    vec = targetRot * vec;
                    var go = Instantiate(TargetPrefab, vec * Radius + origin, Quaternion.identity, transform);
                    go.name = string.Format("Slice {0}, Target {1}", slice, targetCount);
                    _targets.Add(go.GetComponent<EyeTarget>());
                }
            }
        }

        void ProceedToMainScene()
        {
            SceneManager.LoadSceneAsync(MainScene);
        }

        Bounds GetBoundsForRect(RectTransform rt)
        {
            var verts = new Vector3[4];
            rt.GetWorldCorners(verts);
            var min = verts[0];
            var max = verts[0];
            foreach (var vert in verts)
            {
                if (min.x > vert.x) { min.x = vert.x; }
                if (min.y > vert.y) { min.y = vert.y; }
                if (min.z > vert.z) { min.z = vert.z; }
                if (max.x < vert.x) { max.x = vert.x; }
                if (max.y < vert.y) { max.y = vert.y; }
                if (max.z < vert.z) { max.z = vert.z; }
            }
            var toReturn = new Bounds();
            toReturn.SetMinMax(min, max);
            // Reducing bounds by a bit so hopefully you see a bit of the UI before the locator disappears
            toReturn.Expand(-1);
            return toReturn;
        }

        // Update is called once per frame
        void Update()
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            var headPos = Camera.main.transform.position;
            var isVisible = false;
            GameObject toTarget = null;
            float targetDistance = float.MaxValue;
            var targetsRemaining = 0;
            foreach (var target in _targets)
            {
                if(target.IsDone)
                {
                    continue;
                }
                targetsRemaining++;
                var go = target.gameObject;
                var rt = go.GetComponent<RectTransform>();
                var bounds = GetBoundsForRect(rt);
                var visible = GeometryUtility.TestPlanesAABB(planes, bounds);
                if(visible)
                {
                    isVisible = true;
                    break;
                }
                var dist = (go.transform.position - MainLocator.transform.position).magnitude;
                if(dist < targetDistance)
                {
                    toTarget = go;
                    targetDistance = dist;
                }
            }

            if(targetsRemaining == 0)
            {
                _isDone = true;
                ProceedToMainScene();
                return;
            }

            MainLocator.SetVisible(!isVisible);
            MainLocator.UpdateLocator(toTarget, targetsRemaining);
        }
    }
}
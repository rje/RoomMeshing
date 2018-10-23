using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomMapper
{
    public class SphericalTargetGenerator : TargetGenerator
    {
        public int Radius = 2;
        public int TargetsPerSlice = 4;
        public int NumSlices = 3;
        public GameObject TargetPrefab;

        private List<EyeTarget> _targets;

        public override void InitializeGenerator(Locator mainLoc)
        {
            base.InitializeGenerator(mainLoc);
            SpawnTargets();
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

        private void Update()
        {
            if(_isDone)
            {
                return;
            }

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
                return;
            }

            MainLocator.SetVisible(!isVisible);
            MainLocator.UpdateLocator(toTarget, targetsRemaining);
        }
    }
}

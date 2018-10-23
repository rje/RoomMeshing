using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.Experimental.XR;

namespace RoomMeshing
{
    public class ConfidenceBasedTargetGenerator : TargetGenerator
    {
        public float ConfidenceThreshold = 0.8f;
        public float MaxViews = 3;
        public GameObject TargetPrefab;

        private EyeTarget _currentTarget;
        private MLSpatialMapper _mapper;
        private TrackableId _lastTarget = TrackableId.InvalidId;
        float _lastConfidence = 0;
        Dictionary<TrackableId, int> _seenCount;

        public override void InitializeGenerator(Locator mainLoc)
        {
            base.InitializeGenerator(mainLoc);
            _seenCount = new Dictionary<TrackableId, int>();
            _mapper = FindObjectOfType<MLSpatialMapper>();
            _mapper.requestVertexConfidence = true;
            _mapper.RefreshAllMeshes();
            FindNextTarget();
        }

        void FindNextTarget()
        {
            var allTrackables = _mapper.meshIdToGameObjectMap.Keys;
            var confidenceValues = new List<float>(1000);
            var worst = TrackableId.InvalidId;
            var worstConfidence = float.MaxValue;
            foreach (var id in allTrackables)
            {
                if(id == _lastTarget)
                {
                    continue;
                }
                if(_seenCount.ContainsKey(id) && _seenCount[id] >= MaxViews)
                {
                    continue;
                }
                var valuesFound = _mapper.TryGetConfidence(id, confidenceValues);
                if(!valuesFound)
                {
                    continue;
                }
                var confidence = confidenceValues.Average();
                if (confidence < worstConfidence)
                {
                    worst = id;
                    worstConfidence = confidence;
                }
            }

            Debug.LogFormat("Found worst confidence {0} for mesh {1}", worstConfidence, worst);

            if(_lastTarget != TrackableId.InvalidId)
            {
                var lastGO = _mapper.meshIdToGameObjectMap[_lastTarget];
                var lastMR = lastGO.GetComponent<MeshRenderer>();
                lastMR.material.color = Color.blue;
            }

            if(worst == TrackableId.InvalidId)
            {
                return;
            }

            if (worstConfidence > ConfidenceThreshold)
            {
                _isDone = true;
                return;
            }

            if(_seenCount.ContainsKey(worst))
            {
                _seenCount[worst] += 1;
            }
            else
            {
                _seenCount[worst] = 1;
            }

            var meshGO = _mapper.meshIdToGameObjectMap[worst];
            var mr = meshGO.GetComponent<MeshRenderer>();
            mr.material.color = Color.red;
            _lastTarget = worst;
            _lastConfidence = worstConfidence;
            var loc = GetCentroid(meshGO);
            var go = Instantiate(TargetPrefab, loc, Quaternion.identity, transform);
            go.name = string.Format("conf: {0}, id: {1}", worstConfidence, worst);
            _currentTarget = go.GetComponent<EyeTarget>();
        }

        Vector3 GetCentroid(GameObject meshGO)
        {
            var mf = meshGO.GetComponent<MeshFilter>();
            var verts = mf.mesh.vertices;
            var avg = Vector3.zero;
            foreach(var v in verts)
            {
                avg += v;
            }
            avg /= verts.Length;
            return avg;
        }

        private void Update()
        {
            if (_isDone)
            {
                return;
            }

            if (_currentTarget == null || _currentTarget.IsDone)
            {
                if(_currentTarget != null)
                {
                    Destroy(_currentTarget.gameObject);
                }
                FindNextTarget();
                return;
            }

            var planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            var rt = _currentTarget.GetComponent<RectTransform>();
            var bounds = GetBoundsForRect(rt);
            var isVisible = GeometryUtility.TestPlanesAABB(planes, bounds);

            MainLocator.SetVisible(!isVisible);
            MainLocator.UpdateStringUnknownCount = string.Format("Current confidence {0}", _lastConfidence);
            MainLocator.UpdateLocator(_currentTarget.gameObject, -1);
        }
    }
}
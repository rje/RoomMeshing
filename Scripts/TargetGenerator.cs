using UnityEngine;

namespace RoomMapper
{
    public class TargetGenerator : MonoBehaviour
    {
        protected bool _isDone;
        protected Locator MainLocator;

        public virtual bool IsDone()
        {
            return _isDone;
        }

        public virtual void InitializeGenerator(Locator mainLoc)
        {
            _isDone = false;
            MainLocator = mainLoc;
        }

        public virtual void Abort() {
            _isDone = true;
        }

        protected Bounds GetBoundsForRect(RectTransform rt)
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
            // Reducing bounds by a bit so hopefully you see a bit of the UI before the Locator disappears
            toReturn.Expand(-1);
            return toReturn;
        }
    }
}

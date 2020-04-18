using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    public static class ExPhysics
    {
        public static bool PrimitiveCast(Collider collider, Vector3 direction, float castDistance, out float hitDistance,
            IEnumerable<Collider> ignoreColliders = null, IEnumerable<IExTag> ignoreTags = null, int ignoreLayers = Physics.DefaultRaycastLayers)
        {
            var box = collider as BoxCollider;

            if (box != null)
            {
                return BoxCast(box, direction, castDistance, out hitDistance, ignoreColliders, ignoreTags, ignoreLayers);
            }

            var sphere = collider as SphereCollider;

            if (sphere != null)
            {
                return SpherCast(sphere, direction, castDistance, out hitDistance, ignoreColliders, ignoreTags, ignoreLayers);
            }

            var capsule = collider as CapsuleCollider;

            if (capsule != null)
            {
                return CapsuleCast(capsule, direction, castDistance, out hitDistance, ignoreColliders, ignoreTags, ignoreLayers);
            }

            hitDistance = 0;
            return false;
        }

        public static bool BoxCast(BoxCollider box, Vector3 direction, float castDistance, out float hitDistance,
            IEnumerable<Collider> ignoreColliders = null, IEnumerable<IExTag> ignoreTags = null, int ignoreLayers = Physics.DefaultRaycastLayers)
        {
            var colliders = Physics.OverlapBox(box.transform.position + box.transform.TransformVector(box.center), Vector3.Scale(box.size, box.transform.lossyScale) * 0.5f, box.transform.rotation, ignoreLayers, QueryTriggerInteraction.Ignore);

            var hitColliders = ColliderHitCheck(colliders, ignoreColliders, ignoreTags);

            if (hitColliders.Count() > 0)
            {
                hitDistance = 0;
                return true;
            }

            var casts = Physics.BoxCastAll(box.transform.position + box.transform.TransformVector(box.center), Vector3.Scale(box.size, box.transform.lossyScale) * 0.5f, direction, box.transform.rotation, castDistance, ignoreLayers, QueryTriggerInteraction.Ignore);

            var hitCasts = CastHitCheck(casts, ignoreColliders, ignoreTags);

            if (hitCasts.Count() > 0)
            {
                hitDistance = hitCasts.Min(x => x.distance);
                return true;
            }
            else
            {
                hitDistance = castDistance;
                return false;
            }
        }

        public static bool SpherCast(SphereCollider sphere, Vector3 direction, float castDistance, out float hitDistance,
            IEnumerable<Collider> ignoreColliders = null, IEnumerable<IExTag> ignoreTags = null, int ignoreLayers = Physics.DefaultRaycastLayers)
        {
            var colliders = Physics.OverlapSphere(sphere.transform.position + sphere.transform.TransformVector(sphere.center), sphere.radius, ignoreLayers, QueryTriggerInteraction.Ignore);

            var hitColliders = ColliderHitCheck(colliders, ignoreColliders, ignoreTags);

            if (hitColliders.Count() > 0)
            {
                hitDistance = 0;
                return true;
            }

            var casts = Physics.SphereCastAll(sphere.transform.position + sphere.transform.TransformVector(sphere.center), sphere.radius, direction, castDistance, ignoreLayers, QueryTriggerInteraction.Ignore);

            var hitCasts = CastHitCheck(casts, ignoreColliders, ignoreTags);

            if (hitCasts.Count() > 0)
            {
                hitDistance = hitCasts.Min(x => x.distance);
                return true;
            }
            else
            {
                hitDistance = castDistance;
                return false;
            }
        }

        public static bool CapsuleCast(CapsuleCollider capsule, Vector3 direction, float castDistance, out float hitDistance,
            IEnumerable<Collider> ignoreColliders = null, IEnumerable<IExTag> ignoreTags = null, int ignoreLayers = Physics.DefaultRaycastLayers)
        {
            var point1 = capsule.transform.position + GetCapsuleDirection(capsule);
            var point2 = capsule.transform.position - GetCapsuleDirection(capsule);

            var colliders = Physics.OverlapCapsule(point1, point2, capsule.radius, ignoreLayers, QueryTriggerInteraction.Ignore);

            var hitColliders = ColliderHitCheck(colliders, ignoreColliders, ignoreTags);

            if (hitColliders.Count() > 0)
            {
                hitDistance = 0;
                return true;
            }

            var casts = Physics.CapsuleCastAll(point1, point2, capsule.radius, direction, castDistance, ignoreLayers, QueryTriggerInteraction.Ignore);

            var hitCasts = CastHitCheck(casts, ignoreColliders, ignoreTags);

            if (hitCasts.Count() > 0)
            {
                hitDistance = hitCasts.Min(x => x.distance);
                return true;
            }
            else
            {
                hitDistance = castDistance;
                return false;
            }
        }

        private static IEnumerable<Collider> ColliderHitCheck(IEnumerable<Collider> colliders, IEnumerable<Collider> ignoreColliders = null, IEnumerable<IExTag> ignoreTags = null)
        {
            return colliders.Where(x => !x.gameObject.HasExTag(ignoreTags)).Except(ignoreColliders);
        }

        private static IEnumerable<RaycastHit> CastHitCheck(IEnumerable<RaycastHit> casts, IEnumerable<Collider> ignoreColliders = null, IEnumerable<IExTag> ignoreTags = null)
        {
            if (ignoreColliders != null)
            {
                return casts.Where(x => !x.collider.gameObject.HasExTag(ignoreTags)).Where(x => !ignoreColliders.Contains(x.collider));
            }
            else
            {
                return casts.Where(x => !x.collider.gameObject.HasExTag(ignoreTags));
            }
        }

        private static float CalcSizeFromScale(Vector3 lossyScale)
        {
            return (lossyScale.x + lossyScale.y + lossyScale.z) / 3;
        }

        public static Vector3 GetCapsuleDirection(CapsuleCollider capsule)
        {
            return GetCapsuleDirection(capsule.direction, capsule.transform) * capsule.height;
        }

        public static Vector3 GetCapsuleDirection(int index, Transform transform)
        {
            switch (index)
            {
                case 1:
                    return transform.right;

                case 2:
                    return transform.up;

                case 3:
                    return transform.forward;
            }

            return Vector3.zero;
        }

        public static bool PrimitiveCast(IEnumerable<Collider> colliders, Vector3 direction, float distanceMax, out float distance, IEnumerable<Collider> ignoreColliders = null, IEnumerable<IExTag> ignoreTags = null, int ignoreLayers = Physics.DefaultRaycastLayers)
        {
            float min = float.PositiveInfinity;

            foreach (var collider in colliders)
            {
                float tempDistance;

                if (PrimitiveCast(collider, direction, distanceMax, out tempDistance, ignoreColliders, ignoreTags, ignoreLayers) && tempDistance < min)
                {
                    min = tempDistance;
                }
            }

            distance = min;
            return min != float.PositiveInfinity;
        }

        public static bool CheckInternalForClosedMesh(MeshCollider mesh, Vector3 position)
        {
            if (mesh == null) { return false; }

            var point = position + mesh.bounds.size;

            var vector = point - position;

            var direction = vector.normalized;
            var distance = vector.magnitude;

            var ray = new Ray(position, direction);
            var rayReverse = new Ray(point, -direction);

            EHLDebug.DrawRay(ray);

            int count = 0;

            count += CountRaycastHit(mesh, ray, point);
            count += CountRaycastHit(mesh, rayReverse, position);

            return count % 2 == 1;
        }

        private static int CountRaycastHit(MeshCollider mesh, Ray ray, Vector3 target)
        {
            const float mergin = 0.001f;

            int count = 0;
            RaycastHit hit;

            while (mesh.Raycast(ray, out hit, (ray.origin - target).magnitude))
            {
                count++;

                if (count > 50)
                {
                    Debug.LogWarning("Break CountRaycastHit at 50 Count");
                    break;
                }

                ray = new Ray(hit.point + ray.direction * mergin, ray.direction);
            }

            return count;
        }
    }
}
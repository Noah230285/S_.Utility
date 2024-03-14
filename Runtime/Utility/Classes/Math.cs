using static UnityEngine.Rendering.DebugUI;

namespace _S.Utility
{
    public static class CustomMath
    {
        public static Vector2 AngleToVector2(float angle)
        {
            return new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
        }

        public static Vector3 AngleToVector3(float angle)
        {
            return AngleToVector3(angle, Vector3.up);
        }

        public static Vector3 AngleToVector3(float angle, Vector3 upAxis)
        {
            Vector2 vector2 = AngleToVector2(angle);
            return Vector3.ProjectOnPlane(new Vector3(vector2.x, 0, vector2.y), upAxis);
        }

        public static float PositiveSignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            float angle = Vector3.SignedAngle(from, to, axis);
            return angle < 0 ? (-angle) : 360 - angle;
        }

        public static float ClampAngle(float value, float min, float max, bool clampEdges = false)
        {
            value = Mathf.Repeat(value, 360f);
            min = Mathf.Repeat(min, 360f);
            max = Mathf.Repeat(max, 360f);
            float midPoint = min > max ? (max - min) / 2 : (360 - min + max) / 2;
            float offset = clampEdges ? Mathf.DeltaAngle(midPoint + min, 180) : 0;

            value = Mathf.Repeat(value + offset, 360f);
            min = Mathf.Repeat(min + offset, 360f);
            max = Mathf.Repeat(max + offset, 360f);

            if (min > max)
            {
                return InverseClamp(value, max, min) - offset;
            }
            return Mathf.Clamp(value, min, max) - offset;
        }

        public static float MoveTowardsClampedAngle(float current, float target, float min, float max, float maxDelta)
        {
            target = ClampAngle(target, min, max, true);
            float num = Mathf.DeltaAngle(current, target);
            float toAngle = ClampAngle(current + num, min, max);
            float adjustedMax = max < current ? max + 360 : max;

            if (toAngle > adjustedMax)
            {
                num = -(360 - num);
            }

            float adjustedMin = min > current ? min - 360 : min;
            if (toAngle < adjustedMin)
            {
                num = 360 + num;
            }
            //Debug.DrawRay(Vector3.up, AngleToVector3(current), Color.red);
            //Debug.DrawRay(Vector3.up, AngleToVector3(target), Color.green);
            //Debug.DrawRay(Vector3.up, AngleToVector3(num), Color.blue);
            //Debug.DrawRay(Vector3.up, AngleToVector3(min), Color.magenta);
            //Debug.DrawRay(Vector3.up, AngleToVector3(max), Color.black);

            if (0f - maxDelta < num && num < maxDelta)
            {
                return target;
            }

            target = current + num;
            return Mathf.MoveTowards(current, target, maxDelta);
        }

        public static float InverseClamp(float value, float min, float max)
        {
            if (value > min && value < max)
            {
                float mid = (max - min) / 2 + min;
                return value < mid ? min : max;
            }
            return value;
        }
        public static Vector3 XY(this Vector3 v)
        {
            return new Vector3(v.x, v.y, 0);
        }
        public static Vector3 YZ(this Vector3 v)
        {
            return new Vector3(0, v.y, v.z);
        }
        public static Vector3 XZ(this Vector3 v)
        {
            return new Vector3(v.x, 0, v.z);
        }

        public static Vector3 VX(this Vector3 v)
        {
            return new Vector3(v.x, 0, 0);
        }
        public static Vector3 VY(this Vector3 v)
        {
            return new Vector3(0, v.y, 0);
        }
        public static Vector3 VZ(this Vector3 v)
        {
            return new Vector3(0, 0, v.z);
        }

        public static Vector3 RepairHitSurfaceNormal(this RaycastHit hit, int layerMask)
        {
            //if (hit.collider is MeshCollider)
            //{
            //    var collider = hit.collider as MeshCollider;
            //    var mesh = collider.sharedMesh;
            //    var tris = mesh.triangles;
            //    var verts = mesh.vertices;

            //    var v0 = verts[tris[hit.triangleIndex * 3]];
            //    var v1 = verts[tris[hit.triangleIndex * 3 + 1]];
            //    var v2 = verts[tris[hit.triangleIndex * 3 + 2]];

            //    var n = Vector3.Cross(v1 - v0, v2 - v1).normalized;

            //    return hit.transform.TransformDirection(n);
            //}
            //else
            //{
                var p = hit.point + hit.normal * 0.01f;
                Physics.Raycast(p, -hit.normal, out hit, 0.011f, layerMask);
                return hit.normal;
            //}
        }

        public static Vector3 RepairHitSurfaceNormal(this ContactPoint contact, out RaycastHit hit, int layerMask, bool useThis = false)
        {
            float i = useThis ? -1 : 1;
            var p = contact.point + contact.normal * i * 0.01f;
            Debug.DrawLine(contact.point, contact.point + contact.normal * i, Color.cyan, 0.1f);
            Physics.Raycast(p, -contact.normal * i, out hit, 0.011f, layerMask);
            return hit.normal;
        }
    }
}

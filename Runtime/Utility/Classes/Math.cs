using UnityEngine;

namespace _S.Utility
{
    public static class CustomMath
    {
        public static Vector2 AngleToVector2(float angle)
        {
            return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
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

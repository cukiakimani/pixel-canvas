using UnityEngine;
namespace Medvedya.GeometryMath
{
    public struct Plane3d
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector3 upAxis;
        public Vector3 rightAxis;
        public Plane3d(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.normal = rotation * Vector3.forward;
            this.upAxis = rotation * Vector3.up;
            this.rightAxis = rotation * Vector3.right;
        }
        public Vector3 rayCast(Ray ray)
        {
            float t = -Vector3.Dot(ray.origin - position, normal) / Vector3.Dot(ray.direction, normal);

            return ray.origin + ray.direction * t;
        }
        public Vector3 globalToLocal(Vector3 globalPosition)
        {
            Vector3 p = globalPosition - position;
            return new Vector3(Vector3.Dot(p, rightAxis), Vector3.Dot(p, upAxis), Vector3.Dot(p, normal));
        }
    }
    public struct Triangle
    {
        static float Q(Vector2 a, Vector2 b, Vector2 t)
        {
            return t.x * (b.y - a.y) + t.y * (a.x - b.x) + a.y * b.x - a.x * b.y;
        }
        
        public static bool isBelongOriantByClock(Vector2 f1, Vector2 f2, Vector2 f3, Vector2 f)
        {
            return (Q(f1, f2, f) >= 0) && (Q(f2, f3, f) >= 0) && (Q(f3, f1, f) >= 0);
        }
        //wasnt tested
        public static bool isBelong(Vector2 f1, Vector2 f2, Vector2 f3, Vector2 f)
        {
            float q1 = Q(f1, f2, f2);
            float q2 = Q(f2, f3, f);
            float q3 = Q(f3, f1, f);
            return ((q1 >= 0f) && (q2 >= 0f) && (q3 >= 0f)) || ((q1 < 0f) && (q2 < 0f) && (q3 < 0f));
        }
        public static float area(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            Vector2 v1 = p1 - p3;
            Vector2 v2 = p2 - p3;
            return (v1.x * v2.y - v1.y * v2.x) / 2;
        }
        public static Vector3 getBarycentric(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 p)
        {
            Vector3 B = new Vector3();
            B.x = ((v2.y - v3.y) * (p.x - v3.x) + (v3.x - v2.x) * (p.y - v3.y)) /
                ((v2.y - v3.y) * (v1.x - v3.x) + (v3.x - v2.x) * (v1.y - v3.y));
            B.y = ((v3.y - v1.y) * (p.x - v3.x) + (v1.x - v3.x) * (p.y - v3.y)) /
                ((v3.y - v1.y) * (v2.x - v3.x) + (v1.x - v3.x) * (v2.y - v3.y));
            B.z = 1 - B.x - B.y;
            return B;
        }

    }
    public class Line3d
    {
        public static Vector3 closestPointInLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 line = lineEnd - lineStart;
            float t = Vector3.Dot(point - lineStart, line);
            return lineStart + line * t;
        }
        public static Vector3 closestPointInSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 line = lineEnd - lineStart;
            float t = Mathf.Clamp01(Vector3.Dot(point - lineStart, line));
            return lineStart + line * t;
        }
    }
    public static class Vector2Utillites
    {
        public static float AngleFromTo(Vector2 from, Vector2 to)
        {
            float ang = Vector2.Angle(from, to);
            Vector3 cross = Vector3.Cross(from, to);

            if (cross.z > 0)
                ang = 360 - ang;
            return ang;
        }

    }
    public class Polygon
    {
        public static bool isBelong(Vector2[] polyPoints, Vector2 point)
        {
            int npol = polyPoints.Length;
            int j = npol - 1;
            bool c = false;
            for (int i = 0; i < npol; i++)
            {
                if ((((polyPoints[i].y <= point.y) && (point.y < polyPoints[j].y)) || ((polyPoints[j].y <= point.y) && (point.y < polyPoints[i].y))) &&
                (point.x > (polyPoints[j].x - polyPoints[i].x) * (point.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x))
                {
                    c = !c;
                }
                j = i;
            }
            return c;
        }
    }
    public struct Line
    {
        public Vector2 p0;
        public Vector2 p1;
        Line(Vector2 p0, Vector2 p1)
        {
            this.p0 = p0;
            this.p1 = p1;

        }
        public static float distanceFromPointToSegment(Vector2 lineA, Vector2 lineB, Vector2 point)
        {
            return Vector2.Distance(ClosestPointOnSegment(lineA, lineB, point), point);
        }
        public static Vector2 ClosestPointOnLine(Vector2 lineA, Vector2 lineB, Vector2 point)
        {

            Vector2 p2 = lineB - lineA;
            float something = p2.x * p2.x + p2.y * p2.y;
            float u = (((point.x - lineA.x) * p2.x + (point.y - lineA.y) * p2.y) / something);
            //closest point
            return lineA + u * p2;
        }
        public static Vector2 ClosestPointOnSegment(Vector2 segA, Vector2 segB, Vector2 point)
        {
            Vector2 p2 = segB - segA;
            float something = p2.x * p2.x + p2.y * p2.y;
            float u = Mathf.Clamp01(((point.x - segA.x) * p2.x + (point.y - segA.y) * p2.y) / something);
            //closest point
            return segA + u * p2;
        }
        public static Vector2 ClosestPointOnSegment(Line line, Vector2 point)
        {
            return ClosestPointOnSegment(line.p0, line.p1, point);
        }
        public Vector2 closestPointOnSegment(Vector2 point)
        {
            return ClosestPointOnSegment(this, point);
        }


        public static bool isIntersection(Line line1, Line line2)
        {
            return isIntersection(line1.p0, line1.p1, line2.p0, line2.p1);
        }
        public static bool isIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            float v1, v2, v3, v4;
            v1 = (b2.x - b1.x) * (a1.y - b1.y) - (b2.y - b1.y) * (a1.x - b1.x);
            v2 = (b2.x - b1.x) * (a2.y - b1.y) - (b2.y - b1.y) * (a2.x - b1.x);
            v3 = (a2.x - a1.x) * (b1.y - a1.y) - (a2.y - a1.y) * (b1.x - a1.x);
            v4 = (a2.x - a1.x) * (b2.y - a1.y) - (a2.y - a1.y) * (b2.x - a1.x);
            return (v1 * v2 < 0) && (v3 * v4 < 0);
        }
        public static bool intersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 point)
        {
            point = Vector2.zero;

            float x1 = a1.x;
            float y1 = a1.y;
            float x2 = a2.x;
            float y2 = a2.y;
            float x3 = b1.x;
            float y3 = b1.y;
            float x4 = b2.x;
            float y4 = b2.y;
            float t = 0;
            float xa = 0;
            float ya = 0;
            if (Mathf.Abs((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)) < 0.0001)
            {
                return false;
            }
            else
            {
                t = ((y3 - y1) * (x3 - x4) - (x3 - x1) * (y3 - y4)) / ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
                if ((t >= 0) && (t <= 1))
                {
                    xa = x1 + t * (x2 - x1);
                    ya = y1 + t * (y2 - y1);
                    if (((x3 - xa) * (x4 - xa) <= 0) && ((y3 - ya) * (y4 - ya) <= 0))
                    {

                        point = new Vector2(xa, ya);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;

                }
            }
        }
    }
}
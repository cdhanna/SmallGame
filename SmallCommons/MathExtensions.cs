using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SmallCommons
{
    public static class MathExtensions
    {

        public static void IterateOverSides(this Vector2[] a, Action<Vector2> action)
        {
            for (var i = 0; i < a.Length; i++)
            {
                var ni = (1 + i)%a.Length;
                action(a[ni] - a[i]);
            }
        }

        public static void IterateOverPairs(this Vector2[] a, Action<Vector2, Vector2> action)
        {
            for (var i = 0; i < a.Length; i++)
            {
                var ni = (1 + i) % a.Length;
                action(a[i], a[ni]);
            }
        }

        public static void IterateOverAll(this Vector2[] a, Action<Vector2> action)
        {
            for (var i = 0; i < a.Length; i++)
                action(a[i]);
        }

        public static T[] Map<T>(this Vector2[] a, Func<Vector2, T> transform)
        {
            var output = new T[a.Length];
            for (var i = 0; i < a.Length; i ++)
                output[i] = transform(a[i]);
            return output;
        } 

        public static Vector2 Perpendicular(this Vector2 a, Vector2 b)
        {
            return (b - a).Perpendicular();
        }

        public static Vector2 Perpendicular(this Vector2 a)
        {
            return new Vector2(-a.Y, a.X);
        }

        public static Vector2 Normal(this Vector2 a)
        {
            var b = a;
            b.Normalize();
            return b;
        }

        public static float Dot(this Vector2 a, Vector2 b)
        {
            return a.X*b.X + a.Y*b.Y;
        }

        public static Vector2 Scale(this Vector2 a, float x, float y)
        {
            return new Vector2(a.X*x, a.Y*y);
        }
    }
}

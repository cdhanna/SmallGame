using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SmallCommons
{

    public class OverlapInfo
    {
        public bool IsOverlapping { get; private set; }
        public Vector2 Normal { get; private set; }
        public float Overlap { get; private set; }
        public Vector2 Penetration { get; private set; }

        public OverlapInfo(Vector2? normal, float overlap)
        {
            Overlap = overlap;
            IsOverlapping = false;
            if (overlap > 0)
            {

                Normal = normal.Value.Normal();
                IsOverlapping = true;
                Penetration = Normal * overlap;
            }
        }
    }

    public class SATHelper
    {

        public OverlapInfo Check(Vector2[] shapeA, Vector2[] shapeB)
        {
            // get all axis vectors.
            var allAxis = new List<Vector2>();

            shapeA.IterateOverSides( allAxis.Add );
            shapeB.IterateOverSides( allAxis.Add );

            // project shapes onto axis
            var axisComp = allAxis.Select(a =>
            {
                var axisNormal = a.Perpendicular().Normal();

                float aMin, bMin, aMax, bMax; // declare all min and maxes to their opposite values
                aMin = float.MaxValue;
                bMin = float.MaxValue;
                aMax = float.MinValue;
                bMax = float.MinValue;

                float[] aPoints = shapeA.Map(p => axisNormal.Dot(p)); // project all points
                float[] bPoints = shapeB.Map(p => axisNormal.Dot(p));

                aMin = Math.Min(aPoints.Min(), aMin); // grab mins and maxes
                bMin = Math.Min(bPoints.Min(), bMin);
                aMax = Math.Max(aPoints.Max(), aMax);
                bMax = Math.Max(bPoints.Max(), bMax);


                var overlap = -1f; // -1 means no collision is overlap is happening. Start with that.
                var aInsideB = bMin < aMin && aMax < bMax;
                var binsideA = aMin < bMin && bMax < aMax;
                var aMaxInsideB = bMin < aMax && aMax < bMax;
                var aMinInsideB = bMin < aMin && aMin < bMax;

                if (aInsideB)
                {
                    overlap = aMax - aMin;
                } else if (binsideA)
                {
                    overlap = bMax - bMin;
                } else if (aMaxInsideB)
                {
                    overlap = aMax - bMin;
                } else if (aMinInsideB)
                {
                    overlap = bMax - aMin;
                }

                return new OverlapInfo(axisNormal, overlap);

                // handle overlap cases.
                //var aInsideB = aMin > bMin && aMax < bMax && aMin < bMax;
                //var bInsideA = bMin > aMin && bMax < aMax && bMin < aMax;
                //var bMaxInsideA = aMin < bMax && bMax < aMax && bMin < aMin;
                //var bMinInsideA = aMin < bMin && bMin < aMax && bMax > aMax;

                //if (aInsideB || bInsideA || bMaxInsideA || bMinInsideA)
                //    // any are true, then this axis overlaps, and nothing can be said. 
                //{
                //    return new OverlapInfo(axisNormal, 1); // TODO make overlap actually make sense.
                //}
                //else return new OverlapInfo(null, 0); // ah ha, the axis didn't overlap. Important! at least one of these means no collision. 
            });

            if (axisComp.Any(b => !b.IsOverlapping))
            {
                // there is no collision. 
                return new OverlapInfo(null, 0);
            }
            else
            {
                // there is collision!

                var possibles = axisComp.Where(b => b.IsOverlapping);
                var minOverlap = possibles.First();
                possibles.ToList().ForEach(b =>
                {
                    if (b.Overlap < minOverlap.Overlap)
                    {
                        minOverlap = b;
                    }
                });

                return minOverlap;
            }


        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
        public static class Utils
    {
        public static Vector3 NormalizeThisVector(Vector3 vector)
        {
            vector.x = NormalizeThisVectorParameter(vector.x);
            vector.y = NormalizeThisVectorParameter(vector.y);
            vector.z = NormalizeThisVectorParameter(vector.z);
            return vector;
        }

        public static float NormalizeThisVectorParameter(float parameter)
        {
            if (parameter > 180) parameter -= 360;
            return parameter;
        }

        public static void UpdateLayoutGroups(GameObject go)
        {
            var layouts = go.GetComponentsInChildren<RectTransform>();
            foreach (var layout in layouts) LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
        }

        public static List<T> GetJustOneDimensionOfArray<T>(T[,] array, int rowIndex)
        {
            var list = new List<T>();
            for (var i = 0; i < array.GetLength(1); i++) list.Add(array[rowIndex, i]);

            return list;
        }

        public static float Normalize(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }

        public static float DeNormalize(float value, float nonNormalizedMin, float nonNormalizedMax, float goalMin,
            float goalMax)
        {
            return Normalize(value, nonNormalizedMin, nonNormalizedMax) * (goalMax - goalMin) + goalMin;
        }

        public static float DeNormalize(float value, float goalMin, float goalMax)
        {
            return value * (goalMax - goalMin) + goalMin;
        }

        public static double Cross(Vector2 point1, Vector2 point2, Vector2 point3)
        {
            var x1 = point2.x - point1.x;
            var y1 = point2.y - point1.y;
            var x2 = point3.x - point2.x;
            var y2 = point3.y - point2.y;

            return x1 * y2 - y1 * x2;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Random.Range(0, n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void SwapElements<T>(this IList<T> list, T firstElement, T secondElement)
        {
            var firstIndex = 0;
            var secondIndex = 0;
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].Equals(firstElement)) firstIndex = i;
                if (list[i].Equals(secondElement)) secondIndex = i;
            }

            var tmp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = tmp;
        }

        public static bool CompareLists<T>(List<T> aListA, List<T> aListB)
        {
            if (aListA == null || aListB == null || aListA.Count != aListB.Count)
                return false;
            if (aListA.Count == 0)
                return true;
            var lookUp = new Dictionary<T, int>();
            // create index for the first list
            for (var i = 0; i < aListA.Count; i++)
            {
                var count = 0;
                if (!lookUp.TryGetValue(aListA[i], out count))
                {
                    lookUp.Add(aListA[i], 1);
                    continue;
                }

                lookUp[aListA[i]] = count + 1;
            }

            for (var i = 0; i < aListB.Count; i++)
            {
                var count = 0;
                if (!lookUp.TryGetValue(aListB[i], out count))
                    // early exit as the current value in B doesn't exist in the lookUp (and not in ListA)
                    return false;

                count--;
                if (count <= 0)
                    lookUp.Remove(aListB[i]);
                else
                    lookUp[aListB[i]] = count;
            }

            // if there are remaining elements in the lookUp, that means ListA contains elements that do not exist in ListB
            return lookUp.Count == 0;
        }

        public static T PickRandom<T>(this IList<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }

        public static string GetCurrentStateName(this Animator animator, int layer = 0)
        {
            var info = animator.GetCurrentAnimatorStateInfo(layer);

            return (from clip in animator.runtimeAnimatorController.animationClips
                    where info.IsName(clip.name)
                    select clip.name).FirstOrDefault();
        }

        public static void ResetAllTriggers(this Animator animator)
        {
            var parameters = animator.parameters.ToList();

            foreach (var parameter in parameters)
                animator.ResetTrigger(parameter.name);
        }

        public static Transform GetClosestTransform(this List<Transform> ofThese, Transform toThis)
        {
            Transform bestTarget = null;
            var closestDistanceSqr = Mathf.Infinity;
            var currentPosition = toThis.position;
            foreach (var potentialTarget in ofThese)
            {
                var directionToTarget = potentialTarget.position - currentPosition;
                var dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }

            return bestTarget;
        }

        public static bool CanMatchColorsLists(IEnumerable<Color> colors1, IEnumerable<Color> colors2)
        {
            return colors1.Any(x => colors2.Any(y => y == x));
        }

        public static string[] Split(string fullString, char splitChar = ',')
        {
            return fullString.Split(splitChar);
        }

        public static void UpdateUI(GameObject obj)
        {
            var rects = obj.gameObject.GetComponentsInChildren<RectTransform>();
            foreach (var rect in rects) LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }

        public static Bounds GetMaxBounds(GameObject g)
        {
            var b = new Bounds(g.transform.position, Vector3.zero);
            foreach (var r in g.GetComponentsInChildren<Renderer>()) b.Encapsulate(r.bounds);

            return b;
        }

        public static float GetSlope(this Vector2 firstPoint, Vector2 secondPoint)
        {
            var dx = secondPoint.x - firstPoint.x;
            var dy = secondPoint.y - firstPoint.y;

            return dy / dx;
        }

        public static AnimationCurve GenerateCurveWithDesiredKeysCount(this AnimationCurve thisCurve,
            int targetPointCounts,
            float slopeError = 0.001f)
        {
            var curveLength = thisCurve.length;
            if (curveLength <= 0)
                throw new Exception("Curve has no keys");

            var firstCurvePoint = thisCurve[0];
            var lastCurvePoint = thisCurve[curveLength - 1];
            var dt = (lastCurvePoint.time - firstCurvePoint.time) / targetPointCounts;

            var newCurveKeys = GenerateNewCurveKeys(firstCurvePoint, lastCurvePoint, dt, thisCurve);


            return thisCurve.GenerateCurveFromVector2(newCurveKeys, slopeError);
        }

        private static List<Vector2> GenerateNewCurveKeys(Keyframe firstCurvePoint, Keyframe lastCurvePoint, float dt,
            AnimationCurve multiplierAnimCurveY)
        {
            var newCurveKeys = new List<Vector2>();
            for (var time = firstCurvePoint.time; time < lastCurvePoint.time + dt; time += dt)
            {
                var yValMain = multiplierAnimCurveY.Evaluate(time);
                var mainPoint = new Vector2(time, yValMain);
                newCurveKeys.Add(mainPoint);
            }

            return newCurveKeys;
        }

        public static AnimationCurve GenerateCurveFromVector2(this AnimationCurve thisCurve, List<Vector2> newCurveKeys,
            float slopeError)
        {
            var oldCurve = thisCurve;
            thisCurve = new AnimationCurve();
            foreach (var mainPoint in newCurveKeys)
            {
                var time = mainPoint.x;
                var yValLeft = oldCurve.Evaluate(time - slopeError);
                var leftPoint = new Vector2(time - slopeError, yValLeft);
                var leftTangent = leftPoint.GetSlope(mainPoint);

                var yValRight = oldCurve.Evaluate(time + slopeError);
                var rightPoint = new Vector2(time + slopeError, yValRight);
                var rightTangent = mainPoint.GetSlope(rightPoint);

                thisCurve.AddKey(new Keyframe(mainPoint.x, mainPoint.y, leftTangent, rightTangent));
            }

            return thisCurve;
        }

        public static float GetCurrentClipLength(this Animator animator, int layer = 0)
        {
            var info = animator.GetCurrentAnimatorStateInfo(layer);

            return (from clip in animator.runtimeAnimatorController.animationClips
                where info.IsName(clip.name)
                select clip.length).FirstOrDefault();
        }
        
        public static bool IsInState(Animator animator, string stateName, out AnimatorStateInfo animatorStateInfo)
        {
            if (animator != null)
            {
                for (int layerIndex = 0; layerIndex < animator.layerCount; layerIndex++)
                {
                    AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(layerIndex);
                    if (info.IsName(stateName))
                    {
                        animatorStateInfo = info;
                        return true;
                    }
                }
            }
            animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return false;
        }

        public static bool CompareLayerMask(GameObject obj, LayerMask layerMask)
        {
            return (layerMask.value & (1 << obj.layer)) > 0;
        }
    }

}
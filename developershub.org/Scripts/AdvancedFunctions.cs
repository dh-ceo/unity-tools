//-----------------------------------------------------------------------
// Author  : Armin Ahmadi
// Email   : developershub.organization@gmail.com
// Website : www.developershub.org
// Copyright © 2020, Developers Hub
// All rights reserved
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevelopersHub.Unity.Tools
{
    public class AdvancedFunctions : MonoBehaviour
    {

        /// <summary>
        /// Smootly moves a vector3 to another vector3 with desired speed.
        /// </summary>
        /// <param name="source">Position which you want to move from.</param>
        /// <param name="target">Position which you want to reach.</param>
        /// <param name="speed">Move distance per second. Note: Do not multiply delta time to speed.</param>
        public static Vector3 LerpVector3(Vector3 source, Vector3 target, float speed)
        {
            if (source == target || speed <= 0)
            {
                return source;
            }
            float distance = Vector3.Distance(source, target);
            float t = speed * Time.deltaTime;
            if (t > distance)
            {
                t = distance;
            }
            t = t / distance;
            return Vector3.Lerp(source, target, t);
        }

        /// <summary>
        /// Smootly moves a vector2 to another vector2 with desired speed.
        /// </summary>
        /// <param name="source">Position which you want to move from.</param>
        /// <param name="target">Position which you want to reach.</param>
        /// <param name="speed">Move distance per second. Note: Do not multiply delta time to speed.</param>
        public static Vector3 LerpVector2(Vector2 source, Vector2 target, float speed)
        {
            if (source == target || speed <= 0)
            {
                return source;
            }
            float distance = Vector2.Distance(source, target);
            float t = speed * Time.deltaTime;
            if (t > distance)
            {
                t = distance;
            }
            t = t / distance;
            return Vector2.Lerp(source, target, t);
        }

        /// <summary>
        /// Smootly rotates a quaternion to another quaternion with desired speed.
        /// </summary>
        /// <param name="source">Rotation which you want to rotate from.</param>
        /// <param name="target">Rotation which you want to reach.</param>
        /// <param name="speed">Rotate angle per second. Note: Do not multiply delta time to speed.</param>
        public static Quaternion LerpQuaternion(Quaternion source, Quaternion target, float speed)
        {
            if (source == target || speed <= 0)
            {
                return source;
            }
            float angle = Quaternion.Angle(source, target);
            float t = speed * Time.deltaTime;
            if (t > angle)
            {
                t = angle;
            }
            t = t / angle;
            return Quaternion.Lerp(source, target, t);
        }

        /// <summary>
        /// Smootly changes a float to another float with desired speed.
        /// </summary>
        /// <param name="source">Float which you want to change from.</param>
        /// <param name="target">Float which you want to change to.</param>
        /// <param name="speed">Change amount per second. Note: Do not multiply delta time to speed.</param>
        public static float LerpFloat(float source, float target, float speed)
        {
            if (speed <= 0)
            {
                return source;
            }
            float difference = Mathf.Abs(source - target);
            float t = speed * Time.deltaTime;
            if (t > difference)
            {
                t = difference;
            }
            t = t / difference;
            return Mathf.Lerp(source, target, t);
        }

    }
}
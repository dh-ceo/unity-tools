//-----------------------------------------------------------------------
// Author  : Armin Ahmadi
// Gmail   : developershub.organization@gmail.com
// Email   : support@developershub.org
// Website : www.developershub.org
// Copyright © 2020, Developers Hub
// All rights reserved
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEditor;

namespace DevelopersHub.Unity.Tools
{
    public class SpriteAnimator : MonoBehaviour
    {

        [SerializeField] private bool animate = true;
        private GameObject target = null;
        [SerializeField] private Rotation rotation = null;
        [SerializeField] private Projection projection = null;
        [SerializeField] private Scaler scaler = null;
        [SerializeField] private Flotation flotation = null;
        [NonSerialized] public SpriteRenderer spriteRenderer = null;
        [NonSerialized] public Image image = null;
        [NonSerialized] public RectTransform rectTransform = null;

        public enum Axes
        {
            Positive_X, Positive_Y, Positive_Z, Negative_X, Negative_Y, Negative_Z
        }

        [Serializable] public class Projection
        {
            public bool enable = false;
            [Range(0f, 10f)] public float speed = 0.1f;
            public bool loop = true;
            [Range(0f, 10f)] public float loopDelay = 0f;
            public Sprite[] sprites = null;
            [NonSerialized] public float timer = 0;
            [NonSerialized] public float delay = 0;
            [NonSerialized] public int index = 0;
        }

        [Serializable] public class Rotation
        {
            public bool enable = false;
            public Axes axes = Axes.Positive_Y;
            [Range(0f, 1000f)] public float speed = 100f;
            [NonSerialized] public Quaternion original = Quaternion.identity;
        }

        [Serializable] public class Flotation
        {
            public bool enable = false;
            public Axes axes = Axes.Positive_Y;
            [Range(0f, 1000f)] public float speed = 10f;
            [Range(0f, 1000f)] public float amount = 1f;
            [NonSerialized] public float movement = 0;
            [NonSerialized] public bool add = true;
        }

        [Serializable] public class Scaler
        {
            public bool enable = false;
            [Range(0f, 10f)] public float speed = 1f;
            [Range(0f, 10f)] public float amount = 0.5f;
            [NonSerialized] public Vector3 original = Vector3.zero;
            [NonSerialized] public float timer = 0;
            [NonSerialized] public bool up = true;
        }

        private void Start()
        {
            if (target == null)
            {
                target = gameObject;
            }
            image = target.GetComponent<Image>();
            spriteRenderer = target.GetComponent<SpriteRenderer>();
            rectTransform = target.GetComponent<RectTransform>();
            if (rotation != null)
            {
                rotation.original = transform.rotation;
            }
            if (scaler != null)
            {
                scaler.original = transform.localScale;
            }
            Reset();
        }

        public void Reset()
        {
            if (projection != null && (image != null || spriteRenderer != null) && projection.sprites != null && projection.sprites.Length > 0)
            {
                if (spriteRenderer)
                {
                    spriteRenderer.sprite = projection.sprites[0];
                }
                if (image)
                {
                    image.sprite = projection.sprites[0];
                }
            }
            if (rotation != null)
            {
                transform.rotation = rotation.original;
            }
            if (scaler != null)
            {
                transform.localScale = scaler.original;
            }
        }

        private void Update()
        {
            if (!animate)
            {
                return;
            }
            if (projection != null && projection.enable && (image != null || spriteRenderer != null) && projection.sprites != null && projection.sprites.Length > 0)
            {
                if(!projection.loop || (projection.loop && projection.delay <= 0))
                {
                    if (projection.timer >= projection.speed)
                    {
                        projection.timer = 0;
                        projection.index++;
                        if (projection.index >= projection.sprites.Length)
                        {
                            projection.index = 0;
                        }
                        if (spriteRenderer)
                        {
                            spriteRenderer.sprite = projection.sprites[projection.index];
                        }
                        if (image)
                        {
                            image.sprite = projection.sprites[projection.index];
                        }
                    }
                    else
                    {
                        projection.timer += Time.deltaTime;
                    }
                    if(projection.index == 0)
                    {
                        if (projection.loop)
                        {
                            projection.delay = projection.loopDelay;
                        }
                        else
                        {
                            projection.enable = false;
                        }
                    }
                }
                else
                {
                    projection.delay -= Time.deltaTime;
                }
            }
            if(rotation != null && rotation.enable)
            {
                transform.Rotate(GetVector(rotation.axes) * rotation.speed * Time.deltaTime);
            }
            if (scaler != null && scaler.enable)
            {
                if (scaler.up)
                {
                    Vector3 up = new Vector3(scaler.original.x + scaler.amount, scaler.original.y + scaler.amount, scaler.original.z + scaler.amount);
                    if(transform.localScale == up)
                    {
                        scaler.up = false;
                    }
                    else
                    {
                        transform.localScale = AdvancedFunctions.LerpVector3(transform.localScale, up, scaler.speed);
                    }
                }
                else
                {
                    if (transform.localScale == scaler.original)
                    {
                        scaler.up = true;
                    }
                    else
                    {
                        transform.localScale = AdvancedFunctions.LerpVector3(transform.localScale, scaler.original, scaler.speed);
                    }
                }
            }
            if (flotation != null && flotation.enable)
            {
                float n = flotation.speed * Time.deltaTime;
                if(flotation.movement + n > flotation.amount)
                {
                    n = flotation.amount - flotation.movement;
                }
                flotation.movement = flotation.movement + n;
                if (flotation.add)
                {
                    transform.Translate(GetVector(flotation.axes) * n, Space.Self);
                }
                else
                {
                    transform.Translate(GetVector(flotation.axes) * -n, Space.Self);
                }
                if (flotation.movement >= flotation.amount)
                {
                    flotation.add = !flotation.add;
                    flotation.movement = 0;
                }
            }
        }

        private Vector3 GetVector(Axes axes)
        {
            switch (axes)
            {
                case Axes.Positive_X: return Vector3.right;
                case Axes.Positive_Y: return Vector3.up;
                case Axes.Positive_Z: return Vector3.forward;
                case Axes.Negative_X: return -Vector3.right;
                case Axes.Negative_Y: return -Vector3.up;
                case Axes.Negative_Z: return -Vector3.forward;
                default: return Vector3.zero;
            }
        }

    }
}

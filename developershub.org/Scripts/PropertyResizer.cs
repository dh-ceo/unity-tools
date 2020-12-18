//-----------------------------------------------------------------------
// Author  : Armin Ahmadi
// Email   : developershub.organization@gmail.com
// Website : www.developershub.org
// Copyright © 2020, Developers Hub
// All rights reserved
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DevelopersHub.Unity.Tools
{
    #if UNITY_EDITOR
    [ExecuteInEditMode] 
    #endif
    public class PropertyResizer : MonoBehaviour
    {

        private enum Factor
        {
            screenWidth, screenHeight
        }

        private enum Target
        {
            width, height, font
        }

        [Serializable] private class ResizeJob
        {
            public GameObject source = null;
            public Factor factor = Factor.screenHeight;
            public Target target = Target.height;
            [Range(0f, 1f)] public float percent = 0.5f;
        }

        [SerializeField] private ResizeJob[] resizeJobs = null;

        private void Start()
        {
            SetSize();
        }

        #if UNITY_EDITOR
        private float timer = 0;
        private float period = 1;
        private void Update()
        {
            if(timer >= period)
            {
                SetSize();
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        #endif

        private void SetSize()
        {
            if(resizeJobs == null)
            {
                return;
            }
            for (int i = 0; i < resizeJobs.Length; i++)
            {
                if (resizeJobs[i].source)
                {
                    float size = 0;
                    switch (resizeJobs[i].factor)
                    {
                        case Factor.screenWidth:
                            size = Screen.width * resizeJobs[i].percent;
                            break;
                        case Factor.screenHeight:
                            size = Screen.height * resizeJobs[i].percent;
                            break;
                    }
                    switch (resizeJobs[i].target)
                    {
                        case Target.width:
                            RectTransform rectW = resizeJobs[i].source.GetComponent<RectTransform>();
                            if (rectW)
                            {
                                rectW.sizeDelta = new Vector2(size, rectW.sizeDelta.y);
                            }
                            break;
                        case Target.height:
                            RectTransform rectH = resizeJobs[i].source.GetComponent<RectTransform>();
                            if (rectH)
                            {
                                rectH.sizeDelta = new Vector2(rectH.sizeDelta.x, size);
                            }
                            break;
                        case Target.font:
                            Text text = resizeJobs[i].source.GetComponent<Text>();
                            if (text)
                            {
                                text.fontSize = Mathf.RoundToInt(size);
                            }
                            break;
                    }
                }
            }
        }

    }
}
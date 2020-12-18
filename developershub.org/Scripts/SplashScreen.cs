//-----------------------------------------------------------------------
// Author  : Armin Ahmadi
// Email   : developershub.organization@gmail.com
// Website : www.developershub.org
// Copyright © 2020, Developers Hub
// All rights reserved
//-----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;

namespace DevelopersHub.Unity.Tools
{
    public class SplashScreen : MonoBehaviour
    {
        
        [Serializable] private class Element
        {
            public Graphic graphic = null;
            [Range(0f, 3600f)] public float startPause = 0f;
            [Range(0f, 3600f)] public float inTransition = 1.5f;
            [Range(0f, 3600f)] public float midPause = 2f;
            [Range(0f, 3600f)] public float outTransition = 1.5f;
            [Tooltip("If checked then out transition will be ignored and graphic will remain visible.")] public bool keepVisible = false;
        }

        [SerializeField] private UnityEvent onSplashDone = new UnityEvent();
        [SerializeField] private List<Element> elements = new List<Element>();

        private void Start()
        {
            for (int i = elements.Count - 1; i >= 0; i--)
            {
                if(elements[i] != null && elements[i].graphic != null)
                {
                    elements[i].graphic.gameObject.SetActive(true);
                    elements[i].graphic.canvasRenderer.SetAlpha(0.0f);
                }
                else
                {
                    elements.RemoveAt(i);
                }
            }
            NextSplash();
        }

        private void SplashDone()
        {
            if (onSplashDone != null)
            {
                onSplashDone.Invoke();
            }
        }

        private void NextSplash()
        {
            if(elements.Count > 0)
            {
                StartCoroutine(Splash());
            }
            else
            {
                SplashDone();
            }
        }

        private IEnumerator Splash()
        {
            yield return new WaitForSeconds(elements[0].startPause);
            FadeIn(elements[0].graphic, elements[0].inTransition);
            yield return new WaitForSeconds(elements[0].inTransition + elements[0].midPause);
            if (!elements[0].keepVisible)
            {
                FadeOut(elements[0].graphic, elements[0].outTransition);
                yield return new WaitForSeconds(elements[0].outTransition);
            }
            elements.RemoveAt(0);
            NextSplash();
        }

        private void FadeIn(Graphic graphic, float time)
        {
            graphic.CrossFadeAlpha(1.0f, time, false);
        }

        private void FadeOut(Graphic graphic, float time)
        {
            graphic.CrossFadeAlpha(0.0f, time, false);
        }

        private IEnumerator LoadNewScene(int scene)
        {
            // yield return new WaitForSeconds(0.2f);
            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);
            async.allowSceneActivation = false;
            while (!async.isDone)
            {
                float progress = Mathf.Clamp01(async.progress / 0.9f);
                // loadingBar.fillAmount = progress;
                // loadingProgress.text = progress * 100f + "%";
                if (async.progress >= 0.9f)
                {
                    async.allowSceneActivation = true;
                }
                yield return null;
            }
        }

    }
}
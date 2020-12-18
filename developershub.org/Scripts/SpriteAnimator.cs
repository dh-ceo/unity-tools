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
    public class SpriteAnimator : MonoBehaviour
    {

        [SerializeField] private bool animate = true;
        [SerializeField] [Range(0f, 10f)] private float speed = 0.1f;
        [SerializeField] private SpriteRenderer spriteRenderer = null;
        [SerializeField] private Sprite[] sprites = null;

        private float timer = 0;
        private int index = 0;

        private void Start()
        {
            spriteRenderer.sprite = sprites[0];
        }

        private void Update()
        {
            if (!animate || spriteRenderer == null || sprites == null)
            {
                return;
            }
            if(timer >= speed)
            {
                timer = 0;
                index++;
                if(index >= sprites.Length)
                {
                    index = 0;
                }
                spriteRenderer.sprite = sprites[index];
            }
            else
            {
                timer += Time.deltaTime;
            }
        }

    }
}
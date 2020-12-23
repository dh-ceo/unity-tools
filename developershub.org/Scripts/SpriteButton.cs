//-----------------------------------------------------------------------
// Author  : Armin Ahmadi
// Email   : developershub.organization@gmail.com
// Website : www.developershub.org
// Copyright © 2020, Developers Hub
// All rights reserved
//-----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Events;

namespace DevelopersHub.Unity.Tools
{
    public class SpriteButton : MonoBehaviour
    {

        [SerializeField] private bool ignoreTransparent = true;
        private SpriteRenderer spriteRenderer = null;
        [SerializeField] private Color highlight = Color.gray;
        private Color normal = Color.white;
        private Vector2 position = Vector2.zero;
        [SerializeField] private UnityEvent onClick = new UnityEvent();
        private Collider2D col = null;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            col = GetComponent<Collider2D>();
            if (spriteRenderer)
            {
                normal = spriteRenderer.color;
            }
            else
            {
                Debug.LogWarning("This is not a sprite renderer.");
            }
            if(!col)
            {
                Debug.LogWarning("You need to attach a 2d collider to this sprite renderer.");
            }
        }

        private bool held = false;

        private void Update()
        {
            if (!spriteRenderer || !col)
            {
                return;
            }
            bool down = false;
            bool up = false;
            if (Input.touchSupported)
            {
                if(Input.touchCount == 1)
                {
                    if(Input.touches[0].phase == TouchPhase.Began)
                    {
                        down = true;
                    }
                }
                else if (held)
                {
                    up = true;
                }
            }
            else
            {
                down = Input.GetMouseButtonDown(0);
                up = Input.GetMouseButtonUp(0);
            }
            if (down && !held)
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                position = new Vector2(worldPos.x, worldPos.y);
                RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);
                if (hit.collider == col)
                {
                    if (ignoreTransparent)
                    {
                        float hafX = spriteRenderer.sprite.texture.width * 0.5f;
                        float hafY = spriteRenderer.sprite.texture.height * 0.5f;
                        Vector2 delta = (position - new Vector2(transform.position.x, transform.position.y)) * spriteRenderer.sprite.pixelsPerUnit;
                        Color c = spriteRenderer.sprite.texture.GetPixel(Mathf.RoundToInt(delta.x + hafX), Mathf.RoundToInt(delta.y + hafY));
                        if (c.a > 0)
                        {
                            OnDown();
                        }
                    }
                    else
                    {
                        OnDown();
                    }
                }
            }
            if (up && held)
            {
                OnUp();
            }
        }

        private void OnDown()
        {
            held = true;
            spriteRenderer.color = highlight;
        }

        private void OnUp()
        {
            held = false;
            spriteRenderer.color = normal;
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(pos == position)
            {
                OnClick();
            }
        }

        private void OnClick()
        {
            if (onClick != null)
            {
                onClick.Invoke();
            }
        }

    }
}
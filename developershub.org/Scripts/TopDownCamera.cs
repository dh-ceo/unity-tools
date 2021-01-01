//-----------------------------------------------------------------------
// Author  : Armin Ahmadi
// Email   : developershub.organization@gmail.com
// Website : www.developershub.org
// Copyright © 2020, Developers Hub
// All rights reserved
//-----------------------------------------------------------------------

using UnityEngine;

namespace DevelopersHub.Unity.Tools
{
    public class TopDownCamera : MonoBehaviour
    {

        [Header("References")]
        [SerializeField] private Camera _camera = null;

        [Header("Moving")]
        [SerializeField] private bool moveAllow = true;
        [SerializeField] [Range(0.01f, 10f)] private float moveSpeedNormal = 2;
        [SerializeField] [Range(0.01f, 10f)] private float moveSpeedTouch = 0.5f;
        [SerializeField] private Vector3 moveCenter = new Vector3(0, 0, -5);
        [SerializeField] private bool moveLimit = true;
        [SerializeField] [Tooltip("How far camera can show on the right side of the center.")] [Range(0, 1000f)] private float moveLimitRight = 5;
        [SerializeField] [Tooltip("How far camera can show on the left side of the center.")] [Range(0, 1000f)] private float moveLimitLeft = 5;
        [SerializeField] [Tooltip("How far camera can show on the up side of the center.")] [Range(0, 1000f)] private float moveLimitUp = 5;
        [SerializeField] [Tooltip("How far camera can show on the down side of the center.")] [Range(0, 1000f)] private float moveLimitDown = 5;
        [SerializeField] private bool moveSlide = false;
        [SerializeField] [Range(0.01f, 50f)] private float slideSpeedNormal = 20;
        [SerializeField] [Range(0.01f, 50f)] private float slideSpeedTouch = 5;

        [Header("Zooming")]
        [SerializeField] private bool zoomAllow = true;
        [SerializeField] private bool zoomPinch = true;
        [SerializeField] [Range(1f, 500f)] private float zoomSpeedNormal = 50;
        [SerializeField] [Range(0.01f, 10f)] private float zoomSpeedTouch = 0.1f;
        [SerializeField] [Range(0.01f, 50f)] private float zoomDefault = 2;
        [SerializeField] [Range(0.01f, 50f)] private float zoomMin = 1;
        [SerializeField] [Range(0.01f, 50f)] private float zoomMax = 3;

        public bool AllowMove { get { return moveAllow; } set { moveAllow = value; } }
        public bool AllowZoom { get { return zoomAllow; } set { zoomAllow = value; } }

        private bool moving = false;
        private Vector2 moveInput = Vector2.zero;
        private Vector2 lastScreenPos = Vector2.zero;

        private float zoomInput = 0;
        private Vector3 zoomPoint = Vector2.zero;
        private int zoomFinger0 = 0;
        private int zoomFinger1 = 0;

        private bool auto = false;
        private Vector3 autoMoveTarget = Vector2.zero;
        private float autoMoveSpeed = 1;
        private float autoZoomTarget = 1;
        private float autoZoomSpeed = 1;
        
        public void ChangeCenter(Vector3 center)
        {
            moveCenter = center;
        }
        
        public void ChangeMoveLimits(float limitRight, float limitLeft, float limitUp, float LimitDown)
        {
            moveLimitRight = limitRight;
            moveLimitLeft = limitLeft;
            moveLimitUp = limitUp;
            moveLimitDown = LimitDown;
        }

        public void ChangeZoomLimits(float normal, float min, float max)
        {
            zoomDefault = normal;
            zoomMin = min;
            zoomMax = max;
        }

        private void Start()
        {
            if (!_camera)
            {
                _camera = GetComponent<Camera>();
            }
            if (!_camera)
            {
                _camera = Camera.main;
            }
            if (_camera)
            {
                _camera.orthographic = true;
                _camera.orthographicSize = zoomDefault;
            }
        }

        private void Update()
        {
            if (!_camera)
            {
                return;
            }
            if (auto)
            {
                Auto();
            }
            else
            {
                Control();
                Handle();
            }
        }

        private void Control()
        {
            if (Input.touchSupported)
            {
                zoomInput = 0;
                if (Input.touchCount >= 2)
                {
                    Touch touch0 = Input.touches[0];
                    Touch touch1 = Input.touches[1];
                    if(touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
                    {
                        zoomFinger0 = touch0.fingerId;
                        zoomFinger1 = touch1.fingerId;
                    }
                    else
                    {
                        for (int i = 0; i < Input.touchCount; i++)
                        {
                            if (Input.touches[i].fingerId == zoomFinger0)
                            {
                                touch0 = Input.touches[i];
                            }
                            else if (Input.touches[i].fingerId == zoomFinger1)
                            {
                                touch1 = Input.touches[i];
                            }
                        }
                        Vector2 z0 = touch0.position - touch0.deltaPosition;
                        Vector2 z1 = touch1.position - touch1.deltaPosition;
                        zoomInput = Vector2.Distance(z0, z1) - Vector2.Distance(touch0.position, touch1.position);
                        lastScreenPos = Vector2.Lerp(touch0.position, touch1.position, 0.5f);
                        zoomPoint = _camera.ScreenToWorldPoint(lastScreenPos);
                    }
                    moveInput = Vector2.zero;
                    moving = false;
                }
                else if (Input.touchCount == 1)
                {
                    Touch touch0 = Input.touches[0];
                    if(touch0.phase == TouchPhase.Canceled || touch0.phase == TouchPhase.Ended)
                    {
                        moving = false;
                    }
                    else if (touch0.phase == TouchPhase.Began)
                    {
                        moving = true;
                        lastScreenPos = touch0.position;
                    }
                    if (moving)
                    {
                        moveInput = lastScreenPos - touch0.position;
                    }
                    lastScreenPos = touch0.position;
                }
                else if (Input.touchCount == 0)
                {
                    moving = false;
                }
            }
            else
            {
                Vector2 mp = Input.mousePosition;
                if (Input.GetMouseButtonUp(0))
                {
                    moving = false;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    moving = true;
                    lastScreenPos = mp;
                }
                if (moving)
                {
                    moveInput = lastScreenPos - mp;
                }
                zoomInput = Input.GetAxis("Mouse ScrollWheel");
                lastScreenPos = mp;
                zoomPoint = _camera.ScreenToWorldPoint(lastScreenPos);
            }
            if(!moving)
            {
                if (moveSlide && moveInput != Vector2.zero)
                {
                    moveInput = Vector2.Lerp(moveInput, Vector2.zero, (Input.touchSupported ? slideSpeedTouch : slideSpeedNormal) * Time.deltaTime);
                }
                else
                {
                    moveInput = Vector2.zero;
                }
            }
            if (!moveAllow)
            {
                moveInput = Vector2.zero;
                moving = false;
            }
            if (!zoomAllow)
            {
                zoomInput = 0;
            }
        }

        private void Handle()
        {
            float h = _camera.orthographicSize;
            float w = _camera.aspect * h;
            float x = _camera.transform.position.x;
            float y = _camera.transform.position.y;
            if (zoomInput != 0)
            {
                h = _camera.orthographicSize - zoomInput * (Input.touchSupported ? zoomSpeedTouch : zoomSpeedNormal) * Time.deltaTime;
                if (h > zoomMax)
                {
                    h = zoomMax;
                }
                if (h < zoomMin)
                {
                    h = zoomMin;
                }
                if(moveLimit)
                {
                    w = _camera.aspect * h;
                    if (x > moveCenter.x + moveLimitRight - w)
                    {
                        x = moveCenter.x + moveLimitRight - w;
                    }
                    if (x < moveCenter.x - moveLimitLeft + w)
                    {
                        x = moveCenter.x - moveLimitLeft + w;
                    }
                    if (y > moveCenter.y + moveLimitUp - h)
                    {
                        y = moveCenter.y + moveLimitUp - h;
                    }
                    if (y < moveCenter.y - moveLimitDown + h)
                    {
                        y = moveCenter.y - moveLimitDown + h;
                    }
                    _camera.transform.position = new Vector3(x, y, moveCenter.z);
                }
                _camera.orthographicSize = h;
                if (zoomPinch)
                {
                    Vector3 p = _camera.transform.position + zoomPoint - _camera.ScreenToWorldPoint(lastScreenPos);
                    y = p.y;
                    x = p.x;
                    if (x > moveCenter.x + moveLimitRight - w)
                    {
                        x = moveCenter.x + moveLimitRight - w;
                    }
                    if (x < moveCenter.x - moveLimitLeft + w)
                    {
                        x = moveCenter.x - moveLimitLeft + w;
                    }
                    if (y > moveCenter.y + moveLimitUp - h)
                    {
                        y = moveCenter.y + moveLimitUp - h;
                    }
                    if (y < moveCenter.y - moveLimitDown + h)
                    {
                        y = moveCenter.y - moveLimitDown + h;
                    }
                    _camera.transform.position = new Vector3(x, y, moveCenter.z);
                }
                moveInput = Vector2.zero;
            }
            else
            {
                x = _camera.transform.position.x + moveInput.x * (Input.touchSupported ? moveSpeedTouch : moveSpeedNormal) * (h / zoomMax) * Time.deltaTime;
                y = _camera.transform.position.y + moveInput.y * (Input.touchSupported ? moveSpeedTouch : moveSpeedNormal) * (h / zoomMax) * Time.deltaTime;
                if (moveLimit)
                {
                    if (x > moveCenter.x + moveLimitRight - w)
                    {
                        x = moveCenter.x + moveLimitRight - w;
                    }
                    if (x < moveCenter.x - moveLimitLeft + w)
                    {
                        x = moveCenter.x - moveLimitLeft + w;
                    }
                    if (y > moveCenter.y + moveLimitUp - h)
                    {
                        y = moveCenter.y + moveLimitUp - h;
                    }
                    if (y < moveCenter.y - moveLimitDown + h)
                    {
                        y = moveCenter.y - moveLimitDown + h;
                    }
                }
                _camera.transform.position = new Vector3(x, y, moveCenter.z);
            }
        }

        private void Auto()
        {
            bool ax = false;
            bool ay = false;
            bool b = false;
            float h = _camera.orthographicSize;
            float w = _camera.aspect * h;
            if (_camera.transform.position != autoMoveTarget)
            {
                _camera.transform.position = AdvancedFunctions.LerpVector3(_camera.transform.position, autoMoveTarget, autoMoveSpeed);
                float x = _camera.transform.position.x;
                float y = _camera.transform.position.y;
                if (moveLimit)
                {
                    if (x > moveCenter.x + moveLimitRight - w)
                    {
                        x = moveCenter.x + moveLimitRight - w;
                        ax = true;
                    }
                    if (x < moveCenter.x - moveLimitLeft + w)
                    {
                        x = moveCenter.x - moveLimitLeft + w;
                        ax = true;
                    }
                    if (y > moveCenter.y + moveLimitUp - h)
                    {
                        y = moveCenter.y + moveLimitUp - h;
                        ay = true;
                    }
                    if (y < moveCenter.y - moveLimitDown + h)
                    {
                        y = moveCenter.y - moveLimitDown + h;
                        ay = true;
                    }
                }
                _camera.transform.position = new Vector3(x, y, moveCenter.z);
            }
            if(Mathf.Abs(_camera.transform.position.x - autoMoveTarget.x) <= 0.01f)
            {
                ax = true;
            }
            if (Mathf.Abs(_camera.transform.position.y - autoMoveTarget.y) <= 0.01f)
            {
                ay = true;
            }
            if (_camera.orthographicSize != autoZoomTarget)
            {
                _camera.orthographicSize = AdvancedFunctions.LerpFloat(_camera.orthographicSize, autoZoomTarget, autoZoomSpeed);
            }
            else
            {
                b = true;
            }
            if(ax && ay && b)
            {
                auto = false;
            }
        }

        /// <summary>
        /// Smootly move and zoom on target point.
        /// </summary>
        /// <param name="target">Position which you want to move to.</param>
        /// <param name="zoom">Zoom amount that you want to reach.</param>
        /// <param name="moveSpeed">Move speed multiplier.</param>
        /// <param name="zoomSpeed">Zoom speed multiplier. Zero for default value.</param>
        public void LerpTo(Vector2 target, float zoom = 0, float moveSpeed = 1.2f, float zoomSpeed = 0.5f)
        {
            autoMoveTarget = new Vector3(target.x, target.y, moveCenter.z);
            autoZoomTarget = (zoom == 0) ? zoomDefault : zoom;
            autoMoveSpeed = moveSpeed;
            autoZoomSpeed = zoomSpeed;
            auto = true;
            moving = false;
            moveInput = Vector2.zero;
            zoomInput = 0;
        }

        /// <summary>
        /// Immediately jump on target point.
        /// </summary>
        /// <param name="target">Position which you want to jump on.</param>
        /// <param name="zoom">Zoom amount that you want to have.</param>
        public void JumpTo(Vector2 target, float zoom = 0)
        {
            Vector3 point = new Vector3(target.x, target.y, moveCenter.z);
            float h = (zoom == 0) ? zoomDefault : zoom;
            moving = false;
            moveInput = Vector2.zero;
            zoomInput = 0;
            float w = _camera.aspect * h;
            _camera.orthographicSize = h;
            float x = target.x;
            float y = target.y;
            if (moveLimit)
            {
                if (x > moveCenter.x + moveLimitRight - w)
                {
                    x = moveCenter.x + moveLimitRight - w;
                }
                if (x < moveCenter.x - moveLimitLeft + w)
                {
                    x = moveCenter.x - moveLimitLeft + w;
                }
                if (y > moveCenter.y + moveLimitUp - h)
                {
                    y = moveCenter.y + moveLimitUp - h;
                }
                if (y < moveCenter.y - moveLimitDown + h)
                {
                    y = moveCenter.y - moveLimitDown + h;
                }
            }
            _camera.transform.position = new Vector3(x, y, moveCenter.z);
        }

    }
}

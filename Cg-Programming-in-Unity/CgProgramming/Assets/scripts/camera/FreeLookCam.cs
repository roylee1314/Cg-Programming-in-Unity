﻿using UnityEngine;
using System.Collections;
namespace Camera { 
    public class FreeLookCam : MonoBehaviour {
        private Transform mTrans;
        [SerializeField]
        private float m_MoveSpeed = 1f;                      // How fast the rig will move to keep up with the target's position.
        [Range(0f, 10f)]
        [SerializeField]
        private float m_TurnSpeed = 1.5f;   // How fast the rig will rotate from user input.
        [SerializeField]
        private float m_TurnSmoothing = 0.0f;                // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
        [SerializeField]
        private float m_TiltMax = 75f;                       // The maximum value of the x axis rotation of the pivot.
        [SerializeField]
        private float m_TiltMin = 45f;                       // The minimum value of the x axis rotation of the pivot.
        [SerializeField]
        private bool m_LockCursor = false;                   // Whether the cursor should be hidden and locked.
        [SerializeField]
        private bool m_VerticalAutoReturn = true;           // set wether or not the vertical axis should auto return
        private float m_LookAngle;                    // The rig's y axis rotation.
        private float m_TiltAngle;                    // The pivot's x axis rotation.
        private const float k_LookDistance = 100f;    // How far in front of the pivot the character's look target is.
        private Vector3 m_PivotEulers;
        private Quaternion m_PivotTargetRot;
        private Quaternion m_TransformTargetRot;
        // Use this for initialization
        void Start () {
            mTrans = GetComponent<Transform>();
	    }
	
	    // Update is called once per frame
	    void Update () {
            Move();
            Rotate();
       
        }

        void Move()
        {
            var z = Input.GetAxis("Vertical");
            var x = Input.GetAxis("Horizontal");
            Vector3 move = new Vector3(x, 0, z);
            mTrans.position += move;

        }
        
        void Rotate()
        {
            if (Time.timeScale < float.Epsilon)
                return;

            // Read the user input
            var x = Input.GetAxis("Mouse X");
            var y = Input.GetAxis("Mouse Y");

            // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
            m_LookAngle += x * m_TurnSpeed;

            // Rotate the rig (the root object) around Y axis only:
            m_TransformTargetRot = Quaternion.Euler(0f, m_LookAngle, 0f);

            if (m_VerticalAutoReturn)
            {
                // For tilt input, we need to behave differently depending on whether we're using mouse or touch input:
                // on mobile, vertical input is directly mapped to tilt value, so it springs back automatically when the look input is released
                // we have to test whether above or below zero because we want to auto-return to zero even if min and max are not symmetrical.
                m_TiltAngle = y > 0 ? Mathf.Lerp(0, -m_TiltMin, y) : Mathf.Lerp(0, m_TiltMax, -y);
            }
            else
            {
                // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
                m_TiltAngle -= y * m_TurnSpeed;
                // and make sure the new value is within the tilt range
                m_TiltAngle = Mathf.Clamp(m_TiltAngle, -m_TiltMin, m_TiltMax);
            }

            // Tilt input around X is applied to the pivot (the child of this object)
            m_PivotTargetRot = Quaternion.Euler(m_TiltAngle, m_PivotEulers.y, m_PivotEulers.z);

            if (m_TurnSmoothing > 0)
            {
                mTrans.localRotation = Quaternion.Slerp(mTrans.localRotation, m_PivotTargetRot, m_TurnSmoothing * Time.deltaTime);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, m_TransformTargetRot, m_TurnSmoothing * Time.deltaTime);
            }
            else
            {
                mTrans.localRotation = m_PivotTargetRot;
                transform.localRotation = m_TransformTargetRot;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class LightEstimation : MonoBehaviour
{
     [SerializeField]
        [Tooltip("The ARCameraManager which will produce frame events containing light estimation information.")]
        ARCameraManager m_CameraManager;

        /// <summary>
        /// Get or set the <c>ARCameraManager</c>.
        /// </summary>
        public ARCameraManager cameraManager
        {
            get { return m_CameraManager; }
            set
            {
                if (m_CameraManager == value)
                    return;

                if (m_CameraManager != null)
                    m_CameraManager.frameReceived -= FrameChanged;

                m_CameraManager = value;

                if (m_CameraManager != null & enabled)
                    m_CameraManager.frameReceived += FrameChanged;
            }
        }

        /// <summary>
        /// The estimated brightness of the physical environment, if available.
        /// </summary>
        public float? brightness { get; private set; }

        /// <summary>
        /// The estimated color temperature of the physical environment, if available.
        /// </summary>
        public float? colorTemperature { get; private set; }

        /// <summary>
        /// The estimated color correction value of the physical environment, if available.
        /// </summary>
        public Color? colorCorrection { get; private set; }

        public Vector3? mainLightDirection = Vector3.zero;

        [SerializeField] private Transform arrow;

        public bool showArrow = false;
        
        void Awake ()
        {
            m_Light = GetComponent<Light>();
        }

        void OnEnable()
        {
            if (m_CameraManager != null)
                m_CameraManager.frameReceived += FrameChanged;

            if (arrow)
            {
                arrow.gameObject.SetActive(false);
            }

            Application.onBeforeRender += OnBeforeRender;
        }
        
        void OnDisable()
        {
            Application.onBeforeRender -= OnBeforeRender;
            
            if (m_CameraManager != null)
                m_CameraManager.frameReceived -= FrameChanged;
        }

        void OnBeforeRender()
        {
            if (arrow && m_CameraManager)
            {
                var cameraTransform = m_CameraManager.GetComponent<Camera>().transform;
                arrow.position = cameraTransform.position + cameraTransform.forward * 0.25f;
            }
        }
        
        void FrameChanged(ARCameraFrameEventArgs args)
        {
            if (args.lightEstimation.averageBrightness.HasValue)
            {
                brightness = args.lightEstimation.averageBrightness.Value;
                m_Light.intensity = brightness.Value;
            }
            else
            {
                brightness = null;
            }

            if (args.lightEstimation.averageColorTemperature.HasValue)
            {
                colorTemperature = args.lightEstimation.averageColorTemperature.Value;
                m_Light.colorTemperature = colorTemperature.Value;
            }
            else
            {
                colorTemperature = null;
            }

            if (args.lightEstimation.colorCorrection.HasValue)
            {
                colorCorrection = args.lightEstimation.colorCorrection.Value;
                m_Light.color = colorCorrection.Value;
            }
            else
            {
                colorCorrection = null;
            }
            
            if (args.lightEstimation.mainLightDirection.HasValue)
            {
                mainLightDirection = args.lightEstimation.mainLightDirection;
                m_Light.transform.rotation = Quaternion.LookRotation(mainLightDirection.Value);
                if (arrow && showArrow)
                {
                    arrow.gameObject.SetActive(true);
                    arrow.transform.rotation = Quaternion.LookRotation(mainLightDirection.Value);
                }
            }
            else
            {
                arrow?.gameObject.SetActive(false);
            }
        }

        Light m_Light;
    
}

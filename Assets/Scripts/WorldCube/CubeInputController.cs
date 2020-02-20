using System;
using System.IO.Ports;
using Player;
using UnityEngine;
using Utils;

namespace WorldCube
{
    [RequireComponent(typeof(CubeControllerV2))]
    public class CubeInputController : MonoBehaviour
    {
        [Header("Arduino")] public int readTimeout = 7;
        public bool useForcedPort = false;
        public string portString = "COM3";
        public bool disableSerialPort;

        private PlayerGridController m_playerGridController;
        private CubeControllerV2 m_cubeController;
        private SerialPort m_serialPort;

        #region Unity Functions

        private void Start()
        {
            m_playerGridController = GameObject.FindGameObjectWithTag(TagManager.Player)
                .GetComponent<PlayerGridController>();
            m_cubeController = GetComponent<CubeControllerV2>();

            string[] ports = SerialPort.GetPortNames();
            string portName;
            if (disableSerialPort)
            {
                portName = portString;
            }
            else
            {
                portName = ports[0]; // Will be switching to Arduino
                // May not be required as PI4 might be used
            }

            if (useForcedPort)
            {
                portName = portString;
            }

            Debug.Log($"Target Port: {portName}");

            m_serialPort = new SerialPort(portName, 9600);
            m_serialPort.ReadTimeout = readTimeout;
            if (!m_serialPort.IsOpen)
            {
                if (!disableSerialPort)
                {
                    m_serialPort.Open();
                }

                Debug.Log("Port is Closed. Opening");
            }
        }

        private void OnApplicationQuit()
        {
            if (m_serialPort != null)
            {
                m_serialPort.Close();
            }
        }

        private void Update()
        {
            ReadArduinoInput();
            HandleKeyboardInput();
        }

        #endregion

        #region Utility Functions

        #region Arduino

        private void ReadArduinoInput()
        {
            if (m_playerGridController.IsPlayerMoving())
            {
                // Don't allow the cube to move when the player is moving and vice versa
                return;
            }

            try
            {
                string input = m_serialPort.ReadLine();
                Debug.Log(input);

                string[] splitInput = input.Split(':');
                string sideInput = splitInput[0];
                string directionString = splitInput[1];

                switch (input)
                {
                    case "Left":
                    case "Right":
                    case "Front":
                    case "Back":
                    case "Top":
                        sideInput = input;
                        break;

                    default:
                    {
                        bool parseSuccess = int.TryParse(directionString, out int direction);
                        if (!parseSuccess)
                        {
                            return;
                        }

                        switch (sideInput)
                        {
                            case "Left":
                                m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(1, 0, 0), direction);
                                break;

                            case "Right":
                                m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(-1, 0, 0), -direction);
                                break;

                            case "Front":
                                m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 0, 1), direction);
                                break;

                            case "Back":
                                m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 0, -1), -direction);
                                break;

                            case "Top":
                                m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 1, 0), direction);
                                break;

                            default:
                                Debug.Log("Invalid Input Sent");
                                break;
                        }
                    }
                        break;
                }
            }
            catch (TimeoutException)
            {
                // Don't do anything. This is not required as there is no input
            }
            catch (InvalidOperationException)
            {
                // Don't do anything. This is not required as there is nothing connected
            }
        }

        #endregion

        #region Keyboard

        private void HandleKeyboardInput()
        {
            if (m_playerGridController.IsPlayerMoving())
            {
                // Don't allow the cube to move when the player is moving and vice versa
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(-1, 0, 0), 1);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(-1, 0, 0), -1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(1, 0, 0), 1);
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(1, 0, 0), -1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 0, 1), 1);
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 0, 1), -1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 0, -1), 1);
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 0, -1), -1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 1, 0), 1);
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 1, 0), -1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, -1, 0), 1);
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, -1, 0), -1);
            }
        }

        #endregion

        #endregion
    }
}
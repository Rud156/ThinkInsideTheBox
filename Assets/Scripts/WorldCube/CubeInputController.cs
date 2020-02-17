using System;
using System.IO.Ports;
using Player;
using UnityEngine;

namespace WorldCube
{
    [RequireComponent(typeof(CubeControllerV2))]
    public class CubeInputController : MonoBehaviour
    {
        [Header("Player")] public PlayerGridController playerGridController;

        [Header("Arduino")] public int readTimeout = 7;
        public bool useForcedPort = false;
        public string portString = "COM3";
        public bool disableSerialPort;

        private CubeControllerV2 _cubeController;
        private SerialPort _serialPort;

        #region Unity Functions

        private void Start()
        {
            _cubeController = GetComponent<CubeControllerV2>();

            string[] ports = SerialPort.GetPortNames();
            string portName;
            if (disableSerialPort)
            {
                portName = portString;
            }
            else
            {
                portName = ports[0]; // TODO: Use ManagementObject to find the data regarding the port.
                // May not be required as PI4 might be used
            }

            if (useForcedPort)
            {
                portName = portString;
            }

            Debug.Log($"Target Port: {portName}");

            _serialPort = new SerialPort(portName, 9600);
            _serialPort.ReadTimeout = readTimeout;
            if (!_serialPort.IsOpen)
            {
                if (!disableSerialPort)
                {
                    _serialPort.Open();
                }

                Debug.Log("Port is Closed. Opening");
            }
        }

        private void OnApplicationQuit()
        {
            if (_serialPort != null)
            {
                _serialPort.Close();
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
            if (playerGridController.IsPlayerMoving())
            {
                // Don't allow the cube to move when the player is moving and vice versa
                return;
            }

            try
            {
                string input = _serialPort.ReadLine();
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
                                _cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(1, 0, 0), direction);
                                break;

                            case "Right":
                                    _cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(-1, 0, 0), -direction);
                                break;

                            case "Front":
                                    _cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 0, 1), direction);
                                break;

                            case "Back":
                                    _cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 0, -1), -direction);
                                break;

                            case "Top":
                                    _cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 1, 0), direction);
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
            if (playerGridController.IsPlayerMoving())
            {
                // Don't allow the cube to move when the player is moving and vice versa
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                _cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(-1, 0, 0), 1);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                _cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(-1, 0, 0), -1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                _cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(1, 0, 0), 1);
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                _cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(1, 0, 0), -1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                _cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 0, 1), 1);
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                _cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 0, 1), -1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                _cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 0, -1), 1);
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                _cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 0, -1), -1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                _cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 1, 0), 1);
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                _cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 1, 0), -1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                _cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, -1, 0), 1);
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                _cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, -1, 0), -1);
            }
        }

        #endregion

        #endregion
    }
}
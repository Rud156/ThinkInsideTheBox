using CubeData;
using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace WorldCube
{
    [RequireComponent(typeof(CubeControllerV2))]
    public class CubeInputController : MonoBehaviour
    {
        private const string TestPing = "Close";

        [Header("Arduino")] public int readTimeout = 7;
        public bool useForcedPort = false;
        public string portString = "COM3";
        public bool disableSerialPort;

        [Header("Web Sockets")] public string ip;
        public int port;
        public bool disableSocket;

        private Socket m_client;
        private string m_lastDirection;
        private int m_lastInput;
        private bool m_inputSet;

        private CubeControllerV2 m_cubeController;
        private SerialPort m_serialPort;

        #region Unity Functions

        private void Start()
        {
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

            if (!disableSocket)
            {
                ConnectSocket();
            }
        }

        private void OnApplicationQuit()
        {
            if (m_serialPort != null)
            {
                m_serialPort.Close();
            }

            if (m_client != null)
            {
                m_client.Shutdown(SocketShutdown.Both);
                m_client.Close();
            }
        }

        private void Update()
        {
            ReadArduinoInput();
            HandleKeyboardInput();
            HandleSocketControl();
        }

        #endregion

        #region Utility Functions

        #region Sockets

        private void ConnectSocket()
        {
            try
            {
                IPAddress[] ipAddress = Dns.GetHostAddresses(ip);
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress[0], port);

                m_client = new Socket(ipAddress[0].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                m_client.BeginConnect(localEndPoint, new AsyncCallback(HandleSocketConnect), m_client);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private void HandleSocketConnect(IAsyncResult i_ar)
        {
            try
            {
                Socket client = (Socket) i_ar.AsyncState;
                client.EndConnect(i_ar);
                Debug.Log($"Connected To: {client.RemoteEndPoint}");

                Receive(m_client);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private void Receive(Socket i_client)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = i_client;

                // Begin receiving the data from the remote device.  
                i_client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the m_client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject) ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // All the data has arrived; put it in response.  
                if (state.sb.Length > 1)
                {
                    string response = state.sb.ToString();
                    state.sb.Clear();

                    HandleSocketData(response);
                }

                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void HandleSocketData(string i_input)
        {
            if (!i_input.Contains(":") || i_input.Contains(TestPing))
            {
                Debug.Log(TestPing);
                return;
            }

            string[] splitInput = i_input.Split(':');
            string sideInput = splitInput[0];
            string directionString = splitInput[1];

            Debug.Log(i_input);

            bool parseSuccess = int.TryParse(directionString, out int direction);
            if (!parseSuccess)
            {
                return;
            }

            m_lastDirection = sideInput;
            m_lastInput = direction;
            m_inputSet = true;
        }

        private void HandleSocketControl()
        {
            if (!m_inputSet)
            {
                return;
            }

            switch (m_lastDirection)
            {
                case "Left":
                case "Right":
                case "Front":
                case "Back":
                case "Top":
                {
                    switch (m_lastDirection)
                    {
                        case "Left":
                            m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(1, 0, 0), m_lastInput);
                            break;

                        case "Right":
                            m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(-1, 0, 0), -m_lastInput);
                            break;

                        case "Front":
                            m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 0, 1), m_lastInput);
                            break;

                        case "Back":
                            m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(1, 0, 0), -m_lastInput);
                            break;

                        case "Top":
                            m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 1, 0), m_lastInput);
                            break;

                        default:
                            Debug.Log("Invalid Input Sent");
                            break;
                    }
                }
                    break;

                default:
                    // Don't do anything here...
                    break;
            }

            m_inputSet = false;
        }

        #endregion

        #region Arduino

        private void ReadArduinoInput()
        {
            if (disableSerialPort)
            {
                return;
            }

            if (Dummy.Instance.IsPlayerMoving())
            {
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
            //if (m_playerGridController.IsPlayerMoving())
            //{
            //    // Don't allow the cube to move when the player is moving and vice versa
            //    return;
            //}

            // if (Dummy.Instance.IsPlayerMoving())
            // {
            //     return;
            // }

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

        #region Structs

        public class StateObject
        {
            // Client socket.  
            public Socket workSocket = null;

            // Size of receive buffer.  
            public const int BufferSize = 256;

            // Receive buffer.  
            public byte[] buffer = new byte[BufferSize];

            // Received data string.  
            public StringBuilder sb = new StringBuilder();
        }

        #endregion
    }
}
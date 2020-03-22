using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using CubeData;
using CustomCamera;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WorldCube
{
    [RequireComponent(typeof(CubeControllerV2))]
    public class CubeInputController : MonoBehaviour
    {
        private const string RotationStr = "Rotation";
        private const string PressedStr = "Pressed";
        private const string LeftStr = "Left";
        private const string RightStr = "Right";
        private const string TopStr = "Top";
        private const string BottomStr = "Bottom";
        private const string FrontStr = "Front";
        private const string BackStr = "Back";

        [Header("Components")] public CameraController cameraController;
        public DummyManualMovement playerManualMovement;

        [Header("Web Sockets")] public string ip;
        public int port;
        public bool disableSocket;

        [Header("Debug")] public Text rotationDebugText;
        public Transform debugCube;
        public bool debugActive;

        // Sockets
        private string m_testPingRegex = @"Close";
        private Regex m_captureRegex = new Regex(@"\|(.*?)\|", RegexOptions.Compiled);
        private Socket m_socketClient;
        private ConcurrentQueue<PiDataSideInput> m_piDataSideInput;
        private ConcurrentQueue<Vector3> m_piDataRotationInput;
        private ConcurrentQueue<string> m_piPressedInput;

        private CubeControllerV2 m_cubeController;

        #region Unity Functions

        private void Start()
        {
            m_cubeController = GetComponent<CubeControllerV2>();

            m_piDataSideInput = new ConcurrentQueue<PiDataSideInput>();
            m_piDataRotationInput = new ConcurrentQueue<Vector3>();
            m_piPressedInput = new ConcurrentQueue<string>();

            if (!disableSocket)
            {
                ConnectSocket();
            }

            SceneManager.sceneUnloaded += HandleSceneUnload;
        }

        private void OnApplicationQuit() => CloseSocketConnection();

        private void Update()
        {
            HandleKeyboardInput();

            HandleSocketControlSideUpdate();
            HandleSocketControlRotationUpdate();
            HandleSocketPressedInputUpdate();
        }

        #endregion

        #region External Functions

        public void CloseSocketConnection()
        {
            if (m_socketClient != null)
            {
                m_socketClient.Shutdown(SocketShutdown.Both);
                m_socketClient.Close();

                m_socketClient = null;
            }
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

                m_socketClient = new Socket(ipAddress[0].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                m_socketClient.BeginConnect(localEndPoint, new AsyncCallback(HandleSocketConnect), m_socketClient);
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

                Receive(m_socketClient);
            }
            catch (Exception e)
            {
                m_socketClient = null;
                Debug.LogError(e.Message);
            }
        }

        private void Receive(Socket i_client)
        {
            try
            {
                // Create the socketState object.  
                SocketStateObject socketState = new SocketStateObject
                {
                    workSocket = i_client
                };

                // Begin receiving the data from the remote device.  
                i_client.BeginReceive(socketState.buffer, 0, SocketStateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), socketState);
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
                // Retrieve the socketState object and the m_socketClient socket   
                // from the asynchronous socketState object.  
                SocketStateObject socketState = (SocketStateObject) ar.AsyncState;
                Socket client = socketState.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);
                socketState.sb.Append(Encoding.ASCII.GetString(socketState.buffer, 0, bytesRead));

                // All the data has arrived; put it in response.  
                if (socketState.sb.Length > 1)
                {
                    string response = socketState.sb.ToString();
                    socketState.sb.Clear();

                    HandleSocketData(response);
                }

                client.BeginReceive(socketState.buffer, 0, SocketStateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), socketState);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void HandleSocketData(string i_input)
        {
            string normalizedText = Regex.Replace(i_input, m_testPingRegex, "");
            if (string.IsNullOrEmpty(normalizedText) || string.IsNullOrWhiteSpace(normalizedText))
            {
                return;
            }

            MatchCollection matches = m_captureRegex.Matches(normalizedText);

            foreach (Match match in matches)
            {
                string value = match.Groups[1].Value;

                string[] splitInput = value.Split(':');
                string lhs = splitInput[0];
                string rhs = splitInput[1];

                switch (lhs)
                {
                    case RotationStr:
                    {
                        string[] rotations = rhs.Split(',');

                        float xValue = float.Parse(rotations[1]);
                        float yValue = float.Parse(rotations[2]);
                        float zValue = float.Parse(rotations[0]);
                        m_piDataRotationInput.Enqueue(new Vector3(xValue, zValue, yValue));
                    }
                        break;

                    case PressedStr:
                    {
                        m_piPressedInput.Enqueue(rhs);
                    }
                        break;

                    default:
                    {
                        if (Dummy.Instance.IsPlayerMoving())
                        {
                            return;
                        }

                        bool parseSuccess = int.TryParse(rhs, out int direction);
                        if (!parseSuccess)
                        {
                            return;
                        }

                        m_piDataSideInput.Enqueue(new PiDataSideInput()
                        {
                            side = lhs,
                            direction = direction
                        });
                    }
                        break;
                }
            }
        }

        private void HandleSocketControlSideUpdate()
        {
            if (Dummy.Instance.IsPlayerMoving())
            {
                return;
            }

            while (m_piDataSideInput.TryDequeue(out PiDataSideInput piDataInput))
            {
                string sideInput = piDataInput.side;
                int direction = piDataInput.direction;

                switch (sideInput)
                {
                    case LeftStr:
                        m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(1, 0, 0), direction);
                        break;

                    case RightStr:
                        m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(-1, 0, 0), -direction);
                        break;

                    case FrontStr:
                        m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 0, 1), direction);
                        break;

                    case BackStr:
                        m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 0, -1), -direction);
                        break;

                    case TopStr:
                        m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 1, 0), direction);
                        break;

                    case BottomStr:
                        m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, -1, 0), -direction);
                        break;

                    default:
                        // Don't do anything here...
                        break;
                }
            }
        }

        private void HandleSocketControlRotationUpdate()
        {
            while (m_piDataRotationInput.TryDequeue(out Vector3 rotation))
            {
                if (debugActive)
                {
                    rotationDebugText.text = $"Rotation: {rotation}";
                    debugCube.rotation = Quaternion.Euler(rotation);
                }

                cameraController.SetTargetCameraRotation(rotation);
            }
        }

        private void HandleSocketPressedInputUpdate()
        {
            while (m_piPressedInput.TryDequeue(out string o_input))
            {
                playerManualMovement.UpdateMovementDirection(o_input);
            }
        }

        #endregion

        #region Keyboard

        private void HandleKeyboardInput()
        {
            if (Dummy.Instance.IsPlayerMoving())
            {
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

        private void HandleSceneUnload(Scene current) => CloseSocketConnection();

        #endregion

        #region Singleton

        private static CubeInputController _instance;

        public static CubeInputController Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }

            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region Structs

        private struct PiDataSideInput
        {
            public int direction;
            public string side;
        }

        private class SocketStateObject
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

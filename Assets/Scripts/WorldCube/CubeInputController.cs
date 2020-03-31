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
        public float socketPollCheckTime;
        public float socketPollCheckInitialTime;
        public bool disableSocket;

        // Sockets
        private float m_currentSocketPollCheckTime;
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

            m_currentSocketPollCheckTime = socketPollCheckInitialTime;

            if (!disableSocket)
            {
                ConnectSocket();
            }

            SceneManager.sceneUnloaded += HandleSceneUnload;
        }

        private void OnApplicationQuit() => CloseSocketConnection();

        private void Update()
        {
            HandleSocketControlSideUpdate();
            HandleSocketControlRotationUpdate();
            HandleSocketPressedInputUpdate();

            UpdateSocketPollCheckTime();
        }

        #endregion

        #region External Functions

        public void CloseSocketConnection()
        {
            if (m_socketClient != null && !disableSocket)
            {
                try
                {
                    m_socketClient.Shutdown(SocketShutdown.Both);
                    m_socketClient.Close();

                    m_socketClient = null;
                }
                catch (Exception e)
                {
                    Debug.Log($"Socket Close Connection Error: {e.Message}");
                }
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
                m_socketClient = null;
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

        private bool IsSocketConnected()
        {
            if (m_socketClient == null)
            {
                return false;
            }

            try
            {
                return !(m_socketClient.Poll(1, SelectMode.SelectRead) && m_socketClient.Available == 0);
            }
            catch (SocketException)
            {
                return false;
            }
        }

        private void UpdateSocketPollCheckTime()
        {
            if (m_currentSocketPollCheckTime > 0)
            {
                m_currentSocketPollCheckTime -= Time.deltaTime;
                if (m_currentSocketPollCheckTime <= 0)
                {
                    m_currentSocketPollCheckTime = socketPollCheckTime;

                    if (!disableSocket && !IsSocketConnected())
                    {
                        Debug.LogError("Socket Disconnected. Re-connecting");
                        ConnectSocket();
                    }
                }
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
            public readonly byte[] buffer = new byte[BufferSize];

            // Received data string.  
            public readonly StringBuilder sb = new StringBuilder();
        }

        #endregion
    }
}
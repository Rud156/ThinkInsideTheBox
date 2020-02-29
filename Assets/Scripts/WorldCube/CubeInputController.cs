using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace WorldCube
{
    [RequireComponent(typeof(CubeControllerV2))]
    public class CubeInputController : MonoBehaviour
    {
        [Header("Web Sockets")] public string ip;
        public int port;
        public bool disableSocket;

        // Sockets
        private Socket m_socketClient;
        private string m_testPingRegex = @"Close";
        private ConcurrentQueue<PiDataInput> m_PiDataInput;

        private CubeControllerV2 m_cubeController;
        private SerialPort m_serialPort;

        #region Unity Functions

        private void Start()
        {
            m_cubeController = GetComponent<CubeControllerV2>();
            m_PiDataInput = new ConcurrentQueue<PiDataInput>();

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

            if (m_socketClient != null)
            {
                m_socketClient.Shutdown(SocketShutdown.Both);
                m_socketClient.Close();
            }
        }

        private void Update()
        {
            HandleKeyboardInput();
            HandleSocketControlUpdate();
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
                Debug.LogError(e.Message);
            }
        }

        private void Receive(Socket i_client)
        {
            try
            {
                // Create the socketState object.  
                SocketStateObject socketState = new SocketStateObject();
                socketState.workSocket = i_client;

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

            string[] splitInput = i_input.Split(':');
            string sideInput = splitInput[0];
            string directionString = splitInput[1];

            Debug.Log($"Input: {i_input}");

            bool parseSuccess = int.TryParse(directionString, out int direction);
            if (!parseSuccess)
            {
                return;
            }

            m_PiDataInput.Enqueue(new PiDataInput()
            {
                side = sideInput,
                direction = direction
            });
        }

        private void HandleSocketControlUpdate()
        {
            while (m_PiDataInput.TryDequeue(out PiDataInput piDataInput))
            {
                string sideInput = piDataInput.side;
                int direction = piDataInput.direction;

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
                        m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(1, 0, 0), -direction);
                        break;

                    case "Top":
                        m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 1, 0), direction);
                        break;

                    case "Bottom":
                        m_cubeController.CheckAndUpdateRotation(new CubeLayerMaskV2(0, 1, 0), -direction);
                        break;

                    default:
                        // Don't do anything here...
                        break;
                }
            }
        }

        #endregion

        #region Keyboard

        private void HandleKeyboardInput()
        {
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

        private struct PiDataInput
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
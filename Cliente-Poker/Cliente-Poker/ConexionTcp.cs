using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cliente_Poker
{
    public class ConexionTcp
    {
        #region Propiedades
        public TcpClient _tcpClient;//Cliente TCP
        public NetworkStream _networkStream;//Flujo de datos
        public Thread _lectorThread;//Thread para procesar mensajes entrantes
        public BinaryWriter _escritor;//Facilita la escritura de datos
        public BinaryReader _lector;//Facilita la lectura de datos
        private bool conected = false;//Determina si se realizo la conexión
        #endregion

        #region Delegados y Eventos
        public delegate void DataCarrier(string data);//Delegado para recibir mensajes
        public event DataCarrier OnDataRecieved;//Evento cuando se reciben mensajes
        public delegate void DisconnectNotify();//Delegado que notifica cuando se desconecta el jugador
        public event DisconnectNotify OnDisconnect;//Evento cuando se desconecta el jugador
        #endregion

        #region Métodos
        public bool Conectar(string ipadress)
        {//Método que intenta establecer una conexión con el servidor
            try
            {
                _tcpClient = new TcpClient();
                _tcpClient.Connect(IPAddress.Parse(ipadress), 6000);
                _networkStream = _tcpClient.GetStream();
                _escritor = new BinaryWriter(_networkStream);
                _lectorThread = new Thread(Escuchar);
                _lectorThread.Start();
                conected = true;
                return true;
            }
            catch (Exception)
            {
                //MessageBox.Show("Imposible conectarse con el servidor\nIntente mas tarde", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }//Fin método Conectar

        private void Escuchar()
        {//Método que se mantiene a la espera de mensajes, hasta que se cierra la conexion
            _lector = new BinaryReader(_networkStream);
            while (conected)
            {
                try
                {
                    if (OnDataRecieved != null)
                    {
                        var message = _lector.ReadString();
                        OnDataRecieved(message);
                    }
                    //buffer.Clear();
                }
                catch (Exception e)
                {
                    if (e.HResult.ToString() == "-2146232800")
                    {
                        //MessageBox.Show("Se perdio la conexión con el servidor,\nEl cliente se cerrará.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //System.Diagnostics.Process.Start(Application.ExecutablePath);
                        Environment.Exit(Environment.ExitCode);
                    }
                    else
                    {
                        //MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    break;
                }
            }
            OnDisconnect?.Invoke();
        }//Fin método Escuchar

        private void Escribir(string mensaje)
        {//Método que facilita el envio de mensajes al servidor
            try
            {
                _escritor.Write(mensaje);
                _escritor.Flush();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//Fin método Escribir

        public void Enviar(Mensaje mensaje)
        {//Método que utiliza el método Escribir para enviar un mensaje especifico al servidor
            Escribir(mensaje.Completo());
        }//Fin método Enviar

        public bool Desconectar()
        {//Cierra la conexión
            _tcpClient.Close();
            conected = false;
            return true;
        }//Fin método Desconectar
        #endregion
    }//Fin clase ConexionTcp.cs
}

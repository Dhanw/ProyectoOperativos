using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Servidor_BlackJack
{
    public class Jugador
    {
        #region Propiedades
        internal Socket Conexion;//Socket para aceptar una conexion
        public TcpClient TcpClient;//Cliente TCP
        public BinaryReader Lector;//Facilita la lectura de datos
        public BinaryWriter Escritor;//Facilita la escritura de datos
        public NetworkStream SocketStream;//Flujo de datos
        public Thread ThreadLector;//Thread para procesar mensaje entrantes
        public int NumeroJugador;//Determina el número del jugador
        #endregion
        
        #region Constructor
        public Jugador(Socket socket, int njugador)
        {//Constructor público que recibe un cliente TCP y el número del jugador
            Conexion = socket;
            SocketStream = new NetworkStream(Conexion);
            Lector = new BinaryReader(SocketStream);
            Escritor = new BinaryWriter(SocketStream);
            NumeroJugador = njugador;
        }//Fin del constructor
        #endregion

        #region Métodos
        public void Escribir(string mensaje)
        {//Método que se encarga de enviar un mensaje
            try
            {
                Escritor.Write(mensaje);
                Escritor.Flush();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//Fin método Escribir

        public void Enviar(Mensaje mensaje)
        {//Método que utiliza el metodo Escribir, y envia un mensaje en su formato desado
            Escribir(mensaje.Completo());
        }//Fin método Enviar 
        #endregion
    }//Fin clase Jugador.cs
}

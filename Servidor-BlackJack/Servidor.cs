using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using Servidor_BlackJack.Properties;

namespace Servidor_BlackJack
{
    public partial class Servidor : Form
    {
        #region Propiedades
        private TcpListener _tcpListener;//TCPListener
        private Thread _aceptarThread;//Thread para aceptar jugadores
        private List<Jugador> _jugadores = new List<Jugador>();//Lista de jugadores
        private int _njugador = 0;//Número de jugador
        private int _contador = 0;//Cantidad de jugadores 
        private int _total1 = 0;//Puntaje final del jugador 1
        private int _total2 = 0;//Puntaje final del jugador 2
        private Stack<Carta> _deck = new Stack<Carta>();//Stack que representa la baraja de cartas
        private int _maxJugadores = 7; //Cantidad maxima de jugadores permitidos
        private List<int> totals = new List<int> { 0, 0, 0, 0, 0, 0, 0 };
        #endregion

        #region Delegados y Eventos
        public delegate void ClientCarrier(Jugador conexion);//Delegado para jugadores
        public event ClientCarrier OnClientConnected;//Controla cuando un jugador se conecta
        public event ClientCarrier OnClientDisconnected;//Controla cuando un jugador se desconecta
        public delegate void DataRecieved(Jugador conexion, string mensaje);//Delegado para recibir mensaje
        public event DataRecieved OnDataRecieved; //Controla cuando llega un mensaje
        #endregion

        #region Constructor
        public Servidor()
        {
            InitializeComponent();
            //Icon = Resources.UNAICON;
            //Control de eventos
            OnDataRecieved += MensajeRecibido;
            OnClientConnected += JugadorConectado;
            OnClientDisconnected += JugadorDesconectado;
        }
        #endregion

        #region Métodos
        private void Servidor_Load(object sender, EventArgs e)
        {//Método que controla la primera vez que el formulario inicia
            try
            {
                _tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 6000);
                _tcpListener.Start();
                _aceptarThread = new Thread(AceptarJugadores);
                _aceptarThread.Start();
                txtLog.Text = string.Format("{0} >>> Bienvenido...", DateTime.Now);
                CrearDeck();
                txtLog.Text += string.Format("\r\n{0} >>> Esperando jugadores...", DateTime.Now);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//Fin método Servidor_Load

        private void Servidor_FormClosing(object sender, FormClosingEventArgs e)
        {//Método que controla cuando el servidor se cierra
            Environment.Exit(Environment.ExitCode);//Cierra todas las conexiones y referencias al programa
        }//Fin método Servidor_FormClosing

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {//Controla el evento Click en el botón Acerca de...
            var s = new Acerca();
            s.ShowDialog();
        }//Fin acercaDeToolStripMEnuItem_Click

        private void CrearDeck()
        {//Genera 52 cartas y las introduce en el Stack _deck
            _deck = new Stack<Carta>();
            Random rnd = new Random();
            for (int i = 0; i < 52; i++)
            {
                bool nueva = true;
                while (nueva)
                {
                    string fam = "";
                    switch (rnd.Next(1, 5))
                    {//Determina la familia de la carta
                        case 1:
                            fam = "picas";
                            break;
                        case 2:
                            fam = "corazones";
                            break;
                        case 3:
                            fam = "diamantes";
                            break;
                        case 4:
                            fam = "treboles";
                            break;
                    }
                    Carta s = new Carta(fam, rnd.Next(1, 14).ToString());
                    if (!_deck.Contains(s))//Controla que no existan cartas repetidas
                    {
                        _deck.Push(s);
                        nueva = false;
                    }
                    Invoke(new Action(() => tcpBaraja.Value = _deck.Count));
                    Invoke(new Action(() => lblBaraja.Text = _deck.Count.ToString()));
                }
            }
            Invoke(new Action(() => txtLog.Text += string.Format("\r\n{0} >>> Se genero una nueva baraja...", DateTime.Now)));
        }//Fin método CrearDeck

        private void AceptarJugadores()
        {//Método que se mantiene siempre activo en espera de jugadores
            while (true)
            {
                try
                {
                    //TcpClient conexion = _tcpListener.AcceptTcpClient();
                    //Jugador jugador = new Jugador(conexion, _njugador)
                    Jugador jugador = new Jugador(_tcpListener.AcceptSocket(), _njugador)
                    {
                        ThreadLector = new Thread(LeerMensaje)
                    };
                    jugador.ThreadLector.Start(jugador);
                    OnClientConnected?.Invoke(jugador);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }//Fin método AceptarJugadores

        private void LeerMensaje(object jugador)
        {//Método que recibe mensaje
            Jugador _jugador = jugador as Jugador;
            do
            {
                try
                {
                    if (_jugador == null)
                        break;
                    if (OnDataRecieved != null)
                    {
                        var message = _jugador.Lector.ReadString();
                        OnDataRecieved(_jugador, message);
                    }
                }
                catch (Exception e)
                {
                    if (e.HResult.ToString() != "-2146232800")
                        MessageBox.Show(e.Message);
                    break;
                }
            } while (true);

            OnClientDisconnected?.Invoke(_jugador);
        }//Fiin método LeerMensaje

        private void MensajeRecibido(Jugador jugador, string mensaje)
        {//Método que realiza acciones dependiendo del mensaje recibido
            Mensaje _mensaje = new Mensaje(mensaje);
            string s = "";
            int jugadorActual = Convert.ToInt32(_mensaje.Titulo);
            switch (_mensaje.Contenido)
            {
                case "NewCard"://En caso de solicitar una nueva carta se recibe NewCard
                    #region El jugador solicita una carta adicional
                    s = string.Format("\r\n{0} >>> Jugador {1} <<< {2}", DateTime.Now, jugadorActual, "Solicita una carta");
                    Invoke(new Action(() => txtLog.Text += s));
                    Jugar(jugadorActual, false);
                    break;
                #endregion
                default://En caso terminar el turno se recibe el EndTurn_{0} donde 0 representa el puntaje final
                    #region El jugador termina su turno

                    
                    int index = _mensaje.Contenido.IndexOf("_", StringComparison.Ordinal);
                    string s1 = _mensaje.Contenido.Substring(0, index);
                    string s2 = _mensaje.Contenido.Substring(s1.Length + 1);
                    s = string.Format("\r\n{0} >>> Jugador {1} <<< Termina su turno con {2} puntos", DateTime.Now, jugadorActual, s2);
                    Invoke(new Action(() => txtLog.Text += s));

                    for (int i = 0; i < _maxJugadores; i++)
                    {
                        if (i != jugadorActual - 1)
                        {
                            _jugadores[i].Enviar(new Mensaje("jugador" + jugadorActual, s2));
                            int _totaltmp = Convert.ToInt32(s2);
                            if (_totaltmp > 21)
                                _totaltmp = 0;
                            totals[jugadorActual - 1] = _totaltmp;
                        }
                    }
                   // MessageBox.Show(jugadorActual + " -  " + s2);
                    if (jugadorActual == _maxJugadores)
                    {
                        var IndexGanador = 0;
                        for (int i = 0; i < _maxJugadores; i++)
                        {
                            if (totals[IndexGanador] < totals[i]) IndexGanador = i;
                        }


                        s = string.Format("\r\n{0} >>> Jugador {1} >>> Es el ganador ", DateTime.Now, IndexGanador + 1);
                        Invoke(new Action(() => txtLog.Text += s));
                        for (int i = 0; i < _maxJugadores; i++)
                        {
                            if (i == IndexGanador)
                            {
                                _jugadores[i].Enviar(new Mensaje("premio", "ganador"));
                            }
                            else
                            {
                                _jugadores[i].Enviar(new Mensaje("premio", "perdedor"));
                            }
                        }





                        //if (_total1 > _total2)
                        //{
                        //    s = string.Format("\r\n{0} >>> Jugador {1} >>> Es el ganador ", DateTime.Now, "1");
                        //    Invoke(new Action(() => txtLog.Text += s));
                        //    _jugadores[0].Enviar(new Mensaje("premio", "ganador"));
                        //    _jugadores[1].Enviar(new Mensaje("premio", "perdedor"));
                        //}
                        //if (_total1 < _total2)
                        //{
                        //    s = string.Format("\r\n{0} >>> Jugador {1} >>> Es el ganador ", DateTime.Now, "2");
                        //    Invoke(new Action(() => txtLog.Text += s));
                        //    _jugadores[0].Enviar(new Mensaje("premio", "perdedor"));
                        //    _jugadores[1].Enviar(new Mensaje("premio", "ganador"));
                        //}
                        //if (_total1 == _total2)
                        //{
                        //    s = string.Format("\r\n{0} >>> Empate", DateTime.Now);
                        //    Invoke(new Action(() => txtLog.Text += s));
                        //    _jugadores[0].Enviar(new Mensaje("premio", "empate"));
                        //    _jugadores[1].Enviar(new Mensaje("premio", "empate"));
                        //}
                    }
                    TurnoJugador(jugadorActual);

                    break;
                    #endregion
            }
        }//Fin MensajeRecibido

        private void JugadorConectado(Jugador jugador)
        {//Método que controla el evento de un jugador contactado
            lock (_jugadores)
            {//Recibe jugadores y los agrega a la lista de jugadores
                if (!_jugadores.Contains(jugador))
                {
                    _jugadores.Add(jugador);
                    _njugador++;
                }
                Invoke(new Action(() => lblJugadores.Text = string.Format("Jugadores conectados: {0}", _jugadores.Count)));
                Invoke(new Action(() => txtLog.Text += string.Format("\r\n{0} >>> Jugador {1} conectado", DateTime.Now, _njugador)));
                jugador.Enviar(new Mensaje("jugador", _jugadores.Count.ToString()));
                switch (_jugadores.Count)
                {
                    case 1:
                        jugador.Enviar(new Mensaje("esperar", "Esperando por otro jugador"));
                        break;
                    case 2:
                        jugador.Enviar(new Mensaje("esperar", "Esperando por otro jugador"));
                        break;
                    case 3:
                        jugador.Enviar(new Mensaje("esperar", "Esperando por otro jugador"));
                        break;
                    case 4:
                        jugador.Enviar(new Mensaje("esperar", "Esperando por otro jugador"));
                        break;
                    case 5:
                        jugador.Enviar(new Mensaje("esperar", "Esperando por otro jugador"));
                        break;
                    case 6:
                        jugador.Enviar(new Mensaje("esperar", "Esperando por otro jugador"));
                        break;
                    case 7:
                        TurnoJugador(0);
                        Jugar(0, true);
                        break;
                }
            }
        }//Fin método JugadorConectado

        private void JugadorDesconectado(Jugador jugador)
        {//Método que controla el evento cuando un jugador se desconecta
            lock (_jugadores)
            {//Determina si un jugador se desconecta y lo remueve de la lista de jugadores
                if (_jugadores.Contains(jugador))
                {
                    int jugadorIndex = _jugadores.IndexOf(jugador);
                    _jugadores.RemoveAt(jugadorIndex);
                }
                Invoke(new Action(() => lblJugadores.Text = string.Format("Jugadores conectados: {0}", _jugadores.Count)));
                Invoke(new Action(() => txtLog.Text += string.Format("\r\n{0} >>> Jugador {1} desconectado", DateTime.Now, _njugador)));
                #region Un jugador abandono la partida
                if (_jugadores.Count != 0)
                {

                    switch (_njugador)
                    {
                        case 1:
                            _jugadores[1].Enviar(new Mensaje("premio", "ganador"));
                            break;
                        case 2:
                            _jugadores[0].Enviar(new Mensaje("premio", "ganador"));
                            break;
                    }
                }
                #endregion
                _njugador--;
                #region Sin jugadores, reiniciar
                if (_jugadores.Count == 0)
                {
                    _contador = 0;
                    _total1 = 0;
                    _total2 = 0;
                    //Invoke(new Action(() => txtLog.Text = ""));
                    totals = new List<int> { 0, 0, 0, 0, 0, 0, 0 };
                    Invoke(new Action(() => txtLog.Text += string.Format("\r\n{0} >>> Bienvenido...", DateTime.Now)));
                    Invoke(new Action(() => CrearDeck()));
                    Invoke(new Action(() => txtLog.Text += string.Format("\r\n{0} >>> Esperando jugadores...", DateTime.Now)));
                }
                #endregion
            }
        }//Fin método JugadorDesconectado

        private void TurnoJugador(int njugador)
        {//Método que determina el turno de los jugadores
            for (int i = 0; i < _maxJugadores; i++)
            {
                if (i != njugador)
                {
                    _jugadores[i].Enviar(new Mensaje("turno", "false"));
                    _jugadores[i].Enviar(new Mensaje("esperar", "Por favor espere su turno."));
                }
                else
                {
                    _jugadores[i].Enviar(new Mensaje("turno", "true"));
                    _jugadores[i].Enviar(new Mensaje("esperar", "Es tu turno, adelante."));
                }
            }
        }//Fin método TurnoJugador

        private void Jugar(int jugador, bool inicio)
        {//Método que envia las cartas a los jugadores, 2 cartas iniciales y luego 1 carta a solicitud del jugador
            if (inicio)
            {
                for (int i = 0; i < _maxJugadores; i++)
                {
                    Invoke(new Action(() => txtLog.Text += string.Format("\r\n{0} >>> Jugador {1} >>> {2}", DateTime.Now, i + 1, "Mano Inicial")));
                    string s1 = _deck.Pop().StringCarta();
                    _jugadores[i].Enviar(new Mensaje("carta", s1));
                    Invoke(new Action(() => txtLog.Text += string.Format("\r\n{0} >>> Jugador {1} >>> {2}", DateTime.Now, i + 1, s1)));
                    string s2 = _deck.Pop().StringCarta();
                    _jugadores[i].Enviar(new Mensaje("carta", s2));
                    Invoke(new Action(() => txtLog.Text += string.Format("\r\n{0} >>> Jugador {1} >>> {2}", DateTime.Now, i + 1, s2)));
                }
            }
            else
            {
                string s1 = _deck.Pop().StringCarta();
                _jugadores[jugador - 1].Enviar(new Mensaje("carta", s1));
                Invoke(new Action(() => txtLog.Text += string.Format("\r\n{0} >>> Jugador {1} >>> {2}", DateTime.Now, jugador, s1)));
            }
            Invoke(new Action(() => tcpBaraja.Value = _deck.Count));
            Invoke(new Action(() => lblBaraja.Text = _deck.Count.ToString()));
        }//Fin método Jugar
        #endregion

    }//Fin formulario Servidor.cs
}

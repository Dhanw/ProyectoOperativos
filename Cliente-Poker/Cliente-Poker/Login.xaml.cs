using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cliente_Poker
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class Login : Window
    {

        #region Propiedades
        public ConexionTcp Conexion = new ConexionTcp();//Nueva conexion
        public int Numerojugador;//Número del jugador
        private Stack<Carta> _deck = new Stack<Carta>();//Baraja de cartas 
        private string IPAddress = "127.0.0.1";
        private object colorManager;
        private readonly Usuario defaultUser = new Usuario { Username = "local", Password = "local" };

        #endregion
        public Login()
        {
            InitializeComponent();
        }

        private bool Conectar()
        {//Establece una conexión con el servidor
            _deck.Clear();
            if (Conexion.Conectar(IPAddress))
            {
                //lblEstadoconexion.Text = "Conectado";
                //lblEstadoconexion.ForeColor = Color.LightGreen;
                //btConectar.Enabled = false;
                return true;
            }
            return false;
        }//Fin método Conectar

        private void Login_Click(object sender, RoutedEventArgs e)
        {

            IPAddress = (chLocalhost.IsChecked == true) ? "127.0.0.1" : txtIPAdress.Text.Replace(",", ".");
            Usuario loginUser = new Usuario { Username = CampoUsuario.Text, Password = CampoContrasena.Text };
            if (loginUser.Equals(defaultUser))
            {
                if (Conectar())
                {
                    string pass = CampoContrasena.Text;
                    string user = CampoUsuario.Text;
                    if (user != "" && pass != "")
                    {
                        Principal principal = new Principal();
                        principal.Show();
                        this.Close();
                    }
                    else
                    {
                        errormessage.Text = "Jugador no encontrado!\n Vuelve a ingresar los datos!";
                    }
                    //panelLogin.Visible = false;
                    //panelGame.Visible = true;
                    //BackgroundImage = Resources.BlackJackTable;
                }
            }
            else
            {
                //MessageBox.Show("El nombre de usuario o constraseña son incorrectos.\n Intentelo de nuevo", "Error",
                //    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            //string pass = CampoContrasena.Text;
            //string user = CampoUsuario.Text;
            //if (user != "" && pass != "") {
            //    Principal principal = new Principal();
            //    principal.Show();
            //    this.Close();
            //}
            //else {
            //    errormessage.Text = "Jugador no encontrado!\n Vuelve a ingresar los datos!";
            //}


        }
    }
}

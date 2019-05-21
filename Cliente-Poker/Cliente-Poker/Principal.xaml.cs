using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cliente_Poker
{
    /// <summary>
    /// Lógica de interacción para Principal.xaml
    /// </summary>
    public partial class Principal : Window
    {
        public Principal()
        {
            InitializeComponent();
        }

        private void Regresar_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void Raise_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int saldoActual = Convert.ToInt32(CampoSaldoActual.Text);
                int Raise = Convert.ToInt32(CampoRaise.Text);
                int nuevoBote = saldoActual += Raise;
                CampoBote.Text = Convert.ToString(nuevoBote);
                CampoRaise.Text = "";
            }
            catch (Exception ex) { }
        }
    }
}

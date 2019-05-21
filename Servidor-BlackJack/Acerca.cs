using System;
using System.Windows.Forms;
using Servidor_BlackJack.Properties;

namespace Servidor_BlackJack
{
    public partial class Acerca : Form
    {
        #region Constructor
        public Acerca()
        {
            InitializeComponent();
        }
        #endregion
      
        #region Métodos
        private void Acerca_Load(object sender, EventArgs e)
        {//Carga la información en los controles del formulario
            pictureBox1.Image = Resources.Blackjack_ICON;
            lblTitulo.Text = string.Format("{0}\n{1}\n{2}\n{3}\n", "Sistemas Operativos", "Proyecto Final",
                "Profesor Jose Pablo Calvo", "BlackJack Game");
            lblDescripcion.Text = "Software 'servidor' que envia y recibe\n" +
                                  "instrucciones del software 'cliente'.\n" +
                                  "El objetivo de este software es\n" +
                                  "permiterle a un jugador enviar cartas\n" +
                                  "al 'cliente' hasta que decida que\n" +
                                  "quiere terminar su turno, o hasta que\n" +
                                  "la suma de sus cartas sea mayor a 21.\n" +
                                  "El objetivo del juego es que los jugadores\n" +
                                  "tengan cartas que sumen 21 o termine\n" +
                                  "su turno lo mascerca posible de 21.\n" +
                                  "Si ambos jugadores tienen 21 o si ambos\n" +
                                  "se pasan de 21, el juego terminará en\n" +
                                  "empate.";

        }//Fin método Acerca_Load
        #endregion
    }//Fin formulario Acerca.cs
}

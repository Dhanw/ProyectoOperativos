using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_Poker
{
    public class Mensaje
    {
        #region Propiedades
        public string Titulo { get; set; }//Representa el titulo o comando del mensaje
        public string Contenido { get; set; }//Representa el contenido del mensaje
        #endregion

        #region Constructor
        public Mensaje()
        {

        }//Fin constructor vacio

        public Mensaje(string completo)
        {//Construcotr que recibe un string único y separa el titulo del contenido
            int index = completo.IndexOf(":", StringComparison.Ordinal);
            Titulo = completo.Substring(0, index);
            Contenido = completo.Substring(Titulo.Length + 1);
        }//Fin constructor de string único

        public Mensaje(string _titulo, string _contenido)
        {//Constructor que recibe dos string y los establece como propiedades de la clase
            Titulo = _titulo;
            Contenido = _contenido;
        }//Fin constructor que recibe dos string
        #endregion

        #region Métodos
        public string Completo()
        {//Método que retona las propiedades de la clase en un solo string
            return string.Format("{0}:{1}", Titulo, Contenido);
        }//Fin método Completo
        #endregion

    }//Fin clase Mensaje.cs
}

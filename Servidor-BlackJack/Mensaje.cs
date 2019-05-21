
using System;

namespace Servidor_BlackJack
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
        {//Constructor que recibe un string único y separa el titulo del contenido.
            int index = completo.IndexOf(":", StringComparison.Ordinal);
            Titulo = completo.Substring(0, index);
            Contenido = completo.Substring(Titulo.Length + 1);
        }//Fin costructor de string único

        public Mensaje(string titulo, string contenido)
        {//Constructor que recibe dos string y los establece como propiedades de la clase
            this.Titulo = titulo;
            this.Contenido = contenido;
        }//Fin constructor que recibe dos string
        #endregion

        #region Métodos
        public string Completo()
        {//Método que retorna las propiedades de la clase en un solo string
            return string.Format("{0}:{1}", Titulo, Contenido);
        } //Fin método Completo
        #endregion

    }//Fin Clase Mensaje.cs
}

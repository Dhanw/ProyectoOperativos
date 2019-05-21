using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_Poker
{
    public class Carta : IEquatable<Carta>//Usa la interface IEquatable para comparar cartas
    {
        #region Propiedades
        private string _valor { get; }//Representa el valor de la carta (1 hasta 13)
        private string _familia { get; }//Representa la famia de la carta (diamantes, corazones, treboles o picas)
        #endregion

        #region Constructor
        public Carta()
        {

        }//Fin del constructor vacio

        public Carta(string familia, string valor)
        {//Recibe dos string y los asigna a variables privadas
            _valor = valor;
            _familia = familia;
        }//Fin del costructor de dos string

        public Carta(string _carta)
        {//Recibe un string, lo separa y lo asigna a variables privadas
            int index = _carta.IndexOf("_", StringComparison.Ordinal);
            _familia = _carta.Substring(0, index);
            _valor = _carta.Substring(_familia.Length + 1);
        }//Fin del costructor de un string
        #endregion

        #region Métodos
        public string StringCarta()
        {//Devuelve un string con un formato determinado
            string _string = string.Format("{0}_{1}", _familia, _valor);
            return _string;
        }//Fin método StringCarta

        public string NombreCarta()
        {//Determina el nombre de las cartas AS,K,Q y J
            string s1 = _valor;
            if (_valor == "1")
                s1 = "AS";
            if (_valor == "11")
                s1 = "J";
            if (_valor == "12")
                s1 = "Q";
            if (_valor == "13")
                s1 = "K";
            return string.Format("{0} de {1}", s1, _familia);
        }//Fin método NombreCarta

        public int valorSumativo(int total)
        {//Determina el valor númerico de la carta para la suma de los puntos
            int x = Convert.ToInt32(_valor);//Convierte el string _valor en int
            if (x == 1)
            {
                if (total < 21 && total + 11 <= 21)//Si el total de los puntos es menor a 21, y si la suma de 11 + el total 
                                                   //de los puntos no supera 21 el valor del AS será 11
                {
                    x = 11;
                }
            }
            else//Si la carta es J,Q,K el valor sumativo sera de 10
            {
                if (x > 10)
                {
                    x = 10;
                }
            }
            return x;
        }//Fin método valorSumativo

        public string Valor()
        {//Método get de la variable privada _valor
            return _valor;
        }//Fin método Varlor

        public bool Equals(Carta otraCarta)
        {//Override del método Equals para comprar dos cartas
            return _familia == otraCarta._familia &&
                   this._valor == otraCarta._valor;

        }//Fin método Equals 
        #endregion
    }//Fin clase Carta.cs
}

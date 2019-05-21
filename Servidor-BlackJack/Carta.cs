using System;

namespace Servidor_BlackJack
{
    public class Carta : IEquatable<Carta>//Usa la interface IEquatable para comparar cartas
    {
        #region Propiedades
        private string Valor { get; } //Representa el valor de la carta (1 hasta 13)
        private string Familia { get; } //Representa la familia de la carta (diamantes, corazones, treboles o picas)
        #endregion

        #region Costructor
        public Carta(string familia, string valor)
        {
            Valor = valor;
            Familia = familia;
        }//Fin constructor
        #endregion

        #region Métodos
        public string StringCarta()
        {//Retorna un string con la familia y el valor de la carta en un formato especifico
            string _string = string.Format("{0}_{1}", Familia, Valor);
            return _string;
        }//Fin método StringCarta

        public bool Equals(Carta otraCarta)
        {//Override del método Equals para comprar dos cartas
            return Familia == otraCarta.Familia && Valor == otraCarta.Valor;

        }//Fin método Equals 
        #endregion

    }//Fin Clase Carta.cs
}

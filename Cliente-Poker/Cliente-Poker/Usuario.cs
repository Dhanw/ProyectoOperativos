using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente_Poker
{
    public class Usuario
    {
        public string Username { get; set; }
        public string Password { get; set; }

        #region Sobreescribe la funcion Equals()
        public override bool Equals(System.Object obj)
        {
            if (obj == null)
                return false;

            Usuario p = obj as Usuario;
            if ((System.Object)p == null)
                return false;

            return (Username == p.Username) && (Password == p.Password);
        }

        public bool Equals(Usuario p)
        {
            if ((object)p == null)
                return false;

            return (Username == p.Username) && (Password == p.Password);
        }

        public override int GetHashCode()
        {
            return Username.GetHashCode() ^ Password.GetHashCode();
        }
        #endregion
    }
}

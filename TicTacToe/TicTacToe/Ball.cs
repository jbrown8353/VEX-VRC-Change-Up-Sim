using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class Ball
    {
        #region Variables

        private string owner;

        #endregion

        #region Properties
        #endregion

        #region Constructor
        public Ball(string owner)
        {
            this.owner = owner;
        }
        #endregion

        #region Methods

        public string GetOwner()
        {
            return owner;
        }

        

        #endregion
    }
}

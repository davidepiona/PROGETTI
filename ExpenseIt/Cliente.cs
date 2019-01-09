using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseIt
{
    public class Cliente
    {
        private String nomeCliente;
        private String suffisso;
        private int maxId;
        private int lastId;

        public Cliente(string nomeCliente, string suffisso, int lastId, int maxId)
        {
            this.nomeCliente = nomeCliente;
            this.suffisso = suffisso;
            this.maxId = maxId;
            this.lastId = lastId;
        }

        public String getNomeCliente()
        {
            return nomeCliente;
        }

        public String getSuffisso()
        {
            return suffisso;
        }
        public int getMaxId()
        {
            return maxId;
        }
        public int getlastId()
        {
            return lastId;
        }

        public void setMaxId(int maxId)
        {
            this.maxId = maxId;
        }

        public void setLastId(int lastId)
        {
            this.lastId = lastId;
        }

       

    }
}

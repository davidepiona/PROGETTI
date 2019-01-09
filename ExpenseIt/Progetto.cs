using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseIt
{
    public class Progetto
    {
        public string numero { get; set; }
        public string nome { get; set; }
        public string tipoPLC { get; set; }
        public string tipoOP { get; set; }
        public string data { get; set; }
        public string modifica { get; set; }
        public bool? sync { get; set; }

        public Progetto(string numero, string nome, string tipoPCL, string tipoOP, string data)
        {
            this.numero = numero;
            this.nome = nome;
            this.tipoPLC = tipoPCL;
            this.tipoOP = tipoOP;
            this.data = data;
            modifica = null;
            this.sync = false;
            
        }


        public override string ToString()
        {
            return numero;
        }

        public string ToName()
        {
            return numero + "\t" + nome + "\t\t" + tipoPLC;
        }

    }
}

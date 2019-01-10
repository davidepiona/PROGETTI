using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseIt
{
    public class Progetto
    {
        public int numero { get; set; }
        public string nome { get; set; }
        public string tipoPLC { get; set; }
        public string tipoOP { get; set; }
        public string data { get; set; }
        public string modifica { get; set; }
        public bool? sync { get; set; }
        public string cliente { get; set; }
        public string sigla { get; set; }

        public Progetto(int numero, string nome, string tipoPCL, string tipoOP, string data, string cliente)
        {
            this.numero = numero;
            this.nome = nome;
            this.tipoPLC = tipoPCL;
            this.tipoOP = tipoOP;
            this.data = data;
            modifica = null;
            this.sync = false;
            this.cliente = cliente; 
            sigla = cliente + numero;
            
        }


        public override string ToString()
        {
            return cliente+numero;
        }

        public int getNumProject()
        {
            return numero;
        }

        public string ToName()
        {
            return cliente+ numero + "\t" + nome + "\t\t" + tipoPLC;
        }

    }
}

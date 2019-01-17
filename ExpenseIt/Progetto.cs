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
        public string nomeCliente { get; set; }
        public string suffissoCliente { get; set; }
        public string sigla { get; set; }

        public Progetto(int numero, string nome, string tipoPCL, string tipoOP, string data, string nomeCliente, string suffissoCliente)
        {
            this.numero = numero;
            this.nome = nome;
            this.tipoPLC = tipoPCL;
            this.tipoOP = tipoOP;
            this.data = data;
            modifica = null;
            this.sync = false;
            this.nomeCliente = nomeCliente; 
            sigla = suffissoCliente + numero;
            
        }


        public override string ToString()
        {
            return nomeCliente+numero;
        }

        public int getNumProject()
        {
            return numero;
        }

        public string ToName()
        {
            return nomeCliente+ numero + "\t" + nome + "\t\t" + tipoPLC;
        }

        public override bool Equals(object obj)
        {
            var progetto = obj as Progetto;
            return progetto != null &&
                   nome == progetto.nome &&
                   tipoPLC == progetto.tipoPLC &&
                   tipoOP == progetto.tipoOP &&
                   data == progetto.data &&
                   sigla == progetto.sigla;
        }
    }
}

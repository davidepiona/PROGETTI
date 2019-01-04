using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseIt
{
    class Progetto
    {
        private String numero;
        private String nome;
        private String tipoPCL;
        private String tipoOP;
        private String data;

        public Progetto(string numero, string nome, string tipoPCL, string tipoOP, string data)
        {
            this.numero = numero;
            this.nome = nome;
            this.tipoPCL = tipoPCL;
            this.tipoOP = tipoOP;
            this.data = data;
        }

        public string getNumero()
        {
            return numero;
        }
        public string getNome()
        {
            return nome;
        }
        public string getTipoPLC()
        {
            return tipoPCL;
        }
        public string getTipoOp()
        {
            return tipoOP;
        }
        public string getData()
        {
            return data;
        }

        public override string ToString()
        {
            return "N: \t\t"+numero+"\n Nome: \t\t"+nome+"\n TipoPLC: \t"+tipoPCL+"\n TipoOP: \t"+tipoOP+"\n Data: \t\t"+data;
        }

        public string ToName()
        {
            return numero + "\t" + nome + "\t\t" + tipoPCL;
        }
    }
}

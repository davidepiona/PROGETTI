using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseIt
{
    public class Progetto
    {
        /// <summary>
        /// Classe Progetto composta dagli elementi presenti nei file *CLIENTE*.csv
        /// Getter e Setter classici. 
        /// Sono presenti alcuni elementi aggiuntivi: 
        /// - modifica: ultima modifica della cartella relativa al progetto; in seguito ad alcune operazioni viene trovata e aggiunta agli oggetti
        /// - sync: valore che indica lo stato di aggiornamento tra copia locale e copia in DATIsync
        /// - sigla: necessario per aver la prima colonna della tabella compatta (concatenazione di suffisso e numero)
        /// </summary>
        public int numero { get; set; }
        public string nome { get; set; }
        public string tipoPLC { get; set; }
        public string tipoOP { get; set; }
        public string data { get; set; }
        public string modifica { get; set; }
        public bool? sync { get; set; }
        public string nomeCliente { get; set; }
        public string sigla { get; set; }

        /// <summary>
        /// Inizializza gli attributi coi valori ricevuto e inizializza modifica a null, sync a false e sigla come concatenazione di suffisso e numero.
        /// </summary>
        public Progetto(int numero, string nome, string tipoPLC, string tipoOP, string data, string nomeCliente, string suffissoCliente)
        {
            this.numero = numero;
            this.nome = nome;
            this.tipoPLC = tipoPLC;
            this.tipoOP = tipoOP;
            this.data = data;
            this.nomeCliente = nomeCliente;
            modifica = null;
            this.sync = false;
            sigla = suffissoCliente + numero;
        }

        /// <summary>
        /// Utilizzato per cercare i progetti nella lista
        /// </summary>
        /// <returns></returns>
        public string ToName()
        {
            return nomeCliente+ numero + "\t" + nome + "\t\t" + tipoPLC;
        }
    }
}

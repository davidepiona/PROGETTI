using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATA
{
    public class Confronto
    {
        public Progetto prog1 { get; set; }
        public Progetto prog2 { get; set; }
        public int id1 { get; set; }
        public string nome1 { get; set; }
        public string dataCreazione1 { get; set; }
        public bool scelto1 { get; set; }
        public int id2 { get; set; }
        public string nome2 { get; set; }
        public string dataCreazione2 { get; set; }
        public bool scelto2 { get; set; }
        

        public Confronto(Progetto prog1, Progetto prog2)
        {

            this.prog1 = prog1;
            this.prog2 = prog2;
            if(prog1 == null)
            {
                prog1 = new Progetto(0, "-", "-", "-", "-", "-", "-");
            }
            if (prog2 == null)
            {
                prog2 = new Progetto(0, "-", "-", "-", "-", "-", "-");
            }

            this.id1 = prog1.numero;
            this.nome1 = prog1.nome ?? throw new ArgumentNullException(nameof(nome1));
            this.dataCreazione1 = prog1.data ?? throw new ArgumentNullException(nameof(dataCreazione1));
            this.scelto1 = false;
            this.id2 = prog2.numero;
            this.nome2 = prog2.nome ?? throw new ArgumentNullException(nameof(nome2));
            this.dataCreazione2 = prog2.data ?? throw new ArgumentNullException(nameof(dataCreazione2));
            this.scelto2 = false;
        }
    }
}

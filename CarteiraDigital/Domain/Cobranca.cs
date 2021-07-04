using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarteiraDigital.Domain
{
    public class Cobranca
    {
        public string Id { get; set; }
        public string SolicitacaoPagador { get; set; }

        public decimal Valor { get; set; }
        public string DevedorCpf { get; set; }
        public string DevedorNome { get; set; }
    }
}

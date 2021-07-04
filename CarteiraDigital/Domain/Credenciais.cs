using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarteiraDigital.Domain
{
    public class Credenciais
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string caminhoCertificado { get; set; }
        public string chave { get; set; }
        public Enum.Operadora operadora { get; set; }        
    }
}

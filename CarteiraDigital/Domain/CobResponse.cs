using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarteiraDigital.Domain
{
    public class CobResponse
    {
        public CobResponse()
        {
            pix = new List<pixResponse>();
        }

        public string txid { get; set; }
        public cobLocResponse loc { get; set; }
        public string location { get; set; }
        public string status { get; set; }
        public List<pixResponse> pix { get; set; }
    }

    public class cobLocResponse
    {
        public int id { get; set; }
        public string location { get; set; }
        public string tipoCob { get; set; }
        public string criacao { get; set; }
    }

    public class pixResponse
    {
        public string endToEndId { get; set; }
        public string txid { get; set; }
        public string valor { get; set; }
        public string chave { get; set; }
        public string horario { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarteiraDigital.Utils
{
    public class Util
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string GetValorFormatado(decimal valor)
        {
            var cultureInfo = new CultureInfo("en-US");
            return valor.ToString(cultureInfo);
        }
    }
}

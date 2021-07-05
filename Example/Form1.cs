using CarteiraDigital.Domain;
using CarteiraDigital.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Example
{
    public partial class Form1 : Form
    {
        public string _id { get; set; }
        public string _id2 { get; set; }
        public string _idCancelamento { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboOperadora.DataSource = Enum.GetValues(typeof(CarteiraDigital.Enum.Operadora));
        }

        public Credenciais GetCredenciais()
        {
            var credenciais = new Credenciais();
            credenciais.operadora = (CarteiraDigital.Enum.Operadora)cboOperadora.SelectedItem;
            credenciais.caminhoCertificado = txtGerenciaNetCertificado.Text;
            credenciais.client_id = txtGerenciaNetClientID.Text;
            credenciais.client_secret = txtGerenciaNetClientSECRET.Text;
            credenciais.chave = txtGerenciaNetChave.Text;

            return credenciais;
        }

        private void btnGerarCobranca_Click(object sender, EventArgs e)
        {
            var credenciais = GetCredenciais();

            _id = Guid.NewGuid().ToString("N");

            var cobranca = new Cobranca();
            cobranca.Id = _id;
            //cobranca.DevedorNome = "Consumidor Final";
            //cobranca.DevedorCpf = "72088500030";
            cobranca.SolicitacaoPagador = "IzzyWay Tecnologia";
            cobranca.Valor = Convert.ToDecimal(txtValor.Text);

            var carteiraDigitalService = new CarteiraDigitalService(credenciais);
            var result = carteiraDigitalService.Cobranca(cobranca);
            if(!result.Success)
            {
                MessageBox.Show(result.Message);
                return;
            }

            var base64 = result.Result.ImagemQrCode.Replace("data:image/png;base64,", "");
            var pic = Convert.FromBase64String(base64);
            using (MemoryStream ms = new MemoryStream(pic))
            {
                imgQrCode.Image = Image.FromStream(ms);
            }
        }

        private void btnStatusCobranca_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(_id))
            {
                MessageBox.Show("Nenhuma cobrança gerada");
                return;
            }

            var credenciais = GetCredenciais();            
            
            var carteiraDigitalService = new CarteiraDigitalService(credenciais);
            var result = carteiraDigitalService.Status(_id);
            if (!result.Success)
            {
                MessageBox.Show(result.Message);
                return;
            }

            MessageBox.Show("Recebido");

            _id2 = result.Result.Chave;
        }

        private void btnDevolucaoCobranca_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_id))
            {
                MessageBox.Show("Nenhuma cobrança gerada");
                return;
            }

            if (string.IsNullOrEmpty(_id2))
            {
                MessageBox.Show("Status não verificado da cobrança");
                return;
            }

            var credenciais = GetCredenciais();

            var valor = Convert.ToDecimal(txtValor.Text);

            var carteiraDigitalService = new CarteiraDigitalService(credenciais);
            var result = carteiraDigitalService.Devolucao(_id, _id2, valor);
            if (result.Success)
            {
                MessageBox.Show("Devolvido com sucesso");
                return;
            }

            _idCancelamento = result.Result.Chave;
            MessageBox.Show(result.Message);
        }
    }
}

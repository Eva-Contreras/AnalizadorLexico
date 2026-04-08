namespace AnalizadorLexico
{
    public partial class Form1 : Form
    {
        private IdentificadorValido IDV = new();
        public Form1()
        {
            InitializeComponent();
        }
        private void btnRecorrer_Click(object sender, EventArgs e)
        {
            string cadena = txtCadena.Text.Trim();

            var (valido, mensaje, recorrido) = IDV.RecorrerCadena(cadena);

            if (valido)
            {
                txtToken.Text = "IDV";
                txtValor.Text = cadena;
                MessageBox.Show($"{cadena}: {recorrido} TOKEN: IDV",
                                "Recorrido",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            else
            {
                txtToken.Text = "ERROR";
                txtValor.Text = "";
                MessageBox.Show($"{cadena}: {recorrido}\n\n{mensaje}",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
    }
}

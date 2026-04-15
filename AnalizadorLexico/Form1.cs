namespace AnalizadorLexico
{
    public partial class Form1 : Form
    {
        private Recorrido r = new();
        public Form1()
        {
            InitializeComponent();
        }
        private void btnRecorrer_Click(object sender, EventArgs e)
        {
            string cadena = txtCadena.Text.Trim();

            var (valido, resultado, recorrido) = r.RecorrerCadena(cadena);

            if (valido)
            {
                txtToken.Text = resultado;
                txtValor.Text = cadena;

                MessageBox.Show($"{cadena}: {recorrido}\nTOKEN: {resultado}",
                                "Recorrido",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            else
            {
                txtToken.Text = "ERROR";
                txtValor.Text = "";

                MessageBox.Show($"{cadena}: {recorrido}\n\n{resultado}",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
    }
}

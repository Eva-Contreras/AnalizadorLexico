namespace AnalizadorLexico
{
    public partial class Form1 : Form
    {
        private Recorrido r = new();

        public Form1()
        {
            InitializeComponent();

            btnCargar.Click += btnCargar_Click;
            btnEditar.Click += btnEditar_Click;
            btnGuardar.Click += btnGuardarPrograma_Click;
            btnGuardarTokens.Click += btnGuardarTokens_Click;
            btnAnalizar.Click += btnAnalizar_Click;
        }

        private void btnAnalizar_Click(object? sender, EventArgs e)
        {
            string texto = rtxPrograma.Text;

            if (string.IsNullOrWhiteSpace(texto))
            {
                MessageBox.Show("Ingrese un programa para analizar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var (tokens, errores, simbolos) = r.AnalizarPrograma(texto);

            rtxTokens.Clear();

            var tokensPorLinea = tokens.GroupBy(t => t.linea)
                                       .OrderBy(g => g.Key);

            foreach (var grupo in tokensPorLinea)
            {
                int numLinea = grupo.Key;

                var tokensLinea = new List<string>();
                foreach (var (linea, valor, token) in grupo)
                {
                    tokensLinea.Add(token);
                }

                string lineaTokens = string.Join(" ", tokensLinea);
                bool tieneError = grupo.Any(t => t.token == "ERROR");

                rtxTokens.SelectionColor = tieneError ? Color.Red : Color.Black;
                rtxTokens.AppendText($"{numLinea}. {lineaTokens}\n");
                rtxTokens.SelectionColor = Color.Black;
            }

            dgvSimbolos.Rows.Clear();
            foreach (var (id, nombre) in simbolos.OrderBy(s => s.id))
            {
                dgvSimbolos.Rows.Add(id, nombre, "", "");
            }

            dgvErrores.Rows.Clear();
            foreach (var (linea, valor, error) in errores)
                dgvErrores.Rows.Add(linea, $"'{valor}' - {error}");

            lblErrores.Text = $"Total errores: {errores.Count}";
        }

        private void btnCargar_Click(object? sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Archivos de texto|*.txt|Todos|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
                rtxPrograma.Text = File.ReadAllText(ofd.FileName);
        }

        private void btnGuardarPrograma_Click(object? sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivos de texto|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
                File.WriteAllText(sfd.FileName, rtxPrograma.Text);
        }

        private void btnGuardarTokens_Click(object? sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivos de texto|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
                File.WriteAllText(sfd.FileName, rtxTokens.Text);
        }

        private void btnEditar_Click(object? sender, EventArgs e)
        {
            rtxPrograma.ReadOnly = false;
            rtxPrograma.Focus();
        }
    }
}
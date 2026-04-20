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

            lstLineasPrograma.Font = rtxPrograma.Font;
            lstLineasPrograma.BackColor = Color.LightGray;
            lstLineasPrograma.ForeColor = Color.DimGray;
            lstLineasPrograma.IntegralHeight = false;
            lstLineasPrograma.SelectionMode = SelectionMode.None;
            lstLineasPrograma.Height = rtxPrograma.Height;

            rtxPrograma.TextChanged += (s, e) => ActualizarNumerosLinea();
            rtxPrograma.VScroll += (s, e) => SincronizarScroll();
        }
        private void btnAnalizar_Click(object? sender, EventArgs e)
        {
            btnAnalizar.Enabled = false;

            string texto = rtxPrograma.Text.Trim();

            if (string.IsNullOrWhiteSpace(texto))  
            {
                MessageBox.Show("Ingrese un programa para analizar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnAnalizar.Enabled = true;  
                return;
            }

            var (tokens, errores, simbolos) = r.AnalizarPrograma(texto); 

            rtxTokens.Clear();
            var tokensPorLinea = tokens.GroupBy(t => t.linea).OrderBy(g => g.Key);

            foreach (var grupo in tokensPorLinea)
            {
                var tokensLinea = grupo.Select(t => t.token).ToList();
                string lineaTokens = string.Join(" ", tokensLinea);
                bool tieneError = grupo.Any(t => t.token == "ERROR");

                rtxTokens.SelectionColor = tieneError ? Color.Red : Color.Black;
                rtxTokens.AppendText($"{grupo.Key}. {lineaTokens}\n");
            }

            dgvSimbolos.Rows.Clear();
            foreach (var (id, nombre) in simbolos.OrderBy(s => s.id))
                dgvSimbolos.Rows.Add(id, nombre, "", "");

            dgvErrores.Rows.Clear();
            foreach (var (linea, valor, error) in errores)
                dgvErrores.Rows.Add(linea, $"'{valor}' - {error}");

            lblErrores.Text = $"Total errores: {errores.Count}";
            ActualizarNumerosLinea();

            btnAnalizar.Enabled = true; 
        }
        private void btnCargar_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de texto|*.txt|Todos|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    rtxPrograma.Text = File.ReadAllText(ofd.FileName);
                    ActualizarNumerosLinea();
                }
            }
        }
        private void btnGuardarPrograma_Click(object? sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Archivos de texto|*.txt";
                if (sfd.ShowDialog() == DialogResult.OK)
                    File.WriteAllText(sfd.FileName, rtxPrograma.Text);
            }
        }
        private void btnGuardarTokens_Click(object? sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Archivos de texto|*.txt";
                if (sfd.ShowDialog() == DialogResult.OK)
                    File.WriteAllText(sfd.FileName, rtxTokens.Text);
            }
        }
        private void btnEditar_Click(object? sender, EventArgs e)
        {
            rtxPrograma.ReadOnly = false;
            rtxPrograma.Focus();
        }
        private void ActualizarNumerosLinea()
        {
            lstLineasPrograma.Items.Clear();
            for (int i = 1; i <= rtxPrograma.Lines.Length; i++)
                lstLineasPrograma.Items.Add(i.ToString());
        }
        private void SincronizarScroll()
        {
            int firstVisibleChar = rtxPrograma.GetCharIndexFromPosition(new Point(1, 1));
            int primeraLineaVisible = rtxPrograma.GetLineFromCharIndex(firstVisibleChar);

            if (primeraLineaVisible < 0) primeraLineaVisible = 0;

            if (lstLineasPrograma.TopIndex != primeraLineaVisible)
            {
                lstLineasPrograma.TopIndex = primeraLineaVisible;
            }
        }
    }
}
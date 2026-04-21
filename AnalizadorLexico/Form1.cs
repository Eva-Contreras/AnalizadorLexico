namespace AnalizadorLexico
{
    public partial class Form1 : Form
    {
        // Instancia del recorrido para analizar el programa
        private Recorrido r = new();

        public Form1()
        {
            InitializeComponent();

            btnCargar.Click += btnCargar_Click;
            btnEditar.Click += btnEditar_Click;
            btnGuardar.Click += btnGuardarPrograma_Click;
            btnGuardarTokens.Click += btnGuardarTokens_Click;

            rtxPrograma.KeyDown += rtxPrograma_KeyDown;
            rtxPrograma.AcceptsTab = true;

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
            foreach (var (id, nombre) in simbolos)
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
            if (dgvErrores.Rows.Count > 0)
            {
                MessageBox.Show("No se puede guardar, el programa contiene errores léxicos.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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
        private void rtxPrograma_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                rtxPrograma.SelectedText = "     ";
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
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

        private void dgvErrores_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var cellValue = dgvErrores.Rows[e.RowIndex].Cells["colLinea"].Value;
            if (cellValue != null && int.TryParse(cellValue.ToString(), out int lineNumber))
            {
                IrALinea(lineNumber);
            }
        }
        private void IrALinea(int numeroLinea)
        {
            if (numeroLinea < 1 || numeroLinea > rtxPrograma.Lines.Length) return;

            int lineIndex = numeroLinea - 1;
            int charIndex = rtxPrograma.GetFirstCharIndexFromLine(lineIndex);
            if (charIndex < 0) return;

            rtxPrograma.Select(charIndex, 0);
            rtxPrograma.ScrollToCaret();
            rtxPrograma.Focus();

        }

        private void dgvSimbolos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
    }
}
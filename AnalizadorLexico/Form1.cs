namespace AnalizadorLexico
{
    public partial class Form1 : Form
    {
        private Recorrido r = new();
        private AnalizadorSintactico _sintactico = new();
        private AnalizadorSemantico _semantico = new();

        public Form1()
        {
            InitializeComponent();

            btnCargar.Click += btnCargar_Click;
            btnEditar.Click += btnEditar_Click;
            btnGuardar.Click += btnGuardarPrograma_Click;
            btnGuardarTokens.Click += btnGuardarTokens_Click;
            btnSintaxis.Click += btnSintaxis_Click;
            btnGenerar.Click += btnGenerar_Click;
            btnLimpiar.Click += btnLimpiar_Click;
            btnCopiar.Click += btnCopiar_Click;

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

            richTextBox2.Text = rtxPrograma.Text;
        }

        private void btnGenerar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rtxPrograma.Text))
            {
                MessageBox.Show("Ingrese un programa para generar la representación intermedia.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var texto = rtxPrograma.Text.Trim();
            var (tokens, erroresLexicos, _) = r.AnalizarPrograma(texto);

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

            dgvErrores.Rows.Clear();
            foreach (var (linea, valor, error) in erroresLexicos)
                dgvErrores.Rows.Add(linea, $"'{valor}' - {error}");

            lblErrores.Text = $"Total errores: {erroresLexicos.Count}";

            if (erroresLexicos.Count > 0)
            {
                MessageBox.Show("Error léxico detectado. Corrija los errores antes de generar el código intermedio.", "Error léxico",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl1.SelectedTab = tabPage1;
                return;
            }

            bool sintaxisValida = _sintactico.Analizar(tokens);
            rtxSintaxis.Clear();
            dgvSintaxis.Rows.Clear();

            if (!sintaxisValida)
            {
                rtxSintaxis.SelectionColor = Color.Red;
                foreach (string error in _sintactico.Errores)
                {
                    rtxSintaxis.AppendText(error + "\n");

                    string linea = "-";
                    int idx = error.LastIndexOf("línea ");
                    if (idx >= 0)
                    {
                        string resto = error.Substring(idx + 6);
                        string numStr = new string(resto.TakeWhile(char.IsDigit).ToArray());
                        if (!string.IsNullOrEmpty(numStr)) linea = numStr;
                    }
                    dgvSintaxis.Rows.Add(linea, error);
                }

                MessageBox.Show("Error sintáctico detectado. Debe corregir la sintaxis antes de generar el código intermedio.", "Error sintáctico",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl1.SelectedTab = tabPage2;
                return;
            }

            rtxSintaxis.SelectionColor = Color.Green;
            rtxSintaxis.AppendText("Análisis sintáctico completado: Programa aceptado, sin errores.");

            _semantico.AstRoot = _sintactico.AstRoot;
            bool semanticaValida = _semantico.Analizar(tokens);
            richTextBox1.Clear();
            dgvSemantica.Rows.Clear();
            dgvTablaSimbolos.Rows.Clear();

            if (!semanticaValida)
            {
                richTextBox1.SelectionColor = Color.Red;
                foreach (string error in _semantico.Errores)
                {
                    richTextBox1.AppendText(error + "\n");

                    string linea = "-";
                    int idx = error.LastIndexOf("línea ");
                    if (idx >= 0)
                    {
                        string resto = error.Substring(idx + 6);
                        string numStr = new string(resto.TakeWhile(char.IsDigit).ToArray());
                        if (!string.IsNullOrEmpty(numStr)) linea = numStr;
                    }
                    dgvSemantica.Rows.Add(linea, error);
                }

                foreach (var simbolo in _semantico.TablaSimbolosFinal)
                {
                    dgvTablaSimbolos.Rows.Add(
                        simbolo.Id,
                        simbolo.Nombre,
                        simbolo.Tipo,
                        simbolo.Valor,
                        simbolo.Ambito,
                        simbolo.Address);
                }

                MessageBox.Show("Error semántico detectado. Debe corregir la semántica antes de generar el código intermedio.", "Error semántico",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl1.SelectedTab = tabPage3;
                return;
            }

            richTextBox1.SelectionColor = Color.Green;
            richTextBox1.AppendText("Análisis semántico completado: Programa aceptado, sin errores de tipos.");

            foreach (var simbolo in _semantico.TablaSimbolosFinal)
            {
                dgvTablaSimbolos.Rows.Add(
                    simbolo.Id,
                    simbolo.Nombre,
                    simbolo.Tipo,
                    simbolo.Valor,
                    simbolo.Ambito,
                    simbolo.Address);
            }

            richTextBox2.Text = rtxPrograma.Text;
            richErrores.Clear();

            var generador = new CodigoIntermedio();
            var cuadruplos = generador.GenerarDesdeTexto(rtxPrograma.Text);

            if (generador.Errores.Count > 0)
            {
                richErrores.Text = string.Join(Environment.NewLine, generador.Errores);
                dgvCuadruplos.Rows.Clear();
                MessageBox.Show("Se detectaron errores de generación de código intermedio. Revisa el panel de errores.", "Error de generación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl1.SelectedTab = tabPage4;
                return;
            }

            dgvCuadruplos.Rows.Clear();
            foreach (var cuadruplo in cuadruplos)
            {
                dgvCuadruplos.Rows.Add(
                    cuadruplo.Numero,
                    cuadruplo.Operador,
                    cuadruplo.Argumento1,
                    cuadruplo.Argumento2,
                    cuadruplo.Resultado);
            }

            if (cuadruplos.Count == 0)
            {
                MessageBox.Show("No hay instrucciones que generar.", "Código intermedio",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Se generó la representación intermedia correctamente.", "Código intermedio",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            tabControl1.SelectedTab = tabPage4;
        }

        private void btnLimpiar_Click(object? sender, EventArgs e)
        {
            richTextBox2.Clear();
            richTextBox2.Text = rtxPrograma.Text;
            richErrores.Clear();
            dgvCuadruplos.Rows.Clear();
        }

        private void btnCopiar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(richTextBox2.Text))
            {
                MessageBox.Show("No hay contenido para copiar.", "Código intermedio",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Clipboard.SetText(richTextBox2.Text);
            MessageBox.Show("Se copió el contenido al portapapeles.", "Código intermedio",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        private void btnSintaxis_Click(object? sender, EventArgs e)
        {
            if (dgvErrores.Rows.Count > 0)
            {
                MessageBox.Show("Corrija los errores léxicos antes de analizar la sintaxis.",
                 "Errores léxicos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl1.SelectedTab = tabPage1;
                return;
                /*MessageBox.Show("Corrija los errores léxicos antes de analizar la sintaxis.",
                    "Errores léxicos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;*/
            }

            string texto = rtxPrograma.Text.Trim();
            if (string.IsNullOrWhiteSpace(texto))
            {
                MessageBox.Show("Ingrese un programa para analizar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var (tokens, _, _) = r.AnalizarPrograma(texto);

            bool valido = _sintactico.Analizar(tokens);

            rtxSintaxis.Clear();
            dgvSintaxis.Rows.Clear();

            if (valido)
            {
                rtxSintaxis.SelectionColor = Color.Green;
                rtxSintaxis.AppendText("Análisis sintáctico completado: Programa aceptado, sin errores.");
            }
            else
            {
                foreach (string error in _sintactico.Errores)
                {
                    rtxSintaxis.SelectionColor = Color.Red;
                    rtxSintaxis.AppendText(error + "\n");

                    string linea = "-";
                    int idx = error.LastIndexOf("línea ");
                    if (idx >= 0)
                    {
                        string resto = error.Substring(idx + 6);
                        string numStr = new string(resto.TakeWhile(char.IsDigit).ToArray());
                        if (!string.IsNullOrEmpty(numStr)) linea = numStr;
                    }
                    dgvSintaxis.Rows.Add(linea, error);
                }
            }
            /*else
            {
                foreach (string error in _sintactico.Errores)
                {
                    rtxSintaxis.SelectionColor = Color.Red;
                    rtxSintaxis.AppendText(error + "\n");
                    string linea = "-";
                    int idx = error.IndexOf("línea ");
                    if (idx >= 0)
                    {
                        string resto = error.Substring(idx + 6);
                        string numStr = new string(resto.TakeWhile(char.IsDigit).ToArray());
                        if (!string.IsNullOrEmpty(numStr)) linea = numStr;
                    }
                    dgvSintaxis.Rows.Add(linea, error);
                }
            }*/
        }

        private void dgvSintaxis_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var cellValue = dgvSintaxis.Rows[e.RowIndex].Cells[0].Value;
            if (cellValue != null && int.TryParse(cellValue.ToString(), out int lineNumber))
            {
                tabControl1.SelectedTab = tabPage1;
                IrALinea(lineNumber);
            }
        }

        private void btnSemantica_Click(object? sender, EventArgs e)
        {
            // Validar que no haya errores léxicos
            if (dgvErrores.Rows.Count > 0)
            {
                MessageBox.Show("Corrija los errores léxicos antes de analizar la semántica.",
                    "Errores léxicos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl1.SelectedTab = tabPage1;
                return;
            }

            // Validar que no haya errores sintácticos
            if (dgvSintaxis.Rows.Count > 0)
            {
                DialogResult result = MessageBox.Show(
                    "El programa tiene errores sintácticos. ¿Desea continuar con el análisis semántico de todas formas?",
                    "Errores sintácticos", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    tabControl1.SelectedTab = tabPage2;
                    return;
                }
            }

            string texto = rtxPrograma.Text.Trim();
            if (string.IsNullOrWhiteSpace(texto))
            {
                MessageBox.Show("Ingrese un programa para analizar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener tokens del análisis léxico
            var (tokens, _, _) = r.AnalizarPrograma(texto);

            // Generar el AST con los mismos tokens y compartirlo con el análisis semántico
            _sintactico.Analizar(tokens);
            _semantico.AstRoot = _sintactico.AstRoot;

            // Ejecutar análisis semántico
            bool valido = _semantico.Analizar(tokens);

            // Limpiar resultados anteriores
            richTextBox1.Clear();
            dgvSemantica.Rows.Clear();
            dgvTablaSimbolos.Rows.Clear();

            if (valido)
            {
                richTextBox1.SelectionColor = Color.Green;
                richTextBox1.AppendText("Análisis semántico completado: Programa aceptado, sin errores de tipos.");
            }
            else
            {
                richTextBox1.SelectionColor = Color.Red;
                foreach (string error in _semantico.Errores)
                {
                    richTextBox1.AppendText(error + "\n");

                    // Extraer número de línea del error
                    string linea = "-";
                    int idx = error.LastIndexOf("línea ");
                    if (idx >= 0)
                    {
                        string resto = error.Substring(idx + 6);
                        string numStr = new string(resto.TakeWhile(char.IsDigit).ToArray());
                        if (!string.IsNullOrEmpty(numStr)) linea = numStr;
                    }
                    dgvSemantica.Rows.Add(linea, error);
                }
            }

            // Mostrar tabla de símbolos final
            foreach (var simbolo in _semantico.TablaSimbolosFinal)
            {
                dgvTablaSimbolos.Rows.Add(
                    simbolo.Id,
                    simbolo.Nombre,
                    simbolo.Tipo,
                    simbolo.Valor,
                    simbolo.Ambito,
                    simbolo.Address
                );
            }

            // Nota: traza de depuración deshabilitada en la interfaz

            // Cambiar a la pestaña semántica
            tabControl1.SelectedTab = tabPage3;
        }

        private void dgvSemantica_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var cellValue = dgvSemantica.Rows[e.RowIndex].Cells[0].Value;
            if (cellValue != null && int.TryParse(cellValue.ToString(), out int lineNumber))
            {
                tabControl1.SelectedTab = tabPage1;
                IrALinea(lineNumber);
            }
        }
    }
}
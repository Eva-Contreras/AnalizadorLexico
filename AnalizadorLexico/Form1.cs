using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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

            // Eventos de los botones que se conservan.
            btnCargar.Click += btnCargar_Click;
            btnEditar.Click += btnEditar_Click;
            btnGuardar.Click += btnGuardarPrograma_Click;
            btnGuardarTokens.Click += btnGuardarTokens_Click;
            btnSintaxis.Click += btnSintaxis_Click;
            btnGenerar.Click += btnGenerar_Click;


            // Configuracion del editor de codigo.
            rtxPrograma.KeyDown += rtxPrograma_KeyDown;
            rtxPrograma.AcceptsTab = true;
            richTextBox2.ReadOnly = true;
            richTextBox6.ReadOnly = true;

            lstLineasPrograma.Font = rtxPrograma.Font;
            lstLineasPrograma.BackColor = Color.LightGray;
            lstLineasPrograma.ForeColor = Color.DimGray;
            lstLineasPrograma.IntegralHeight = false;
            lstLineasPrograma.SelectionMode = SelectionMode.None;
            lstLineasPrograma.Height = rtxPrograma.Height;

            rtxPrograma.TextChanged += (s, e) =>
                ActualizarNumerosLinea();

            rtxPrograma.VScroll += (s, e) =>
                SincronizarScroll();

            // Limpiar las representaciones intermedias al iniciar.
            LimpiarSalidasCodigoIntermedio();
        }

        // ========================================================
        // GENERAR CODIGO INTERMEDIO
        // ========================================================

        private void btnGenerar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rtxPrograma.Text))
            {
                MessageBox.Show(
                    "Ingrese un programa para generar la representación intermedia.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            string texto = rtxPrograma.Text.Trim();

            // ====================================================
            // 1. ANALISIS LEXICO
            // ====================================================

            var (tokens, erroresLexicos, _) =
                r.AnalizarPrograma(texto);

            rtxTokens.Clear();

            var tokensPorLinea = tokens
                .GroupBy(t => t.linea)
                .OrderBy(g => g.Key);

            foreach (var grupo in tokensPorLinea)
            {
                var tokensLinea = grupo
                    .Select(t => t.token)
                    .ToList();

                string lineaTokens = string.Join(" ", tokensLinea);

                bool tieneError = grupo.Any(t => t.token == "ERROR");

                rtxTokens.SelectionColor =
                    tieneError ? Color.Red : Color.Black;

                rtxTokens.AppendText(
                    $"{grupo.Key}. {lineaTokens}\n"
                );
            }

            dgvErrores.Rows.Clear();

            foreach (var (linea, valor, error) in erroresLexicos)
            {
                dgvErrores.Rows.Add(
                    linea,
                    $"'{valor}' - {error}"
                );
            }

            lblErrores.Text =
                $"Total errores: {erroresLexicos.Count}";

            if (erroresLexicos.Count > 0)
            {
                LimpiarSalidasCodigoIntermedio();

                MessageBox.Show(
                    "Error léxico detectado. Corrija los errores antes de generar el código intermedio.",
                    "Error léxico",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                tabControl1.SelectedTab = tabPage1;

                return;
            }

            // ====================================================
            // 2. ANALISIS SINTACTICO
            // ====================================================

            bool sintaxisValida = _sintactico.Analizar(tokens);

            rtxSintaxis.Clear();
            dgvSintaxis.Rows.Clear();

            if (!sintaxisValida)
            {
                rtxSintaxis.SelectionColor = Color.Red;

                foreach (string error in _sintactico.Errores)
                {
                    rtxSintaxis.AppendText(error + "\n");

                    string linea = ExtraerNumeroLinea(error);

                    dgvSintaxis.Rows.Add(linea, error);
                }

                LimpiarSalidasCodigoIntermedio();

                MessageBox.Show(
                    "Error sintáctico detectado. Debe corregir la sintaxis antes de generar el código intermedio.",
                    "Error sintáctico",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                tabControl1.SelectedTab = tabPage2;

                return;
            }

            rtxSintaxis.SelectionColor = Color.Green;

            rtxSintaxis.AppendText(
                "Análisis sintáctico completado: Programa aceptado, sin errores."
            );

            // ====================================================
            // 3. ANALISIS SEMANTICO
            // ====================================================

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

                    string linea = ExtraerNumeroLinea(error);

                    dgvSemantica.Rows.Add(linea, error);
                }

                MostrarTablaSimbolos();

                LimpiarSalidasCodigoIntermedio();

                MessageBox.Show(
                    "Error semántico detectado. Debe corregir la semántica antes de generar el código intermedio.",
                    "Error semántico",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                tabControl1.SelectedTab = tabPage3;

                return;
            }

            richTextBox1.SelectionColor = Color.Green;

            richTextBox1.AppendText(
                "Análisis semántico completado: Programa aceptado, sin errores de tipos."
            );

            MostrarTablaSimbolos();

            // ====================================================
            // 4. GENERAR LAS CINCO REPRESENTACIONES
            // ====================================================

            richTextBox2.Text = rtxPrograma.Text;

            // Limpiar las salidas anteriores.
            LimpiarSalidasCodigoIntermedio();

            CodigoIntermedio generador = new();

            var cuadruplos =
                generador.GenerarDesdeTexto(rtxPrograma.Text);

            // ----------------------------------------------------
            // 4.1 CUADRUPLOS
            // ----------------------------------------------------

            foreach (var cuadruplo in cuadruplos)
            {
                dgvCuadruplos.Rows.Add(
                    cuadruplo.Numero,
                    cuadruplo.Operador,
                    cuadruplo.Argumento1,
                    cuadruplo.Argumento2,
                    cuadruplo.Resultado
                );
            }

            // ----------------------------------------------------
            // 4.2 TRIPLOS
            // ----------------------------------------------------

            foreach (var triplo in generador.Triplos)
            {
                dgvTriplos.Rows.Add(
                    triplo.Numero,
                    triplo.Operador,
                    triplo.Argumento1,
                    triplo.Argumento2,
                    triplo.Resultado
                );
            }

            // ----------------------------------------------------
            // 4.3 CODIGO P
            // ----------------------------------------------------

            richTextBox3.Clear();

            foreach (string instruccion in generador.CodigoP)
            {
                richTextBox3.AppendText(instruccion + Environment.NewLine);
            }

            // ----------------------------------------------------
            // 4.4 CÓDIGO DE TRES DIRECCIONES
            // ----------------------------------------------------

            richTextBox2.Text = generador.CodigoTresDireccionesCuadruplos;
            richTextBox6.Text = generador.CodigoTresDireccionesTriplos;

            // ----------------------------------------------------
            // 4.5 NOTACION POLACA POSTFIJA
            // ----------------------------------------------------

            richTextBox4.Text = generador.NotacionPostfija;

            // ----------------------------------------------------
            // 4.6 NOTACION POLACA PREFIJA
            // ----------------------------------------------------

            richTextBox5.Text = generador.NotacionPrefija;

            // ====================================================
            // 5. MOSTRAR RESULTADO
            // ====================================================

            if (cuadruplos.Count == 0)
            {
                MessageBox.Show(
                    "No se encontraron asignaciones aritméticas compatibles para generar las representaciones intermedias.",
                    "Código intermedio",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            else
            {
                MessageBox.Show(
                    "Se generaron las representaciones intermedias correctamente.",
                    "Código intermedio",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }

            tabControl1.SelectedTab = tabPage4;
        }

        // ========================================================
        // LIMPIAR LAS CINCO SALIDAS
        // ========================================================

        private void LimpiarSalidasCodigoIntermedio()
        {
            dgvCuadruplos.Rows.Clear();
            dgvTriplos.Rows.Clear();

            richTextBox2.Clear();
            richTextBox6.Clear();
            richTextBox3.Clear();
            richTextBox4.Clear();
            richTextBox5.Clear();
        }

        // ========================================================
        // MOSTRAR TABLA DE SIMBOLOS
        // ========================================================

        private void MostrarTablaSimbolos()
        {
            dgvTablaSimbolos.Rows.Clear();

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
        }

        // ========================================================
        // EXTRAER NUMERO DE LINEA DE UN ERROR
        // ========================================================

        private string ExtraerNumeroLinea(string error)
        {
            string linea = "-";

            int indice = error.LastIndexOf("línea ");

            if (indice >= 0)
            {
                string resto = error.Substring(indice + 6);

                string numero = new string(
                    resto.TakeWhile(char.IsDigit).ToArray()
                );

                if (!string.IsNullOrEmpty(numero))
                {
                    linea = numero;
                }
            }

            return linea;
        }

        // ========================================================
        // ANALISIS LEXICO INDEPENDIENTE
        // ========================================================

        private void btnAnalizar_Click(object? sender, EventArgs e)
        {
            btnAnalizar.Enabled = false;

            try
            {
                string texto = rtxPrograma.Text.Trim();

                if (string.IsNullOrWhiteSpace(texto))
                {
                    MessageBox.Show(
                        "Ingrese un programa para analizar.",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                var (tokens, errores, simbolos) =
                    r.AnalizarPrograma(texto);

                rtxTokens.Clear();

                var tokensPorLinea = tokens
                    .GroupBy(t => t.linea)
                    .OrderBy(g => g.Key);

                foreach (var grupo in tokensPorLinea)
                {
                    var tokensLinea = grupo
                        .Select(t => t.token)
                        .ToList();

                    string lineaTokens =
                        string.Join(" ", tokensLinea);

                    bool tieneError =
                        grupo.Any(t => t.token == "ERROR");

                    rtxTokens.SelectionColor =
                        tieneError ? Color.Red : Color.Black;

                    rtxTokens.AppendText(
                        $"{grupo.Key}. {lineaTokens}\n"
                    );
                }

                dgvSimbolos.Rows.Clear();

                foreach (var (id, nombre) in simbolos)
                {
                    dgvSimbolos.Rows.Add(
                        id,
                        nombre,
                        "",
                        ""
                    );
                }

                dgvErrores.Rows.Clear();

                foreach (var (linea, valor, error) in errores)
                {
                    dgvErrores.Rows.Add(
                        linea,
                        $"'{valor}' - {error}"
                    );
                }

                lblErrores.Text =
                    $"Total errores: {errores.Count}";

                ActualizarNumerosLinea();
            }
            finally
            {
                btnAnalizar.Enabled = true;
            }
        }

        // ========================================================
        // CARGAR PROGRAMA
        // ========================================================

        private void btnCargar_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new())
            {
                ofd.Filter =
                    "Archivos de texto|*.txt|Todos|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    rtxPrograma.Text =
                        File.ReadAllText(ofd.FileName);

                    ActualizarNumerosLinea();
                }
            }
        }

        // ========================================================
        // GUARDAR PROGRAMA
        // ========================================================

        private void btnGuardarPrograma_Click(
            object? sender,
            EventArgs e)
        {
            using (SaveFileDialog sfd = new())
            {
                sfd.Filter = "Archivos de texto|*.txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(
                        sfd.FileName,
                        rtxPrograma.Text
                    );
                }
            }
        }

        // ========================================================
        // GUARDAR TOKENS
        // ========================================================

        private void btnGuardarTokens_Click(
            object? sender,
            EventArgs e)
        {
            if (dgvErrores.Rows.Count > 0)
            {
                MessageBox.Show(
                    "No se puede guardar, el programa contiene errores léxicos.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            using (SaveFileDialog sfd = new())
            {
                sfd.Filter = "Archivos de texto|*.txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(
                        sfd.FileName,
                        rtxTokens.Text
                    );
                }
            }
        }

        // ========================================================
        // HABILITAR EDICION DEL PROGRAMA
        // ========================================================

        private void btnEditar_Click(object? sender, EventArgs e)
        {
            rtxPrograma.ReadOnly = false;

            rtxPrograma.Focus();
        }

        // ========================================================
        // INSERTAR TABULACION EN EL EDITOR
        // ========================================================

        private void rtxPrograma_KeyDown(
            object? sender,
            KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                rtxPrograma.SelectedText = "     ";

                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        // ========================================================
        // ACTUALIZAR NUMEROS DE LINEA
        // ========================================================

        private void ActualizarNumerosLinea()
        {
            lstLineasPrograma.Items.Clear();

            for (int i = 1; i <= rtxPrograma.Lines.Length; i++)
            {
                lstLineasPrograma.Items.Add(i.ToString());
            }
        }

        // ========================================================
        // SINCRONIZAR SCROLL DEL EDITOR
        // ========================================================

        private void SincronizarScroll()
        {
            int primerCaracterVisible =
                rtxPrograma.GetCharIndexFromPosition(
                    new Point(1, 1)
                );

            int primeraLineaVisible =
                rtxPrograma.GetLineFromCharIndex(
                    primerCaracterVisible
                );

            if (primeraLineaVisible < 0)
            {
                primeraLineaVisible = 0;
            }

            if (lstLineasPrograma.TopIndex != primeraLineaVisible)
            {
                lstLineasPrograma.TopIndex =
                    primeraLineaVisible;
            }
        }

        // ========================================================
        // NAVEGAR DESDE LA TABLA DE ERRORES LEXICOS
        // ========================================================

        private void dgvErrores_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            var valor =
                dgvErrores.Rows[e.RowIndex]
                    .Cells["colLinea"].Value;

            if (valor != null &&
                int.TryParse(valor.ToString(), out int numeroLinea))
            {
                IrALinea(numeroLinea);
            }
        }

        private void IrALinea(int numeroLinea)
        {
            if (numeroLinea < 1 ||
                numeroLinea > rtxPrograma.Lines.Length)
            {
                return;
            }

            int indiceLinea = numeroLinea - 1;

            int indiceCaracter =
                rtxPrograma.GetFirstCharIndexFromLine(indiceLinea);

            if (indiceCaracter < 0)
            {
                return;
            }

            rtxPrograma.Select(indiceCaracter, 0);
            rtxPrograma.ScrollToCaret();
            rtxPrograma.Focus();
        }

        // ========================================================
        // ANALISIS SINTACTICO INDEPENDIENTE
        // ========================================================

        private void btnSintaxis_Click(
            object? sender,
            EventArgs e)
        {
            if (dgvErrores.Rows.Count > 0)
            {
                MessageBox.Show(
                    "Corrija los errores léxicos antes de analizar la sintaxis.",
                    "Errores léxicos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                tabControl1.SelectedTab = tabPage1;

                return;
            }

            string texto = rtxPrograma.Text.Trim();

            if (string.IsNullOrWhiteSpace(texto))
            {
                MessageBox.Show(
                    "Ingrese un programa para analizar.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            var (tokens, _, _) = r.AnalizarPrograma(texto);

            bool valido = _sintactico.Analizar(tokens);

            rtxSintaxis.Clear();
            dgvSintaxis.Rows.Clear();

            if (valido)
            {
                rtxSintaxis.SelectionColor = Color.Green;

                rtxSintaxis.AppendText(
                    "Análisis sintáctico completado: Programa aceptado, sin errores."
                );
            }
            else
            {
                foreach (string error in _sintactico.Errores)
                {
                    rtxSintaxis.SelectionColor = Color.Red;

                    rtxSintaxis.AppendText(error + "\n");

                    string linea = ExtraerNumeroLinea(error);

                    dgvSintaxis.Rows.Add(linea, error);
                }
            }
        }

        // ========================================================
        // NAVEGAR DESDE LA TABLA DE ERRORES SINTACTICOS
        // ========================================================

        private void dgvSintaxis_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            var valor =
                dgvSintaxis.Rows[e.RowIndex].Cells[0].Value;

            if (valor != null &&
                int.TryParse(valor.ToString(), out int numeroLinea))
            {
                tabControl1.SelectedTab = tabPage1;

                IrALinea(numeroLinea);
            }
        }

        // ========================================================
        // ANALISIS SEMANTICO INDEPENDIENTE
        // ========================================================

        private void btnSemantica_Click(
            object? sender,
            EventArgs e)
        {
            // Validar errores lexicos.
            if (dgvErrores.Rows.Count > 0)
            {
                MessageBox.Show(
                    "Corrija los errores léxicos antes de analizar la semántica.",
                    "Errores léxicos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                tabControl1.SelectedTab = tabPage1;

                return;
            }

            // Validar errores sintacticos.
            if (dgvSintaxis.Rows.Count > 0)
            {
                DialogResult resultado = MessageBox.Show(
                    "El programa tiene errores sintácticos. ¿Desea continuar con el análisis semántico de todas formas?",
                    "Errores sintácticos",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resultado == DialogResult.No)
                {
                    tabControl1.SelectedTab = tabPage2;

                    return;
                }
            }

            string texto = rtxPrograma.Text.Trim();

            if (string.IsNullOrWhiteSpace(texto))
            {
                MessageBox.Show(
                    "Ingrese un programa para analizar.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            // Obtener tokens.
            var (tokens, _, _) = r.AnalizarPrograma(texto);

            // Generar el AST.
            _sintactico.Analizar(tokens);

            _semantico.AstRoot = _sintactico.AstRoot;

            // Ejecutar analisis semantico.
            bool valido = _semantico.Analizar(tokens);

            richTextBox1.Clear();

            dgvSemantica.Rows.Clear();

            dgvTablaSimbolos.Rows.Clear();

            if (valido)
            {
                richTextBox1.SelectionColor = Color.Green;

                richTextBox1.AppendText(
                    "Análisis semántico completado: Programa aceptado, sin errores de tipos."
                );
            }
            else
            {
                richTextBox1.SelectionColor = Color.Red;

                foreach (string error in _semantico.Errores)
                {
                    richTextBox1.AppendText(error + "\n");

                    string linea = ExtraerNumeroLinea(error);

                    dgvSemantica.Rows.Add(linea, error);
                }
            }

            MostrarTablaSimbolos();

            tabControl1.SelectedTab = tabPage3;
        }

        // ========================================================
        // NAVEGAR DESDE LA TABLA DE ERRORES SEMANTICOS
        // ========================================================

        private void dgvSemantica_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            var valor =
                dgvSemantica.Rows[e.RowIndex].Cells[0].Value;

            if (valor != null &&
                int.TryParse(valor.ToString(), out int numeroLinea))
            {
                tabControl1.SelectedTab = tabPage1;

                IrALinea(numeroLinea);
            }
        }
    }
}
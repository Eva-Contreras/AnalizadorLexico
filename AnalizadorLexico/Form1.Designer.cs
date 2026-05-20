namespace AnalizadorLexico
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            groupBox1 = new GroupBox();
            rtxPrograma = new RichTextBox();
            lstLineasPrograma = new ListBox();
            lblTablaErrores = new Label();
            lblTablaSimbolos = new Label();
            lblArchivoTokens = new Label();
            lblErrores = new Label();
            lblProgramaFuente = new Label();
            dgvSimbolos = new DataGridView();
            colNum = new DataGridViewTextBoxColumn();
            colNombre = new DataGridViewTextBoxColumn();
            colTipo = new DataGridViewTextBoxColumn();
            colValor = new DataGridViewTextBoxColumn();
            btnAnalizar = new Button();
            dgvErrores = new DataGridView();
            colLinea = new DataGridViewTextBoxColumn();
            colError = new DataGridViewTextBoxColumn();
            btnGuardarTokens = new Button();
            btnGuardar = new Button();
            rtxTokens = new RichTextBox();
            btnEditar = new Button();
            btnCargar = new Button();
            lblDiseñadores = new Label();
            pictureBox1 = new PictureBox();
            groupBox2 = new GroupBox();
            btnSintaxis = new Button();
            rtxSintaxis = new RichTextBox();
            label1 = new Label();
            dgvSintaxis = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            pictureBox2 = new PictureBox();
            label2 = new Label();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSimbolos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvErrores).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSintaxis).BeginInit();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.GradientInactiveCaption;
            groupBox1.Controls.Add(rtxPrograma);
            groupBox1.Controls.Add(lstLineasPrograma);
            groupBox1.Controls.Add(lblTablaErrores);
            groupBox1.Controls.Add(lblTablaSimbolos);
            groupBox1.Controls.Add(lblArchivoTokens);
            groupBox1.Controls.Add(lblErrores);
            groupBox1.Controls.Add(lblProgramaFuente);
            groupBox1.Controls.Add(dgvSimbolos);
            groupBox1.Controls.Add(btnAnalizar);
            groupBox1.Controls.Add(dgvErrores);
            groupBox1.Controls.Add(btnGuardarTokens);
            groupBox1.Controls.Add(btnGuardar);
            groupBox1.Controls.Add(rtxTokens);
            groupBox1.Controls.Add(btnEditar);
            groupBox1.Controls.Add(btnCargar);
            groupBox1.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            groupBox1.Location = new Point(5, 62);
            groupBox1.Margin = new Padding(2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(2);
            groupBox1.Size = new Size(771, 490);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Analizador Léxico";
            // 
            // rtxPrograma
            // 
            rtxPrograma.BorderStyle = BorderStyle.None;
            rtxPrograma.Font = new Font("Segoe UI", 8F);
            rtxPrograma.Location = new Point(63, 44);
            rtxPrograma.Margin = new Padding(2);
            rtxPrograma.Name = "rtxPrograma";
            rtxPrograma.Size = new Size(249, 205);
            rtxPrograma.TabIndex = 1;
            rtxPrograma.Text = "";
            // 
            // lstLineasPrograma
            // 
            lstLineasPrograma.BackColor = SystemColors.HighlightText;
            lstLineasPrograma.BorderStyle = BorderStyle.None;
            lstLineasPrograma.Font = new Font("Segoe UI", 8F);
            lstLineasPrograma.ForeColor = SystemColors.HighlightText;
            lstLineasPrograma.FormattingEnabled = true;
            lstLineasPrograma.IntegralHeight = false;
            lstLineasPrograma.Location = new Point(30, 44);
            lstLineasPrograma.Margin = new Padding(2);
            lstLineasPrograma.Name = "lstLineasPrograma";
            lstLineasPrograma.SelectionMode = SelectionMode.None;
            lstLineasPrograma.Size = new Size(76, 205);
            lstLineasPrograma.TabIndex = 20;
            // 
            // lblTablaErrores
            // 
            lblTablaErrores.AutoSize = true;
            lblTablaErrores.Font = new Font("Segoe UI", 12F);
            lblTablaErrores.Location = new Point(104, 310);
            lblTablaErrores.Margin = new Padding(2, 0, 2, 0);
            lblTablaErrores.Name = "lblTablaErrores";
            lblTablaErrores.Size = new Size(120, 21);
            lblTablaErrores.TabIndex = 19;
            lblTablaErrores.Text = "Tabla de errores";
            // 
            // lblTablaSimbolos
            // 
            lblTablaSimbolos.AutoSize = true;
            lblTablaSimbolos.Font = new Font("Segoe UI", 12F);
            lblTablaSimbolos.Location = new Point(517, 310);
            lblTablaSimbolos.Margin = new Padding(2, 0, 2, 0);
            lblTablaSimbolos.Name = "lblTablaSimbolos";
            lblTablaSimbolos.Size = new Size(133, 21);
            lblTablaSimbolos.TabIndex = 18;
            lblTablaSimbolos.Text = "Tabla de símbolos";
            // 
            // lblArchivoTokens
            // 
            lblArchivoTokens.AutoSize = true;
            lblArchivoTokens.Font = new Font("Segoe UI", 12F);
            lblArchivoTokens.Location = new Point(496, 23);
            lblArchivoTokens.Margin = new Padding(2, 0, 2, 0);
            lblArchivoTokens.Name = "lblArchivoTokens";
            lblArchivoTokens.Size = new Size(134, 21);
            lblArchivoTokens.TabIndex = 17;
            lblArchivoTokens.Text = "Archivo de tokens";
            // 
            // lblErrores
            // 
            lblErrores.AutoSize = true;
            lblErrores.Font = new Font("Segoe UI", 10F);
            lblErrores.Location = new Point(4, 468);
            lblErrores.Margin = new Padding(2, 0, 2, 0);
            lblErrores.Name = "lblErrores";
            lblErrores.Size = new Size(88, 19);
            lblErrores.TabIndex = 14;
            lblErrores.Text = "Total errores:";
            // 
            // lblProgramaFuente
            // 
            lblProgramaFuente.AutoSize = true;
            lblProgramaFuente.Font = new Font("Segoe UI", 12F);
            lblProgramaFuente.Location = new Point(97, 23);
            lblProgramaFuente.Margin = new Padding(2, 0, 2, 0);
            lblProgramaFuente.Name = "lblProgramaFuente";
            lblProgramaFuente.Size = new Size(127, 21);
            lblProgramaFuente.TabIndex = 16;
            lblProgramaFuente.Text = "Programa fuente";
            // 
            // dgvSimbolos
            // 
            dgvSimbolos.AllowUserToAddRows = false;
            dgvSimbolos.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 8F);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvSimbolos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvSimbolos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSimbolos.Columns.AddRange(new DataGridViewColumn[] { colNum, colNombre, colTipo, colValor });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 8F);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgvSimbolos.DefaultCellStyle = dataGridViewCellStyle2;
            dgvSimbolos.Location = new Point(398, 331);
            dgvSimbolos.Margin = new Padding(2);
            dgvSimbolos.Name = "dgvSimbolos";
            dgvSimbolos.ReadOnly = true;
            dgvSimbolos.RowHeadersWidth = 62;
            dgvSimbolos.Size = new Size(360, 135);
            dgvSimbolos.TabIndex = 8;
            // 
            // colNum
            // 
            colNum.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            colNum.HeaderText = "#";
            colNum.MinimumWidth = 8;
            colNum.Name = "colNum";
            colNum.ReadOnly = true;
            colNum.Width = 39;
            // 
            // colNombre
            // 
            colNombre.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colNombre.HeaderText = "Nombre";
            colNombre.MinimumWidth = 8;
            colNombre.Name = "colNombre";
            colNombre.ReadOnly = true;
            // 
            // colTipo
            // 
            colTipo.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colTipo.HeaderText = "Tipo de Dato";
            colTipo.MinimumWidth = 8;
            colTipo.Name = "colTipo";
            colTipo.ReadOnly = true;
            // 
            // colValor
            // 
            colValor.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colValor.HeaderText = "Valor";
            colValor.MinimumWidth = 8;
            colValor.Name = "colValor";
            colValor.ReadOnly = true;
            // 
            // btnAnalizar
            // 
            btnAnalizar.BackColor = SystemColors.ActiveCaption;
            btnAnalizar.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnAnalizar.ForeColor = SystemColors.ControlLightLight;
            btnAnalizar.Location = new Point(316, 123);
            btnAnalizar.Margin = new Padding(2);
            btnAnalizar.Name = "btnAnalizar";
            btnAnalizar.Size = new Size(111, 38);
            btnAnalizar.TabIndex = 2;
            btnAnalizar.Text = "Analizar";
            btnAnalizar.UseVisualStyleBackColor = false;
            btnAnalizar.Click += btnAnalizar_Click;
            // 
            // dgvErrores
            // 
            dgvErrores.AllowUserToAddRows = false;
            dgvErrores.BorderStyle = BorderStyle.None;
            dgvErrores.CausesValidation = false;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Control;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 8F);
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            dgvErrores.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dgvErrores.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvErrores.Columns.AddRange(new DataGridViewColumn[] { colLinea, colError });
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Window;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 8F);
            dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
            dgvErrores.DefaultCellStyle = dataGridViewCellStyle4;
            dgvErrores.Location = new Point(4, 331);
            dgvErrores.Margin = new Padding(2);
            dgvErrores.Name = "dgvErrores";
            dgvErrores.ReadOnly = true;
            dgvErrores.RowHeadersWidth = 62;
            dgvErrores.Size = new Size(374, 135);
            dgvErrores.TabIndex = 7;
            dgvErrores.CellDoubleClick += dgvErrores_CellDoubleClick;
            // 
            // colLinea
            // 
            colLinea.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            colLinea.HeaderText = "Línea";
            colLinea.MinimumWidth = 8;
            colLinea.Name = "colLinea";
            colLinea.ReadOnly = true;
            colLinea.Width = 59;
            // 
            // colError
            // 
            colError.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colError.HeaderText = "Error";
            colError.MinimumWidth = 8;
            colError.Name = "colError";
            colError.ReadOnly = true;
            // 
            // btnGuardarTokens
            // 
            btnGuardarTokens.BackColor = SystemColors.ActiveCaption;
            btnGuardarTokens.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnGuardarTokens.ForeColor = SystemColors.ControlLightLight;
            btnGuardarTokens.Location = new Point(517, 256);
            btnGuardarTokens.Margin = new Padding(2);
            btnGuardarTokens.Name = "btnGuardarTokens";
            btnGuardarTokens.Size = new Size(93, 46);
            btnGuardarTokens.TabIndex = 6;
            btnGuardarTokens.Text = "Guardar Tokens";
            btnGuardarTokens.UseVisualStyleBackColor = false;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = SystemColors.ActiveCaption;
            btnGuardar.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnGuardar.ForeColor = SystemColors.ControlLightLight;
            btnGuardar.Location = new Point(234, 256);
            btnGuardar.Margin = new Padding(2);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(99, 46);
            btnGuardar.TabIndex = 5;
            btnGuardar.Text = "Guardar Programa";
            btnGuardar.UseVisualStyleBackColor = false;
            // 
            // rtxTokens
            // 
            rtxTokens.BackColor = SystemColors.Window;
            rtxTokens.BorderStyle = BorderStyle.None;
            rtxTokens.Font = new Font("Segoe UI", 8F);
            rtxTokens.Location = new Point(432, 47);
            rtxTokens.Margin = new Padding(2);
            rtxTokens.Name = "rtxTokens";
            rtxTokens.ReadOnly = true;
            rtxTokens.Size = new Size(267, 205);
            rtxTokens.TabIndex = 9;
            rtxTokens.Text = "";
            // 
            // btnEditar
            // 
            btnEditar.BackColor = SystemColors.ActiveCaption;
            btnEditar.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnEditar.ForeColor = SystemColors.ControlLightLight;
            btnEditar.Location = new Point(118, 256);
            btnEditar.Margin = new Padding(2);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new Size(99, 46);
            btnEditar.TabIndex = 4;
            btnEditar.Text = "Editar Programa";
            btnEditar.UseVisualStyleBackColor = false;
            // 
            // btnCargar
            // 
            btnCargar.BackColor = SystemColors.ActiveCaption;
            btnCargar.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnCargar.ForeColor = SystemColors.ControlLightLight;
            btnCargar.Location = new Point(4, 256);
            btnCargar.Margin = new Padding(2);
            btnCargar.Name = "btnCargar";
            btnCargar.Size = new Size(99, 46);
            btnCargar.TabIndex = 3;
            btnCargar.Text = "Cargar Programa";
            btnCargar.UseVisualStyleBackColor = false;
            // 
            // lblDiseñadores
            // 
            lblDiseñadores.AutoSize = true;
            lblDiseñadores.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDiseñadores.Location = new Point(94, 3);
            lblDiseñadores.Margin = new Padding(2, 0, 2, 0);
            lblDiseñadores.Name = "lblDiseñadores";
            lblDiseñadores.Size = new Size(313, 57);
            lblDiseñadores.TabIndex = 3;
            lblDiseñadores.Text = "Diseñadores:\r\n- 23100150 Eva Guadalupe Contreras Antúnez\r\n- 23100201 Anapaula Rendón Montalvo";
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Image = Properties.Resources.Logo;
            pictureBox1.Location = new Point(41, 6);
            pictureBox1.Margin = new Padding(2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(58, 46);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            // 
            // groupBox2
            // 
            groupBox2.BackColor = SystemColors.GradientInactiveCaption;
            groupBox2.Controls.Add(btnSintaxis);
            groupBox2.Controls.Add(rtxSintaxis);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(dgvSintaxis);
            groupBox2.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            groupBox2.Location = new Point(37, 65);
            groupBox2.Margin = new Padding(2);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(2);
            groupBox2.Size = new Size(713, 490);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            groupBox2.Text = "Analizador Sintáctico";
            // 
            // btnSintaxis
            // 
            btnSintaxis.BackColor = SystemColors.ActiveCaption;
            btnSintaxis.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnSintaxis.ForeColor = SystemColors.ControlLightLight;
            btnSintaxis.Location = new Point(4, 188);
            btnSintaxis.Margin = new Padding(2);
            btnSintaxis.Name = "btnSintaxis";
            btnSintaxis.Size = new Size(99, 38);
            btnSintaxis.TabIndex = 21;
            btnSintaxis.Text = "Sintaxis";
            btnSintaxis.UseVisualStyleBackColor = false;
            btnSintaxis.Click += btnSintaxis_Click;
            // 
            // rtxSintaxis
            // 
            rtxSintaxis.BackColor = SystemColors.Window;
            rtxSintaxis.BorderStyle = BorderStyle.None;
            rtxSintaxis.Font = new Font("Segoe UI", 8F);
            rtxSintaxis.Location = new Point(107, 228);
            rtxSintaxis.Margin = new Padding(2);
            rtxSintaxis.Name = "rtxSintaxis";
            rtxSintaxis.ReadOnly = true;
            rtxSintaxis.Size = new Size(505, 247);
            rtxSintaxis.TabIndex = 21;
            rtxSintaxis.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(122, 31);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(138, 21);
            label1.TabIndex = 20;
            label1.Text = "Errores Sintácticos";
            // 
            // dgvSintaxis
            // 
            dgvSintaxis.AllowUserToAddRows = false;
            dgvSintaxis.BorderStyle = BorderStyle.None;
            dgvSintaxis.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSintaxis.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2 });
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = SystemColors.Window;
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 8F);
            dataGridViewCellStyle5.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.False;
            dgvSintaxis.DefaultCellStyle = dataGridViewCellStyle5;
            dgvSintaxis.Location = new Point(107, 63);
            dgvSintaxis.Margin = new Padding(2);
            dgvSintaxis.Name = "dgvSintaxis";
            dgvSintaxis.ReadOnly = true;
            dgvSintaxis.RowHeadersWidth = 62;
            dgvSintaxis.Size = new Size(505, 122);
            dgvSintaxis.TabIndex = 19;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridViewTextBoxColumn1.HeaderText = "Línea";
            dataGridViewTextBoxColumn1.MinimumWidth = 8;
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            dataGridViewTextBoxColumn1.Width = 84;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTextBoxColumn2.HeaderText = "Error";
            dataGridViewTextBoxColumn2.MinimumWidth = 8;
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(793, 588);
            tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            tabPage1.BackColor = SystemColors.GradientActiveCaption;
            tabPage1.Controls.Add(pictureBox2);
            tabPage1.Controls.Add(lblDiseñadores);
            tabPage1.Controls.Add(groupBox1);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(785, 560);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Analizador Léxico";
            // 
            // tabPage2
            // 
            tabPage2.BackColor = SystemColors.GradientActiveCaption;
            tabPage2.Controls.Add(label2);
            tabPage2.Controls.Add(pictureBox1);
            tabPage2.Controls.Add(groupBox2);
            tabPage2.ForeColor = SystemColors.ControlText;
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(785, 560);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Analizador Sintáctico";
            // 
            // pictureBox2
            // 
            pictureBox2.BorderStyle = BorderStyle.FixedSingle;
            pictureBox2.Image = Properties.Resources.Logo;
            pictureBox2.Location = new Point(24, 5);
            pictureBox2.Margin = new Padding(2);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(66, 53);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 5;
            pictureBox2.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label2.Location = new Point(103, 3);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(313, 57);
            label2.TabIndex = 6;
            label2.Text = "Diseñadores:\r\n- 23100150 Eva Guadalupe Contreras Antúnez\r\n- 23100201 Anapaula Rendón Montalvo";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientActiveCaption;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(814, 596);
            Controls.Add(tabControl1);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CÇ - Comme ci, comme ça (Beta)";
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSimbolos).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvErrores).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSintaxis).EndInit();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private GroupBox groupBox1;
        private Button btnAnalizar;
        private Button btnCargar;
        private Button btnEditar;
        private Button btnGuardar;
        private RichTextBox rtxTokens;
        private Button btnGuardarTokens;
        private Label lblDiseñadores;
        private DataGridView dgvErrores;
        private Label lblErrores;
        private DataGridView dgvSimbolos;
        private Label lblProgramaFuente;
        private PictureBox pictureBox1;
        private Label lblArchivoTokens;
        private Label lblTablaErrores;
        private Label lblTablaSimbolos;
        private DataGridViewTextBoxColumn colLinea;
        private DataGridViewTextBoxColumn colError;
        private DataGridViewTextBoxColumn colNum;
        private DataGridViewTextBoxColumn colNombre;
        private DataGridViewTextBoxColumn colTipo;
        private DataGridViewTextBoxColumn colValor;
        private ListBox lstLineasPrograma;
        private RichTextBox rtxPrograma;
        private GroupBox groupBox2;
        private Label label1;
        private DataGridView dgvSintaxis;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private RichTextBox rtxSintaxis;
        private Button btnSintaxis;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private PictureBox pictureBox2;
        private Label label2;
    }
}

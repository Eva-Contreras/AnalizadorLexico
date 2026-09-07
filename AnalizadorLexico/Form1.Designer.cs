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
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
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
            pictureBox2 = new PictureBox();
            tabPage2 = new TabPage();
            label2 = new Label();
            tabPage3 = new TabPage();
            label3 = new Label();
            pictureBox3 = new PictureBox();
            groupBox3 = new GroupBox();
            btnSemantica = new Button();
            richTextBox1 = new RichTextBox();
            label4 = new Label();
            dgvSemantica = new DataGridView();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            label5 = new Label();
            dgvTablaSimbolos = new DataGridView();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn8 = new DataGridViewTextBoxColumn();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSimbolos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvErrores).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSintaxis).BeginInit();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSemantica).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvTablaSimbolos).BeginInit();
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
            groupBox1.Location = new Point(7, 103);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1101, 800);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Analizador Léxico";
            // 
            // rtxPrograma
            // 
            rtxPrograma.BorderStyle = BorderStyle.None;
            rtxPrograma.Font = new Font("Segoe UI", 8F);
            rtxPrograma.Location = new Point(90, 73);
            rtxPrograma.Name = "rtxPrograma";
            rtxPrograma.Size = new Size(356, 342);
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
            lstLineasPrograma.Location = new Point(43, 73);
            lstLineasPrograma.Name = "lstLineasPrograma";
            lstLineasPrograma.SelectionMode = SelectionMode.None;
            lstLineasPrograma.Size = new Size(109, 342);
            lstLineasPrograma.TabIndex = 20;
            // 
            // lblTablaErrores
            // 
            lblTablaErrores.AutoSize = true;
            lblTablaErrores.Font = new Font("Segoe UI", 12F);
            lblTablaErrores.Location = new Point(6, 518);
            lblTablaErrores.Name = "lblTablaErrores";
            lblTablaErrores.Size = new Size(183, 32);
            lblTablaErrores.TabIndex = 19;
            lblTablaErrores.Text = "Tabla de errores";
            // 
            // lblTablaSimbolos
            // 
            lblTablaSimbolos.AutoSize = true;
            lblTablaSimbolos.Font = new Font("Segoe UI", 12F);
            lblTablaSimbolos.Location = new Point(581, 518);
            lblTablaSimbolos.Name = "lblTablaSimbolos";
            lblTablaSimbolos.Size = new Size(204, 32);
            lblTablaSimbolos.TabIndex = 18;
            lblTablaSimbolos.Text = "Tabla de símbolos";
            // 
            // lblArchivoTokens
            // 
            lblArchivoTokens.AutoSize = true;
            lblArchivoTokens.Font = new Font("Segoe UI", 12F);
            lblArchivoTokens.Location = new Point(709, 38);
            lblArchivoTokens.Name = "lblArchivoTokens";
            lblArchivoTokens.Size = new Size(206, 32);
            lblArchivoTokens.TabIndex = 17;
            lblArchivoTokens.Text = "Archivo de tokens";
            // 
            // lblErrores
            // 
            lblErrores.AutoSize = true;
            lblErrores.Font = new Font("Segoe UI", 10F);
            lblErrores.Location = new Point(351, 520);
            lblErrores.Name = "lblErrores";
            lblErrores.Size = new Size(124, 28);
            lblErrores.TabIndex = 14;
            lblErrores.Text = "Total errores:";
            // 
            // lblProgramaFuente
            // 
            lblProgramaFuente.AutoSize = true;
            lblProgramaFuente.Font = new Font("Segoe UI", 12F);
            lblProgramaFuente.Location = new Point(139, 38);
            lblProgramaFuente.Name = "lblProgramaFuente";
            lblProgramaFuente.Size = new Size(193, 32);
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
            dgvSimbolos.Location = new Point(566, 552);
            dgvSimbolos.Name = "dgvSimbolos";
            dgvSimbolos.ReadOnly = true;
            dgvSimbolos.RowHeadersWidth = 62;
            dgvSimbolos.Size = new Size(466, 225);
            dgvSimbolos.TabIndex = 8;
            // 
            // colNum
            // 
            colNum.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            colNum.HeaderText = "#";
            colNum.MinimumWidth = 8;
            colNum.Name = "colNum";
            colNum.ReadOnly = true;
            colNum.Width = 55;
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
            btnAnalizar.Location = new Point(451, 205);
            btnAnalizar.Name = "btnAnalizar";
            btnAnalizar.Size = new Size(159, 63);
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
            dgvErrores.Location = new Point(6, 552);
            dgvErrores.Name = "dgvErrores";
            dgvErrores.ReadOnly = true;
            dgvErrores.RowHeadersWidth = 62;
            dgvErrores.Size = new Size(534, 225);
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
            colLinea.Width = 83;
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
            btnGuardarTokens.Location = new Point(739, 427);
            btnGuardarTokens.Name = "btnGuardarTokens";
            btnGuardarTokens.Size = new Size(133, 77);
            btnGuardarTokens.TabIndex = 6;
            btnGuardarTokens.Text = "Guardar Tokens";
            btnGuardarTokens.UseVisualStyleBackColor = false;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = SystemColors.ActiveCaption;
            btnGuardar.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnGuardar.ForeColor = SystemColors.ControlLightLight;
            btnGuardar.Location = new Point(334, 427);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(141, 77);
            btnGuardar.TabIndex = 5;
            btnGuardar.Text = "Guardar Programa";
            btnGuardar.UseVisualStyleBackColor = false;
            // 
            // rtxTokens
            // 
            rtxTokens.BackColor = SystemColors.Window;
            rtxTokens.BorderStyle = BorderStyle.None;
            rtxTokens.Font = new Font("Segoe UI", 8F);
            rtxTokens.Location = new Point(617, 78);
            rtxTokens.Name = "rtxTokens";
            rtxTokens.ReadOnly = true;
            rtxTokens.Size = new Size(381, 342);
            rtxTokens.TabIndex = 9;
            rtxTokens.Text = "";
            // 
            // btnEditar
            // 
            btnEditar.BackColor = SystemColors.ActiveCaption;
            btnEditar.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnEditar.ForeColor = SystemColors.ControlLightLight;
            btnEditar.Location = new Point(169, 427);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new Size(141, 77);
            btnEditar.TabIndex = 4;
            btnEditar.Text = "Editar Programa";
            btnEditar.UseVisualStyleBackColor = false;
            // 
            // btnCargar
            // 
            btnCargar.BackColor = SystemColors.ActiveCaption;
            btnCargar.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnCargar.ForeColor = SystemColors.ControlLightLight;
            btnCargar.Location = new Point(6, 427);
            btnCargar.Name = "btnCargar";
            btnCargar.Size = new Size(141, 77);
            btnCargar.TabIndex = 3;
            btnCargar.Text = "Cargar Programa";
            btnCargar.UseVisualStyleBackColor = false;
            // 
            // lblDiseñadores
            // 
            lblDiseñadores.AutoSize = true;
            lblDiseñadores.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDiseñadores.Location = new Point(134, 5);
            lblDiseñadores.Name = "lblDiseñadores";
            lblDiseñadores.Size = new Size(450, 84);
            lblDiseñadores.TabIndex = 3;
            lblDiseñadores.Text = "Diseñadores:\r\n- 23100150 Eva Guadalupe Contreras Antúnez\r\n- 23100201 Anapaula Rendón Montalvo";
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Image = Properties.Resources.Logo;
            pictureBox1.Location = new Point(59, 10);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(82, 75);
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
            groupBox2.Location = new Point(53, 108);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(1019, 798);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            groupBox2.Text = "Analizador Sintáctico";
            // 
            // btnSintaxis
            // 
            btnSintaxis.BackColor = SystemColors.ActiveCaption;
            btnSintaxis.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnSintaxis.ForeColor = SystemColors.ControlLightLight;
            btnSintaxis.Location = new Point(6, 313);
            btnSintaxis.Name = "btnSintaxis";
            btnSintaxis.Size = new Size(141, 63);
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
            rtxSintaxis.Location = new Point(153, 380);
            rtxSintaxis.Name = "rtxSintaxis";
            rtxSintaxis.ReadOnly = true;
            rtxSintaxis.Size = new Size(721, 398);
            rtxSintaxis.TabIndex = 21;
            rtxSintaxis.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(174, 52);
            label1.Name = "label1";
            label1.Size = new Size(207, 32);
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
            dgvSintaxis.Location = new Point(153, 105);
            dgvSintaxis.Name = "dgvSintaxis";
            dgvSintaxis.ReadOnly = true;
            dgvSintaxis.RowHeadersWidth = 62;
            dgvSintaxis.Size = new Size(721, 203);
            dgvSintaxis.TabIndex = 19;
            dgvSintaxis.CellDoubleClick += dgvSintaxis_CellDoubleClick;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridViewTextBoxColumn1.HeaderText = "Línea";
            dataGridViewTextBoxColumn1.MinimumWidth = 8;
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            dataGridViewTextBoxColumn1.Width = 122;
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
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(13, 13);
            tabControl1.Margin = new Padding(4, 5, 4, 5);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1144, 953);
            tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            tabPage1.BackColor = SystemColors.GradientActiveCaption;
            tabPage1.Controls.Add(pictureBox2);
            tabPage1.Controls.Add(lblDiseñadores);
            tabPage1.Controls.Add(groupBox1);
            tabPage1.Location = new Point(4, 34);
            tabPage1.Margin = new Padding(4, 5, 4, 5);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(4, 5, 4, 5);
            tabPage1.Size = new Size(1136, 915);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Analizador Léxico";
            // 
            // pictureBox2
            // 
            pictureBox2.BorderStyle = BorderStyle.FixedSingle;
            pictureBox2.Image = Properties.Resources.Logo;
            pictureBox2.Location = new Point(13, 8);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(93, 87);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 5;
            pictureBox2.TabStop = false;
            // 
            // tabPage2
            // 
            tabPage2.BackColor = SystemColors.GradientActiveCaption;
            tabPage2.Controls.Add(label2);
            tabPage2.Controls.Add(pictureBox1);
            tabPage2.Controls.Add(groupBox2);
            tabPage2.ForeColor = SystemColors.ControlText;
            tabPage2.Location = new Point(4, 34);
            tabPage2.Margin = new Padding(4, 5, 4, 5);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(4, 5, 4, 5);
            tabPage2.Size = new Size(1136, 915);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Analizador Sintáctico";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label2.Location = new Point(147, 5);
            label2.Name = "label2";
            label2.Size = new Size(450, 84);
            label2.TabIndex = 6;
            label2.Text = "Diseñadores:\r\n- 23100150 Eva Guadalupe Contreras Antúnez\r\n- 23100201 Anapaula Rendón Montalvo";
            // 
            // tabPage3
            // 
            tabPage3.BackColor = SystemColors.GradientActiveCaption;
            tabPage3.Controls.Add(groupBox3);
            tabPage3.Controls.Add(label3);
            tabPage3.Controls.Add(pictureBox3);
            tabPage3.Location = new Point(4, 34);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(1136, 915);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Analizador Semántico";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label3.Location = new Point(105, 12);
            label3.Name = "label3";
            label3.Size = new Size(450, 84);
            label3.TabIndex = 8;
            label3.Text = "Diseñadores:\r\n- 23100150 Eva Guadalupe Contreras Antúnez\r\n- 23100201 Anapaula Rendón Montalvo";
            // 
            // pictureBox3
            // 
            pictureBox3.BorderStyle = BorderStyle.FixedSingle;
            pictureBox3.Image = Properties.Resources.Logo;
            pictureBox3.Location = new Point(17, 17);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(82, 75);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 7;
            pictureBox3.TabStop = false;
            // 
            // groupBox3
            // 
            groupBox3.BackColor = SystemColors.GradientInactiveCaption;
            groupBox3.Controls.Add(label5);
            groupBox3.Controls.Add(dgvTablaSimbolos);
            groupBox3.Controls.Add(btnSemantica);
            groupBox3.Controls.Add(richTextBox1);
            groupBox3.Controls.Add(label4);
            groupBox3.Controls.Add(dgvSemantica);
            groupBox3.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            groupBox3.Location = new Point(17, 111);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(1019, 798);
            groupBox3.TabIndex = 9;
            groupBox3.TabStop = false;
            groupBox3.Text = "Analizador Semántico";
            // 
            // btnSemantica
            // 
            btnSemantica.BackColor = SystemColors.ActiveCaption;
            btnSemantica.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnSemantica.ForeColor = SystemColors.ControlLightLight;
            btnSemantica.Location = new Point(6, 245);
            btnSemantica.Name = "btnSemantica";
            btnSemantica.Size = new Size(141, 63);
            btnSemantica.TabIndex = 21;
            btnSemantica.Text = "Semantica";
            btnSemantica.UseVisualStyleBackColor = false;
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = SystemColors.Window;
            richTextBox1.BorderStyle = BorderStyle.None;
            richTextBox1.Font = new Font("Segoe UI", 8F);
            richTextBox1.Location = new Point(153, 330);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new Size(721, 174);
            richTextBox1.TabIndex = 21;
            richTextBox1.Text = "";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(153, 54);
            label4.Name = "label4";
            label4.Size = new Size(216, 32);
            label4.TabIndex = 20;
            label4.Text = "Errores Semánticos";
            // 
            // dgvSemantica
            // 
            dgvSemantica.AllowUserToAddRows = false;
            dgvSemantica.BorderStyle = BorderStyle.None;
            dgvSemantica.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSemantica.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4 });
            dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = SystemColors.Window;
            dataGridViewCellStyle7.Font = new Font("Segoe UI", 8F);
            dataGridViewCellStyle7.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = DataGridViewTriState.False;
            dgvSemantica.DefaultCellStyle = dataGridViewCellStyle7;
            dgvSemantica.Location = new Point(153, 105);
            dgvSemantica.Name = "dgvSemantica";
            dgvSemantica.ReadOnly = true;
            dgvSemantica.RowHeadersWidth = 62;
            dgvSemantica.Size = new Size(721, 203);
            dgvSemantica.TabIndex = 19;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridViewTextBoxColumn3.HeaderText = "Línea";
            dataGridViewTextBoxColumn3.MinimumWidth = 8;
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.ReadOnly = true;
            dataGridViewTextBoxColumn3.Width = 122;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTextBoxColumn4.HeaderText = "Error";
            dataGridViewTextBoxColumn4.MinimumWidth = 8;
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(168, 521);
            label5.Name = "label5";
            label5.Size = new Size(204, 32);
            label5.TabIndex = 23;
            label5.Text = "Tabla de símbolos";
            // 
            // dgvTablaSimbolos
            // 
            dgvTablaSimbolos.AllowUserToAddRows = false;
            dgvTablaSimbolos.BorderStyle = BorderStyle.None;
            dgvTablaSimbolos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTablaSimbolos.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn5, dataGridViewTextBoxColumn6, dataGridViewTextBoxColumn7, dataGridViewTextBoxColumn8 });
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = SystemColors.Window;
            dataGridViewCellStyle6.Font = new Font("Segoe UI", 8F);
            dataGridViewCellStyle6.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            dgvTablaSimbolos.DefaultCellStyle = dataGridViewCellStyle6;
            dgvTablaSimbolos.Location = new Point(153, 555);
            dgvTablaSimbolos.Name = "dgvTablaSimbolos";
            dgvTablaSimbolos.ReadOnly = true;
            dgvTablaSimbolos.RowHeadersWidth = 62;
            dgvTablaSimbolos.Size = new Size(721, 225);
            dgvTablaSimbolos.TabIndex = 22;
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewTextBoxColumn5.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridViewTextBoxColumn5.HeaderText = "#";
            dataGridViewTextBoxColumn5.MinimumWidth = 8;
            dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            dataGridViewTextBoxColumn5.ReadOnly = true;
            dataGridViewTextBoxColumn5.Width = 70;
            // 
            // dataGridViewTextBoxColumn6
            // 
            dataGridViewTextBoxColumn6.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTextBoxColumn6.HeaderText = "Nombre";
            dataGridViewTextBoxColumn6.MinimumWidth = 8;
            dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn7
            // 
            dataGridViewTextBoxColumn7.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTextBoxColumn7.HeaderText = "Tipo de Dato";
            dataGridViewTextBoxColumn7.MinimumWidth = 8;
            dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn8
            // 
            dataGridViewTextBoxColumn8.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTextBoxColumn8.HeaderText = "Valor";
            dataGridViewTextBoxColumn8.MinimumWidth = 8;
            dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            dataGridViewTextBoxColumn8.ReadOnly = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientActiveCaption;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(1167, 977);
            Controls.Add(tabControl1);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CÇ - Comme ci, comme ça (Beta)";
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
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSemantica).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvTablaSimbolos).EndInit();
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
        private TabPage tabPage3;
        private GroupBox groupBox3;
        private Button btnSemantica;
        private RichTextBox richTextBox1;
        private Label label4;
        private DataGridView dgvSemantica;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private Label label3;
        private PictureBox pictureBox3;
        private Label label5;
        private DataGridView dgvTablaSimbolos;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
    }
}

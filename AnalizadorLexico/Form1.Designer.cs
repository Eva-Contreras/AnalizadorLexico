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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            groupBox1 = new GroupBox();
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
            rtxPrograma = new RichTextBox();
            lblDiseñadores = new Label();
            pictureBox1 = new PictureBox();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSimbolos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvErrores).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.GradientInactiveCaption;
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
            groupBox1.Controls.Add(rtxPrograma);
            groupBox1.Font = new Font("Segoe UI", 14F);
            groupBox1.Location = new Point(12, 103);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1282, 844);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Analizador Léxico";
            // 
            // lblTablaErrores
            // 
            lblTablaErrores.AutoSize = true;
            lblTablaErrores.Font = new Font("Segoe UI", 12F);
            lblTablaErrores.Location = new Point(249, 540);
            lblTablaErrores.Name = "lblTablaErrores";
            lblTablaErrores.Size = new Size(183, 32);
            lblTablaErrores.TabIndex = 19;
            lblTablaErrores.Text = "Tabla de errores";
            // 
            // lblTablaSimbolos
            // 
            lblTablaSimbolos.AutoSize = true;
            lblTablaSimbolos.Font = new Font("Segoe UI", 12F);
            lblTablaSimbolos.Location = new Point(870, 540);
            lblTablaSimbolos.Name = "lblTablaSimbolos";
            lblTablaSimbolos.Size = new Size(204, 32);
            lblTablaSimbolos.TabIndex = 18;
            lblTablaSimbolos.Text = "Tabla de símbolos";
            // 
            // lblArchivoTokens
            // 
            lblArchivoTokens.AutoSize = true;
            lblArchivoTokens.Font = new Font("Segoe UI", 12F);
            lblArchivoTokens.Location = new Point(870, 38);
            lblArchivoTokens.Name = "lblArchivoTokens";
            lblArchivoTokens.Size = new Size(206, 32);
            lblArchivoTokens.TabIndex = 17;
            lblArchivoTokens.Text = "Archivo de tokens";
            // 
            // lblErrores
            // 
            lblErrores.AutoSize = true;
            lblErrores.Font = new Font("Segoe UI", 10F);
            lblErrores.Location = new Point(38, 803);
            lblErrores.Name = "lblErrores";
            lblErrores.Size = new Size(124, 28);
            lblErrores.TabIndex = 14;
            lblErrores.Text = "Total errores:";
            // 
            // lblProgramaFuente
            // 
            lblProgramaFuente.AutoSize = true;
            lblProgramaFuente.Font = new Font("Segoe UI", 12F);
            lblProgramaFuente.Location = new Point(182, 38);
            lblProgramaFuente.Name = "lblProgramaFuente";
            lblProgramaFuente.Size = new Size(193, 32);
            lblProgramaFuente.TabIndex = 16;
            lblProgramaFuente.Text = "Programa fuente";
            // 
            // dgvSimbolos
            // 
            dgvSimbolos.AllowUserToAddRows = false;
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
            dgvSimbolos.Location = new Point(682, 575);
            dgvSimbolos.Name = "dgvSimbolos";
            dgvSimbolos.ReadOnly = true;
            dgvSimbolos.RowHeadersWidth = 62;
            dgvSimbolos.Size = new Size(582, 225);
            dgvSimbolos.TabIndex = 15;
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
            btnAnalizar.Location = new Point(546, 222);
            btnAnalizar.Name = "btnAnalizar";
            btnAnalizar.Size = new Size(159, 64);
            btnAnalizar.TabIndex = 2;
            btnAnalizar.Text = "Analizar";
            btnAnalizar.UseVisualStyleBackColor = false;
            btnAnalizar.Click += btnAnalizar_Click;
            // 
            // dgvErrores
            // 
            dgvErrores.AllowUserToAddRows = false;
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
            dgvErrores.Location = new Point(38, 575);
            dgvErrores.Name = "dgvErrores";
            dgvErrores.ReadOnly = true;
            dgvErrores.RowHeadersWidth = 62;
            dgvErrores.Size = new Size(623, 225);
            dgvErrores.TabIndex = 13;
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
            btnGuardarTokens.Location = new Point(722, 427);
            btnGuardarTokens.Name = "btnGuardarTokens";
            btnGuardarTokens.Size = new Size(159, 100);
            btnGuardarTokens.TabIndex = 12;
            btnGuardarTokens.Text = "Guardar Tokens";
            btnGuardarTokens.UseVisualStyleBackColor = false;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = SystemColors.ActiveCaption;
            btnGuardar.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnGuardar.ForeColor = SystemColors.ControlLightLight;
            btnGuardar.Location = new Point(368, 427);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(159, 100);
            btnGuardar.TabIndex = 10;
            btnGuardar.Text = "Guardar Programa";
            btnGuardar.UseVisualStyleBackColor = false;
            // 
            // rtxTokens
            // 
            rtxTokens.Font = new Font("Segoe UI", 8F);
            rtxTokens.Location = new Point(722, 73);
            rtxTokens.Name = "rtxTokens";
            rtxTokens.Size = new Size(489, 338);
            rtxTokens.TabIndex = 11;
            rtxTokens.Text = "";
            // 
            // btnEditar
            // 
            btnEditar.BackColor = SystemColors.ActiveCaption;
            btnEditar.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnEditar.ForeColor = SystemColors.ControlLightLight;
            btnEditar.Location = new Point(203, 427);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new Size(159, 100);
            btnEditar.TabIndex = 9;
            btnEditar.Text = "Editar Programa";
            btnEditar.UseVisualStyleBackColor = false;
            // 
            // btnCargar
            // 
            btnCargar.BackColor = SystemColors.ActiveCaption;
            btnCargar.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnCargar.ForeColor = SystemColors.ControlLightLight;
            btnCargar.Location = new Point(38, 427);
            btnCargar.Name = "btnCargar";
            btnCargar.Size = new Size(159, 100);
            btnCargar.TabIndex = 8;
            btnCargar.Text = "Cargar Programa";
            btnCargar.UseVisualStyleBackColor = false;
            // 
            // rtxPrograma
            // 
            rtxPrograma.Font = new Font("Segoe UI", 8F);
            rtxPrograma.Location = new Point(38, 73);
            rtxPrograma.Name = "rtxPrograma";
            rtxPrograma.Size = new Size(489, 338);
            rtxPrograma.TabIndex = 8;
            rtxPrograma.Text = "";
            // 
            // lblDiseñadores
            // 
            lblDiseñadores.AutoSize = true;
            lblDiseñadores.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDiseñadores.Location = new Point(101, 9);
            lblDiseñadores.Name = "lblDiseñadores";
            lblDiseñadores.Size = new Size(450, 84);
            lblDiseñadores.TabIndex = 3;
            lblDiseñadores.Text = "Diseñadores:\r\n- 23100150 Eva Guadalupe Contreras Antúnez\r\n- 23100201 Anapaula Rendón Montalvo";
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Image = Properties.Resources.Logo;
            pictureBox1.Location = new Point(13, 18);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(82, 75);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientActiveCaption;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(1313, 959);
            Controls.Add(pictureBox1);
            Controls.Add(groupBox1);
            Controls.Add(lblDiseñadores);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CÇ - Comme ci, comme ça (Beta)";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSimbolos).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvErrores).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private GroupBox groupBox1;
        private Button btnAnalizar;
        private RichTextBox rtxPrograma;
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
    }
}

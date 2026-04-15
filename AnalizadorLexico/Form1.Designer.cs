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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            groupBox1 = new GroupBox();
            txtValor = new TextBox();
            txtToken = new TextBox();
            lblValor = new Label();
            lblToken = new Label();
            lblResultado = new Label();
            btnRecorrer = new Button();
            txtCadena = new TextBox();
            lblCadena = new Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.GradientInactiveCaption;
            groupBox1.Controls.Add(txtValor);
            groupBox1.Controls.Add(txtToken);
            groupBox1.Controls.Add(lblValor);
            groupBox1.Controls.Add(lblToken);
            groupBox1.Controls.Add(lblResultado);
            groupBox1.Controls.Add(btnRecorrer);
            groupBox1.Controls.Add(txtCadena);
            groupBox1.Controls.Add(lblCadena);
            groupBox1.Font = new Font("Segoe UI", 14F);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(591, 373);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Analizador Léxico";
            // 
            // txtValor
            // 
            txtValor.Font = new Font("Segoe UI", 10F);
            txtValor.Location = new Point(169, 282);
            txtValor.Name = "txtValor";
            txtValor.ReadOnly = true;
            txtValor.Size = new Size(298, 34);
            txtValor.TabIndex = 7;
            // 
            // txtToken
            // 
            txtToken.Font = new Font("Segoe UI", 10F);
            txtToken.Location = new Point(169, 218);
            txtToken.Name = "txtToken";
            txtToken.ReadOnly = true;
            txtToken.Size = new Size(298, 34);
            txtToken.TabIndex = 6;
            // 
            // lblValor
            // 
            lblValor.AutoSize = true;
            lblValor.Font = new Font("Segoe UI", 10F);
            lblValor.Location = new Point(70, 282);
            lblValor.Name = "lblValor";
            lblValor.Size = new Size(61, 28);
            lblValor.TabIndex = 5;
            lblValor.Text = "Valor:";
            // 
            // lblToken
            // 
            lblToken.AutoSize = true;
            lblToken.Font = new Font("Segoe UI", 10F);
            lblToken.Location = new Point(70, 218);
            lblToken.Name = "lblToken";
            lblToken.Size = new Size(67, 28);
            lblToken.TabIndex = 4;
            lblToken.Text = "Token:";
            // 
            // lblResultado
            // 
            lblResultado.AutoSize = true;
            lblResultado.Font = new Font("Segoe UI", 14F);
            lblResultado.Location = new Point(237, 157);
            lblResultado.Name = "lblResultado";
            lblResultado.Size = new Size(138, 38);
            lblResultado.TabIndex = 3;
            lblResultado.Text = "Resultado";
            // 
            // btnRecorrer
            // 
            btnRecorrer.BackColor = SystemColors.ActiveCaption;
            btnRecorrer.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnRecorrer.ForeColor = SystemColors.ControlLightLight;
            btnRecorrer.Location = new Point(412, 69);
            btnRecorrer.Name = "btnRecorrer";
            btnRecorrer.Size = new Size(159, 64);
            btnRecorrer.TabIndex = 2;
            btnRecorrer.Text = "Recorrer";
            btnRecorrer.UseVisualStyleBackColor = false;
            btnRecorrer.Click += btnRecorrer_Click;
            // 
            // txtCadena
            // 
            txtCadena.Font = new Font("Segoe UI", 10F);
            txtCadena.Location = new Point(79, 85);
            txtCadena.Name = "txtCadena";
            txtCadena.Size = new Size(298, 34);
            txtCadena.TabIndex = 1;
            // 
            // lblCadena
            // 
            lblCadena.AutoSize = true;
            lblCadena.Font = new Font("Segoe UI", 10F);
            lblCadena.Location = new Point(134, 54);
            lblCadena.Name = "lblCadena";
            lblCadena.Size = new Size(183, 28);
            lblCadena.TabIndex = 0;
            lblCadena.Text = "Ingrese una cadena:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientActiveCaption;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(618, 399);
            Controls.Add(groupBox1);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CÇ - Comme ci, comme ça (Beta)";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private GroupBox groupBox1;
        private Label lblCadena;
        private Label lblResultado;
        private Button btnRecorrer;
        private TextBox txtCadena;
        private TextBox txtValor;
        private TextBox txtToken;
        private Label lblValor;
        private Label lblToken;
    }
}

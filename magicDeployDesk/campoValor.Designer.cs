
namespace magicDeployDesk
{
    partial class campoValor
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelPropiedadDefault = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_nombreCampo = new System.Windows.Forms.Label();
            this.txt_valorCampo = new System.Windows.Forms.TextBox();
            this.panelPropiedadDefault.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelPropiedadDefault
            // 
            this.panelPropiedadDefault.ColumnCount = 1;
            this.panelPropiedadDefault.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelPropiedadDefault.Controls.Add(this.lbl_nombreCampo, 0, 0);
            this.panelPropiedadDefault.Controls.Add(this.txt_valorCampo, 0, 1);
            this.panelPropiedadDefault.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPropiedadDefault.Location = new System.Drawing.Point(0, 0);
            this.panelPropiedadDefault.Name = "panelPropiedadDefault";
            this.panelPropiedadDefault.RowCount = 2;
            this.panelPropiedadDefault.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelPropiedadDefault.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.panelPropiedadDefault.Size = new System.Drawing.Size(436, 49);
            this.panelPropiedadDefault.TabIndex = 3;
            // 
            // lbl_nombreCampo
            // 
            this.lbl_nombreCampo.AutoSize = true;
            this.lbl_nombreCampo.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbl_nombreCampo.Location = new System.Drawing.Point(3, 0);
            this.lbl_nombreCampo.Name = "lbl_nombreCampo";
            this.lbl_nombreCampo.Size = new System.Drawing.Size(44, 17);
            this.lbl_nombreCampo.TabIndex = 0;
            this.lbl_nombreCampo.Text = "Campo";
            this.lbl_nombreCampo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_nombreCampo.UseCompatibleTextRendering = true;
            // 
            // txt_valorCampo
            // 
            this.txt_valorCampo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_valorCampo.Location = new System.Drawing.Point(3, 20);
            this.txt_valorCampo.Name = "txt_valorCampo";
            this.txt_valorCampo.Size = new System.Drawing.Size(430, 23);
            this.txt_valorCampo.TabIndex = 1;
            // 
            // campoValor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelPropiedadDefault);
            this.Name = "campoValor";
            this.Size = new System.Drawing.Size(436, 49);
            this.panelPropiedadDefault.ResumeLayout(false);
            this.panelPropiedadDefault.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Label lbl_nombreCampo;
        public System.Windows.Forms.TextBox txt_valorCampo;
        public System.Windows.Forms.TableLayoutPanel panelPropiedadDefault;
    }
}

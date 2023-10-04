using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace magicDeployDesk
{
    public partial class campoValor : UserControl
    {

        public campoValor(string nombre, string valor,bool soloLectura)
        {
            InitializeComponent();
            lbl_nombreCampo.Text = nombre;
            txt_valorCampo.Text = valor;
            txt_valorCampo.Enabled = !soloLectura;
            this.Name = "campoValor_" + nombre;
        }
        public campoValor(campoConfig campo, bool soloLectura)
        {
            InitializeComponent();
            lbl_nombreCampo.Text = campo.nombre;
            txt_valorCampo.Text = campo.valor;
            txt_valorCampo.Enabled = !soloLectura;
            this.Name = "campoValor_" + campo.nombre;
        }
    }
}

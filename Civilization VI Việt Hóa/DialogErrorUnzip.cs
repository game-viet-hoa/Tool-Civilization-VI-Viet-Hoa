using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Civilization_VI_Việt_Hóa
{
    public partial class DialogErrorUnzip : Form
    {
        private string strMaster;

        public DialogErrorUnzip()
        {
            InitializeComponent();
        }

        public DialogErrorUnzip(string strMaster)
        {
            this.strMaster = strMaster;
            InitializeComponent();

            linkLabel1.Text = "https://github.com/xkvnn/Civilization-VI-Viet-Hoa/archive/" + strMaster + ".zip";

            label3.Text = "Đổi tên file vừa tải về sang '" + strMaster + ".zip' rồi thực hiện lại.";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/xkvnn/Civilization-VI-Viet-Hoa/archive/" + strMaster + ".zip");
        }
    }
}

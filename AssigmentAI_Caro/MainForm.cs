using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssigmentAI_Caro
{
    public partial class frmMain : Form
    {
        private BanCo banCo;
        public frmMain()
        {
            InitializeComponent();
        }

        public void vanMoi()
        {
            banCo.lamMoiBanCo();
            banCo.nguoiThang = NguoiChoi.chuadi;
            banCo.veBanCo();
        }

        public void hienThongBao(int i)
        {
            switch (i)
            {
                case 1:
                    MessageBox.Show("Bạn đã thắng! \nBạn có muốn chơi tiếp không?", "Thông Báo", MessageBoxButtons.YesNo);
                    break;

                case 2:
                    MessageBox.Show("Rất tiếc, Bạn đã thua!", "Thông Báo", MessageBoxButtons.OK);
                    break;

                case 3:
                    
                    break;
            }
        }

        private void pictureBox4_MouseHover(object sender, EventArgs e)
        {
            pictureBox4.Image = Image.FromFile("..//..//New game.png");
        }

        private void pictureBox4_MouseLeave(object sender, EventArgs e)
        {
            pictureBox4.Image = Image.FromFile("..//..//New game copy.png");
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            vanMoi();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            banCo = new BanCo(this);
            this.vanMoi();
        }

        private void frmMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                banCo.clickChonNuocDi(e);
            }
        }
    }
}

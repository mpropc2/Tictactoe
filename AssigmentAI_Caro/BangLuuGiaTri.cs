using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssigmentAI_Caro
{
    class BangLuuGiaTri
    {
        public int rong, cao;
        public int[,] bangGiaTri; //ma trận 2 chiều lưu giá trị các ô được định giá trên bàn cờ

        public BangLuuGiaTri(BanCo banco)
        {
            this.rong = banco.rong;
            this.cao = banco.cao;
            bangGiaTri = new int[cao, rong];
        }

        //làm mới bảng giá trị
        public void lamMoiBang()
        {
            for(int dong = 0; dong < cao; dong++)
            {
                for(int cot = 0; cot < rong; cot++)
                {
                    bangGiaTri[dong, cot] = 0;
                }
            }
        }

        //chọn ra Node có điểm lớn nhất
        public Node GetMaxNode()
        {
            int r, c, MaxValue = 0;
            Node n = new Node();

            for (r = 0; r < cao; r++)
                for (c = 0; c < rong; c++)
                    if (bangGiaTri[r, c] > MaxValue)
                    {
                        n.chiSoDong = r; n.chiSoCot = c;
                        MaxValue = bangGiaTri[r, c];
                    }

            return n;
        }
    }
}

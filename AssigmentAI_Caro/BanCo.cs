using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssigmentAI_Caro
{
    public enum NguoiChoi  {Nguoi = 1, May = 2, chuadi = 0, rangoai = -1};
    public struct Node
    {
        public int chiSoDong;
        public int chiSoCot;
    }
    class BanCo
    {
        public frmMain formBanCo;
        public NguoiChoi nguoiChoiHienTai = NguoiChoi.chuadi;
        public NguoiChoi[,] trangThaiBanCo;
        public int rong, cao;
        public NguoiChoi nguoiThang = NguoiChoi.chuadi;
        public int messId = 0;
        public BangLuuGiaTri bangLuuGiaTri;
        //hình ảnh
        public Bitmap bmpNguoi = new Bitmap("..//..//bmpHuman.bmp");
        public Bitmap bmpMay = new Bitmap("..//..//bmpMachine.bmp");
        public Bitmap bmpChuaDi = new Bitmap("..//..//bmpNone.bmp");

        //ham khoi tao
        public BanCo(frmMain formBanCo)
        {
            rong = 30;
            cao = 15;
            trangThaiBanCo = new NguoiChoi[cao, rong];
            bangLuuGiaTri = new BangLuuGiaTri(this);
            lamMoiBanCo();
            this.formBanCo = formBanCo;
        }

        //thiết lập lại trạng thái bàn cờ
        public void lamMoiBanCo()
        {
            int r, c;
            //Thiet lap lai gia tri trang thai cac o tren ban co
            for (r = 0; r < cao; r++)
            {
                for (c = 0; c < rong; c++)
                {
                    trangThaiBanCo[r, c] = NguoiChoi.chuadi;
                }
            }

            nguoiChoiHienTai = NguoiChoi.Nguoi;
        }

        //vẽ bàn cờ
        public void veBanCo()
        {
            Graphics g = formBanCo.CreateGraphics();// tu khoa hinh ve do hoa 
            Rectangle rect;//hinh chu nhat 

            for (int r = 0; r < cao; r++)
                for (int c = 0; c < rong; c++)
                //in ra so dong va so cot 
                {
                    rect = new Rectangle(c*20, r*20 + 50, 20, 20);
                    if (trangThaiBanCo[r, c] == NguoiChoi.chuadi)// la o bang 
                        g.DrawImage(bmpChuaDi, rect);
                    if (trangThaiBanCo[r, c] == NguoiChoi.Nguoi)//  nut tron la quan ta  
                        g.DrawImage(bmpNguoi, rect);
                    if (trangThaiBanCo[r, c] == NguoiChoi.May)// quan dich
                        g.DrawImage(bmpMay, rect);
                }
        }

        //sự kiện click chuột
        public void clickChonNuocDi(MouseEventArgs e)
        {
            Random rand = new Random();
            int count = rand.Next(4);
            int r = ((e.Y - 50)/ 20);
            int c = (e.X/ 20);
            Node node = new Node();

            if (r <= 15 && c <= 30)
            {
                //chỉ cho chọn khi ô đó chưa có người đi
                if (trangThaiBanCo[r, c] == NguoiChoi.chuadi)
                {
                    //Người chơi đi cờ
                    if (nguoiChoiHienTai == NguoiChoi.Nguoi && nguoiThang == NguoiChoi.chuadi)
                    {
                        trangThaiBanCo[r, c] = nguoiChoiHienTai;
                        nguoiChoiHienTai = NguoiChoi.May;

                        nguoiThang = kiemtraKetThuc(r, c);
                        if (nguoiThang != NguoiChoi.chuadi)
                        {
                            if (nguoiThang == NguoiChoi.Nguoi) formBanCo.hienThongBao(1);
                            if (nguoiThang == NguoiChoi.May) formBanCo.hienThongBao(2);
                        }
                    }

                    //máy đi
                    if (nguoiChoiHienTai == NguoiChoi.May && nguoiThang == NguoiChoi.chuadi)
                    {

                        // Tim nuoc di chien thang.
                        bangLuuGiaTri.lamMoiBang();
                        timDuongDi();

                        if (Win) // Tim thay.
                        {
                            node = WinMoves[1];
                        }
                        else
                        {
                            bangLuuGiaTri.lamMoiBang();
                            luongGiaBanCo(NguoiChoi.May);
                            node = bangLuuGiaTri.GetMaxNode();
                            if (!Lose)
                                for (int i = 0; i < count; i++)
                                {
                                    bangLuuGiaTri.bangGiaTri[node.chiSoDong, node.chiSoCot] = 0;
                                    node = bangLuuGiaTri.GetMaxNode();
                                }
                        }
                        // máy đi quân
                        r = node.chiSoDong; c = node.chiSoCot;
                        trangThaiBanCo[r, c] = nguoiChoiHienTai;
                        nguoiChoiHienTai = NguoiChoi.Nguoi;

                        // Kiem tra tran dau ket thuc chua ?
                        nguoiThang = kiemtraKetThuc(r, c);
                        if (nguoiThang != NguoiChoi.chuadi)
                        {
                            if (nguoiThang == NguoiChoi.Nguoi) formBanCo.hienThongBao(1);
                            if (nguoiThang == NguoiChoi.May) formBanCo.hienThongBao(2);
                        }
                    }
                }
                veBanCo();
            }
        }

        //các mức điểm phòng thủ tấn công
        public int[] DScore = new int[5] { 0, 1, 9, 85, 769 };
        public int[] AScore = new int[5] { 0, 2, 28, 256, 2308 };

        //lượng giá cho bàn cờ để chọn nước đi cho máy
        private void luongGiaBanCo(NguoiChoi nguoichoi)
        {
            int rw, cl, i;
            int soNuocNguoi, soNuocMay;
            
            // Luong gia cho hang.
            for (rw = 0; rw < cao; rw++)
                for (cl = 1; cl < rong - 4; cl++)
                {
                    soNuocNguoi = 0; soNuocMay = 0;
                    for (i = 0; i < 5; i++)
                    {
                        if (trangThaiBanCo[rw, cl + i] == NguoiChoi.Nguoi) soNuocNguoi++;
                        if (trangThaiBanCo[rw, cl + i] == NguoiChoi.May) soNuocMay++;
                    }
                    // Luong gia...
                    if (soNuocNguoi * soNuocMay == 0 && soNuocNguoi != soNuocMay)
                        for (i = 0; i < 5; i++)
                            if (trangThaiBanCo[rw, cl + i] == NguoiChoi.chuadi)
                            {
                                if (soNuocMay == 0)
                                {
                                    if (nguoichoi == NguoiChoi.May) bangLuuGiaTri.bangGiaTri[rw, cl + i] += DScore[soNuocNguoi];
                                    else bangLuuGiaTri.bangGiaTri[rw, cl + i] += AScore[soNuocNguoi];
                                }
                                if (soNuocNguoi == 0)
                                {
                                    if (nguoichoi == NguoiChoi.Nguoi) bangLuuGiaTri.bangGiaTri[rw, cl + i] += DScore[soNuocMay];
                                    else bangLuuGiaTri.bangGiaTri[rw, cl + i] += AScore[soNuocMay];
                                }
                                if ((soNuocNguoi == 4 || soNuocMay == 4)
                                    && (trangThaiBanCo[rw, cl + i - 1] == NguoiChoi.chuadi || trangThaiBanCo[rw, cl + i + 1] == NguoiChoi.chuadi))
                                    bangLuuGiaTri.bangGiaTri[rw, cl + i] *= 2;
                            }
                }
           
            // Luong gia cho cot.
            for (cl = 0; cl < rong; cl++)
                for (rw = 1; rw < cao - 5; rw++)
                {
                    soNuocNguoi = 0; soNuocMay = 0;
                    for (i = 0; i < 5; i++)
                    {
                        if (trangThaiBanCo[rw + i, cl] == NguoiChoi.Nguoi) soNuocNguoi++;
                        if (trangThaiBanCo[rw + i, cl] == NguoiChoi.May) soNuocMay++;
                    }
                    // Luong gia...
                    if (soNuocNguoi * soNuocMay == 0 && soNuocMay != soNuocNguoi)
                        for (i = 0; i < 5; i++)
                            if (trangThaiBanCo[rw + i, cl] == NguoiChoi.chuadi)
                            {
                                if (soNuocMay == 0)
                                {
                                    if (nguoichoi == NguoiChoi.May) bangLuuGiaTri.bangGiaTri[rw + i, cl] += DScore[soNuocNguoi];
                                    else bangLuuGiaTri.bangGiaTri[rw + i, cl] += AScore[soNuocNguoi];
                                }
                                if (soNuocNguoi == 0)
                                {
                                    if (nguoichoi == NguoiChoi.Nguoi) bangLuuGiaTri.bangGiaTri[rw + i, cl] += DScore[soNuocMay];
                                    else bangLuuGiaTri.bangGiaTri[rw + i, cl] += AScore[soNuocMay];

                                }
                                if ((soNuocNguoi == 4 || soNuocMay == 4)
                                    && (trangThaiBanCo[rw + i - 1, cl] == NguoiChoi.chuadi || trangThaiBanCo[rw + i + 1, cl] == NguoiChoi.chuadi))
                                    bangLuuGiaTri.bangGiaTri[rw + i, cl] *= 2;
                            }
                }


            // Luong gia cho duong cheo xuong.
            for (rw = 1; rw < cao - 5; rw++)
                for (cl = 1; cl < rong - 4; cl++)
                {
                    soNuocNguoi = 0; soNuocMay = 0;
                    for (i = 0; i < 5; i++)
                    {
                        if (trangThaiBanCo[rw + i, cl + i] == NguoiChoi.Nguoi) soNuocNguoi++;
                        if (trangThaiBanCo[rw + i, cl + i] == NguoiChoi.May) soNuocMay++;
                    }
                    // Luong gia...
                    if (soNuocNguoi * soNuocMay == 0 && soNuocMay != soNuocNguoi)
                        for (i = 0; i < 5; i++)
                            if (trangThaiBanCo[rw + i, cl + i] == NguoiChoi.chuadi)
                            {
                                if (soNuocMay == 0)
                                {
                                    if (nguoichoi == NguoiChoi.May) bangLuuGiaTri.bangGiaTri[rw + i, cl + i] += DScore[soNuocNguoi];
                                    else bangLuuGiaTri.bangGiaTri[rw + i, cl + i] += AScore[soNuocNguoi];
                                }
                                if (soNuocNguoi == 0)
                                {
                                    if (nguoichoi == NguoiChoi.Nguoi) bangLuuGiaTri.bangGiaTri[rw + i, cl + i] += DScore[soNuocMay];
                                    else bangLuuGiaTri.bangGiaTri[rw + i, cl + i] += AScore[soNuocMay];
                                }
                                if ((soNuocNguoi == 4 || soNuocMay == 4)
                                    && (trangThaiBanCo[rw + i - 1, cl + i - 1] == NguoiChoi.chuadi || trangThaiBanCo[rw + i + 1, cl + i + 1] == NguoiChoi.chuadi))
                                    bangLuuGiaTri.bangGiaTri[rw + i, cl + i] *= 2;
                            }
                }

            // Luong gia cho duong cheo len.
            for (rw = 5; rw < cao - 4; rw++)
                for (cl = 1; cl < rong - 4; cl++)
                {
                    soNuocMay = 0; soNuocNguoi = 0;
                    for (i = 0; i < 5; i++)
                    {
                        if (trangThaiBanCo[rw - i, cl + i] == NguoiChoi.Nguoi) soNuocNguoi++;
                        if (trangThaiBanCo[rw - i, cl + i] == NguoiChoi.May) soNuocMay++;
                    }
                    // Luong gia...
                    if (soNuocNguoi * soNuocMay == 0 && soNuocNguoi != soNuocMay)
                        for (i = 0; i < 5; i++)
                            if (trangThaiBanCo[rw - i, cl + i] == NguoiChoi.chuadi)
                            {
                                if (soNuocMay == 0)
                                {
                                    if (nguoichoi == NguoiChoi.May) bangLuuGiaTri.bangGiaTri[rw - i, cl + i] += DScore[soNuocNguoi];
                                    else bangLuuGiaTri.bangGiaTri[rw - i, cl + i] += AScore[soNuocNguoi];
                                }
                                if (soNuocNguoi == 0)
                                {
                                    if (nguoichoi == NguoiChoi.Nguoi) bangLuuGiaTri.bangGiaTri[rw - i, cl + i] += DScore[soNuocMay];
                                    else bangLuuGiaTri.bangGiaTri[rw - i, cl + i] += AScore[soNuocMay];
                                }
                                if ((soNuocNguoi == 4 || soNuocMay == 4)
                                    && (trangThaiBanCo[rw - i + 1, cl + i - 1] == NguoiChoi.chuadi || trangThaiBanCo[rw - i - 1, cl + i + 1] == NguoiChoi.chuadi))
                                    bangLuuGiaTri.bangGiaTri[rw - i, cl + i] *= 2;
                            }
                }
        }

        // Sinh nuoc di - do thong minh cua may.
        public int Depth = 0;
        static public int MaxDepth = 21;
        static public int MaxBreadth = 8;

        public Node[] WinMoves = new Node[MaxDepth + 1];
        public Node[] MyMoves = new Node[MaxBreadth + 1];
        public Node[] HisMoves = new Node[MaxBreadth + 1];
        public bool Win, Lose;

        //Sinh nước đi cho máy
        public void sinhNuocDi()
        {
            if (Depth >= MaxDepth) return;
            Depth++;
            bool lose = false;
            Win = false;

            Node MyNode = new Node();   // nước đi của người chơi
            Node HisNode = new Node();  // nước đi của máy
            int count = 0;

            // lượng giá cho bàn cờ
            luongGiaBanCo(NguoiChoi.May);

            // Lay MaxBreadth nuoc di tot nhat.
            for (int i = 1; i <= MaxBreadth; i++)
            {
                MyNode = bangLuuGiaTri.GetMaxNode();
                MyMoves[i] = MyNode;
                bangLuuGiaTri.bangGiaTri[MyNode.chiSoDong, MyNode.chiSoCot] = 0;
            }
            // Lay nuoc di ra khoi danh sach - Danh thu nuoc di.
            count = 0;
            while (count < MaxBreadth)
            {
                count++;
                MyNode = MyMoves[count];
                WinMoves.SetValue(MyNode, Depth);
                trangThaiBanCo[MyNode.chiSoDong, MyNode.chiSoCot] = NguoiChoi.May;

                //Tìm nước đi tối ưu của đối thủ
                bangLuuGiaTri.lamMoiBang();
                luongGiaBanCo(NguoiChoi.Nguoi);
                for (int i = 1; i <= MaxBreadth; i++)
                {
                    HisNode = bangLuuGiaTri.GetMaxNode();
                    HisMoves[i] = HisNode;
                    bangLuuGiaTri.bangGiaTri[HisNode.chiSoDong, HisNode.chiSoCot] = 0;
                }

                for (int i = 1; i <= MaxBreadth; i++)
                {
                    HisNode = HisMoves[i];
                    trangThaiBanCo[HisNode.chiSoDong, HisNode.chiSoCot] = NguoiChoi.Nguoi;
                    // Kiem tra ket qua nuoc di.
                    if (kiemtraKetThuc(MyNode.chiSoDong, MyNode.chiSoCot) == NguoiChoi.May)
                        Win = true;
                    if (kiemtraKetThuc(HisNode.chiSoDong, HisNode.chiSoCot) == NguoiChoi.Nguoi)
                        lose = true;

                    if (lose)
                    {
                        //loại bỏ nước đi thử
                        Lose = true;
                        trangThaiBanCo[HisNode.chiSoDong, HisNode.chiSoCot] = NguoiChoi.chuadi;
                        trangThaiBanCo[MyNode.chiSoDong, MyNode.chiSoCot] = NguoiChoi.chuadi;
                        return;
                    }

                    if (Win)
                    {
                        // Loai nuoc di thu.
                        trangThaiBanCo[HisNode.chiSoDong, HisNode.chiSoCot] = NguoiChoi.chuadi;
                        trangThaiBanCo[MyNode.chiSoDong, MyNode.chiSoCot] = NguoiChoi.chuadi;
                        return;
                    }
                    else sinhNuocDi(); // tim tiep.
                    // Loai nuoc di thu.
                    trangThaiBanCo[HisNode.chiSoDong, HisNode.chiSoCot] = NguoiChoi.chuadi;
                }

                trangThaiBanCo[MyNode.chiSoDong, MyNode.chiSoCot] = NguoiChoi.chuadi;
            }
        }

        //tìm đường đi cho máy từ hàm sinh nước đi
        public void timDuongDi()
        {
            Win = Lose = false;
            // Xoa mang duong di.
            WinMoves = new Node[MaxDepth + 1];
            for (int i = 0; i <= MaxDepth; i++)
                WinMoves[i] = new Node();

            // Xoa stack.
            for (int i = 0; i < MaxBreadth; i++)
                MyMoves[i] = new Node();

            Depth = 0;
            sinhNuocDi();
            if (Win && !Lose) formBanCo.hienThongBao(3);
        }

        //kiểm tra xem ván cờ đã kết thúc chưa
        public NguoiChoi kiemtraKetThuc(int rw, int cl)
        {
            bool nguoi;
            bool may;

            //kiểm tra trên hàng
            for (int c = 1; c <= rong - 5; c++)
            {
                nguoi = true;
                may = true;

                for (int i = 0; i < 5; i++)
                {
                    if (trangThaiBanCo[rw, c + i] != NguoiChoi.Nguoi)
                        nguoi = false;
                    if (trangThaiBanCo[rw, c + i] != NguoiChoi.May)
                        may = false;
                }

                if (nguoi) return NguoiChoi.Nguoi;
                if (may) return NguoiChoi.May;
            }

            //kiểm tra trên cột
            for (int r = 1; r <= cao - 5; r++)
            {
                nguoi = true;
                may = true;

                for (int i = 0; i < 5; i++)
                {
                    if (trangThaiBanCo[r + i, cl] != NguoiChoi.Nguoi)
                        nguoi = false;
                    if (trangThaiBanCo[r + i, cl] != NguoiChoi.May)
                        may = false;
                }

                if (nguoi) return NguoiChoi.Nguoi;
                if (may) return NguoiChoi.May;
            }

            //kiểm tra đường chéo xuống
            int dong = rw;
            int cot = cl;
            //di chuyển về đầu đường chéo
            while (dong > 1 && cot > 1)
            { 
                dong--; 
                cot--; 
            }

            while (dong <= cao - 5 && cot <= rong - 5)
            {
                nguoi = true;
                may = true;
                for (int i = 0; i < 5; i++)
                {
                    if (trangThaiBanCo[dong + i, cot + i] != NguoiChoi.Nguoi)
                        nguoi = false;
                    if (trangThaiBanCo[dong + i, cot + i] != NguoiChoi.May)
                        may = false;
                }
                if (nguoi) return NguoiChoi.Nguoi;
                if (may) return NguoiChoi.May;
                dong++; cot++;
            }

            //kiểm tra đường chéo lên
            dong = rw; cot = cl;
            //di chuyển về đầu đường chéo
            //dong < cao - 1 vì khi dòng đã tăng lên bằng chiều cao thì mới kiểm tra điều kiện
            //nên giới hạn lại là cao - 1
            while (dong < cao-1 && cot > 1) 
            { 
                dong++; 
                cot--; 
            }
            while (dong >= 5 && cot <= rong - 5)
            {
                nguoi = true;
                may = true;
                for (int i = 0; i < 5; i++)
                {
                    if (trangThaiBanCo[dong - i, cot + i] != NguoiChoi.Nguoi)
                        nguoi = false;
                    if (trangThaiBanCo[dong - i, cot + i] != NguoiChoi.May)
                        may = false;
                }
                if (nguoi) return NguoiChoi.Nguoi;
                if (may) return NguoiChoi.May;
                dong--; cot++;
            }

            return NguoiChoi.chuadi;
        }
    }
}

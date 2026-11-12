using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace BKYFix;

public class ZhuChuang : Form
{
    static readonly Color diSe = Color.FromArgb(243, 240, 232);
    static readonly Color kaSe = Color.FromArgb(255, 252, 247);
    static readonly Color ziSe = Color.FromArgb(30, 36, 48);
    static readonly Color huiSe = Color.FromArgb(94, 102, 114);
    static readonly Color xianSe = Color.FromArgb(217, 211, 199);
    static readonly Color lvSe = Color.FromArgb(14, 107, 79);
    static readonly Color lvLiang = Color.FromArgb(18, 128, 94);
    static readonly Color lvAn = Color.FromArgb(10, 84, 62);

    Panel yeAuto;
    Panel yeTuo;
    TuoKuang tuoKuang;
    Button btnAuto;
    Button btnTuo;
    Button btnKai;
    Label lblZt;
    TextBox txtJi;
    Label lblVer;
    bool mang;
    bool zaiAuto = true;

    public ZhuChuang()
    {
        Text = "百科园FixTool";
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        BackColor = diSe;
        ForeColor = ziSe;
        ClientSize = new Size(920, 660);
        MinimumSize = new Size(820, 600);
        AutoScaleMode = AutoScaleMode.Dpi;
        DoubleBuffered = true;

        TableLayoutPanel gen = new TableLayoutPanel();
        gen.Dock = DockStyle.Fill;
        gen.ColumnCount = 1;
        gen.RowCount = 4;
        gen.BackColor = diSe;
        gen.Padding = new Padding(22, 18, 22, 16);
        gen.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
        gen.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        gen.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        gen.RowStyles.Add(new RowStyle(SizeType.Absolute, 190));
        Controls.Add(gen);

        gen.Controls.Add(ZuoTou(out lblVer), 0, 0);
        gen.Controls.Add(ZuoQie(out btnAuto, out btnTuo), 0, 1);

        Panel ye = new Panel();
        ye.Dock = DockStyle.Fill;
        ye.BackColor = diSe;
        ye.Margin = new Padding(0, 4, 0, 8);
        yeAuto = ZuoAutoYe(out btnKai, out lblZt);
        yeTuo = ZuoTuoYe(out tuoKuang);
        ye.Controls.Add(yeAuto);
        ye.Controls.Add(yeTuo);
        gen.Controls.Add(ye, 0, 2);
        gen.Controls.Add(ZuoJiLu(out txtJi), 0, 3);

        btnAuto.Click += delegate { HuanYe(true); };
        btnTuo.Click += delegate { HuanYe(false); };
        btnKai.Click += async delegate { await ZiDong(); };
        GuaTuo(this);
        HuanYe(true);

        Load += delegate
        {
            lblVer.Text = NetBanben.Shuoming();
            if (!NetBanben.You48)
            {
                btnKai.Enabled = false;
                JiLu("本机没有 .NET Framework 4.8 已停用迁移");
            }
            else
            {
                JiLu("自动侦测模式:自动查找考试客户端");
                JiLu("拖入识别:手动拖入客户端");
            }
        };
    }

    Panel ZuoTou(out Label ver)
    {
        TableLayoutPanel tou = new TableLayoutPanel();
        tou.Dock = DockStyle.Fill;
        tou.ColumnCount = 1;
        tou.RowCount = 2;
        tou.BackColor = diSe;
        tou.Margin = new Padding(0);
        tou.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        tou.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));

        Label biao = new Label();
        biao.Text = "百科园FixTool";
        biao.Dock = DockStyle.Fill;
        biao.Font = new Font(Font.FontFamily, 16F, FontStyle.Bold, GraphicsUnit.Point);
        biao.ForeColor = ziSe;
        biao.BackColor = diSe;
        biao.TextAlign = ContentAlignment.MiddleLeft;

        ver = new Label();
        ver.Text = "正在检查 .NET Framework…";
        ver.Dock = DockStyle.Fill;
        ver.ForeColor = huiSe;
        ver.BackColor = diSe;
        ver.TextAlign = ContentAlignment.MiddleLeft;

        tou.Controls.Add(biao, 0, 0);
        tou.Controls.Add(ver, 0, 1);
        return tou;
    }

    Control ZuoQie(out Button ziDong, out Button tuo)
    {
        TableLayoutPanel tiao = new TableLayoutPanel();
        tiao.Dock = DockStyle.Fill;
        tiao.ColumnCount = 2;
        tiao.RowCount = 1;
        tiao.BackColor = diSe;
        tiao.Margin = new Padding(0, 0, 0, 8);
        tiao.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        tiao.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        ziDong = ZuoQieNiu("自动侦测");
        tuo = ZuoQieNiu("拖入识别");
        ziDong.Margin = new Padding(0, 4, 6, 4);
        tuo.Margin = new Padding(6, 4, 0, 4);
        tiao.Controls.Add(ziDong, 0, 0);
        tiao.Controls.Add(tuo, 1, 0);
        return tiao;
    }

    Panel ZuoAutoYe(out Button kai, out Label zt)
    {
        TableLayoutPanel ye = new TableLayoutPanel();
        ye.Dock = DockStyle.Fill;
        ye.ColumnCount = 1;
        ye.RowCount = 3;
        ye.BackColor = kaSe;
        ye.Padding = new Padding(28, 22, 28, 18);
        ye.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
        ye.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        ye.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        Label shuo = new Label();
        shuo.Dock = DockStyle.Fill;
        shuo.BackColor = kaSe;
        shuo.ForeColor = huiSe;
        shuo.Text = "本工具用于修复百科园缺失.NET3.5以及安装缓慢导致的百科园无法启动XD";

        kai = ZuoLvNiu("开始侦测并启动");
        kai.Dock = DockStyle.Fill;
        kai.Margin = new Padding(0, 6, 0, 8);

        zt = new Label();
        zt.Dock = DockStyle.Fill;
        zt.ForeColor = ziSe;
        zt.BackColor = kaSe;
        zt.Text = "尚未侦测";
        zt.TextAlign = ContentAlignment.TopLeft;

        ye.Controls.Add(shuo, 0, 0);
        ye.Controls.Add(kai, 0, 1);
        ye.Controls.Add(zt, 0, 2);
        return ye;
    }

    Panel ZuoTuoYe(out TuoKuang kuang)
    {
        TableLayoutPanel ye = new TableLayoutPanel();
        ye.Dock = DockStyle.Fill;
        ye.ColumnCount = 1;
        ye.RowCount = 2;
        ye.BackColor = kaSe;
        ye.Padding = new Padding(28, 22, 28, 18);
        ye.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        ye.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        Label shuo = new Label();
        shuo.Dock = DockStyle.Fill;
        shuo.BackColor = kaSe;
        shuo.ForeColor = huiSe;
        shuo.Text = "把考试客户端快捷方式拖进窗口 程序解析快捷方式指向的目录 并写入修复配置";

        kuang = new TuoKuang();
        kuang.Dock = DockStyle.Fill;
        kuang.Margin = new Padding(0, 4, 0, 0);
        ye.Controls.Add(shuo, 0, 0);
        ye.Controls.Add(kuang, 0, 1);
        return ye;
    }

    Control ZuoJiLu(out TextBox ji)
    {
        TableLayoutPanel he = new TableLayoutPanel();
        he.Dock = DockStyle.Fill;
        he.ColumnCount = 1;
        he.RowCount = 2;
        he.BackColor = diSe;
        he.Margin = new Padding(0);
        he.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        he.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        Label biao = new Label();
        biao.Text = "记录";
        biao.Dock = DockStyle.Fill;
        biao.ForeColor = huiSe;
        biao.BackColor = diSe;
        biao.TextAlign = ContentAlignment.MiddleLeft;

        ji = new TextBox();
        ji.Dock = DockStyle.Fill;
        ji.Multiline = true;
        ji.ReadOnly = true;
        ji.ScrollBars = ScrollBars.Vertical;
        ji.BorderStyle = BorderStyle.None;
        ji.BackColor = Color.FromArgb(250, 248, 244);
        ji.ForeColor = ziSe;
        ji.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        ji.HideSelection = false;

        Panel hezi = new Panel();
        hezi.Dock = DockStyle.Fill;
        hezi.BackColor = Color.FromArgb(250, 248, 244);
        hezi.Padding = new Padding(10, 8, 10, 8);
        hezi.Margin = new Padding(0);
        hezi.Controls.Add(ji);

        he.Controls.Add(biao, 0, 0);
        he.Controls.Add(hezi, 0, 1);
        return he;
    }

    Button ZuoQieNiu(string wen)
    {
        Button niu = new Button();
        niu.Text = wen;
        niu.Dock = DockStyle.Fill;
        niu.FlatStyle = FlatStyle.Flat;
        niu.Cursor = Cursors.Hand;
        niu.Font = new Font(Font.FontFamily, 11F, FontStyle.Bold, GraphicsUnit.Point);
        niu.FlatAppearance.BorderSize = 1;
        return niu;
    }

    Button ZuoLvNiu(string wen)
    {
        Button niu = new Button();
        niu.Text = wen;
        niu.Height = 46;
        niu.FlatStyle = FlatStyle.Flat;
        niu.BackColor = lvSe;
        niu.ForeColor = Color.White;
        niu.Cursor = Cursors.Hand;
        niu.Font = new Font(Font.FontFamily, 11F, FontStyle.Bold, GraphicsUnit.Point);
        niu.Margin = new Padding(0, 4, 0, 16);
        niu.FlatAppearance.BorderSize = 0;
        niu.FlatAppearance.MouseOverBackColor = lvLiang;
        niu.FlatAppearance.MouseDownBackColor = lvAn;
        return niu;
    }

    void HuanYe(bool ziDong)
    {
        zaiAuto = ziDong;
        yeAuto.Visible = ziDong;
        yeTuo.Visible = !ziDong;
        if (ziDong)
            yeAuto.BringToFront();
        else
            yeTuo.BringToFront();
        ShangSe(btnAuto, ziDong);
        ShangSe(btnTuo, !ziDong);
    }

    static void ShangSe(Button niu, bool xuan)
    {
        niu.BackColor = xuan ? lvSe : kaSe;
        niu.ForeColor = xuan ? Color.White : ziSe;
        niu.FlatAppearance.BorderColor = xuan ? lvSe : xianSe;
        niu.FlatAppearance.MouseOverBackColor = xuan ? lvLiang : Color.FromArgb(245, 242, 234);
        niu.FlatAppearance.MouseDownBackColor = xuan ? lvAn : Color.FromArgb(236, 232, 223);
    }

    void GuaTuo(Control kong)
    {
        kong.AllowDrop = true;
        kong.DragEnter += TuoJin;
        kong.DragDrop += async delegate (object s, DragEventArgs e) { await TuoXia(e); };
        kong.DragLeave += delegate { tuoKuang.Liang = false; };
        foreach (Control zi in kong.Controls)
            GuaTuo(zi);
    }

    void TuoJin(object s, DragEventArgs e)
    {
        bool yao = false;
        foreach (string lu in NaTuo(e))
        {
            if (ShiLnkHuoExe(lu))
            {
                yao = true;
                break;
            }
        }
        e.Effect = yao ? DragDropEffects.Copy : DragDropEffects.None;
        if (!zaiAuto)
            HuanYe(false);
        tuoKuang.Liang = yao;
    }

    async Task TuoXia(DragEventArgs e)
    {
        tuoKuang.Liang = false;
        if (mang)
            return;

        List<string> lie = new List<string>();
        foreach (string lu in NaTuo(e))
        {
            if (ShiLnkHuoExe(lu))
                lie.Add(lu);
        }
        if (lie.Count == 0)
        {
            JiLu("请拖入考试客户端快捷方式（.lnk）。");
            return;
        }

        HuanYe(false);
        mang = true;
        SheMang(true);
        try
        {
            if (!NetBanben.You48)
            {
                JiLu(NetBanben.Shuoming());
                return;
            }

            foreach (string f in lie)
                await ChuLiTuo(f);
        }
        finally
        {
            mang = false;
            SheMang(false);
        }
    }

    async Task ZiDong()
    {
        if (mang)
            return;

        mang = true;
        SheMang(true);
        try
        {
            if (!NetBanben.You48)
            {
                JiLu(NetBanben.Shuoming());
                return;
            }

            JiLu("正在查找考试客户端…");
            Update();
            ZhaoJieGuo zhao = ZhaoKehu.Zhao();
            if (zhao.mb == null)
            {
                foreach (string t in zhao.tishi)
                    JiLu(t);
                lblZt.Text = "没有找到Client 可以把快捷方式拖到「拖入识别」";
                return;
            }

            Mubiao mb = zhao.mb;
            lblZt.Text = "来源：" + mb.src + Environment.NewLine + "启动：" + mb.exe;
            JiLu("来源：" + mb.src);
            JiLu("目标：" + mb.exe);
            await XieBingQi(mb, true);
        }
        finally
        {
            mang = false;
            SheMang(false);
        }
    }

    async Task ChuLiTuo(string lu)
    {
        JiLu("收到：" + lu);
        Mubiao mb;
        string cuo;
        if (!CongTuoNa(lu, out mb, out cuo))
        {
            JiLu(cuo);
            tuoKuang.Wen = "未能识别";
            return;
        }

        tuoKuang.Wen = Path.GetFileName(mb.src) + Environment.NewLine + mb.exe;
        JiLu("解析到：" + mb.exe);
        await XieBingQi(mb, false);
    }

    async Task XieBingQi(Mubiao mb, bool yaoQi)
    {
        string mulu = Path.GetDirectoryName(mb.exe) ?? "";
        XieJieGuo jie = Peizhi.XieMulu(mulu);
        if (!jie.ok && jie.yaoAdmin)
        {
            foreach (string h in jie.hua)
                JiLu(h);
            JiLu("需要管理员权限才能写入 正在请求提升…");
            bool ti = await TiQuan(mulu);
            if (!ti)
            {
                JiLu("没有获得管理员权限 配置未写入");
                return;
            }
            jie = Peizhi.XieMulu(mulu);
        }

        foreach (string h in jie.hua)
            JiLu(h);
        if (!jie.ok)
            return;

        if (!yaoQi)
        {
            JiLu("配置已处理 按拖入模式不启动客户端");
            return;
        }

        QiDong(mb);
    }

    void QiDong(Mubiao mb)
    {
        string jinCheng = Path.GetFileNameWithoutExtension(mb.exe);
        if (Process.GetProcessesByName(jinCheng).Length > 0)
        {
            JiLu(jinCheng + " 已在运行 没有重复启动");
            return;
        }

        try
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = mb.exe;
            psi.WorkingDirectory = mb.gzd;
            psi.Arguments = mb.cs;
            psi.UseShellExecute = true;
            Process.Start(psi);
            string wei = string.IsNullOrWhiteSpace(mb.cs) ? "" : " " + mb.cs;
            JiLu("已启动 " + Path.GetFileName(mb.exe) + wei);
        }
        catch (Exception ex)
        {
            JiLu("启动失败：" + ex.Message);
        }
    }

    static async Task<bool> TiQuan(string mulu)
    {
        string exe = Environment.ProcessPath;
        if (string.IsNullOrEmpty(exe))
            exe = Application.ExecutablePath;
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = exe;
            psi.Arguments = "--apply-quiet \"" + mulu.Replace("\"", "") + "\"";
            psi.WorkingDirectory = mulu;
            psi.UseShellExecute = true;
            psi.Verb = "runas";
            using (Process p = Process.Start(psi))
            {
                if (p == null)
                    return false;
                await p.WaitForExitAsync();
                return p.ExitCode == 0;
            }
        }
        catch (Win32Exception)
        {
            return false;
        }
    }

    static bool CongTuoNa(string lu, out Mubiao mb, out string cuo)
    {
        mb = new Mubiao();
        mb.src = lu;
        string hou = Path.GetExtension(lu);
        string exe;
        string gzd = "";
        string cs = "";

        if (string.Equals(hou, ".lnk", StringComparison.OrdinalIgnoreCase))
        {
            if (!LnkDu.Du(lu, out exe, out gzd, out cs, out cuo))
                return false;
        }
        else if (string.Equals(hou, ".exe", StringComparison.OrdinalIgnoreCase))
        {
            if (!File.Exists(lu))
            {
                cuo = "文件不存在：" + lu;
                return false;
            }
            exe = lu;
        }
        else
        {
            cuo = "请拖入 .lnk 快捷方式";
            return false;
        }

        if (!Peizhi.ShiRuKou(exe))
        {
            cuo = "这不是考试客户端 骸子 需要指向 ClientUpdate.exe ExamClient.exe ClientIPEdit.exe ";
            return false;
        }

        string dir = Path.GetDirectoryName(exe) ?? "";
        if (string.IsNullOrWhiteSpace(gzd) || !Directory.Exists(gzd))
            gzd = dir;

        mb.exe = exe;
        mb.gzd = gzd;
        mb.cs = cs;
        mb.src = lu;
        cuo = "";
        return true;
    }

    void SheMang(bool b)
    {
        btnKai.Enabled = !b && NetBanben.You48;
        UseWaitCursor = b;
    }

    void JiLu(string hua)
    {
        txtJi.AppendText(DateTime.Now.ToString("HH:mm:ss") + "  " + hua + Environment.NewLine);
    }

    static bool ShiLnkHuoExe(string lu)
    {
        string hou = Path.GetExtension(lu);
        return string.Equals(hou, ".lnk", StringComparison.OrdinalIgnoreCase)
            || string.Equals(hou, ".exe", StringComparison.OrdinalIgnoreCase);
    }

    static string[] NaTuo(DragEventArgs e)
    {
        if (e.Data == null || !e.Data.GetDataPresent(DataFormats.FileDrop))
            return new string[0];
        return e.Data.GetData(DataFormats.FileDrop) as string[] ?? new string[0];
    }

    class TuoKuang : Panel
    {
        bool liang;
        string wen = "将考试客户端快捷方式拖到这里";

        public TuoKuang()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            Margin = new Padding(0, 8, 0, 0);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Liang
        {
            get { return liang; }
            set
            {
                if (liang == value)
                    return;
                liang = value;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Wen
        {
            get { return wen; }
            set
            {
                wen = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent != null ? Parent.BackColor : kaSe);
            Rectangle kuang = new Rectangle(1, 8, Math.Max(1, Width - 3), Math.Max(1, Height - 12));
            using (GraphicsPath xing = YuanJiao(kuang, 18))
            using (SolidBrush tian = new SolidBrush(liang ? Color.FromArgb(213, 235, 227) : Color.FromArgb(232, 242, 237)))
            using (Pen bi = new Pen(liang ? lvSe : Color.FromArgb(120, 156, 144), 1.8f))
            {
                bi.DashStyle = DashStyle.Dash;
                g.FillPath(tian, xing);
                g.DrawPath(bi, xing);
            }

            string tiShi = "支持桌面或开始菜单里的 .lnk 快捷方式";
            using (Font da = new Font(Font.FontFamily, 14F, FontStyle.Bold, GraphicsUnit.Point))
            using (Font xiao = new Font(Font.FontFamily, 9F, FontStyle.Regular, GraphicsUnit.Point))
            using (SolidBrush biZi = new SolidBrush(ziSe))
            using (SolidBrush biHui = new SolidBrush(huiSe))
            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Near;
                SizeF daSize = g.MeasureString(wen, da, kuang.Width - 48);
                SizeF xiaoSize = g.MeasureString(tiShi, xiao);
                float kuai = daSize.Height + 10 + xiaoSize.Height;
                float y = kuang.Top + (kuang.Height - kuai) / 2;
                RectangleF daKuang = new RectangleF(kuang.Left + 24, y, kuang.Width - 48, daSize.Height);
                g.DrawString(wen, da, biZi, daKuang, sf);
                g.DrawString(tiShi, xiao, biHui, new RectangleF(kuang.Left, y + daSize.Height + 10, kuang.Width, xiaoSize.Height + 4), sf);
            }
        }

        static GraphicsPath YuanJiao(Rectangle k, int r)
        {
            int d = r * 2;
            GraphicsPath lu = new GraphicsPath();
            lu.AddArc(k.Left, k.Top, d, d, 180, 90);
            lu.AddArc(k.Right - d, k.Top, d, d, 270, 90);
            lu.AddArc(k.Right - d, k.Bottom - d, d, d, 0, 90);
            lu.AddArc(k.Left, k.Bottom - d, d, d, 90, 90);
            lu.CloseFigure();
            return lu;
        }
    }
}

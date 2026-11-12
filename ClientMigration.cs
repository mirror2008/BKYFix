using System.Diagnostics;
using System.Reflection;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;

namespace BKYFix;

class Mubiao
{
    public string exe = "";
    public string gzd = "";
    public string cs = "";
    public string src = "";
}

class ZhaoJieGuo
{
    public Mubiao mb;
    public List<string> tishi = new List<string>();
}

class XieJieGuo
{
    public bool ok;
    public bool yaoAdmin;
    public int xieLe;
    public int meiGai;
    public List<string> hua = new List<string>();
}

class NetBanben
{
    public const int v48 = 528040;
    public const int v481 = 533320;

    public static bool You48
    {
        get { return DuRelease() >= v48; }
    }

    public static string Shuoming()
    {
        int rel = DuRelease();
        if (rel >= v481)
            return ".NET4.8.1就绪";
        if (rel >= v48)
            return ".NET Framework 4.8就绪";
        if (rel > 0)
            return "未检测到.NET4.8 请去 https://dotnet.microsoft.com/zh-cn/download/dotnet-framework/thank-you/net481-web-installer 下载后再试";
        return "没有检测到 .NET Framework 4.x";
    }

    public static int DuRelease()
    {
        int zuiGao = 0;
        RegistryView[] liangGe = new RegistryView[] { RegistryView.Registry64, RegistryView.Registry32 };
        foreach (RegistryView shiTu in liangGe)
        {
            try
            {
                using (RegistryKey jiChu = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, shiTu))
                using (RegistryKey xiang = jiChu.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"))
                {
                    object zhi = xiang == null ? null : xiang.GetValue("Release");
                    if (zhi is int shu && shu > zuiGao)
                        zuiGao = shu;
                }
            }
            catch (Exception ex)
            {
                if (!(ex is SecurityException || ex is UnauthorizedAccessException || ex is IOException))
                    throw;
            }
        }
        return zuiGao;
    }
}

class LnkDu
{
    public static bool Du(string lnk, out string mubiao, out string gzd, out string cs, out string cuo)
    {
        mubiao = "";
        gzd = "";
        cs = "";
        cuo = "";

        object ke = null;
        object kuai = null;
        try
        {
            Type lei = Type.GetTypeFromProgID("WScript.Shell");
            if (lei == null)
            {
                cuo = "快捷方式读取失败";
                return false;
            }

            ke = Activator.CreateInstance(lei);
            if (ke == null)
            {
                cuo = "快捷方式读取失败";
                return false;
            }

            kuai = ke.GetType().InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, ke, new object[] { lnk });
            if (kuai == null)
            {
                cuo = "无法读取快捷方式";
                return false;
            }

            mubiao = QuZi(kuai, "TargetPath");
            gzd = QuZi(kuai, "WorkingDirectory");
            cs = QuZi(kuai, "Arguments");
        }
        catch (Exception ex)
        {
            cuo = "读取快捷方式失败" + ex.Message;
            return false;
        }
        finally
        {
            Fang(kuai);
            Fang(ke);
        }

        if (string.IsNullOrWhiteSpace(mubiao))
        {
            cuo = "快捷方式没有目标程序";
            return false;
        }

        if (!Path.IsPathRooted(mubiao) && !string.IsNullOrWhiteSpace(gzd))
            mubiao = Path.GetFullPath(Path.Combine(gzd, mubiao));

        if (!File.Exists(mubiao))
        {
            cuo = "快捷方式指向的文件不存在" + mubiao;
            return false;
        }
        return true;
    }

    static string QuZi(object com, string ming)
    {
        object zhi = com.GetType().InvokeMember(ming, BindingFlags.GetProperty, null, com, null);
        return zhi as string ?? "";
    }

    static void Fang(object com)
    {
        if (com != null && System.Runtime.InteropServices.Marshal.IsComObject(com))
            System.Runtime.InteropServices.Marshal.ReleaseComObject(com);
    }
}

class ZhaoKehu
{
    class Hou
    {
        public Mubiao mb;
        public int fen;
    }

    static readonly string[] guanjian = new string[] { "考试", "Exam", "BKY", "bky" };

    public static ZhaoJieGuo Zhao()
    {
        ZhaoJieGuo jie = new ZhaoJieGuo();
        List<Hou> lie = new List<Hou>();

        foreach (string gen in GenMulu())
        {
            if (!Directory.Exists(gen))
                continue;

            foreach (string lnk in SouLnk(gen))
            {
                string ming = Path.GetFileName(lnk);
                bool xiang = false;
                foreach (string ci in guanjian)
                {
                    if (ming.IndexOf(ci, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        xiang = true;
                        break;
                    }
                }
                if (!xiang)
                    continue;

                string mb, gzd, cs;
                if (!LnkDu.Du(lnk, out mb, out gzd, out cs, out _))
                    continue;
                if (!Peizhi.ShiRuKou(mb))
                    continue;

                Hou h = new Hou();
                h.mb = ZuoMb(mb, gzd, cs, lnk);
                h.fen = DaFen(lnk, mb);
                lie.Add(h);
            }
        }

        foreach (DriveInfo pan in DriveInfo.GetDrives())
        {
            if (!pan.IsReady || pan.DriveType != DriveType.Fixed)
                continue;
            string mulu = Path.Combine(pan.RootDirectory.FullName, "BKYExam", "Client");
            KanMulu(lie, mulu, "安装目录 " + mulu);
        }

        KanXieZai(lie);

        if (lie.Count == 0)
        {
            jie.tishi.Add("未找到考试客户端。");
            jie.tishi.Add("可以改用拖入识别 把快捷方式拖进窗口试试:D");
            return jie;
        }

        Hou zui = lie[0];
        foreach (Hou h in lie)
        {
            if (h.fen > zui.fen)
                zui = h;
        }
        jie.mb = zui.mb;
        return jie;
    }

    static void KanXieZai(List<Hou> lie)
    {
        RegistryView[] liangGe = new RegistryView[] { RegistryView.Registry64, RegistryView.Registry32 };
        foreach (RegistryView shiTu in liangGe)
        {
            try
            {
                using (RegistryKey jiChu = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, shiTu))
                using (RegistryKey xie = jiChu.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
                {
                    if (xie == null)
                        continue;

                    foreach (string zi in xie.GetSubKeyNames())
                    {
                        using (RegistryKey xiang = xie.OpenSubKey(zi))
                        {
                            if (xiang == null)
                                continue;

                            string xianShi = xiang.GetValue("DisplayName") as string ?? "";
                            string weiZhi = xiang.GetValue("InstallLocation") as string ?? "";
                            string xieChuan = xiang.GetValue("UninstallString") as string ?? "";
                            bool xiangGuan = xianShi.IndexOf("考试", StringComparison.OrdinalIgnoreCase) >= 0
                                || weiZhi.IndexOf("BKYExam", StringComparison.OrdinalIgnoreCase) >= 0
                                || xieChuan.IndexOf("BKYExam", StringComparison.OrdinalIgnoreCase) >= 0;
                            if (!xiangGuan)
                                continue;

                            if (!string.IsNullOrWhiteSpace(weiZhi))
                                KanMulu(lie, weiZhi.Trim().Trim('"'), "安装位置 " + weiZhi);

                            string lu = ChouLu(xieChuan);
                            if (!string.IsNullOrWhiteSpace(lu))
                            {
                                string dir = Path.GetDirectoryName(lu);
                                if (!string.IsNullOrWhiteSpace(dir))
                                    KanMulu(lie, dir, "卸载程序所在目录");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!(ex is SecurityException || ex is UnauthorizedAccessException || ex is IOException))
                    throw;
            }
        }
    }

    static void KanMulu(List<Hou> lie, string mulu, string src)
    {
        string geng = Path.Combine(mulu, "ClientUpdate.exe");
        string kao = Path.Combine(mulu, "ExamClient.exe");
        string exe = "";
        if (File.Exists(geng))
            exe = geng;
        else if (File.Exists(kao))
            exe = kao;
        if (exe.Length == 0)
            return;

        Hou h = new Hou();
        h.mb = ZuoMb(exe, mulu, "", src);
        h.fen = DaFen(src, exe);
        lie.Add(h);
    }

    static Mubiao ZuoMb(string exe, string gzd, string cs, string src)
    {
        string dir = Path.GetDirectoryName(exe) ?? "";
        if (string.IsNullOrWhiteSpace(gzd) || !Directory.Exists(gzd))
            gzd = dir;

        Mubiao mb = new Mubiao();
        mb.exe = exe;
        mb.gzd = gzd;
        mb.cs = cs.Trim();
        mb.src = src;
        return mb;
    }

    static int DaFen(string src, string exe)
    {
        string srcMing = Path.GetFileName(src);
        string exeMing = Path.GetFileName(exe);
        int fen = 0;
        if (string.Equals(srcMing, "考试客户端.lnk", StringComparison.OrdinalIgnoreCase))
            fen += 100;
        else if (srcMing.IndexOf("考试客户端", StringComparison.OrdinalIgnoreCase) >= 0)
            fen += 80;
        else if (srcMing.IndexOf("考试", StringComparison.OrdinalIgnoreCase) >= 0)
            fen += 40;

        if (string.Equals(exeMing, "ClientUpdate.exe", StringComparison.OrdinalIgnoreCase))
            fen += 40;
        else if (string.Equals(exeMing, "ExamClient.exe", StringComparison.OrdinalIgnoreCase))
            fen += 30;
        else if (string.Equals(exeMing, "ClientIPEdit.exe", StringComparison.OrdinalIgnoreCase))
            fen += 10;

        if (src.IndexOf("Desktop", StringComparison.OrdinalIgnoreCase) >= 0 || src.IndexOf('桌') >= 0)
            fen += 15;
        if (src.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
            fen += 5;
        return fen;
    }

    static IEnumerable<string> GenMulu()
    {
        yield return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        yield return Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
        yield return Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
        yield return Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
    }

    static IEnumerable<string> SouLnk(string gen)
    {
        Stack<string> dai = new Stack<string>();
        dai.Push(gen);
        while (dai.Count > 0)
        {
            string dir = dai.Pop();
            string[] wj;
            string[] zi;
            try
            {
                wj = Directory.GetFiles(dir, "*.lnk");
                zi = Directory.GetDirectories(dir);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException || ex is IOException || ex is SecurityException)
                    continue;
                throw;
            }

            foreach (string f in wj)
                yield return f;
            foreach (string d in zi)
                dai.Push(d);
        }
    }

    static string ChouLu(string wen)
    {
        string s = wen.Trim();
        if (s.Length == 0)
            return "";
        if (s[0] == '"')
        {
            int wei = s.IndexOf('"', 1);
            return wei > 1 ? s.Substring(1, wei - 1) : "";
        }

        int kong = s.IndexOf(' ');
        return kong < 0 ? s : s.Substring(0, kong);
    }
}

class Peizhi
{
    public const string sku48 = ".NETFramework,Version=v4.8";

    static readonly string[] ruKou = new string[] { "ClientUpdate.exe", "ExamClient.exe", "ClientIPEdit.exe" };

    public static bool ShiRuKou(string lujing)
    {
        string ming = Path.GetFileName(lujing);
        foreach (string n in ruKou)
        {
            if (string.Equals(n, ming, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public static XieJieGuo XieMulu(string mulu)
    {
        XieJieGuo jie = new XieJieGuo();
        if (!Directory.Exists(mulu))
        {
            jie.hua.Add("目录不存在 " + mulu);
            return jie;
        }

        List<string> exes = new List<string>();
        foreach (string ming in ruKou)
        {
            string p = Path.Combine(mulu, ming);
            if (File.Exists(p))
                exes.Add(p);
        }
        if (exes.Count == 0)
        {
            jie.hua.Add("这个目录里没有考试客户端程序 " + mulu);
            return jie;
        }

        bool zaiShi = false;
        foreach (string exe in exes)
        {
            string cfg = exe + ".config";
            string xian = Path.GetFileName(cfg);
            try
            {
                if (YiJing48(cfg))
                {
                    jie.meiGai++;
                    jie.hua.Add(xian + " 已是 4.8 配置");
                    continue;
                }

                XieWenJian(cfg);
                jie.xieLe++;
                jie.hua.Add("已写入 " + xian);
            }
            catch (Exception ex)
            {
                zaiShi = true;
                if (MeiQuan(ex))
                {
                    jie.yaoAdmin = true;
                    jie.hua.Add(xian + " 没有写入权限");
                }
                else
                {
                    jie.hua.Add(xian + " 写入失败 " + ex.Message);
                }
            }
        }

        jie.ok = !zaiShi;
        return jie;
    }

    static bool YiJing48(string cfg)
    {
        if (!File.Exists(cfg))
            return false;
        if (new FileInfo(cfg).Length == 0)
            return false;

        XDocument doc;
        try
        {
            doc = XDocument.Load(cfg);
        }
        catch (XmlException)
        {
            return false;
        }

        XElement kai = QuStartup(doc);
        if (kai == null)
            return false;

        string jiu = (string)kai.Attribute("useLegacyV2RuntimeActivationPolicy");
        if (!string.Equals(jiu, "true", StringComparison.OrdinalIgnoreCase))
            return false;

        XNamespace ns = kai.Name.Namespace;
        List<XElement> rt = new List<XElement>(kai.Elements(ns + "supportedRuntime"));
        if (rt.Count != 1)
            return false;
        return (string)rt[0].Attribute("version") == "v4.0"
            && (string)rt[0].Attribute("sku") == sku48;
    }

    static void XieWenJian(string cfg)
    {
        XDocument doc;
        if (!File.Exists(cfg) || new FileInfo(cfg).Length == 0)
        {
            doc = XinDoc();
        }
        else
        {
            try
            {
                doc = XDocument.Load(cfg);
            }
            catch (XmlException)
            {
                throw new InvalidDataException("现有配置不是有效的XML已跳过 避免覆盖原文件");
            }

            XElement gen = doc.Root;
            if (gen == null || gen.Name.LocalName != "configuration")
                throw new InvalidDataException("配置根节点不是 configuration 已跳过 ");

            XNamespace ns = gen.Name.Namespace;
            XElement kai = gen.Element(ns + "startup");
            if (kai == null)
            {
                kai = new XElement(ns + "startup");
                gen.Add(kai);
            }

            kai.SetAttributeValue("useLegacyV2RuntimeActivationPolicy", "true");
            List<XElement> jiu = new List<XElement>(kai.Elements(ns + "supportedRuntime"));
            foreach (XElement zi in jiu)
                zi.Remove();
            kai.Add(new XElement(
                ns + "supportedRuntime",
                new XAttribute("version", "v4.0"),
                new XAttribute("sku", sku48)));
        }

        if (doc.Declaration == null)
            doc.Declaration = new XDeclaration("1.0", "utf-8", null);

        if (File.Exists(cfg))
        {
            FileAttributes shu = File.GetAttributes(cfg);
            if ((shu & FileAttributes.ReadOnly) != 0)
                File.SetAttributes(cfg, shu & ~FileAttributes.ReadOnly);
        }

        XmlWriterSettings she = new XmlWriterSettings();
        she.Indent = true;
        she.Encoding = new UTF8Encoding(false);
        she.OmitXmlDeclaration = false;
        using (XmlWriter xie = XmlWriter.Create(cfg, she))
        {
            doc.Save(xie);
        }
    }

    static XElement QuStartup(XDocument doc)
    {
        XElement gen = doc.Root;
        if (gen == null || gen.Name.LocalName != "configuration")
            return null;
        return gen.Element(gen.Name.Namespace + "startup");
    }

    static XDocument XinDoc()
    {
        return new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(
                "configuration",
                new XElement(
                    "startup",
                    new XAttribute("useLegacyV2RuntimeActivationPolicy", "true"),
                    new XElement(
                        "supportedRuntime",
                        new XAttribute("version", "v4.0"),
                        new XAttribute("sku", sku48)))));
    }

    static bool MeiQuan(Exception ex)
    {
        if (ex is UnauthorizedAccessException)
            return true;
        return ex is IOException && (ex.HResult == 5 || ex.HResult == unchecked((int)0x80070005));
    }
}

class ZiCe
{
    public static int Pao()
    {
        string gen = Path.Combine(Path.GetTempPath(), "ks48-" + Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(gen);
            string[] mingArr = new string[] { "ClientUpdate.exe", "ExamClient.exe", "ClientIPEdit.exe" };
            foreach (string ming in mingArr)
                File.WriteAllBytes(Path.Combine(gen, ming), new byte[] { 0x4D, 0x5A });

            string yuShe = Path.Combine(gen, "ExamClient.exe.config");
            File.WriteAllText(yuShe,
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                "<configuration>\r\n" +
                "  <appSettings>\r\n" +
                "    <add key=\"keep\" value=\"yes\" />\r\n" +
                "  </appSettings>\r\n" +
                "</configuration>\r\n",
                new UTF8Encoding(false));

            string lnk = Path.Combine(gen, "考试客户端.lnk");
            ZuoLnk(lnk, Path.Combine(gen, "ClientUpdate.exe"), gen);

            string mb, gzd, cuo;
            if (!LnkDu.Du(lnk, out mb, out gzd, out _, out cuo))
                return Gua("快捷方式解析失败：" + cuo);
            if (!mb.EndsWith("ClientUpdate.exe", StringComparison.OrdinalIgnoreCase))
                return Gua("快捷方式目标不正确：" + mb);
            if (!string.Equals(Path.GetFullPath(gzd), Path.GetFullPath(gen), StringComparison.OrdinalIgnoreCase))
                return Gua("工作目录不正确：" + gzd);

            XieJieGuo diYi = Peizhi.XieMulu(gen);
            if (!diYi.ok || diYi.xieLe != 3)
                return Gua("首次写入结果异常：" + string.Join(" | ", diYi.hua.ToArray()));

            XDocument cun = XDocument.Load(yuShe);
            string liu = cun.Root == null ? null : (string)cun.Root.Element("appSettings").Element("add").Attribute("value");
            if (liu != "yes")
                return Gua("原有 appSettings 被覆盖。");
            string sku = (string)cun.Root.Element("startup").Element("supportedRuntime").Attribute("sku");
            if (sku != Peizhi.sku48)
                return Gua("sku 不正确：" + sku);

            XieJieGuo diEr = Peizhi.XieMulu(gen);
            if (!diEr.ok || diEr.meiGai != 3 || diEr.xieLe != 0)
                return Gua("重复写入不应改文件：" + string.Join(" | ", diEr.hua.ToArray()));

            string jiShi = Path.Combine(Environment.SystemDirectory, "notepad.exe");
            string bie = Path.Combine(gen, "其他.lnk");
            ZuoLnk(bie, jiShi, Environment.SystemDirectory);
            string bieMb;
            if (!LnkDu.Du(bie, out bieMb, out _, out _, out cuo))
                return Gua("对照快捷方式解析失败：" + cuo);
            if (Peizhi.ShiRuKou(bieMb))
                return Gua("非考试程序被当成了客户端。");

            if (Directory.Exists(@"C:\BKYExam\Client"))
            {
                ZhaoJieGuo zhao = ZhaoKehu.Zhao();
                if (zhao.mb == null)
                    return Gua("本机有 BKYExam 但自动侦测没有找到客户端 ");
                if (zhao.mb.exe.IndexOf("BKYExam", StringComparison.OrdinalIgnoreCase) < 0)
                    return Gua("自动侦测目标异常" + zhao.mb.exe);
                Console.WriteLine("本机侦测" + zhao.mb.src + " -> " + zhao.mb.exe);
            }

            if (!NetBanben.You48)
                return Gua("本机未安装 .NET Framework 4.8 ");

            Console.WriteLine("self-test ok");
            return 0;
        }
        catch (Exception ex)
        {
            return Gua(ex.ToString());
        }
        finally
        {
            try
            {
                if (Directory.Exists(gen))
                    Directory.Delete(gen, true);
            }
            catch (IOException)
            {
            }
        }
    }

    static void ZuoLnk(string lnk, string mb, string gzd)
    {
        Type lei = Type.GetTypeFromProgID("WScript.Shell");
        if (lei == null)
            throw new InvalidOperationException("没有 WScript.Shell");
        object ke = Activator.CreateInstance(lei);
        if (ke == null)
            throw new InvalidOperationException("无法创建 WScript.Shell");
        object kuai = ke.GetType().InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, ke, new object[] { lnk });
        if (kuai == null)
            throw new InvalidOperationException("无法创建快捷方式");
        kuai.GetType().InvokeMember("TargetPath", BindingFlags.SetProperty, null, kuai, new object[] { mb });
        kuai.GetType().InvokeMember("WorkingDirectory", BindingFlags.SetProperty, null, kuai, new object[] { gzd });
        kuai.GetType().InvokeMember("Save", BindingFlags.InvokeMethod, null, kuai, null);
    }

    static int Gua(string hua)
    {
        Console.Error.WriteLine(hua);
        return 1;
    }
}

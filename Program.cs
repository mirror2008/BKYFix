namespace BKYFix;

static class Program
{
    [STAThread]
    static int Main(string[] args)
    {
        if (args.Length >= 2 && args[0] == "--apply-quiet")
        {
            var mulu = args[1].Trim().Trim('"');
            return Peizhi.XieMulu(mulu).ok ? 0 : 1;
        }

        if (args.Length > 0 && args[0] == "--self-test")
            return ZiCe.Pao();

        ApplicationConfiguration.Initialize();
        Application.Run(new ZhuChuang());
        return 0;
    }
}

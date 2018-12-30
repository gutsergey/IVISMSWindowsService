using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace IVISMSWindowsService
{
    public class Configuration
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string connectionString = "";

        private bool debug = false;
        private int interval = 0;
        private int duetime = 0;
        private string urlpost = "";
        private string urlget = "";
        private string from = "";
        private string user = "";
        private string password = "";
        private string getrequest = "";
        private string sender = "";
        private string emailaddress = "";
        private string content = "";
        private Dictionary<string, string> vdnscontents = null;

        private static Configuration instance = null;
        public static void Initialize()
        {
            if (instance == null)
                instance = new Configuration();

            instance.connectionString = Cfg.GetConnectionString("IVIDatabase");
            instance.debug = Cfg.GetValueBool("debug", false);
            instance.interval = Cfg.GetValueInt("interval", 60);
            instance.duetime = Cfg.GetValueInt("duetime", 5);
            instance.urlpost = Cfg.GetValue("urlpost");
            instance.urlget = Cfg.GetValue("urlget");
            instance.from = Cfg.GetValue("from");
            instance.user = Cfg.GetValue("user");
            instance.password = Cfg.GetValue("password");
            instance.password = Cfg.GetValue("password");
            instance.sender = Cfg.GetValue("sender");
            instance.content = Cfg.GetValue("content");
            instance.emailaddress = Cfg.GetValue("emailaddress");
            instance.getrequest = Cfg.GetValue("getrequest");

            instance.vdnscontents = new Dictionary<string, string>();
            for (int i = 0; i < 100; i++)
            {
                string vdn = Cfg.GetValue("vdn" + i.ToString("00"));
                string content = Cfg.GetValue("content" + i.ToString("00"));
                if (vdn.Trim().Length > 0)
                {
                    content = content.Trim().Length <= 0 ? instance.content : content;
                    foreach (string s in ParseVdn(vdn))
                    {
                        instance.vdnscontents.Add(s, content);
                    }
                }
            }
            ///////////////////////////

            PrintConfig();
        }
        public static Configuration Instance { get => instance; }
        public string ConnectionString { get => connectionString; }
        public bool Debug { get => debug; }
        public int Interval { get => interval; }
        public int Duetime { get => duetime; }
        public string Urlpost { get => urlpost; }
        public string Urlget { get => urlget; }
        public string From { get => from; }
        public string User { get => user; }
        public string Password { get => password; }
        public string Getrequest { get => getrequest; }
        public string Sender { get => sender; }
        public string Emailaddress { get => emailaddress; }
        public string Content { get => content; }
        public Dictionary<string, string> Vdnscontents { get => vdnscontents; }

        private static void PrintConfig()
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string json = JsonConvert.SerializeObject(instance, Formatting.Indented);
            log.Info("-----------------------> version: " + version.ToString());
            log.Info(json);
        }

        private static List<string> ParseVdn(string input)
        {
            List<string> output = new List<string>();
            string[] arr = input.Split(new char[]{ ',', ';' });
            List<string> list = new List<string>(arr);
            foreach(string s in list)
            {
                if (s.Contains("-"))
                {
                    string[] arr1 = s.Split('-');
                    if (long.TryParse(arr1[0], out long min) && long.TryParse(arr1[1], out long max))
                    {
                        for (long l = min; l <= max; l++)
                        {
                            output.Add(l.ToString());
                        }
                    }
                }
                else
                {
                    if (long.TryParse(s, out long value))
                        output.Add(s);
                }

            }
            return output;
        }

    }

}

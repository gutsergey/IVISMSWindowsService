using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVISMSWindowsService
{
    public class MainService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private System.Threading.TimerCallback cb;
        private System.Threading.Timer timer;

        private static bool isRunning = false;
        public void Start()
        {
            StartTimer();
        }
        private void StartTimer()
        {
            int duetime = Configuration.Instance.Duetime * 1000 ;
            int interval = Configuration.Instance.Interval * 1000;
            PrintInterval(duetime, interval);

            cb = new System.Threading.TimerCallback(ProcessTimerEvent);

            isRunning = false;

            timer = new System.Threading.Timer(cb, null, duetime, interval);
        }
        private void PrintInterval(int duetime, int interval)
        {
            log.Info("-------------->");
            log.Info("duetime: " + duetime);
            log.Info("interval: " + interval);
            log.Info("<--------------");
        }
        private void ProcessTimerEvent(object obj)
        {
            if (isRunning)
            {
                return;
            }
            isRunning = true;
            Do();
            isRunning = false;
        }

        private void Do()
        {
            log.Info("-------------->");
            Database db = new Database();
            List<SMS> sms = db.GetRecords();
            log.Info("sms.Count: " + sms?.Count);
            int result = 999;   // 999 is an error
            string xmlresponse = "";
            foreach(SMS s in sms ?? Enumerable.Empty<SMS>())
            {
                // string xmlresponse = sendsms(s);
                if (s.phonenumber.Length == 10 && s.phonenumber.Substring(0, 2).Equals("05"))
                {
                    log.Info("Send sms to " + s.phonenumber);
                    result = SMSSender.SendSMSbyGet(s, out xmlresponse);
                    db.UpdateResult(s, result, xmlresponse);
                }
                else
                    log.Info("phone number is incorrect " + s.phonenumber);
            }
            log.Info("<--------------");

        }
    }

}

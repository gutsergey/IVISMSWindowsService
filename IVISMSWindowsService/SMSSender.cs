using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IVISMSWindowsService
{
    public class SMSSender
    {
        /*
            http://la.cellactpro.com/http_req.asp?FROM=Tikshoovacco&USER=Tikshoovacco&PASSWORD=ccbb891&APP=LA&CMD=sendtextmt&SENDER=0522385127&CONTENT=xxx@yyy.com&TO=0522385127&SN=SMS&MSGID=00001001271533034376&CONFMAIL=This%20message%20is%20sending%20from%20xxx
            <PALO>
                <RESULT>False</RESULT>
                <DESCRIPTION>Invalid account</DESCRIPTION>
            </PALO>

            <PALO>
                <RESULT>False</RESULT>
                <DESCRIPTION>
                    cvc-pattern-valid: Value '089262894' is not facet-valid with respect to pattern '05[0,1,2,3,4,5,6,7,8,9]-?[0-9]{7}|\+[0-9]{7,16}|([\+\.a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+(([a-zA-Z0-9_-])*\.([a-zA-Z0-9_-])+)+' for type 'CELLACT-MOBILE-INTERNATIONAL-NUMBER'.
                </DESCRIPTION>
            </PALO>
        */
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static int SendSMSbyGet(SMS sms, out string xmlresponse)
        {
            string content = "";

            xmlresponse = "";
            int res = 0;
            ////"FROM={FROM}&amp;USER={USER}&amp;PASSWORD={PASSWORD}&amp;APP=LA&amp;CMD=sendtextmt&amp;SENDER={MSISDN}&amp;CONTENT={TEXT}&amp;TO={PHONENUMBER}&amp;SN=SMS&amp;MSGID={UCID}&amp;CONFMAIL={EMAILADDRESS}" 
            string url = Configuration.Instance.Urlget;
            string getrequest = Configuration.Instance.Getrequest;
            getrequest = getrequest.Replace("{FROM}", sms.from);
            getrequest = getrequest.Replace("{USER}", sms.user);
            getrequest = getrequest.Replace("{PASSWORD}", sms.password);
            getrequest = getrequest.Replace("{MSISDN}", sms.sender);

            if (Configuration.Instance.Vdnscontents.Keys.Contains(sms.vdn))
            {
                content = Configuration.Instance.Vdnscontents[sms.vdn];
            }
            else
                content = Configuration.Instance.Content;

            getrequest = getrequest.Replace("{TEXT}", System.Web.HttpUtility.UrlEncode(content));
            getrequest = getrequest.Replace("{PHONENUMBER}", sms.phonenumber);
            getrequest = getrequest.Replace("{UCID}", sms.ucid);
            getrequest = getrequest.Replace("{EMAILADDRESS}", sms.emailaddress);

            url += "?" + getrequest;
            log.Info("url: " + url);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            log.Info("1");
            request.AutomaticDecompression = DecompressionMethods.GZip;
            log.Info("2");
            // http://la.cellactpro.com/http_req.asp?FROM=Tikshoovacco&USER=Tikshoovacco&PASSWORD=ccbb891&APP=LA&CMD=sendtextmt&SENDER=MACCABI&CONTENT=Test+SMS+message&TO=0522385127&SN=SMS&MSGID=00001000151533034343
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    log.Info("3");
                    xmlresponse = reader.ReadToEnd();
                    log.Info("4");
                }
                log.Info("response: " + xmlresponse);

                XElement palo = XElement.Parse(xmlresponse);
                string x = palo.Element("RESULT")?.Value;
                bool.TryParse(x, out bool boolres);
                res = boolres ? 910 : 999;
            }
            catch (Exception exc)
            {
                log.Error(exc.Message);
                log.Error(exc.StackTrace);
                res = 998;
            }

            log.Info("res: " + res);
            // 910 - result OK
            // 999 - result BAD
            return res;
        }
    }
}

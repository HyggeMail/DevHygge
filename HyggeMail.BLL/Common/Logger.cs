using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using HyggeMail.BLL.Models;

namespace HyggeMail.BLL.Common
{

    public class LogMessage
    {
        public DateTime CreatedOn { get; set; }
        public string Message { get; set; }
    }

    public class Logger
    {

        ////http://joshclose.github.io/CsvHelper/

        private static string headers = "Message,CreatedOn";
        private static string apiLogheaders = "Url,Data,Headers,IPAddress,CreatedOn,HttpMethod";
        private static string GetFileToUse(bool forApiLog = false, DateTime? dt = null)
        {
            string folderName = forApiLog ? "ApiLogs" : "Logs";
            string headersToUse = forApiLog ? apiLogheaders : headers;
            var directoryPath = HostingEnvironment.MapPath("~/Uploads/" + folderName);
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
            directoryPath = directoryPath + "/";
            var date = dt.HasValue ? dt.Value.ToString("dd_MM_yyy") : DateTime.Now.ToString("dd_MM_yyy");
            var filePath = directoryPath + string.Format("{0}.csv", date);
            if (!File.Exists(filePath))
            {
                filePath = directoryPath + string.Format("{0}.csv", date);
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Close();
                    using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.WriteLine(headersToUse);
                            sw.Dispose();
                        }
                    }
                }
            }
            return filePath;
        }



        public static void Log(string message)
        {
            var model = new LogMessage { Message = message, CreatedOn = DateTime.Now };
            using (FileStream fs = new FileStream(GetFileToUse(), FileMode.Append, FileAccess.Write))
            {
                using (System.IO.TextWriter writer = new StreamWriter(fs))
                {
                    var csv = new CsvWriter(writer);
                    csv.WriteRecord(model);
                    writer.Dispose();
                }
            }
        }

        public static void Log(ApiLogModel model)
        {
            using (FileStream fs = new FileStream(GetFileToUse(true), FileMode.Append, FileAccess.Write))
            {
                using (System.IO.TextWriter writer = new StreamWriter(fs))
                {
                    var csv = new CsvWriter(writer);
                    csv.WriteRecord(model);
                    writer.Dispose();
                }
            }
        }


        public static List<ApiLogModel> GetLogs(DateTime? dt)
        {
            if (!dt.HasValue) dt = DateTime.Now;
            List<ApiLogModel> data = new List<ApiLogModel>();
            using (FileStream fs = new FileStream(GetFileToUse(true, dt), FileMode.Open, FileAccess.Read))
            {
                using (System.IO.TextReader reader = new StreamReader(fs))
                {
                    var csv = new CsvReader(reader);
                    data = csv.GetRecords<ApiLogModel>().ToList();
                }
            }

           
            return data;
        }









    }

}

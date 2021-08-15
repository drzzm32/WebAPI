using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebAPI.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly DataContext _context;
        

        public ApiController(DataContext context)
        {
            _context = context;

            if (_context.DataItems.Count() == 0)
            {
                if (_context.DataItems.Count() == 0)
                {
                    _context.DataItems.Add(new DataItem { ID = "null" });
                    _context.SaveChanges();
                }
            }
        }

        [HttpGet()]
        public object Main()
        {
            return "Hello!";
        }

        [HttpGet("{args}")]
        public object Main(string args)
        {
            if (args == "index")
                return Index();

            if (!args.Contains(Utility.SEPHEAD))
                return "Something happened.";
            if (args.Split(Utility.SEPHEAD).Length > 2)
                return "Something happened.";
            string arg = args.Split(Utility.SEPHEAD)[1];
            switch (args.Split(Utility.SEPHEAD)[0])
            {
                case "test":
                    return Utility.GetArg(arg, "value");
                case "get":
                    return Get(arg);
                case "set":
                    return Set(arg);
            }

            return NotFound();
        }

        public object Index()
        {
            string content = "<html lang=\"zh-cn\"><head><title>雾霾检测系统 - PMSensor with ASP.NET Core and Raspberry Pi</title><meta charset=\"utf-8\"><meta http-equiv=\"refresh\" content=\"5\"></head><body><table border=\"1\" cellpadding=\"8\"><tr><th>监测点名称</th><th>上传时间</th><th>PM2.5浓度（ug/m3）</th><th>PM10浓度（ug/m3）</th><th>环境温度（℃）</th><th>环境湿度（%）</th><th>大气压（kPa）</th></tr>";

            foreach (DataItem item in _context.DataItems)
            {
                content += "<tr>";

                content += "<th>" + item.ID + "</th>";
                content += "<th>" + item.Time + "</th>";
                content += "<th>" + item.PM25 + "</th>";
                content += "<th>" + item.PM10 + "</th>";
                content += "<th>" + item.Temper + "</th>";
                content += "<th>" + item.Humi + "</th>";
                content += "<th>" + item.Pressure + "</th>";

                content += "</tr>";
            }

            content += "</table></body></html>";

            ContentResult result = new ContentResult();
            result.ContentType = "text/html";
            result.Content = content;
            return result;
        }

        public object Get(string arg)
        {
            string id = Utility.GetArg(arg, "id");
            if (id == arg)
                return "ERR: " + arg;

            DataItem item = _context.DataItems.FirstOrDefault(t => t.ID == id);
            if (item == null)
                return "NONE: " + arg;

            return new ObjectResult(item.SetResult("GOT"));
        }

        public object Set(string arg)
        {
            DataItem item = DataItem.Make(arg);
            if (item == null)
                return "ERR: " + arg;

            string id = item.ID;
            DataItem old = _context.DataItems.FirstOrDefault(t => t.ID == id);
            if (old == null)
            {
                _context.DataItems.Add(item);
                _context.SaveChanges();
                return new ObjectResult(item.SetResult("ADDED"));
            }
            else
            {
                item.CopyTo(old);
                _context.SaveChanges();
                return new ObjectResult(old.SetResult("UPDATED"));
            }
        }
    }
}
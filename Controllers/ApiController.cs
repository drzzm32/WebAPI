using Microsoft.AspNetCore.Mvc;
using System.Threading;
using WebAPI.Models;
using System.Linq;
using System;

namespace WebAPI.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly DataContext _context;
        private static readonly object _lock = new object();

        public ApiController(DataContext context)
        {
            _context = context;

            if (_context.DataItems.Count() == 0)
            {
                _context.FromBinary("data.bin");
                _context.SaveChanges();
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
                case "index":
                    if (VerifyKey(Utility.GetArg(arg, "key")))
                        return Index();
                    else return "Key incorrect.";
            }

            return NotFound();
        }

        public bool VerifyKey(string key)
        {
            DateTime now = DateTime.Now;
            byte[] bytes = Convert.FromBase64String(key);

            string str = "";
            foreach (byte b in bytes)
                str += (char)b;
            DateTime time = DateTime.Parse(str);

            return Math.Abs(now.Subtract(time).TotalSeconds) < 128;
        }

        public object Index()
        {
            string content = "<html lang=\"zh-cn\"><head><title>NyaSamaStat - MC Mod Stat with ASP.NET Core</title><meta charset=\"utf-8\"></head><body><table border=\"1\" cellpadding=\"8\"><tr><th>ID</th><th>Last Login</th><th>Side</th><th>Address</th><th>Players</th></tr>";

            Monitor.Enter(_lock);
            foreach (DataItem item in _context.DataItems)
            {
                content += "<tr>";

                content += "<th>" + item.ID + "</th>";
                content += "<th>" + item.Time.ToString("yyyy-MM-ddTHH:mm:ss") + "</th>";
                content += "<th>" + item.Side + "</th>";
                content += "<th>" + item.Addr + "</th>";
                content += "<th>" + item.Players + "</th>";

                content += "</tr>";
            }
            Monitor.Exit(_lock);

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

            Monitor.Enter(_lock);
            DataItem item = _context.DataItems.FirstOrDefault(t => t.ID == id);
            Monitor.Exit(_lock);
            if (item == null)
                return "NONE: " + arg;

            return "GOT: " + item.ToString();
        }

        public object Set(string arg)
        {
            DataItem item = DataItem.Make(arg);
            if (item == null)
                return "ERR: " + arg;
            if (!item.Verify())
                return "VERIFY ERR: " + arg;
            if (Math.Abs(item.Time.Subtract(DateTime.Now).TotalSeconds) > 128)
                return "TIMESTAMP ERR: " + arg;

            string id = item.ID;
            try
            {
                Monitor.Enter(_lock);
                DataItem old = _context.DataItems.FirstOrDefault(t => t.ID == id);
                if (old == null)
                {
                    _context.DataItems.Add(item);
                    _context.SaveChanges();
                    _context.ToBinary("data.bin");
                    return "ADDED: " + item.ToString();
                }
                else
                {
                    item.CopyTo(old);
                    _context.SaveChanges();
                    _context.ToBinary("data.bin");
                    return "UPDATED: " + item.ToString();
                }
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }
    }
}
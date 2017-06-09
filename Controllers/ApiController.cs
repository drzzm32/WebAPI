using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using System.Linq;

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
                _context.DataItems.Add(new DataItem { ID = "null" });
                _context.DataItems.Add(new DataItem { ID = "test" });
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
            if (args == "index")
                return _context.DataItems.ToList();

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
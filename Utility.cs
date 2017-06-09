using System.Linq;

namespace WebAPI
{
    public class Utility
    {
        public static readonly char SEPHEAD = '~';
        public static readonly char SEPBODY = '&';

        public static string GetArg(string args, string name)
        {
            if (args.Contains(name))
            {
                int pos = args.IndexOf(name);
                string sub = args.Substring(pos + name.Length + 1);
                if (sub.Contains(SEPBODY))
                {
                    return sub.Split(SEPBODY)[0];
                }
                return sub;
            }
            return args;
        }
    }
}

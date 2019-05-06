using System;

namespace WebAPI.Models
{
    public class DataItem
    {
        public readonly int MASK = 0xFFFF;

        public string Result { get;  set; }

        public string ID { get; set; }
        public DateTime Time { get; set; }
        public string Side { get; set; }
        public string Addr { get; set; }
        public int Players { get; set; }

        public int ChkSum { get; set; }

        public void CopyTo(DataItem item)
        {
            item.ID = ID;
            item.Time = Time;
            item.Side = Side;
            item.Addr = Addr;
            item.Players = Players;
        }

        public DataItem SetResult(string result)
        {
            DataItem tmp = new DataItem();
            this.CopyTo(tmp);
            tmp.Result = result;
            return tmp;
        }

        public static DataItem Make(string args)
        {
            DataItem item = new DataItem();
            string tmp;

            tmp = Utility.GetArg(args, "id");
            if (tmp != args) item.ID = tmp;
            else return null;
            tmp = Utility.GetArg(args, "time");
            if (tmp != args) item.Time = DateTime.Parse(tmp);
            else return null;
            tmp = Utility.GetArg(args, "side");
            if (tmp != args) item.Side = tmp;
            tmp = Utility.GetArg(args, "addr");
            if (tmp != args) item.Addr = tmp;
            tmp = Utility.GetArg(args, "chksum");
            if (tmp != args) item.ChkSum = int.Parse(tmp);

            return item;
        }

        public bool Verify()
        {
            int sum = 0;
            foreach (char c in ID)
            {
                sum += c; sum &= MASK;
            }
            foreach (char c in Time.ToString())
            {
                sum += c; sum &= MASK;
            }
            foreach (char c in Side)
            {
                sum += c; sum &= MASK;
            }
            foreach (char c in Addr)
            {
                sum += c; sum &= MASK;
            }

            return sum == ChkSum;
        }
    }
}

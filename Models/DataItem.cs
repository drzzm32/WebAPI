using System;

namespace WebAPI.Models
{
    public class DataItem
    {
        public string Result { get;  set; }
        public string ID { get; set; }
        public DateTime Time { get; set; }
        public int PM25 { get; set; }
        public int PM10 { get; set; }
        public float Temper { get; set; }
        public float Humi { get; set; }
        public float Pressure { get; set; }

        public void CopyTo(DataItem item)
        {
            item.ID = ID;
            item.Time = Time;
            item.PM25 = PM25;
            item.PM10 = PM10;
            item.Temper = Temper;
            item.Humi = Humi;
            item.Pressure = Pressure;
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
            tmp = Utility.GetArg(args, "pm25");
            if (tmp != args) item.PM25 = int.Parse(tmp);
            tmp = Utility.GetArg(args, "pm10");
            if (tmp != args) item.PM10 = int.Parse(tmp);
            tmp = Utility.GetArg(args, "temper");
            if (tmp != args) item.Temper = float.Parse(tmp);
            tmp = Utility.GetArg(args, "humi");
            if (tmp != args) item.Humi = float.Parse(tmp);
            tmp = Utility.GetArg(args, "pressure");
            if (tmp != args) item.Pressure = float.Parse(tmp);

            return item;
        }
    }
}

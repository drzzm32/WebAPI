using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;

using System.Runtime.Serialization.Formatters;

namespace WebAPI.Models
{
    [Serializable]
    public class DataItem : ISerializable
    {
        public readonly int MASK = 0xFFFF;

        public string ID { get; set; }
        public DateTime Time { get; set; }
        public string Side { get; set; }
        public string Addr { get; set; }
        public string Players { get; set; }

        public int ChkSum { get; set; }

        public DataItem()
        {
            ID = "null";
            Time = DateTime.MinValue;
            Side = "null";
            Addr = "null";
            Players = "null";
        }

        public void CopyTo(DataItem item)
        {
            item.ID = ID;
            item.Time = Time;
            item.Side = Side;
            item.Addr = Addr;
            item.Players = Players;
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
            tmp = Utility.GetArg(args, "players");
            if (tmp != args) item.Players = tmp;
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
            foreach (char c in Time.ToString("yyyy-MM-ddTHH:mm:ss"))
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
            foreach (char c in Players)
            {
                sum += c; sum &= MASK;
            }

            return sum == ChkSum;
        }

        public override string ToString()
        {
            return "{ " +
                "\"ID\": " + "\"" + ID + "\", " +
                "\"Time\":" + "\"" + Time.ToString("yyyy-MM-ddTHH:mm:ss") + "\", " +
                "\"Side\":" + "\"" + Side + "\", " +
                "\"Addr\":" + "\"" + Addr + "\", " +
                "\"Players\":" + "\"" + Players + "\"" +
                " }";
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected DataItem(SerializationInfo info, StreamingContext context)
        {
            Type basetype = this.GetType().BaseType;
            MemberInfo[] mi = FormatterServices.GetSerializableMembers(basetype, context);

            for (int i = 0; i < mi.Length; i++)
            {
                FieldInfo fi = (FieldInfo)mi[0];
                object objValue = info.GetValue(basetype.FullName + "+" + fi.Name, fi.FieldType);
                fi.SetValue(this, objValue);
            }

            ID = info.GetString("ID");
            Time = info.GetDateTime("Time");
            Side = info.GetString("Side");
            Addr = info.GetString("Addr");
            Players = info.GetString("Players");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID);
            info.AddValue("Time", Time);
            info.AddValue("Side", Side);
            info.AddValue("Addr", Addr);
            info.AddValue("Players", Players);
            
            Type basetype = this.GetType().BaseType;
            MemberInfo[] mi = FormatterServices.GetSerializableMembers(basetype, context);

            for (int i = 0; i < mi.Length; i++)
                info.AddValue(basetype.FullName + "+" + mi[i].Name, ((FieldInfo)mi[i]).GetValue(this));
        }
    }
}

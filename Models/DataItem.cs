using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace WebAPI.Models
{
    [Serializable]
    public class DataItem : ISerializable
    {
        public string Result { get;  set; }
        public string ID { get; set; }
        public DateTime Time { get; set; }
        public string Data { get; set; }

        public DataItem()
        {
            ID = "null";
            Time = DateTime.MinValue;
            Data = "null";
        }

        public void CopyTo(DataItem item)
        {
            item.ID = ID;
            item.Time = Time;
            item.Data = Data;
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
            tmp = Utility.GetArg(args, "data");
            if (tmp != args) item.Data = tmp;

            return item;
        }

        public override string ToString()
        {
            return "{ " +
                "\"ID\": " + "\"" + ID + "\", " +
                "\"Time\":" + "\"" + Time.ToString("yyyy-MM-ddTHH:mm:ss") + "\", " +
                "\"Data\":" + "\"" + Data + "\"" +
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
            Data = info.GetString("Data");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID);
            info.AddValue("Time", Time);
            info.AddValue("Data", Data);

            Type basetype = this.GetType().BaseType;
            MemberInfo[] mi = FormatterServices.GetSerializableMembers(basetype, context);

            for (int i = 0; i < mi.Length; i++)
                info.AddValue(basetype.FullName + "+" + mi[i].Name, ((FieldInfo)mi[i]).GetValue(this));
        }
    }
}

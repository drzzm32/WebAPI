using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using System.Runtime.Serialization.Formatters.Binary;

namespace WebAPI.Models
{
    public class DataContext : DbContext
    {

        public void ToBinary(string path)
        {
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                DataItems.ToList()
                formatter.Serialize(stream, DataItems.T.ToArray());
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                stream.Close();
            }
        }

        public static void FromBinary(Map map, string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File not found.", path);
            }
            FileStream stream = new FileStream(path, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                object obj = formatter.Deserialize(stream);
                if (!(obj is IObject[]))
                {
                    throw new FileLoadException("File is incorrect.", path);
                }
                map.core.Add((IObject[])obj);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                stream.Close();
            }
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<DataItem> DataItems { get; set; }
    }
}

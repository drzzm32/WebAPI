using System;
using System.IO;
using System.Threading;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace WebAPI.Models
{
    public class DataContext : DbContext
    {
        private static readonly object _lock = new object();

        public void ToBinary(string path)
        {
            List<DataItem> items = new List<DataItem>();
            foreach (DataItem i in DataItems)
                items.Add(i);

            if (Monitor.TryEnter(_lock))
            {
                try
                {
                    FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, items.ToArray());
                    stream.Close();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }
        }

        public void FromBinary(string path)
        {
            if (!File.Exists(path))
                return;
            if (File.ReadAllBytes(path).Length == 0)
                return;

            if (Monitor.TryEnter(_lock))
            {
                try
                {
                    FileStream stream = new FileStream(path, FileMode.Open);
                    BinaryFormatter formatter = new BinaryFormatter();
                    object obj = formatter.Deserialize(stream);
                    stream.Close();

                    if (!(obj is DataItem[]))
                    {
                        throw new FileLoadException("File is incorrect.", path);
                    }

                    List<DataItem> items = new List<DataItem>();
                    foreach (DataItem i in DataItems)
                        items.Add(i);
                    foreach (DataItem i in items)
                        DataItems.Remove(i);

                    items.Clear();
                    items.AddRange((DataItem[])obj);
                    Dictionary<string, DataItem> dict = new Dictionary<string, DataItem>();
                    foreach (DataItem i in items)
                        dict.Add(i.ID, i);

                    DataItems.AddRange(dict.Values);
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<DataItem> DataItems { get; set; }
    }
}

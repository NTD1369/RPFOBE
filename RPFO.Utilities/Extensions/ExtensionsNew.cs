using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RPFO.Utilities.Extensions
{
    public static class ExtensionsNew
    {
        public static int ToInt(this TimeSpan time)
        {
            int.TryParse(time.ToString("hhmm"), out int res);
            return res;
        }
        public static DataTable ConvertListToDataTable<T>(this List<T> iList, DataTable dataTable)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            
            if (dataTable == null)
            {
                dataTable = new DataTable();
                for (int i = 0; i < props.Count; i++)
                {
                    PropertyDescriptor propertyDescriptor = props[i];
                    Type type = propertyDescriptor.PropertyType;

                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        type = Nullable.GetUnderlyingType(type);

                    dataTable.Columns.Add(propertyDescriptor.Name, type);
                }
                object[] values = new object[props.Count];
                foreach (T iListItem in iList)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(iListItem);
                    }
                    dataTable.Rows.Add(values);

                }
            }
            else
            {
                object[] values = new object[dataTable.Columns.Count];
                foreach (T iListItem in iList)
                {
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        for(int j = 0; j < props.Count; j++)
                        {   
                            if(dataTable.Columns[i].ColumnName.ToLower() == "rowguid")
                            {
                                values[i] = Guid.NewGuid();
                            }    
                            else
                            {
                                if (dataTable.Columns[i].ColumnName.ToLower() == props[j].Name.ToLower())
                                    values[i] = props[j].GetValue(iListItem);
                            }
                           
                        }     
                    }
                    dataTable.Rows.Add(values);

                }

            }    
           
            return dataTable;
        }
        public static DataTable ToDataTable(this IEnumerable<dynamic> dynamics)
        {
            try
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(dynamics);
                DataTable dataTable = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(DataTable));
                return dataTable;
            }
            catch
            {
                return null;
            }
        }

        public static T Clone<T>(this T source)
        {
            if (Object.ReferenceEquals(source, null))
                return default(T);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(source);
            T data = (T)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(T));
            return data;
        }

        ///// <summary>
        ///// Kiểm tra data trong chuỗi text
        ///// </summary>
        ///// <param name="text">True có tồn tại giá trị khác</param>
        ///// <param name="data">False không tồn tại giá trị khác</param>
        ///// <returns></returns>
        //public static bool CheckExistDataNotContains(this string text, string data)
        //{
        //    if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(data))
        //        return false;
        //    string[] lst = text.Replace(data, "").Trim().Split(',', StringSplitOptions.RemoveEmptyEntries);
        //    return lst.Length != 0;
        //}

        public static string HexToString(this string hexText)
        {
            if (string.IsNullOrEmpty(hexText) || hexText.Length % 2 == 1)
            {
                return string.Empty;
            }
            byte[] bb = Enumerable.Range(0, hexText.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hexText.Substring(x, 2), 16))
                             .ToArray();
            return System.Text.Encoding.ASCII.GetString(bb);
            // or System.Text.Encoding.UTF7.GetString
            // or System.Text.Encoding.UTF8.GetString
            // or System.Text.Encoding.Unicode.GetString
            // or etc.
        }

        public static string ToHexString(this string normalText)
        {
            //byte[] bytes = Encoding.UTF8.GetBytes(normalText);
            //string hexString = Convert.ToHexString(bytes);

            return string.Join("", normalText.Select(c => ((int)c).ToString("X2")));
        }

        /// <summary> Convert a string of hex digits (ex: E4 CA B2) to a byte array. </summary>
        /// <param name="s"> The string containing the hex digits (with or without spaces). </param>
        /// <returns> Returns an array of bytes. </returns>
        public static byte[] HexStringToByteArray(this string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        /// <summary> Converts an array of bytes into a formatted string of hex digits (ex: E4 CA B2)</summary>
        /// <param name="data"> The array of bytes to be translated into a string of hex digits. </param>
        /// <returns> Returns a well formatted string of hex digits with spacing. </returns>
        public static string ByteArrayToHexString(this byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            return sb.ToString().ToUpper();
        }
    }
}

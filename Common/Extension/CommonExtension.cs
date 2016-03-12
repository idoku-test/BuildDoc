using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.Collections;

namespace Common
{
    public static class CommonExtension
    {
        public static IList<T> ToList<T>(this IList sourceList)
        {
            Type listType = sourceList.GetType();
            Type[] genericTyeps = listType.GetGenericArguments();
            if (genericTyeps.Count() == 1)
            {
                Type entityType = genericTyeps[0];
                return CollectionHelper.ConvertTo<T>(sourceList, entityType);
            }
            else
            {
                return null;
            }
        }       

        public static List<List<T>> Split<T>(this List<T> theList, int chunkSize)
        {
            if (!theList.Any())
            {
                return new List<List<T>>();
            }

            List<List<T>> result = new List<List<T>>();
            List<T> currentList = new List<T>();
            result.Add(currentList);

            int i = 0;
            foreach (T item in theList)
            {
                if (i >= chunkSize)
                {
                    i = 0;
                    currentList = new List<T>();
                    result.Add(currentList);
                }
                i += 1;
                currentList.Add(item);
            }
            return result;
        }

        public static DataTable ToDataTable(this IList sourceList)
        {
            Type listType = sourceList.GetType();
            Type[] genericTyeps = listType.GetGenericArguments();
            if (genericTyeps.Count() == 1)
            {
                Type entityType = genericTyeps[0];
                return CollectionHelper.ConvertTo(sourceList, entityType); 
            }
            else
            {
                return null;
            }
        }
    }

    public class CollectionHelper
    {
        private CollectionHelper()
        {
        }

        public static DataTable ConvertTo<T>(IList<T> list)
        {
            DataTable table = CreateTable<T>();
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (T item in list)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item);
                }

                table.Rows.Add(row);
            }

            return table;
        }

        public static DataTable ConvertTo(IList sourceList, Type entityType)
        {
            DataTable table = CreateTable(entityType);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (object item in sourceList)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item);
                }

                table.Rows.Add(row);
            }

            return table;
        }

        public static IList<T> ConvertTo<T>(IList<DataRow> rows)
        {
            IList<T> list = null;

            if (rows != null)
            {
                list = new List<T>();

                foreach (DataRow row in rows)
                {
                    T item = CreateItem<T>(row);
                    list.Add(item);
                }
            }

            return list;
        }

        public static IList<T> ConvertTo<T>(DataTable table)
        {
            if (table == null)
            {
                return null;
            }

            List<DataRow> rows = new List<DataRow>();

            foreach (DataRow row in table.Rows)
            {
                rows.Add(row);
            }

            return ConvertTo<T>(rows);
        }

        public static IList<T> ConvertTo<T>(IList sourceList, Type entityType)
        {
            IList<T> list = null;

            if (sourceList != null)
            {
                list = new List<T>();

                foreach (var ety in sourceList)
                {
                    T item = CreateItem<T>(ety, entityType);
                    list.Add(item);
                }
            }

            return list;
        }

        public static IList<T> ConvertTo<T, S>(IList<S> sourceList)
        {
            IList<T> list = null;

            if (sourceList != null)
            {
                list = new List<T>();

                foreach (var ety in sourceList)
                {
                    T item = CreateItem<T, S>(ety);
                    list.Add(item);
                }
            }

            return list;
        }

        public static T CreateItem<T>(DataRow row)
        {
            T obj = default(T);
            if (row != null)
            {
                obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in row.Table.Columns)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName);
                    try
                    {
                        object value = row[column.ColumnName];
                        prop.SetValue(obj, value, null);
                    }
                    catch
                    {
                        // You can log something here  
                        throw;
                    }
                }
            }

            return obj;
        }

        public static DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            return table;
        }

        public static DataTable CreateTable(Type entityType)
        {
            DataTable table = new DataTable(entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            return table;
        }

        public static T CreateItem<T>(object ety, Type entityType)
        {
            T obj = default(T);
            if (ety != null)
            {
                obj = Activator.CreateInstance<T>();

                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

                foreach (PropertyDescriptor sourceProp in properties)
                {
                    object value = sourceProp.GetValue(ety);
                    PropertyInfo tagProp = obj.GetType().GetProperty(sourceProp.Name);
                    if ( tagProp.CanWrite )
                        tagProp.SetValue(obj, value, null);
                }
            }
            return obj;
        }

        public static T CreateItem<T, S>(S ety)
        {
            T obj = default(T);
            if (ety != null)
            {
                obj = Activator.CreateInstance<T>();

                Type entityType = typeof(S);
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

                foreach (PropertyDescriptor sourceProp in properties)
                {
                    object value = sourceProp.GetValue(ety);
                    PropertyInfo tagProp = obj.GetType().GetProperty(sourceProp.Name);
                    tagProp.SetValue(obj, value, null);
                }
            }
            return obj;
        }
    }  
  
}

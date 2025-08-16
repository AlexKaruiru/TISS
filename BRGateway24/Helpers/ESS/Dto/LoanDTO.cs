using System.Data;
using System.Reflection;

namespace BRGateway24.Helpers.ESS.Dto
{
    public class LoanDTO
    {
        public void InsertModelIntoDataTable<T>(DataTable dt, T model) where T : class
        {
            if (dt == null)
                throw new ArgumentNullException(nameof(dt));
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            DataRow row = dt.NewRow();
            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                string columnName = GetColumnName(property);

                if (!dt.Columns.Contains(columnName))
                    continue;

                object value = property.GetValue(model);
                row[columnName] = value ?? DBNull.Value;
            }
            dt.Rows.Add(row);
        }

        public DataTable CreateDataTableForModel<T>() where T : class
        {
            return CreateDataTableForModel(typeof(T));
        }

        public DataTable CreateDataTableForModel(Type modelType)
        {
            DataTable dt = new DataTable(modelType.Name);
            PropertyInfo[] properties = modelType.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                string columnName = GetColumnName(property);
                Type columnType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                if (!dt.Columns.Contains(columnName))
                {
                    dt.Columns.Add(columnName, columnType);
                }
            }
            return dt;
        }

        [AttributeUsage(AttributeTargets.Property)]
        public class ColumnNameAttribute : Attribute
        {
            public string Name { get; }

            public ColumnNameAttribute(string name)
            {
                Name = name;
            }
        }
        private string GetColumnName(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            var columnAttr = property.GetCustomAttribute<ColumnNameAttribute>();
            return columnAttr != null ? columnAttr.Name : property.Name;
        }
    }
}



using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace LinmaluMyDB
{
    public enum LinmaluDBType { MySQL, MsSQL };

    public class LinmaluDB
    {
        public static LinmaluDB createDB(LinmaluDBType dt, string server, string port, string uid, string pwd)
        {
            switch (dt)
            {
                case LinmaluDBType.MySQL:
                    return new LinmaluMySQL(server, port, uid, pwd);
                case LinmaluDBType.MsSQL:
                    return new LinmaluMsSQL(server, port, uid, pwd);
                default:
                    return null;
            }
        }

        protected string connection;

        public LinmaluDB(string server, string port, string uid, string pwd)
        {
        }
        public virtual List<string> showDatabases()
        {
            return new List<string>();
        }
        public virtual List<string> showTables(string database)
        {
            return new List<string>();
        }
        public virtual List<string> showColumnsType(string database, string table)
        {
            return new List<string>();
        }
        public virtual List<List<string>> showColumns(string database, string table)
        {
            return new List<List<string>>();
        }
        public virtual List<List<string>> showDatas(string database, string table, string[] columns)
        {
            return new List<List<string>>();
        }
        public virtual string countDatas(string database, string table, string sub)
        {
            return "0";
        }
        public virtual int insertDatas(string database, string table, string[] datas)
        {
            return 0;
        }
        public virtual int updateDatas(string database, string table, Dictionary<string, string> map, string sub)
        {
            return 0;
        }
        public virtual int deleteDatas(string database, string table, string sub)
        {
            return 0;
        }
    }

    //MySQL Class
    public class LinmaluMySQL : LinmaluDB
    {
        public LinmaluMySQL(string server, string port, string uid, string pwd) : base(server, port, uid, pwd)
        {
            connection = "Server = " + server + "; Port = " + (port == "" ? "3306" : port) + "; UID = " + uid + "; PWD = " + pwd + "; Charset = utf8;";
        }
        public override List<string> showDatabases()
        {
            List<string> list = base.showDatabases();
            using(MySqlConnection db = new MySqlConnection(connection))
            {
                db.Open();
                using(MySqlCommand cmd = new MySqlCommand("SHOW DATABASES", db))
                {
                    using(MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            list.Add(reader[0].ToString());
                        }
                    }
                }
            }
            return list;
        }
        public override List<string> showTables(string database)
        {
            List<string> list = base.showDatabases();
            using(MySqlConnection db = new MySqlConnection(connection + "Database = " + database + ";"))
            {
                db.Open();
                using(MySqlCommand cmd = new MySqlCommand("SHOW TABLES", db))
                {
                    using(MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            list.Add(reader[0].ToString());
                        }
                    }
                }
            }
            return list;
        }
        public override List<string> showColumnsType(string database, string table)
        {
            List<string> list = base.showColumnsType(database, table);
            using(MySqlConnection db = new MySqlConnection(connection + "Database = " + database + ";"))
            {
                db.Open();
                using(MySqlCommand cmd = new MySqlCommand("SHOW COLUMNS FROM " + table, db))
                {
                    using(MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            list.Clear();
                            for(int i = 0; i < reader.FieldCount; i++)
                            {
                                list.Add(reader.GetName(i));
                            }
                        }
                    }
                }
            }
            return list;
        }
        public override List<List<string>> showColumns(string database, string table)
        {
            List<List<string>> list = base.showColumns(database, table);
            using(MySqlConnection db = new MySqlConnection(connection + "Database = " + database + ";"))
            {
                db.Open();
                using(MySqlCommand cmd = new MySqlCommand("SHOW COLUMNS FROM " + table, db))
                {
                    using(MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            List<string> fields = new List<string>();
                            for(int i = 0; i < reader.FieldCount; i++)
                            {
                                fields.Add(reader[i].ToString());
                            }
                            list.Add(fields);
                        }
                    }
                }
            }
            return list;
        }
        public override List<List<string>> showDatas(string database, string table, string[] columns)
        {
            List<List<string>> list = base.showDatas(database, table, columns);
            using(MySqlConnection db = new MySqlConnection(connection + "Database = " + database + ";"))
            {
                db.Open();
                using(MySqlCommand cmd = new MySqlCommand("SELECT * FROM " + table, db))
                {
                    using(MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            List<string> fields = new List<string>();
                            foreach(string column in columns)
                            {
                                fields.Add(reader[column].ToString());
                            }
                            list.Add(fields);
                        }
                    }
                }
            }
            return list;
        }
        public override string countDatas(string database, string table, string sub)
        {
            using(MySqlConnection db = new MySqlConnection(connection + "Database = " + database + ";"))
            {
                db.Open();
                using(MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM " + table + " " + sub, db))
                {
                    using(MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            return reader["COUNT(*)"].ToString();
                        }
                    }
                }
            }
            return "0";
        }
        public override int insertDatas(string database, string table, string[] datas)
        {
            string value = "";
            foreach (string data in datas)
            {
                value += (value == "" ? "" : ", ") + "'" + data + "'";
            }
            using(MySqlConnection db = new MySqlConnection(connection + "Database = " + database + ";"))
            {
                db.Open();
                using(MySqlCommand cmd = new MySqlCommand("INSERT INTO " + table + " VALUES (" + value + ")", db))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        public override int updateDatas(string database, string table, Dictionary<string, string> map, string sub)
        {
            string value = "";
            foreach (KeyValuePair<string, string> pair in map)
            {
                value += (value == "" ? "" : ", ") + pair.Key + " = '" + pair.Value + "'";
            }
            using(MySqlConnection db = new MySqlConnection(connection + "Database = " + database + ";"))
            {
                db.Open();
                using(MySqlCommand cmd = new MySqlCommand("UPDATE " + table + "  SET " + value + " " + sub, db))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        public override int deleteDatas(string database, string table, string sub)
        {
            using(MySqlConnection db = new MySqlConnection(connection + "Database = " + database + ";"))
            {
                db.Open();
                using(MySqlCommand cmd = new MySqlCommand("DELETE FROM " + table + " " + sub, db))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }

    //MsSQL Class
    public class LinmaluMsSQL : LinmaluDB
    {
        public LinmaluMsSQL(string server, string port, string uid, string pwd) : base(server, port, uid, pwd)
        {
            connection = "Server = " + server + "," + (port == "" ? "1433" : port) + "; UID = " + uid + "; PWD = " + pwd + ";";
        }
        public override List<string> showDatabases()
        {
            List<string> list = base.showDatabases();
            using(SqlConnection db = new SqlConnection(connection))
            {
                db.Open();
                using(SqlCommand cmd = new SqlCommand("SELECT NAME FROM SYS.SYSDATABASES", db))
                {
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            list.Add(reader[0].ToString());
                        }
                    }
                }
            }
            return list;
        }
        public override List<string> showTables(string database)
        {
            List<string> list = base.showDatabases();
            using(SqlConnection db = new SqlConnection(connection + "Database = " + database + ";"))
            {
                db.Open();
                using(SqlCommand cmd = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_CATALOG = '" + database + "'", db))
                {
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            list.Add(reader[0].ToString());
                        }
                    }
                }
            }
            return list;
        }
        public override List<string> showColumnsType(string database, string table)
        {
            List<string> list = base.showColumnsType(database, table);
            using(SqlConnection db = new SqlConnection(connection + "Database = " + database + ";"))
            {
                db.Open();
                using(SqlCommand cmd = new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + table + "'", db))
                {
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            list.Clear();
                            for(int i = 0; i < reader.FieldCount; i++)
                            {
                                list.Add(reader.GetName(i));
                            }
                        }
                    }
                }
            }
            return list;
        }
        public override List<List<string>> showColumns(string database, string table)
        {
            List<List<string>> list = base.showColumns(database, table);
            using(SqlConnection db = new SqlConnection(connection + "Database = " + database + ";"))
            {
                db.Open();
                using(SqlCommand cmd = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + table + "'", db))
                {
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            List<string> fields = new List<string>();
                            for(int i = 0; i < reader.FieldCount; i++)
                            {
                                fields.Add(reader[i].ToString());
                            }
                            list.Add(fields);
                        }
                    }
                }
            }
            return list;
        }
        public override List<List<string>> showDatas(string database, string table, string[] columns)
        {
            List<List<string>> list = base.showDatas(database, table, columns);
            using(SqlConnection db = new SqlConnection(connection + "Database = " + database + ";"))
            {
                db.Open();
                using(SqlCommand cmd = new SqlCommand("SELECT * FROM " + table, db))
                {
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            List<string> fields = new List<string>();
                            foreach(string column in columns)
                            {
                                fields.Add(reader[column].ToString());
                            }
                            list.Add(fields);
                        }
                    }
                }
            }
            return list;
        }
        public override string countDatas(string database, string table, string sub)
        {
            using(SqlConnection db = new SqlConnection(connection + "Database = " + database + ";"))
            {
                db.Open();
                using(SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM " + table + " " + sub, db))
                {
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            return reader["COUNT(*)"].ToString();
                        }
                    }
                }
            }
            return "0";
        }
        public override int insertDatas(string database, string table, string[] datas)
        {
            string value = "";
            foreach (string data in datas)
            {
                value += (value == "" ? "" : ", ") + "'" + data + "'";
            }
            using(SqlConnection db = new SqlConnection(connection + "Database = " + database + ";"))
            {
                db.Open();
                using(SqlCommand cmd = new SqlCommand("INSERT INTO " + table + " VALUES (" + value + ")", db))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        public override int updateDatas(string database, string table, Dictionary<string, string> map, string sub)
        {
            string value = "";
            foreach (KeyValuePair<string, string> pair in map)
            {
                value += (value == "" ? "" : ", ") + pair.Key + " = '" + pair.Value + "'";
            }
            using(SqlConnection db = new SqlConnection(connection + "Database = " + database + ";"))
            {
                db.Open();
                using(SqlCommand cmd = new SqlCommand("UPDATE " + table + "  SET " + value + " " + sub, db))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        public override int deleteDatas(string database, string table, string sub)
        {
            using(SqlConnection db = new SqlConnection(connection + "Database = " + database + ";"))
            {
                db.Open();
                using(SqlCommand cmd = new SqlCommand("DELETE FROM " + table + " " + sub, db))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

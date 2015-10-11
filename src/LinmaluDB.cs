using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LinmaluMyDB
{
    public enum LinmaluDBType { MySQL };

    public class LinmaluDB
    {
        public static LinmaluDB createDB(LinmaluDBType dt, string server, string port, string uid, string pwd)
        {
            switch(dt)
            {
                case LinmaluDBType.MySQL:
                    return new LinmaluMySQL(server, port, uid, pwd);
                //case LinmaluDBType.MsSQL:
                //    return new LinmaluMsSQL(server, port, uid, pwd);
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
        public virtual void Dispose()
        {
        }
    }

    public class LinmaluMySQL : LinmaluDB
    {
        private MySqlConnection db;

        public LinmaluMySQL(string server, string port, string uid, string pwd) : base(server, port, uid, pwd)
        {
            connection = "Server = " + server + "; UID = " + uid + "; PWD = " + pwd + "; " + (port == "" ? "" : "; Port = " + port + ";");
            db = new MySqlConnection(connection + "Charset = utf8");
            db.Open();
        }
        public override List<string> showDatabases()
        {
            List<string> list = base.showDatabases();
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
            return list;
        }
        public override List<string> showTables(string database)
        {
            List<string> list = base.showDatabases();
            using(MySqlCommand cmd = new MySqlCommand("SHOW TABLES FROM " + database, db))
            {
                using(MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        list.Add(reader[0].ToString());
                    }
                }
            }
            return list;
        }
        public override List<List<string>> showColumns(string database, string table)
        {
            List<List<string>> list = base.showColumns(database, table);
            using(MySqlCommand cmd = new MySqlCommand("SHOW COLUMNS FROM " + database + "." + table, db))
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
            return list;
        }
        public override List<List<string>> showDatas(string database, string table, string[] columns)
        {
            List<List<string>> list = base.showDatas(database, table, columns);
            using(MySqlCommand cmd = new MySqlCommand("SELECT * FROM " + database + "." + table, db))
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
            return list;
        }
        public override string countDatas(string database, string table, string sub)
        {
            using(MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM " + database + "." + table + " " + sub, db))
            {
                using(MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        return reader["COUNT(*)"].ToString();
                    }
                }
            }
            return "0";
        }
        public override int insertDatas(string database, string table, string[] datas)
        {
            string value = "";
            foreach(string data in datas)
            {
                value += (value == "" ? "" : ", ") + "'" + data + "'";
            }
            using(MySqlCommand cmd = new MySqlCommand("INSERT INTO " + database + "." + table + " VALUES (" + value + ")", db))
            {
                return cmd.ExecuteNonQuery();
            }
        }
        public override int updateDatas(string database, string table, Dictionary<string, string> map, string sub)
        {
            string value = "";
            foreach(KeyValuePair<string, string> pair in map)
            {
                value += (value == "" ? "" : ", ") + pair.Key + " = '" + pair.Value + "'";
            }
            using(MySqlCommand cmd = new MySqlCommand("UPDATE " + database + "." + table + "  SET " + value + " " + sub, db))
            {
                return cmd.ExecuteNonQuery();
            }
        }
        public override int deleteDatas(string database, string table, string sub)
        {
            using(MySqlCommand cmd = new MySqlCommand("DELETE FROM " + database + "." + table + " " + sub, db))
            {
                return cmd.ExecuteNonQuery();
            }
        }
        public override void Dispose()
        {
            db.Dispose();
        }
    }

    public class LinmaluMsSQL : LinmaluDB
    {
        private SqlConnection db;

        public LinmaluMsSQL(string server, string port, string uid, string pwd) : base(server, port, uid, pwd)
        {
            if(port != "")
            {
                port = "," + port;
            }
            connection = "Server = " + server + "; UID = " + uid + "; PWD = " + pwd + ";";
            db = new SqlConnection(connection);
            db.Open();
        }
        public override List<string> showDatabases()
        {
            List<string> list = base.showDatabases();
            using(SqlCommand cmd = new SqlCommand("SHOW DATABASES", db))
            {
                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        list.Add(reader[0].ToString());
                    }
                }
            }
            return list;
        }
        public override List<string> showTables(string database)
        {
            List<string> list = base.showDatabases();
            using(SqlCommand cmd = new SqlCommand("SHOW TABLES FROM " + database, db))
            {
                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        list.Add(reader[0].ToString());
                    }
                }
            }
            return list;
        }
        public override List<List<string>> showColumns(string database, string table)
        {
            List<List<string>> list = base.showColumns(database, table);
            using(SqlCommand cmd = new SqlCommand("SHOW COLUMNS FROM " + database + "." + table, db))
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
            return list;
        }
        public override List<List<string>> showDatas(string database, string table, string[] columns)
        {
            List<List<string>> list = base.showDatas(database, table, columns);
            using(SqlCommand cmd = new SqlCommand("SELECT * FROM " + database + "." + table, db))
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
            return list;
        }
        public override string countDatas(string database, string table, string sub)
        {
            using(SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM " + database + "." + table + " " + sub, db))
            {
                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        return reader["COUNT(*)"].ToString();
                    }
                }
            }
            return "0";
        }
        public override int insertDatas(string database, string table, string[] datas)
        {
            string value = "";
            foreach(string data in datas)
            {
                value += (value == "" ? "" : ", ") + "'" + data + "'";
            }
            using(SqlCommand cmd = new SqlCommand("INSERT INTO " + database + "." + table + " VALUES (" + value + ")", db))
            {
                return cmd.ExecuteNonQuery();
            }
        }
        public override int updateDatas(string database, string table, Dictionary<string, string> map, string sub)
        {
            string value = "";
            foreach(KeyValuePair<string, string> pair in map)
            {
                value += (value == "" ? "" : ", ") + pair.Key + " = '" + pair.Value + "'";
            }
            using(SqlCommand cmd = new SqlCommand("UPDATE " + database + "." + table + "  SET " + value + " " + sub, db))
            {
                return cmd.ExecuteNonQuery();
            }
        }
        public override int deleteDatas(string database, string table, string sub)
        {
            using(SqlCommand cmd = new SqlCommand("DELETE FROM " + database + "." + table + " " + sub, db))
            {
                return cmd.ExecuteNonQuery();
            }
        }
        public override void Dispose()
        {
            db.Dispose();
        }
    }
}

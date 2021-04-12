using System;
using System.Linq;
using lib_backend.Models;
using lib_backend.Services;
using SqlSugar;

namespace lib_backend
{
    /// <summary>
    /// </summary>
    public class DbContext
    {
        //注意：不能写成静态的，不能写成静态的
        public SqlSugarClient Db; //用来处理事务多表查询和复杂的操作

        public DbContext()
        {
            Db = new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = GlobalVar.ConnectString,
                DbType = DbType.MySql,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            //调试代码 用来打印SQL 
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" +
                                  Db.Utilities.SerializeObject(
                                      pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };
        }

        public SimpleClient<ReaderInfo> ReaderDb => new SimpleClient<ReaderInfo>(Db); //用来处理ReaderInfo表的常用操作

        //静态调用的写法
        public static SqlSugarClient DBstatic =>
            new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = GlobalVar.ConnectString,
                DbType = DbType.MySql,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
    }
}
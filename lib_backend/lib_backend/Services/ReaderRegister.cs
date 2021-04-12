using System;
using lib_backend.Models;
using MySql.Data.MySqlClient;

namespace lib_backend.Services
{
    public class ReaderRegister : IReaderRegister
    {
        /// <summary>
        ///     接受注册信息，并将用户信息写入数据库
        /// </summary>
        /// <param name="ls">注册信息参数列表</param>
        /// <exception cref="MySqlException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns>用户 ID</returns>
        public int AccepteRegister(RegisterModel reg)
        {
            try
            {
                _ = GlobalFunc.CheckRegisterInput(reg);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (reg != null)
            {
                var newReader = new ReaderInfo();
                reg.Salt = Guid.NewGuid().ToString();
                reg.PasswordHash = GlobalFunc.EncryptPassword(reg.Password, reg.Salt);
                newReader.SetInitial(reg);
                try
                {
                    var id = DbContext.DBstatic.Insertable(newReader).ExecuteReturnIdentity();
                    Console.WriteLine("注册成功");
                    return id;
                }
                catch (MySqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            throw new Exception("Failed to register");
        }
    }
}
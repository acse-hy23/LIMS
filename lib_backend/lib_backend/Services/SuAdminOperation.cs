using System;
using System.Collections.Generic;
using lib_backend.Models;
using MySql.Data.MySqlClient;

namespace lib_backend.Services
{
    public class SuAdminOperation : ISuAdminOperation
    {
        /// <summary>
        ///     单例模式
        /// </summary>
        public static SuAdminOperation Instance { get; } = new SuAdminOperation();

        /// <summary>
        ///     查询所有读者信息
        /// </summary>
        /// <returns>读者实体列表</returns>
        /// <exception cref="MySqlException"></exception>
        /// <exception cref="Exception"></exception>
        public List<ReaderInfoModel> GetAllReaders()
        {
            List<ReaderInfoModel> readers;
            try
            {
                readers = DbContext.DBstatic.Queryable<ReaderInfo>()
                    .Select(f => new ReaderInfoModel
                    {
                        ID = f.ID, Contact = f.Contact, Credit_Score = f.Credit_Score, Gender = f.Gender, Name = f.Name
                    }).ToList();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return readers;
        }

        /// <summary>
        ///     获取所有管理员信息
        /// </summary>
        /// <returns>管理员实体列表</returns>
        public List<AdminInfoModel> GetAllAdmins()
        {
            List<AdminInfoModel> admins;
            try
            {
                admins = DbContext.DBstatic.Queryable<AdminInfo>()
                    .Select(f => new AdminInfoModel {ID = f.ID, Name = f.Name, Contact = f.Contact, Gender = f.Gender})
                    .ToList();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return admins;
        }

        /// <summary>
        ///     创建管理员账户
        /// </summary>
        /// <param name="ls"></param>
        /// <exception cref="MySqlException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns>用户 ID</returns>
        public int CreateAdminAccount(RegisterModel reg)
        {
            int newAdId;
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
                var newAdmin = new AdminInfo();
                reg.Salt = Guid.NewGuid().ToString();
                reg.PasswordHash = GlobalFunc.EncryptPassword(reg.Password, reg.Salt);
                newAdmin.SetInitial(reg);
                try
                {
                    newAdId = DbContext.DBstatic.Insertable(newAdmin).ExecuteReturnIdentity();
                }
                catch (MySqlException ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new Exception("Failed to create account");
            }

            return newAdId;
        }
    }
}
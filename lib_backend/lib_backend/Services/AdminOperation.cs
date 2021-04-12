using System;
using System.Collections.Generic;
using lib_backend.Models;
using MySql.Data.MySqlClient;

namespace lib_backend.Services
{
    public class AdminOperation : IAdminOperation
    {
        /// <summary>
        ///     单例模式
        /// </summary>
        public static AdminOperation Instance { get; } = new AdminOperation();

        /// <summary>
        ///     获取管理员信息
        /// </summary>
        /// <param name="userID">用户 ID</param>
        /// <returns>信息字典{ 信息类型, 信息内容 }</returns>
        public AdminInfoModel GetAdminInfo(int userID)
        {
            AdminInfoModel info = null;
            if (GlobalFunc.FindPersonById(userID))
                info = DbContext.DBstatic.Queryable<AdminInfo>()
                    .Select(f => new AdminInfoModel
                        {ID = f.ID, Contact = f.Contact, Name = f.Name, Gender = f.Gender})
                    .Where(it => it.ID == userID).Single();
            return info;
        }

        /// <summary>
        ///     修改管理员信息
        /// </summary>
        /// <param name="mod"></param>
        /// <exception cref="MySqlException">更新失败</exception>
        public void AdminModifyInfo(AdminInfoModel mod)
        {
            AdminInfoModel info;
            try
            {
                info = GetAdminInfo(mod.ID);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            info.UpdateInfo(mod);
            var newInfo = new AdminInfo().UpdateInfo(info);
            try
            {
                DbContext.DBstatic.Updateable(newInfo).IgnoreColumns(true, ignoreAllDefaultValue: true)
                    .ExecuteCommand();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.ToString());
                //throw ex;
            }
        }

        /// <summary>
        ///     获取读者信息
        /// </summary>
        /// <param name="userID">用户 ID</param>
        /// <returns>用户信息模型</returns>
        public ReaderInfoModel GetReaderInfo(int userID)
        {
            ReaderInfoModel info = null;
            if (GlobalFunc.FindPersonById(userID))
                info = DbContext.DBstatic.Queryable<ReaderInfo>()
                    .Select(f => new ReaderInfoModel
                    {
                        ID = f.ID, Contact = f.Contact, Name = f.Name, Gender = f.Gender, Credit_Score = f.Credit_Score
                    })
                    .Where(it => it.ID == userID).Single();
            return info;
        }

        /// <summary>
        ///     修改读者信息
        /// </summary>
        /// <param name="mod"></param>
        /// <exception cref="MySqlException">更新失败</exception>
        public void ReaderModifyInfo(ReaderInfoModel mod)
        {
            var tmpDic = GetReaderInfo(mod.ID);
            tmpDic.UpdateInfo(mod);
            var newInfo = new ReaderInfo().UpdateInfo(tmpDic);
            try
            {
                DbContext.DBstatic.Updateable(newInfo).IgnoreColumns(true, ignoreAllDefaultValue: true)
                    .ExecuteCommand();
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
        ///     修改密码
        /// </summary>
        /// <param name="idi">管理员 ID</param>
        /// <param name="oldPw">原密码</param>
        /// <param name="newPw">新密码</param>
        public void AdminPasswordChange(int idi, string newPw, string oldPw = null)
        {
            if (!GlobalFunc.IsValidPassword(newPw)) throw new Exception("Password Invalid");
            if (DbContext.DBstatic.Queryable<AdminInfo>().InSingle(idi) == null) throw new Exception("Invalid ID");
            if (oldPw == null || GlobalFunc.VerifyPassword(idi, oldPw))
            {
                var salt = Guid.NewGuid().ToString();
                var pwHash = GlobalFunc.EncryptPassword(newPw, salt);
                var newInfo = new RegisterModel
                {
                    ID = idi,
                    PasswordHash = pwHash,
                    Salt = salt
                };
                AdminInfoModel tmpDic;
                try
                {
                    tmpDic = GetAdminInfo(idi);
                }
                catch (MySqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                newInfo.UpdateInfo(tmpDic);
                var adminPW = new AdminInfo().UpdatePassword(newInfo);
                try
                {
                    DbContext.DBstatic.Updateable(adminPW).IgnoreColumns(true, ignoreAllDefaultValue: true)
                        .ExecuteCommand();
                }
                catch (MySqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (!GlobalFunc.VerifyPassword(idi, newPw)) throw new Exception("Failed Change");
            }
            else
            {
                throw new Exception("Wrong Password");
            }
        }
    }
}
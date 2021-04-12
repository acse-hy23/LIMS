using System;
using lib_backend.Models;
using MySql.Data.MySqlClient;

namespace lib_backend.Services
{
    public class ReaderInfoOperation : IReaderInfoOperation
    {
        public static ReaderInfoOperation Instance { get; } = new ReaderInfoOperation();

        /// <summary>
        ///     获取读者信息
        /// </summary>
        /// <param name="userID">用户 ID</param>
        /// <returns>信息字典{ 信息类型, 信息内容 }</returns>
        public ReaderInfoModel GetReaderInfo(int userID)
        {
            ReaderInfoModel info = null;
            if (userID < 10000) throw new Exception("Permission Denied");

            if (GlobalFunc.FindPersonById(userID))
                try
                {
                    info = DbContext.DBstatic.Queryable<ReaderInfo>()
                        .Select(f => new ReaderInfoModel
                        {
                            ID = f.ID, Contact = f.Contact, Name = f.Name, Gender = f.Gender,
                            Credit_Score = f.Credit_Score
                        })
                        .Where(it => it.ID == userID).Single();
                }
                catch
                {
                    throw new Exception($"Failed to query user information with id: {userID}");
                }

            return info;
        }

        /// <summary>
        ///     修改读者信息
        /// </summary>
        /// <param name="mod"></param>
        /// <exception cref="MySqlException">更新失败</exception>
        /// <exception cref="Exception">输入错误</exception>
        public void ReaderModifyInfo(ReaderInfoModel mod)
        {
            try
            {
                GlobalFunc.InfoCheck(mod, mod.Contact != null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (mod.ID < 10000) throw new Exception("Permission Denied");

            ReaderInfoModel tmpDic;
            try
            {
                tmpDic = GetReaderInfo(mod.ID);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

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
        ///     修改密码
        /// </summary>
        /// <param name="id">读者 ID</param>
        /// <param name="oldPw">原密码</param>
        /// <param name="newPw">新密码</param>
        public void ReaderPasswordChange(int id, string newPw, string oldPw = null)
        {
            if (!GlobalFunc.IsValidPassword(newPw)) throw new Exception("Password Invalid");
            if (DbContext.DBstatic.Queryable<ReaderInfo>().InSingle(id) == null) throw new Exception("Invalid ID");
            if (oldPw == null || GlobalFunc.VerifyPassword(id, oldPw))
            {
                var salt = Guid.NewGuid().ToString();
                var pwHash = GlobalFunc.EncryptPassword(newPw, salt);
                var newInfo = new RegisterModel
                {
                    ID = id,
                    PasswordHash = pwHash,
                    Salt = salt
                };
                ReaderInfoModel tmpDic;
                try
                {
                    tmpDic = GetReaderInfo(id);
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
                var readerPw = new ReaderInfo().UpdatePassword(newInfo);
                try
                {
                    DbContext.DBstatic.Updateable(readerPw).IgnoreColumns(true, ignoreAllDefaultValue: true)
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

                if (!GlobalFunc.VerifyPassword(id, newPw)) throw new Exception("Failed Change");
            }
            else
            {
                throw new Exception("Wrong Password");
            }
        }
    }
}
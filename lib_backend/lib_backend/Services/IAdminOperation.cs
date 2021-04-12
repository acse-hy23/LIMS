using System;
using System.Collections.Generic;
using lib_backend.Models;

namespace lib_backend.Services
{
    public interface IAdminOperation
    {
        /// <summary>
        ///     获取管理员信息
        /// </summary>
        /// <param name="userID">用户 ID</param>
        /// <returns>管理员信息模型实体</returns>
        public AdminInfoModel GetAdminInfo(int userID);

        /// <summary>
        ///     修改管理员信息
        /// </summary>
        /// <param name="mod"></param>
        /// <exception cref="MySqlException">更新失败</exception>
        public void AdminModifyInfo(AdminInfoModel mod);

        /// <summary>
        ///     获取读者信息
        /// </summary>
        /// <param name="userID">用户 ID</param>
        /// <returns>信息字典{ 信息类型, 信息内容 }</returns>
        public ReaderInfoModel GetReaderInfo(int userID);

        /// <summary>
        ///     修改读者信息
        /// </summary>
        /// <param name="mod"></param>
        /// <exception cref="MySqlException">更新失败</exception>
        public void ReaderModifyInfo(ReaderInfoModel mod);

        /// <summary>
        ///     查询所有读者信息
        /// </summary>
        /// <returns>读者实体列表</returns>
        /// <exception cref="MySqlException"></exception>
        /// <exception cref="Exception"></exception>
        public List<ReaderInfoModel> GetAllReaders();

        /// <summary>
        ///     修改密码
        /// </summary>
        /// <param name="idi">管理员 ID</param>
        /// <param name="oldPw">原密码</param>
        /// <param name="newPw">新密码</param>
        public void AdminPasswordChange(int idi, string newPw, string oldPw = null);
    }
}
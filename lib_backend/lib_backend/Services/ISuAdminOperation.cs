using System;
using System.Collections.Generic;
using lib_backend.Models;

namespace lib_backend.Services
{
    public interface ISuAdminOperation
    {
        /// <summary>
        ///     查询所有读者信息
        /// </summary>
        /// <returns>读者实体列表</returns>
        /// <exception cref="MySqlException"></exception>
        /// <exception cref="Exception"></exception>
        public List<ReaderInfoModel> GetAllReaders();

        /// <summary>
        ///     获取所有管理员信息
        /// </summary>
        /// <returns>管理员实体列表</returns>
        public List<AdminInfoModel> GetAllAdmins();

        /// <summary>
        ///     创建管理员账户
        /// </summary>
        /// <param name="reg"></param>
        /// <exception cref="MySqlException"></exception>
        /// <exception cref="Exception"></exception>
        public int CreateAdminAccount(RegisterModel reg);
    }
}
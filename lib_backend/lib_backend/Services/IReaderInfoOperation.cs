using lib_backend.Models;

namespace lib_backend.Services
{
    public interface IReaderInfoOperation
    {
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
        ///     修改密码
        /// </summary>
        /// <param name="id">读者 ID</param>
        /// <param name="oldPw">原密码</param>
        /// <param name="newPw">新密码</param>
        public void ReaderPasswordChange(int id, string newPw, string oldPw = null);
    }
}
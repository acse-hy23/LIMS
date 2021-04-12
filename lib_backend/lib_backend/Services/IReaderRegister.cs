using lib_backend.Models;

namespace lib_backend.Services
{
    public interface IReaderRegister
    {
        /// <summary>
        ///     接受注册信息，并将用户信息写入数据库
        /// </summary>
        /// <param name="ls"></param>
        /// <returns>用户 ID</returns>
        int AccepteRegister(RegisterModel reg);
    }
}
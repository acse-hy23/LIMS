using System;
using System.Text;
using lib_backend.Models;
using lib_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace lib_backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        /// <summary>
        ///     提交注册信息 API
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="contact">联系方式</param>
        /// <returns>注册成功返回用户 ID，注册失败返回 error</returns>
        [HttpPost]
        public IActionResult Submit([FromQuery] string name, [FromQuery] string password, [FromQuery] string contact)
        {
            name = GlobalFunc.MyUrlDeCode(name, Encoding.UTF8);
            var info = new RegisterModel
            {
                Name = name,
                Password = password,
                Contact = contact
            };
            var newId = 0;
            IReaderRegister reg = new ReaderRegister();
            try
            {
                newId = reg.AccepteRegister(info);
            }
            catch (Exception ex)
            {
                return Ok(new {error = ex.Message});
            }

            return Ok(new {id = newId});
        }
    }
}
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using lib_backend.Models;
using lib_backend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lib_backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class InfoModifyController : ControllerBase
    {
        private readonly IAdminOperation ad = AdminOperation.Instance;
        private readonly IReaderInfoOperation rd = ReaderInfoOperation.Instance;

        /// <summary>
        ///     修改用户个人信息接口
        /// </summary>
        /// <param name="contact">联系方式</param>
        /// <param name="name">用户名</param>
        /// <param name="gender">性别</param>
        /// <returns>成功返回修改后的用户信息 json，失败返回 error</returns>
        [HttpPut]
        public IActionResult ModInfo(string contact = null, [FromQuery] string name = null,
            [FromQuery] string gender = null)
        {
            name = GlobalFunc.MyUrlDeCode(name, Encoding.UTF8);
            gender = GlobalFunc.MyUrlDeCode(gender, Encoding.UTF8);
            var auth = HttpContext.AuthenticateAsync();
            var id = Convert.ToInt32(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.NameIdentifier))
                ?.Value);
            if (id >= 10000)
            {
                var newInfo = new ReaderInfoModel
                {
                    ID = id,
                    Name = name,
                    Contact = contact,
                    Gender = gender
                };
                try
                {
                    rd.ReaderModifyInfo(newInfo);
                }
                catch (Exception ex)
                {
                    return Ok(new {error = ex.Message});
                }
            }
            else if (id < 10000 && id >= 1)
            {
                var newInfo = new AdminInfoModel
                {
                    ID = id,
                    Name = name,
                    Contact = contact,
                    Gender = gender
                };
                try
                {
                    ad.AdminModifyInfo(newInfo);
                }
                catch (Exception ex)
                {
                    return Ok(new {error = ex.Message});
                }
            }
            else
            {
                return Ok(new {error = "Invalid Id"});
            }

            return Ok(GlobalFunc.GetBasicInfo(id));
        }

        /// <summary>
        ///     用户修改密码
        /// </summary>
        /// <param name="oldPw">原密码</param>
        /// <param name="newPw">新密码</param>
        /// <returns>成功返回用户信息 json，失败返回 error</returns>
        [HttpPut]
        public IActionResult ModPw([FromQuery] string oldPw, [FromQuery] string newPw)
        {
            var auth = HttpContext.AuthenticateAsync();
            var id = Convert.ToInt32(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.NameIdentifier))
                ?.Value);
            if (id >= 10000)
                try
                {
                    rd.ReaderPasswordChange(id, newPw, oldPw);
                }
                catch (Exception ex)
                {
                    return Ok(new {error = ex.Message});
                }
            else if (id < 10000 && id >= 1)
                try
                {
                    ad.AdminPasswordChange(id, newPw, oldPw);
                }
                catch (Exception ex)
                {
                    return Ok(new {error = ex.Message});
                }
            else
                return Ok(new {error = "Invalid Id"});

            return Ok(GlobalFunc.GetBasicInfo(id));
        }
    }
}
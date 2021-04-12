using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using lib_backend.Models;
using lib_backend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace lib_backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AdminOpController : ControllerBase
    {
        private readonly IAdminOperation ad = AdminOperation.Instance;
        private readonly IReaderInfoOperation rd = ReaderInfoOperation.Instance;
        private readonly ISuAdminOperation su = SuAdminOperation.Instance;

        /// <summary>
        ///     SU 管理员操作，查询全部读者
        /// </summary>
        /// <returns>读者实体列表</returns>
        [HttpGet]
        public IActionResult GetReaderList()
        {
            var auth = HttpContext.AuthenticateAsync();
            var id = Convert.ToInt32(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.NameIdentifier))
                ?.Value);
            if (GlobalFunc.IsAdminId(id))
                try
                {
                    var readerList = ad.GetAllReaders();
                    return Ok(new {status = "success", data = readerList});
                }
                catch (MySqlException ex)
                {
                    return Ok(new {error = ex.Message});
                }
                catch (Exception ex)
                {
                    return Ok(new {error = ex.Message});
                }

            return Ok(new {error = "Need admin authority"});
        }

        /// <summary>
        ///     SU 管理员操作，查询全部管理员信息
        /// </summary>
        /// <returns>管理员实体列表</returns>
        [HttpGet]
        public IActionResult GetAdminList()
        {
            var auth = HttpContext.AuthenticateAsync();
            var id = Convert.ToInt32(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.NameIdentifier))
                ?.Value);
            if (id == 1)
                try
                {
                    var adminList = su.GetAllAdmins();
                    return Ok(adminList);
                }
                catch (MySqlException ex)
                {
                    return Ok(new {error = ex.Message});
                }
                catch (Exception ex)
                {
                    return Ok(new {error = ex.Message});
                }

            return Ok(new {error = "Need su authority"});
        }

        /// <summary>
        ///     创建管理员账户
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="contact">联系方式</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateAdminAccount([FromQuery] string name, [FromQuery] string contact, string password)
        {
            name = GlobalFunc.MyUrlDeCode(name, Encoding.UTF8);
            name = GlobalFunc.MyUrlDeCode(name, Encoding.UTF8);
            var auth = HttpContext.AuthenticateAsync();
            var id = Convert.ToInt32(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.NameIdentifier))
                ?.Value);
            if (id == 1)
            {
                var info = new RegisterModel
                {
                    Name = name,
                    Contact = contact,
                    Password = password
                };
                var newId = 0;
                try
                {
                    newId = su.CreateAdminAccount(info);
                }
                catch (Exception ex)
                {
                    return Ok(new {error = ex.Message});
                }

                return Ok(new {id = newId});
            }

            return Ok(new {error = "Need su authority"});
        }

        /// <summary>
        ///     管理员修改读者信息
        /// </summary>
        /// <param name="id">要修改的读者信息</param>
        /// <param name="contact">联系方式</param>
        /// <param name="name">用户名</param>
        /// <param name="gender">性别</param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult ModReaderInfo([FromQuery] string id, [FromQuery] string contact = null,
            [FromQuery] string name = null, [FromQuery] string gender = null)
        {
            name = GlobalFunc.MyUrlDeCode(name, Encoding.UTF8);
            gender = GlobalFunc.MyUrlDeCode(gender, Encoding.UTF8);
            try
            {
                GlobalFunc.GetBasicInfo(Convert.ToInt32(id));
            }
            catch (Exception ex)
            {
                return Ok(new {error = ex.Message});
            }

            var newInfo = new ReaderInfoModel
            {
                ID = Convert.ToInt32(id),
                Name = name,
                Contact = contact,
                Gender = gender
            };
            try
            {
                ad.ReaderModifyInfo(newInfo);
            }
            catch (Exception ex)
            {
                return Ok(new {error = ex.Message});
            }

            return Ok(new {status = "success", data = GlobalFunc.GetBasicInfo(Convert.ToInt32(id))});
        }

        /// <summary>
        ///     su 管理员重置用户密码
        /// </summary>
        /// <param name="modId">要重置密码的用户 ID</param>
        /// <param name="newPw">新密码</param>
        /// <returns>被重置密码的用户 ID 和新密码</returns>
        [HttpPut]
        public IActionResult ResetUserPw([FromQuery] int modId, [FromQuery] string newPw)
        {
            if (modId <= 0) return Ok(new {error = "Invalid ID"});
            var auth = HttpContext.AuthenticateAsync();
            var id = Convert.ToInt32(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.NameIdentifier))
                ?.Value);
            if (id != 1) return Ok(new {error = "Need su authority"});
            if (modId < 10000)
                try
                {
                    ad.AdminPasswordChange(modId, newPw);
                }
                catch (Exception ex)
                {
                    return Ok(new {error = ex.Message});
                }
            else
                try
                {
                    rd.ReaderPasswordChange(modId, newPw);
                }
                catch (Exception ex)
                {
                    return Ok(new {error = ex.Message});
                }

            return Ok(new {id = modId, password = newPw});
        }
    }
}
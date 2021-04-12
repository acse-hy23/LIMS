using System;
using System.Linq;
using System.Security.Claims;
using lib_backend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lib_backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class HomeController : Controller
    {
        /// <summary>
        ///     个人信息页面相关接口，需要身份验证
        /// </summary>
        /// <returns>验证成功返回请求的相应信息，验证失败返回 401 Unauthorized 错误页面</returns>
        [HttpGet]
        public IActionResult Info()
        {
            //这里是使用 JWT 并识别身份的一个例子
            //以下两行在验证通过之后用于获取我们在 payload 中自定义的那部分内容，即用户 ID
            var auth = HttpContext.AuthenticateAsync();
            var id = Convert.ToInt32(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.NameIdentifier))
                ?.Value);
            return Ok(GlobalFunc.GetBasicReaderInfoJson(id));
        }

        /// <summary>
        ///     获取用户的超期图书列表用于提醒
        /// </summary>
        /// <returns>超期未还的图书列表</returns>
        [HttpGet]
        public IActionResult Tips()
        {
            var auth = HttpContext.AuthenticateAsync();
            var id = Convert.ToInt32(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.NameIdentifier))
                ?.Value);
            return Ok(GlobalFunc.GetTips(id));
        }
    }
}
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using lib_backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace lib_backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : Controller
    {
        /// <summary>
        ///     提交登录信息验证
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns>提交登录信息，返回 token</returns>
        [HttpPost]
        public IActionResult Submit([FromQuery] string id, [FromQuery] string password)
        {
            return GetToken(id, password);
        }

        /// <summary>
        ///     私有方法，获取 JWT Token
        /// </summary>
        /// <param name="id">用户 ID</param>
        /// <param name="password">用户密码</param>
        /// <returns>Token</returns>
        [HttpGet]
        private IActionResult GetToken(string id, string password)
        {
            try
            {
                //调用密码验证的方法，若密码不正确或其他输入不正确等情况抛出异常
                GlobalFunc.VerifyPassword(Convert.ToInt64(id), password);
            }
            catch (MySqlException e)
            {
                return Ok(new {DatabaseError = e.Message});
            }
            catch (Exception e)
            {
                return Ok(new {Error = e.Message});
            }

            //生成 token 的 payload 部分
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Exp,
                    $"{new DateTimeOffset(DateTime.Now.AddMinutes(30)).ToUnixTimeSeconds()}"),
                new Claim(ClaimTypes.NameIdentifier, id) //自定义的 payload 部分，包含了用户的 ID 用于识别身份
            };
            //生成密钥用于签名
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GlobalVar.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //生成 token
            var token = new JwtSecurityToken(
                GlobalVar.Domain,
                GlobalVar.Domain,
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);
            //返回 token 给客户端使用
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
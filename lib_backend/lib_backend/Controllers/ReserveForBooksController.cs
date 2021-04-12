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
    public class ReservationForBooksController : Controller
    {
        /// <summary>
        ///     预约书
        /// </summary>
        /// <param name="bookId">书的id</param>
        /// <returns>如果发生错误则返回错误信息，如果没有错误则返回预约成功的读者id即书的id</returns>
        [Authorize]
        [HttpGet]
        public IActionResult ReserveForBooks(int bookId)
        {
            var auth = HttpContext.AuthenticateAsync();
            var readerId = Convert.ToInt32(auth.Result.Principal.Claims
                .First(t => t.Type.Equals(ClaimTypes.NameIdentifier))?.Value);
            if (!Helper.checkReaderId(readerId))
                return Ok(new {error = "不存在该读者id"});
            if (!Helper.checkBookId(bookId))
                return Ok(new {error = "不存在该书id"});
            if (Helper.checkReservationForBooks(readerId, bookId))
                return Ok(new {error = "预约失败"});
            return Ok(new {id = readerId, book = bookId});
        }
    }
}
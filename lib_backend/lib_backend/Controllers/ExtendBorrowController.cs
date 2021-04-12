using lib_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lib_backend.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class ExtendBorrowController : Controller
    {
        /// <summary>
        ///     延长借书时间
        /// </summary>
        /// <param name="borrowId">借书id</param>
        /// <returns>如果发生错误则返回错误信息，如果没有错误则返回成功信息</returns>
        [HttpPut]
        public IActionResult ExtendBorrow(int borrowId)
        {
            if (!Helper.checkBorrowed(borrowId))
                return Ok(new {error = "不存在该借书id"});
            if (!Helper.ableToExtend(borrowId))
                return Ok(new {error = "续借失败"});
            return Ok(new {success = borrowId});
        }
    }
}
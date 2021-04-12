using System;
using System.Linq;
using System.Security.Claims;
using lib_backend.Models;
using lib_backend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lib_backend.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class StudyRoomController : Controller
    {
        /// <summary>
        ///     获取自习室信息列表
        /// </summary>
        /// <returns>自习室信息</returns>
        [HttpGet]
        public IActionResult GetStudyRooms()
        {
            var studyRooms = DbContext.DBstatic.Queryable<StudyRoom>().ToList();
            return Ok(studyRooms);
        }

        [HttpGet]
        /// <summary>
        /// 预定自习室
        /// </summary>
        /// <param name="roomId">自习室id</param>
        /// <param name="seatId">位置id</param>
        /// <param name="time">预定的时间</param>
        /// <returns>如果发生错误则分别返回情况，如果没有错误则返回预约成功的信息</returns>
        public IActionResult ReserveForRooms(int roomId, string seatId, DateTime date)
        {
            var auth = HttpContext.AuthenticateAsync();
            var readerId = Convert.ToInt32(auth.Result.Principal.Claims
                .First(t => t.Type.Equals(ClaimTypes.NameIdentifier))?.Value);
            if (!Helper.checkReaderId(readerId))
                return Ok(new {error = "不存在该读者id"});
            if (!Helper.checkSeat(seatId))
                return Ok(new {error = "不存在该座位id"});
            if (Helper.checkReservationForRooms(readerId, roomId, seatId, date))
                return Ok(new {error = "预约失败"});
            return Ok(new
            {
                id = readerId,
                date = date.Year.ToString() + '-' + date.Month + '-' + date.Day,
                room = roomId,
                seat = seatId
            });
        }
    }
}
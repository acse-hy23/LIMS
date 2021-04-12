using System;
using System.Linq;
using System.Security.Claims;
using lib_backend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lib_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecommendationsController : Controller
    {
        private readonly IBookRepository _bookRepository;

        public RecommendationsController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        /// <summary>
        ///     获取全部的推荐书目
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAllRecommendations()
        {
            var recomFromRepo = _bookRepository.AllRecommendation();

            if (recomFromRepo == null || recomFromRepo.Count() <= 0) return Ok("没有推荐的书籍");
            return Ok(recomFromRepo);
        }

        /// <summary>
        ///     根据读者Id获取推荐书目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetRecommendationsByReaderId(int id)
        {
            var recomFromRepo = _bookRepository.RecommendationByReaderId(id);

            if (recomFromRepo == null || recomFromRepo.Count() <= 0) return Ok("没有该用户的推荐书籍");
            return Ok(recomFromRepo);
        }

        /// <summary>
        ///     获取没有处理的推荐请求
        /// </summary>
        /// <returns></returns>
        [HttpGet("unprocessed")]
        public IActionResult GetUnprocessedRecommendations()
        {
            var recomFromRepo = _bookRepository.UnprocessedRecommendation();

            if (recomFromRepo == null || recomFromRepo.Count() <= 0) return Ok(new {error = "没有未处理的推荐书籍"});
            return Ok(recomFromRepo);
        }

        /// <summary>
        ///     推荐书
        /// </summary>
        /// <param name="name"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult RecommendBook([FromQuery] string name, string reason)
        {
            var auth = HttpContext.AuthenticateAsync();
            var userId = Convert.ToInt32(auth.Result.Principal.Claims
                .First(t => t.Type.Equals(ClaimTypes.NameIdentifier))?.Value);
            var result = _bookRepository.RecommendBook(userId, name, reason);
            return Ok(result);
        }

        /// <summary>
        ///     修改推荐书目信息/状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut]
        public IActionResult Put([FromQuery] int recommendId, int state)
        {
            var result = _bookRepository.ProcessRecommendation(recommendId, state);
            return Ok(result);
        }
    }
}
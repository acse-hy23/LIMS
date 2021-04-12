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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        /// <summary>
        /// 获取全部的书籍
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetBooks()
        {
            var booksFromRepo = _bookRepository.GetBooks();

            if (booksFromRepo == null || booksFromRepo.Count() <= 0) return Ok("没有书籍");
            return Ok(booksFromRepo);
        }

        /// <summary>
        /// 通过Id获取某一本书的信息
        /// </summary>
        /// <param name="id"></param> 书的Id
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetBooksById(int id)
        {
            var auth = HttpContext.AuthenticateAsync();
            var userId = Convert.ToInt32(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.NameIdentifier))?.Value);
            if (GlobalFunc.IsAdminId(userId)) return Ok(new { error = "Need admin authority" });
            var bookFromRepo = _bookRepository.GetBookById(id);
            if (bookFromRepo == null) return Ok(new { error = $"没有ID为{id}的书籍" });
            return Ok(bookFromRepo);
        }

        /// <summary>
        /// 通过Name获取匹配的书的列表
        /// </summary>
        /// <param name="name"></param> 书的名字
        /// <returns></returns>
        [HttpGet("search")]
        public IActionResult GetBooksByName([FromQuery] string name)
        {
            name = GlobalFunc.MyUrlDeCode(name, Encoding.UTF8);
            var bookFromRepo = _bookRepository.GetBooksByName(name);
            if (bookFromRepo == null) return Ok(new { error = $"没有名字为{name}的书籍" });
            return Ok(bookFromRepo);
        }

        /// <summary>
        ///  新建书籍
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateBook([FromBody] Book book)
        {
            var createdBook = _bookRepository.CreateBook(book);
            return Ok(createdBook);
        }

        /// <summary>
        /// 修改书籍
        /// </summary>
        /// <param name="book"></param>
        [HttpPut]
        public IActionResult ChangeBook([FromBody] Book book)
        {
            var result = _bookRepository.ChangeBook(book);
            return Ok(result);
        }

        /// <summary>
        /// 删除书籍
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            var result = _bookRepository.DeleteBook(id);
            return Ok(result);
        }

        /// <summary>
        /// 获取该用户的所有借阅记录
        /// </summary>
        /// <returns></returns>
        [HttpGet("borrow")]
        public IActionResult BorrowBook()
        {
            var auth = HttpContext.AuthenticateAsync();
            var userId = Convert.ToInt32(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.NameIdentifier))?.Value);
            var result = _bookRepository.GetBorrowsByReaderId(userId);
            return Ok(result);
        }

        /// <summary>
        /// 获取一本书的借阅记录
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        [HttpGet("borrow/{bookId}")]
        public IActionResult BorrowRecordOfBook(int bookId)
        {
            var result = _bookRepository.GetBorrowsByBookId(bookId);
            return Ok(result);
        }

        //可能需要一个获取该用户所有未还的书的接口


        /// <summary>
        /// 借书
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("borrow/{bookId}")]
        public IActionResult BorrowBook(int bookId)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                var userId = Convert.ToInt32(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.NameIdentifier))?.Value);
                var result = _bookRepository.BorrowBook(userId, bookId);
                var borrowTime = result.Borrow_Time.ToString("yyyy-MM-dd");
                var expireTime = result.Expire_Time.ToString("yyyy-MM-dd");
                return Ok(new
                {
                    result.Borrow_id,
                    result.Book_id,
                    result.Reader_id,
                    result.State,
                    borrowTime,
                    expireTime
                });
            }
            catch (Exception ex) { return Ok(new { error = ex.Message }); }
        }

        /// <summary>
        /// 还书
        /// </summary>
        /// <param name="borrowId"></param>
        /// <returns></returns>
        [HttpPost("return")]
        public IActionResult ReturnBook(int bookId)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                var userId = Convert.ToInt32(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.NameIdentifier))?.Value);
                if (!GlobalFunc.IsAdminId(userId)) return Ok(new { error = "Need admin authority" });
                var result = _bookRepository.ReturnBook(bookId);
                var borrowTime = result.Borrow_Time.ToString("yyyy-MM-dd");
                var returnTime = result.Return_Time.ToString("yyyy-MM-dd");
                var expireTime = result.Expire_Time.ToString("yyyy-MM-dd");
                var expired = result.Expire_Time < result.Return_Time;
                return Ok(new
                {
                    result.Borrow_id,
                    result.Book_id,
                    result.Reader_id,
                    result.State,
                    borrowTime,
                    returnTime,
                    expireTime,
                    expired
                });
            }
            catch (Exception ex)
            {
                return Ok(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 获取热门书籍
        /// </summary>
        /// <returns></returns>
        [HttpGet("popular")]
        public IActionResult GetPopularBooks()
        {
            try
            {
                return Ok(_bookRepository.GetBookBorrowTimes());
            }
            catch (Exception ex) { return Ok(new { error = ex.Message }); }
        }
    }
}

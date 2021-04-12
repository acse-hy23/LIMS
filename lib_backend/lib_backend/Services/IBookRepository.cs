using System.Collections.Generic;
using lib_backend.Models;

namespace lib_backend.Services
{
    public interface IBookRepository
    {
        /// <summary>
        ///     获取全部书籍
        /// </summary>
        /// <returns></returns>
        IEnumerable<Book> GetBooks();

        /// <summary>
        ///     根据名字查询相关书籍
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        IEnumerable<Book> GetBooksByName(string Name);

        /// <summary>
        ///     根据ID查找书籍
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Book GetBookById(int Id);

        /// <summary>
        ///     创建书籍信息
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        Book CreateBook(Book book);

        /// <summary>
        ///     修改书籍信息
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        Book ChangeBook(Book book);

        /// <summary>
        ///     删除书籍信息
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        int DeleteBook(int bookId);

        /// <summary>
        ///     借书
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        Borrows BorrowBook(int userId, int bookId);

        /// <summary>
        ///     还书
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="borrowId"></param>
        /// <returns></returns>
        Borrows ReturnBook(int bookId);

        /// <summary>
        ///     获取此人所有借阅记录
        /// </summary>
        /// <param name="readerId"></param>
        /// <returns></returns>
        IEnumerable<Borrows> GetBorrowsByReaderId(int readerId);

        /// <summary>
        ///     获取本书的借阅记录
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        IEnumerable<Borrows> GetBorrowsByBookId(int bookId);

        /// <summary>
        ///     推荐书
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        bool RecommendBook(int userId, string name, string reason);

        /// <summary>
        ///     处理推荐
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        int ProcessRecommendation(int recommendId, int state);

        /// <summary>
        ///     检索全部推荐
        /// </summary>
        /// <returns></returns>
        IEnumerable<Recommend> AllRecommendation();

        /// <summary>
        ///     检索未处理推荐
        /// </summary>
        /// <returns></returns>
        IEnumerable<Recommend> UnprocessedRecommendation();

        /// <summary>
        ///     按照读者ID检索推荐
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        IEnumerable<Recommend> RecommendationByReaderId(int Id);

        /// <summary>
        ///     获取所有书籍被借阅的次数
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object> GetBookBorrowTimes();
    }
}
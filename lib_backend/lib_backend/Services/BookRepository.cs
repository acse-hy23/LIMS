using System;
using System.Collections.Generic;
using lib_backend.Models;
using SqlSugar;

namespace lib_backend.Services
{
    public class BookRepository : IBookRepository
    {
        /// <summary>
        ///     存储mock数据
        /// </summary>
        private List<Book> _books;

        public BookRepository()
        {
            if (_books == null) InitializeBooks();
        }

        ///无数据库测试
        //public Book GetBookByName(string Name)
        //{
        //    return _books.FirstOrDefault(n => n.Bookname == Name);
        //    throw new NotImplementedException();
        //}

        /// <summary>
        ///     根据ID获取书
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Book GetBookById(int Id)
        {
            var book = DbContext.DBstatic.Queryable<Book>().InSingle(Id);
            return book;
            throw new NotImplementedException();
        }

        /// <summary>
        ///     获取全部书籍
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Book> GetBooks()
        {
            var books = DbContext.DBstatic.Queryable<Book>().ToList();
            return books;
            throw new NotImplementedException();
        }

        /// <summary>
        ///     根据书名匹配书籍
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        IEnumerable<Book> IBookRepository.GetBooksByName(string Name)
        {
            var books = DbContext.DBstatic.Queryable<Book>().Where(it => it.Book_name.Contains(Name)).ToList();
            return books;
            throw new NotImplementedException();
        }

        /// <summary>
        ///     新建书籍信息
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public Book CreateBook(Book book)
        {
            var createdBook = DbContext.DBstatic.Saveable(book).ExecuteReturnEntity();
            return createdBook;
            throw new NotImplementedException();
        }

        /// <summary>
        ///     修改书籍信息
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public Book ChangeBook(Book book)
        {
            var result = DbContext.DBstatic.Saveable(book).ExecuteReturnEntity();
            return result;
            throw new NotImplementedException();
        }

        /// <summary>
        ///     删除书籍信息
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public int DeleteBook(int bookId)
        {
            var result = DbContext.DBstatic.Deleteable<Book>().Where(new Book {Id = bookId}).ExecuteCommand();
            return result;
            throw new NotImplementedException();
        }

        /// <summary>
        ///     获取此人的借阅记录
        /// </summary>
        /// <param name="readerId"></param>
        /// <returns></returns>
        public IEnumerable<Borrows> GetBorrowsByReaderId(int readerId)
        {
            var result = DbContext.DBstatic.Queryable<Borrows>().Where(it => it.Reader_id == readerId).ToList();
            return result;
            throw new NotImplementedException();
        }

        /// <summary>
        ///     获取本书的借阅记录
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public IEnumerable<Borrows> GetBorrowsByBookId(int bookId)
        {
            var result = DbContext.DBstatic.Queryable<Borrows>().Where(it => it.Book_id == bookId).ToList();
            return result;
            throw new NotImplementedException();
        }

        /// <summary>
        ///     借书
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public Borrows BorrowBook(int userId, int bookId)
        {
            if (!GetBookState(bookId)) throw new Exception("Books lent out");
            if (!GlobalFunc.GetBorrowState(userId)) throw new Exception("Low credit score");
            //修改书的状态，前端应该已经完成对book的状态的检测
            DbContext.DBstatic.Updateable<Book>().SetColumns(it => new Book {State = 1}).Where(it => it.Id == bookId)
                .ExecuteCommand();

            //插入borrow表
            var result = DbContext.DBstatic.Saveable(new Borrows
            {
                Reader_id = userId,
                Book_id = bookId,
                Renew = 0,
                Borrow_Time = DateTime.Now.Date,
                Expire_Time = DateTime.Now.Date.AddMonths(1),
                State = 0
            }).ExecuteReturnEntity();
            return result;
            throw new NotImplementedException();
        }


        /// <summary>
        ///     还书
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="borrowId"></param>
        /// <returns></returns>
        public Borrows ReturnBook(int bookId)
        {
            //修改书的状态
            try
            {
                var borrowId = GetBorrowIdWithBookId(bookId);
                DbContext.DBstatic.Updateable<Book>().SetColumns(it => new Book {State = 0})
                    .Where(it => it.Id == bookId).ExecuteCommand();
                ReturnResult(borrowId);
                DbContext.DBstatic.Updateable<Borrows>().SetColumns(it => new Borrows
                {
                    State = 1,
                    Return_Time = DateTime.Now.Date
                }).Where(it => it.Borrow_id == borrowId).ExecuteCommand();
                return DbContext.DBstatic.Queryable<Borrows>().InSingle(borrowId);
            }
            catch (Exception e)
            {
                throw e;
            }

            //throw new NotImplementedException();
        }

        /// <summary>
        ///     推荐书籍
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="_name"></param>
        /// <param name="_reason"></param>
        /// <returns></returns>
        public bool RecommendBook(int userId, string _name, string _reason)
        {
            DbContext.DBstatic.Saveable(new Recommend
            {
                recommender = userId,
                name = _name,
                reason = _reason,
                state = 0
            }).ExecuteReturnEntity();
            return true;
            throw new NotImplementedException();
        }

        /// <summary>
        ///     处理（未实现）
        /// </summary>
        /// <param name="recommend"></param>
        /// <returns></returns>
        public int ProcessRecommendation(int recommendId, int _state)
        {
            var result = DbContext.DBstatic.Updateable<Recommend>().SetColumns(it => new Recommend {state = _state})
                .Where(it => it.id == recommendId).ExecuteCommand();
            return result;
            throw new NotImplementedException();
        }

        /// <summary>
        ///     检索全部推荐
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Recommend> AllRecommendation()
        {
            var recommendations = DbContext.DBstatic.Queryable<Recommend>().ToList();
            return recommendations;
            throw new NotImplementedException();
        }

        /// <summary>
        ///     检索没有处理过的推荐
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Recommend> UnprocessedRecommendation()
        {
            var recommendations = DbContext.DBstatic.Queryable<Recommend>().Where(it => it.state == 0).ToList();
            return recommendations;
            throw new NotImplementedException();
        }

        /// <summary>
        ///     检索该读者的推荐
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public IEnumerable<Recommend> RecommendationByReaderId(int Id)
        {
            var recommendations = DbContext.DBstatic.Queryable<Recommend>().Where(it => it.recommender == Id).ToList();
            return recommendations;
            throw new NotImplementedException();
        }

        /// <summary>
        ///     获取所有书籍被借阅的次数
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object> GetBookBorrowTimes()
        {
            //太踏马鬼畜了这玩意
            try
            {
                var ls = DbContext.DBstatic
                    .Queryable<Borrows, Book>((br, bk) => new object[]
                    {
                        JoinType.Left, br.Book_id == bk.Id
                    })
                    .GroupBy((br, bk) => bk.Isbn)
                    .Having((br, bk) => SqlFunc.AggregateCount(bk.Isbn) > 0)
                    .Select((br, bk) => new PopBook {ISBN = bk.Isbn, Times = SqlFunc.AggregateCount(bk.Isbn)})
                    .MergeTable()
                    .Mapper(pb => { pb.GetName(); })
                    .OrderBy(pb => pb.Times, OrderByType.Desc)
                    .ToList();
                return ls;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     Mock 数据
        /// </summary>
        private void InitializeBooks()
        {
            _books = new List<Book>
            {
                new Book
                {
                    Id = 1,
                    Book_name = "book1",
                    Publishing_house = "AAA"
                },
                new Book
                {
                    Id = 2,
                    Book_name = "book2",
                    Publishing_house = "BBB"
                }
            };
        }

        public int GetBorrowIdWithBookId(int bookId)
        {
            int borrowId;
            try
            {
                borrowId = DbContext.DBstatic.Queryable<Borrows>()
                    .Where(br => br.Book_id == bookId && br.State != 1)
                    .Select(br => br.Borrow_id)
                    .First();
                if (borrowId == 0) throw new Exception($"Failed to query unreturn book id:{borrowId}");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return borrowId;
        }

        /// <summary>
        ///     还书后根据借阅 ID 查询是否超期，若超期则扣信用分
        /// </summary>
        /// <param name="borrowId"></param>
        public void ReturnResult(int borrowId)
        {
            try
            {
                GlobalFunc.RefreshBookState();
                var state = DbContext.DBstatic.Queryable<Borrows>()
                    .Where(br => br.Borrow_id == borrowId)
                    .Select(br => br.State)
                    .First();
                if (state == -1)
                {
                    var userId = DbContext.DBstatic.Queryable<Borrows>()
                        .Where(br => br.Borrow_id == borrowId)
                        .Select(rd => rd.Reader_id)
                        .First();
                    var score = DbContext.DBstatic.Queryable<ReaderInfo>()
                        .Where(rd => rd.ID == userId)
                        .Select(rd => rd.Credit_Score)
                        .First();
                    var setScore = score - 30;
                    if (setScore < 0) setScore = 0;
                    DbContext.DBstatic.Updateable<Reader>()
                        .SetColumns(rd => new Reader
                        {
                            credit_score = setScore
                        })
                        .Where(rd => rd.id == userId)
                        .ExecuteCommand();
                }
            }
            catch
            {
                throw new Exception("Credit modification failed");
            }
        }

        /// <summary>
        ///     获取书籍的可借阅状态
        /// </summary>
        /// <returns></returns>
        public bool GetBookState(int bookid)
        {
            var state = DbContext.DBstatic.Queryable<Book>()
                .Where(bk => bk.Id == bookid)
                .Select(bk => bk.State)
                .Single();
            return state == 0;
        }
    }
}
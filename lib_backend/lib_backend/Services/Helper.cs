using System;
using lib_backend.Models;

namespace lib_backend.Services
{
    public class Helper
    {
        /// <summary>
        ///     查询读者是否存在
        /// </summary>
        /// <param name="readerId">读者id</param>
        /// <returns>是否存在</returns>
        public static bool checkReaderId(int readerId)
        {
            var reader = DbContext.DBstatic.Queryable<Reader>().InSingle(readerId);
            if (reader == null) return false;
            return true;
        }

        /// <summary>
        ///     查询座位是否存在
        /// </summary>
        /// <param name="seatId">座位id</param>
        /// <returns>是否存在座位</returns>
        public static bool checkSeat(string seatId)
        {
            var seat = DbContext.DBstatic.Queryable<Seat>().InSingle(seatId);
            if (seat == null) return false;
            return true;
        }

        /// <summary>
        ///     查询书是否存在
        /// </summary>
        /// <param name="bookId">书的id</param>
        /// <returns>是否存在这本书</returns>
        public static bool checkBookId(int bookId)
        {
            var book = DbContext.DBstatic.Queryable<Book>().InSingle(bookId);
            if (book == null) return false;
            return true;
        }

        /// <summary>
        ///     确认是否已经被预约，如果还没有被预约则预约该本书
        /// </summary>
        /// <param name="readerId">读者id</param>
        /// <param name="bookId">书id</param>
        /// <returns>是否已经被预约</returns>
        public static bool checkReservationForBooks(int readerId, int bookId)
        {
            var reservation = DbContext.DBstatic.Queryable<ReservationForBooks>().Where(it => it.book_id == bookId)
                .ToList();
            if (reservation.Count != 0) return true;

            var reservationForBooks = new ReservationForBooks
            {
                book_id = bookId,
                reader_id = readerId
            };
            DbContext.DBstatic.Insertable(reservationForBooks).ExecuteCommand();
            return false;
        }

        /// <summary>
        ///     是否能够续借，如果能够续借则延期一个月
        /// </summary>
        /// <param name="borrowId">借书id</param>
        /// <returns>是否能够续借</returns>
        public static bool ableToExtend(int borrowId)
        {
            var borrows = DbContext.DBstatic.Queryable<Borrows>().InSingle(borrowId);
            if (borrows.Renew == 2) return false;

            var extendBorrows = new Borrows
            {
                Borrow_id = borrowId,
                Book_id = borrows.Book_id,
                Reader_id = borrows.Reader_id,
                State = borrows.State,
                Renew = borrows.Renew + 1,
                Borrow_Time = borrows.Borrow_Time,
                Return_Time = borrows.Return_Time,
                Expire_Time = borrows.Expire_Time.AddMonths(1)
            };
            DbContext.DBstatic.Updateable(extendBorrows).ExecuteCommand();
            return true;
        }

        /// <summary>
        ///     确认自习室座位是否能够预约
        /// </summary>
        /// <param name="readerId">读者id</param>
        /// <param name="roomId">自习室id</param>
        /// <param name="seatId">座位id</param>
        /// <param name="date">预约日期</param>
        /// <returns></returns>
        public static bool checkReservationForRooms(int readerId, int roomId, string seatId, DateTime date)
        {
            var readerReservation = DbContext.DBstatic.Queryable<ReservationForRooms>()
                .Where(it => it.readerId == readerId && it.reserve_date == date.Date).ToList();
            var seatReservation = DbContext.DBstatic.Queryable<ReservationForRooms>()
                .Where(it => it.seatId == seatId && it.roomId == roomId && it.reserve_date == date.Date).ToList();
            if (readerReservation.Count != 0) return true;

            if (seatReservation.Count != 0) return true;

            var reservationForRooms = new ReservationForRooms
            {
                readerId = readerId,
                roomId = roomId,
                seatId = seatId,
                reserve_date = date.Date
            };
            DbContext.DBstatic.Insertable(reservationForRooms).ExecuteCommand();
            return false;
        }

        /// <summary>
        ///     确认是否已经被借
        /// </summary>
        /// <param name="borrowId">借书id</param>
        /// <returns>是否已经被借</returns>
        public static bool checkBorrowed(int borrowId)
        {
            var borrows = DbContext.DBstatic.Queryable<Borrows>().InSingle(borrowId);
            if (borrows == null) return false;
            return true;
        }
    }
}
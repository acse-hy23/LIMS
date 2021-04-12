using SqlSugar;

namespace lib_backend.Models
{
    [SugarTable("reservation_books")]
    public class ReservationForBooks
    {
        public int book_id { get; set; }

        public int reader_id { get; set; }
    }
}
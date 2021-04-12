using SqlSugar;

namespace lib_backend.Models
{
    [SugarTable("book_place")]
    public class BookPlace
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public int Floor { get; set; }

        public int Bookshelf { get; set; }

        public int Which_row { get; set; }
    }
}
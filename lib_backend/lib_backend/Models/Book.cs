using System;
using SqlSugar;

namespace lib_backend.Models
{
    [SugarTable("book")]
    public class Book
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public string Book_name { get; set; }

        public string Author { get; set; }

        public string Publishing_house { get; set; }

        public string Isbn { get; set; }

        public DateTime Publication_date { get; set; }

        public string Introduction { get; set; }

        public int State { get; set; } // 0在库 1外借 -1封存

        public int Place_id { get; set; }
    }

    public class PopBook
    {
        public string ISBN { get; set; }
        public string Name { get; private set; }
        public int Times { get; set; }

        public void GetName()
        {
            Name = DbContext.DBstatic.Queryable<Book>()
                .Where(bk => bk.Isbn == ISBN)
                .Select(bk => bk.Book_name)
                .First();
        }
    }
}
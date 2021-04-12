using System;
using SqlSugar;

namespace lib_backend.Models
{
    [SugarTable("borrows")]
    public class Borrows
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Borrow_id { get; set; }

        public int Book_id { get; set; }

        public int Reader_id { get; set; }

        public int Renew { get; set; }

        public int State { get; set; } //0 在借/续借, 1 已经归还(结束), -1逾期

        public DateTime Borrow_Time { get; set; }

        public DateTime Expire_Time { get; set; }

        public DateTime Return_Time { get; set; }
    }
}
using SqlSugar;

namespace lib_backend.Models
{
    [SugarTable("seat")]
    public class Seat
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]

        public string seat_id { get; set; }

        public int room_id { get; set; }

        public int is_used { get; set; }
    }
}
using System;
using SqlSugar;

namespace lib_backend.Models
{
    [SugarTable("reservation_rooms")]
    public class ReservationForRooms
    {
        public int roomId { get; set; }

        public int readerId { get; set; }

        public string seatId { get; set; }

        public DateTime reserve_date { get; set; }
    }
}
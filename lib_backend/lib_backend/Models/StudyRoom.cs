using SqlSugar;

namespace lib_backend.Models
{
    [SugarTable("self_study_room")]
    public class StudyRoom
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int room_id { get; set; }

        public string place { get; set; }

        public int capacity { get; set; }

        public int state { get; set; }
    }
}
using SqlSugar;

namespace lib_backend.Models
{
    [SugarTable("recommend")]
    public class Recommend
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }

        public int recommender { get; set; }

        public string name { get; set; }

        public string reason { get; set; }

        public int state { get; set; } //0未处理, 1批准购买, 2已购买, -1不批准
    }
}
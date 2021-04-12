using SqlSugar;

namespace lib_backend.Models
{
    [SugarTable("reader")]
    public class Reader
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }

        public string name { get; set; }

        public string passwordhash { get; set; }

        public string gender { get; set; }

        public int credit_score { get; set; }

        public string contact { get; set; }

        public string salt { get; set; }
    }
}
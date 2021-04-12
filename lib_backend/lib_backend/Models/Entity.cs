using SqlSugar;

namespace lib_backend.Models
{
    [SugarTable("reader")]
    public class ReaderInfo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; private set; }

        public string Name { get; private set; }
        public string Contact { get; private set; }
        public string PasswordHash { get; private set; }
        public string Gender { get; private set; }
        public int Credit_Score { get; private set; }
        public string Salt { get; private set; }

        /// <summary>
        ///     初始化一个用户，用于注册
        ///     输入一个字典类型，其键分别为
        ///     Contact, Name, PasswordHash, Salt
        /// </summary>
        /// <param name="init"></param>
        /// <returns>构造的用户</returns>
        public ReaderInfo SetInitial(RegisterModel init)
        {
            Contact = init.Contact;
            Name = init.Name;
            PasswordHash = init.PasswordHash;
            Credit_Score = 100;
            Gender = "未知";
            Salt = init.Salt;
            return this;
        }

        /// <summary>
        ///     初始化一个临时的用户变量，用于更新信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ReaderInfo UpdateInfo(ReaderInfoModel info)
        {
            if (info.ID == 0)
                return null;
            ID = info.ID;
            Name = info.Name;
            Contact = info.Contact;
            Gender = info.Gender;
            return this;
        }

        public ReaderInfo UpdatePassword(RegisterModel newPw)
        {
            if (newPw.ID == 0)
                return null;
            ID = newPw.ID;
            PasswordHash = newPw.PasswordHash;
            Salt = newPw.Salt;
            return this;
        }
    }

    [SugarTable("admin")]
    public class AdminInfo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        public string Name { get; set; }
        public string Contact { get; set; }
        public string PasswordHash { get; set; }
        public string Gender { get; set; }
        public string Salt { get; set; }

        /// <summary>
        ///     初始化一个用户，用于注册
        ///     输入一个字典类型，其键分别为
        ///     Contact, Name, PasswordHash, Salt
        /// </summary>
        /// <param name="init"></param>
        /// <returns>构造的用户</returns>
        public AdminInfo SetInitial(RegisterModel init)
        {
            Contact = init.Contact;
            Name = init.Name;
            PasswordHash = init.PasswordHash;
            Gender = "未知";
            Salt = init.Salt;
            return this;
        }

        /// <summary>
        ///     初始化一个临时的用户变量，用于更新信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public AdminInfo UpdateInfo(AdminInfoModel ls)
        {
            if (ls.ID == 0)
                return null;
            ID = ls.ID;
            Name = ls.Name;
            Contact = ls.Contact;
            Gender = ls.Gender;
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="newPw"></param>
        /// <returns></returns>
        public AdminInfo UpdatePassword(RegisterModel newPw)
        {
            if (newPw.ID == 0)
                return null;
            ID = newPw.ID;
            PasswordHash = newPw.PasswordHash;
            Salt = newPw.Salt;
            return this;
        }
    }

    /// <summary>
    ///     注册信息模型，用于注册和修改密码
    /// </summary>
    public class RegisterModel : InfoModel
    {
        public string Salt { get; set; }
        public string PasswordHash { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    ///     读者信息模型，用于查询读者信息
    /// </summary>
    public class ReaderInfoModel : InfoModel
    {
        public int Credit_Score { get; set; }
    }

    /// <summary>
    ///     管理员信息模型，用于查询管理员信息
    /// </summary>
    public class AdminInfoModel : InfoModel
    {
    }

    public abstract class InfoModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Gender { get; set; }

        public InfoModel UpdateInfo(InfoModel mod)
        {
            if (mod.Name != null) Name = mod.Name;
            if (mod.Contact != null) Contact = mod.Contact;
            if (mod.Gender != null) Gender = mod.Gender;
            return this;
        }
    }
}
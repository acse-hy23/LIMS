using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using lib_backend.Models;
using MySql.Data.MySqlClient;
using SqlSugar;

namespace lib_backend.Services
{
    public static class GlobalFunc
    {
        /// <summary>
        ///     初始化连接字符串
        /// </summary>
        public static void InitializeConnStr()
        {
            //string password = null;
            //if (System.IO.File.Exists("password.txt")) { password = System.IO.File.ReadAllText("password.txt"); }
            //else { throw new Exception("Database password not Found"); }
            GlobalVar.ConnectString =
                "server=rm-uf68636als4edtp7i2o.mysql.rds.aliyuncs.com;database=lib_manage;uid=admin123;pwd=admin-123";
        }

        /// <summary>
        ///     初始化 JWT 验证用的密钥
        /// </summary>
        public static void InitializeSecret()
        {
            //string sec = null;
            ////从本地文件读取密钥
            //if (File.Exists("secret.txt"))
            //    sec = File.ReadAllText("secret.txt");
            //else
            //    throw new Exception("JWT secret not Found");
            //GlobalVar.Secret = sec;
            GlobalVar.Secret = "Im3CPuKioUlfdVjFVzHeXCzkS//lXryWw2TvSNzK8Tc=";
        }

        /// <summary>
        ///     判断输入的注册信息是否合法
        /// </summary>
        /// <param name="reg">输入参数列表</param>
        /// <returns>合法则返回去空格后的注册信息清单，否则返回 null</returns>
        /// <exception cref="Exception">若输入不合法，抛出异常</exception>
        public static RegisterModel CheckRegisterInput(RegisterModel reg)
        {
            /*todo: rewrite password check method*/
            try
            {
                InfoCheck(reg, true, reg.Password);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return reg;
        }

        /// <summary>
        ///     检查用户信息输入合法性
        /// </summary>
        /// <param name="info">用户信息模型</param>
        /// <param name="checkContactUsed">是否检查手机号重复</param>
        /// <param name="password">注册和修改时检查密码</param>
        /// <exception cref="Exception">问题字段</exception>
        public static void InfoCheck(InfoModel info, bool checkContactUsed, string password = null)
        {
            try
            {
                if (info.Contact != null)
                {
                    IsHandset(info.Contact);
                    if (checkContactUsed) HandsetUsed(info.Contact);
                }

                if (info.Gender != null) IsValidGender(info.Gender);
                if (info.Name != null) IsValidUsername(info.Name);
                if (password != null) IsValidPassword(password);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     校验当前字符串是否符合手机号规范
        /// </summary>
        /// <param name="str_handset">待检验的字符串</param>
        /// <returns></returns>
        public static bool IsHandset(string str_handset)
        {
            if (Regex.IsMatch(str_handset, @"^1[3456789]\d{9}$") && str_handset.Length == 11)
                return true;
            throw new Exception("Invalid phone number");
        }

        /// <summary>
        ///     当前手机号是否已经被使用
        /// </summary>
        /// <param name="contact">手机号</param>
        /// <returns></returns>
        /// <exception cref="Exception">该手机号已被占用</exception>
        public static bool HandsetUsed(string contact)
        {
            var userId = DbContext.DBstatic.Queryable<AdminInfo>()
                .Select(f => new {f.ID, f.Contact})
                .Where(it => it.Contact == contact).First();
            if (userId == null)
                userId = DbContext.DBstatic.Queryable<ReaderInfo>()
                    .Select(f => new {f.ID, f.Contact})
                    .Where(it => it.Contact == contact).First();
            if (userId == null)
                return false;
            throw new Exception("Handset Used");
        }

        public static bool IsValidGender(string gender)
        {
            if (gender == "男" || gender == "女" || gender == "未知") return true;
            throw new Exception("Invalid Gender");
        }

        /// <summary>
        ///     校验当前字符串是否过长
        /// </summary>
        /// <param name="str">待检验的字符串</param>
        /// <param name="str_name">待校验的字符串名称</param>
        /// <param name="length">字符串最大长度</param>
        /// <returns></returns>
        public static bool StringTooLong(string str, string str_name, int length)
        {
            if (str.Length <= length)
                return false;
            throw new Exception(string.Format("参数:{0} 过长，应在{1}字符以内", str_name, length));
        }

        /// <summary>
        ///     检测输入是否为合法的密码
        /// </summary>
        /// <param name="password">输入密码</param>
        /// <returns></returns>
        /// <exception cref="Exception">密码不合法</exception>
        public static bool IsValidPassword(string password)
        {
            var rg = new Regex("^[A-Za-z0-9]{8,24}");
            if (rg.IsMatch(password) && password.Length <= 24) return true;
            throw new Exception("Invalid password");
        }

        /// <summary>
        ///     检测输入是否为合法的用户名
        /// </summary>
        /// <param name="name">输入用户名</param>
        /// <returns></returns>
        /// <exception cref="Exception">用户名不合法</exception>
        public static bool IsValidUsername(string name)
        {
            var rg = new Regex(@"^[\u4e00-\u9fa5A-Za-z0-9]{3,16}");
            if (rg.IsMatch(name) && name.Length <= 16) return true;
            throw new Exception("Invalid username");
        }

        /// <summary>
        ///     仅控制台调试用，输入密码
        /// </summary>
        /// <returns></returns>
        public static string InputPassword()
        {
            var password = "";
            while (true)
            {
                //存储用户输入的按键，并且在输入的位置不显示字符
                var ck = Console.ReadKey(true);
                //判断用户是否按下的Enter键
                if (ck.Key != ConsoleKey.Enter)
                {
                    if (ck.Key != ConsoleKey.Backspace)
                    {
                        //将用户输入的字符存入字符串中
                        password += ck.KeyChar.ToString();
                        //将用户输入的字符替换为*
                        Console.Write("*");
                    }
                    else
                    {
                        //删除错误的字符
                        if (password.Length > 0)
                        {
                            password = password.Remove(password.Length - 1);
                            Console.Write("\b \b");
                        }
                    }
                }
                else
                {
                    Console.WriteLine();
                    break;
                }
            }

            return password;
        }

        /// <summary>
        ///     将输入的明文密码加密
        /// </summary>
        /// <param name="password">密码</param>
        /// <param name="salt">盐,请使用 Guid.NewGuid().ToString() 来生成</param>
        /// <returns>密码加盐后的哈希</returns>
        public static string EncryptPassword(string password, string salt)
        {
            var passwordAndSaltBytes = Encoding.UTF8.GetBytes(password + salt);
            var hashBytes = new SHA256Managed().ComputeHash(passwordAndSaltBytes);
            var hashString = Convert.ToBase64String(hashBytes);
            return hashString;
        }

        /// <summary>
        ///     通过 ID 判断是否存在该用户
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static bool FindPersonById(int userID)
        {
            if (IsAdminId(userID))
            {
                var tmp = DbContext.DBstatic.Queryable<AdminInfo>().InSingle(userID);
                if (tmp != null) return true;
            }
            else
            {
                var tmp = DbContext.DBstatic.Queryable<ReaderInfo>().InSingle(userID);
                if (tmp != null) return true;
            }

            return false;
        }

        /// <summary>
        ///     通过手机号判断是否存在该用户
        /// </summary>
        /// <param name="contact"></param>
        /// <returns>用户 ID</returns>
        public static int FindPersonByContact(string contact)
        {
            var userId = DbContext.DBstatic.Queryable<AdminInfo>()
                .Select(f => new {f.ID, f.Contact})
                .Where(it => it.Contact == contact).First();
            if (userId == null)
                userId = DbContext.DBstatic.Queryable<ReaderInfo>()
                    .Select(f => new {f.ID, f.Contact})
                    .Where(it => it.Contact == contact).First();
            if (userId != null)
                return userId.ID;
            throw new Exception($"No such user registered with phone number {contact}");
        }

        /// <summary>
        ///     获取用户的非敏感信息(如密码)
        /// </summary>
        /// <param name="userID">用户 ID</param>
        /// <returns>用户信息字典</returns>
        public static InfoModel GetBasicInfo(int userID)
        {
            if (IsAdminId(userID))
                try
                {
                    var tmp = DbContext.DBstatic.Queryable<AdminInfo>()
                        .Select(f => new AdminInfoModel
                            {ID = f.ID, Name = f.Name, Contact = f.Contact, Gender = f.Gender})
                        .Single(f => f.ID == userID);
                    return tmp;
                }
                catch (MySqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            try
            {
                var tmp = DbContext.DBstatic.Queryable<ReaderInfo>()
                    .Select(f => new ReaderInfoModel
                    {
                        ID = f.ID, Name = f.Name, Contact = f.Contact, Gender = f.Gender, Credit_Score = f.Credit_Score
                    })
                    .Single(f => f.ID == userID);
                return tmp;
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     获取用户的非敏感信息(如密码)
        /// </summary>
        /// <param name="userID">用户 ID</param>
        /// <returns>用户信息(json 形式)</returns>
        public static string GetBasicReaderInfoJson(int userID)
        {
            string json = null;
            if (IsAdminId(userID))
                try
                {
                    json = DbContext.DBstatic.Queryable<AdminInfo>()
                        .Select(f => new {f.ID, f.Name, f.Contact, f.Gender})
                        .Where(it => it.ID == userID).ToJson();
                }
                catch (MySqlException ex)
                {
                    throw ex;
                }
            else
                try
                {
                    json = DbContext.DBstatic.Queryable<ReaderInfo>()
                        .Select(f => new {f.ID, f.Name, f.Contact, f.Gender, f.Credit_Score})
                        .Where(it => it.ID == userID).ToJson();
                }
                catch (MySqlException ex)
                {
                    throw ex;
                }

            return json;
        }

        /// <summary>
        ///     验证用户密码是否正确
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <param name="input">输入的密码</param>
        /// <returns></returns>
        public static bool VerifyPassword(long userId, string input)
        {
            bool login;
            if (Regex.IsMatch(userId.ToString(), @"^1[3456789]\d{9}$") && userId.ToString().Length == 11)
                userId = FindPersonByContact(userId.ToString());
            else
                try
                {
                    Convert.ToInt32(userId);
                }
                catch
                {
                    throw new FormatException("Invalid input");
                }

            if (!FindPersonById(Convert.ToInt32(userId)))
                throw new Exception($"No such user information {userId} found");
            if (IsAdminId(Convert.ToInt32(userId)))
            {
                var getPersonById = DbContext.DBstatic.Queryable<AdminInfo>().InSingle(userId);
                login = getPersonById.PasswordHash == EncryptPassword(input, getPersonById.Salt);
            }
            else
            {
                var getPersonById = DbContext.DBstatic.Queryable<ReaderInfo>().InSingle(userId);
                login = getPersonById.PasswordHash == EncryptPassword(input, getPersonById.Salt);
            }

            if (!login) throw new Exception("Wrong password");
            return login;
        }

        /// <summary>
        ///     通过用户ID判断是否为管理员
        /// </summary>
        /// <param name="id"></param>
        /// <returns>若小于 10000 且大于 0 返回是，否则返回否</returns>
        public static bool IsAdminId(int id)
        {
            return id < 10000 && id >= 1;
        }

        /// <summary>
        ///     获取用户可借阅状态
        /// </summary>
        /// <param name="id">用户 ID</param>
        /// <returns></returns>
        public static bool GetBorrowState(int id)
        {
            if (IsAdminId(id)) return true;

            var score = DbContext.DBstatic.Queryable<ReaderInfo>().InSingle(id).Credit_Score;
            return score >= 60;
            throw new Exception($"Failed to query user information with id: {id}");
        }

        /// <summary>
        ///     刷新数据库中的图书超期状态
        /// </summary>
        public static void RefreshBookState()
        {
            DbContext.DBstatic.Updateable<Borrows>()
                .SetColumns(bk => new Borrows {State = -1})
                .Where(bk => bk.Expire_Time < DateTime.Now && bk.State == 0)
                .ExecuteCommand();
        }

        /// <summary>
        ///     获取用户的超期图书列表
        /// </summary>
        /// <param name="id">用户 ID</param>
        /// <returns></returns>
        public static object GetTips(int id)
        {
            var ls = DbContext.DBstatic
                .Queryable<ReaderInfo, Borrows, Book>((rd, br, bk) => new object[]
                {
                    JoinType.Left, rd.ID == br.Reader_id,
                    JoinType.Left, br.Book_id == bk.Id
                })
                .Where((rd, br, bk) => br.State == -1 && rd.ID == id)
                .Select((rd, br, bk) => bk.Book_name).ToJson();
            return ls;
        }

        /// <summary>
        ///     解码并重编码 URL 中的中文字符
        /// </summary>
        /// <param name="str">编码后的字符串</param>
        /// <param name="encoding">编码方式，一般使用 Encoding.UTF8</param>
        /// <returns>中文字符串</returns>
        public static string MyUrlDeCode(string str, Encoding encoding)
        {
            if (encoding == null)
            {
                var utf8 = Encoding.UTF8;
                //首先用utf-8进行解码                    
                var code = HttpUtility.UrlDecode(str.ToUpper(), utf8);
                //将已经解码的字符再次进行编码.
                var encode = HttpUtility.UrlEncode(code, utf8).ToUpper();
                if (str == encode)
                    encoding = Encoding.UTF8;
                else
                    encoding = Encoding.GetEncoding("gb2312");
            }

            return HttpUtility.UrlDecode(str, encoding);
        }
    }
}
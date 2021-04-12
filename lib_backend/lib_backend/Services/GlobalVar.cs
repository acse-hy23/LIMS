using System;

namespace lib_backend.Services
{
    public class GlobalVar : MarshalByRefObject
    {
        public static string ConnectString = "";

        //用于 JWT 签名的密钥
        public static string Secret = "";
        public static string Domain = "https://localhost:5001";
    }
}
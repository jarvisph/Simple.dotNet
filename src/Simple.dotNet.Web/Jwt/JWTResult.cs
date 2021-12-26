namespace Simple.dotNet.Web.Jwt
{
    public struct JWTResult
    {
        public JWTResult(string token, long expires)
        {
            this.Access_token = token;
            this.Expires_in = expires;
        }
        /// <summary>
        /// token字符串
        /// </summary>
        public string Access_token;
        /// <summary>
        /// 过期时差
        /// </summary>
        public long Expires_in;
    }
}

using Simple.Core.Domain.Enums;

namespace Simple.Core.Domain.Model
{
    public class AccountModel
    {
        /// <summary>
        /// 账号ID
        /// </summary>
        public int AccountID { get; set; }
        /// <summary>
        /// 账号名称
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleID { get; set; }
        public OperateType Type { get; set; }

        public static implicit operator int(AccountModel account)
        {
            return account.AccountID;
        }
    }
}

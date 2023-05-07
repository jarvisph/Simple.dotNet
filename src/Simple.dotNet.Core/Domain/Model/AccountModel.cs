using Simple.Core.Domain.Enums;

namespace Simple.Core.Domain.Model
{
    public class AccountModel
    {
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public OperateType Type { get; set; }

        public static implicit operator int(AccountModel account)
        {
            return account.AccountID;
        }
    }
}

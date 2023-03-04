using etesami.Models;
using Kaspid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace Kaspid.Models.Utility
{
    /// <summary>
    /// Summary description for Result
    /// </summary>
    public class ApiResult
    {
        public string Result;
        public int ErrorNumber;
        public string ErrorMsg;
        public object Object;
        public object Decription;
        public string TokenId;
        private Guid? UserId;
        private int SessionTime = 780;
        public ApiResult()
        {

        }
        public ApiResult(string _tokenId)
        {
            this.TokenId = _tokenId;
        }

        public void GetUserInfo()
        {


        }

        public void SetUserInfo()
        {


        }

        public string JsonOutPut(ResultType.Status _Status, ApiErorr.Erorr Error, object Obj = null, object Decription = null)
        {
            this.Result = ResultType.StatusPersianName(_Status);
            this.ErrorNumber = (byte)Error;
            this.ErrorMsg = ApiErorr.OperationPersianName(Error);

            this.Object = Obj;

            this.Decription = Decription;
            string json = new JavaScriptSerializer().Serialize(this);
            return json;
        }

    }
}

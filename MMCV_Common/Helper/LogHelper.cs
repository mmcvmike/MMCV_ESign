using SAB.Library.Core.FileService;
using System;
using System.Configuration;

namespace MMCV_Common
{
    public class LogHelper
    {
        /// </summary>
        private static volatile LogHelper _instance;

        /// <summary>
        /// The synchronize root
        /// </summary>
        private static object _syncRoot = new object();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static LogHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new LogHelper();
                        }
                    }
                }
                return _instance;
            }
        }
        public string GetConnectionStringDS()
        {
            return ConfigurationManager.ConnectionStrings["ESign_ConnectionString"].ConnectionString;
        }

        public ResultMessageBO WriteLog(string strTitle, Exception objEx, string strEvent, string strModuleName, string strUsername = "system", int locationId = 0)
        {
            SAB.Library.Data.FileLogger.LogAction(objEx);
            return MethodHelper.Instance.FillResultMessage(true, ErrorTypes.Others, strTitle, objEx.ToString());
        }
        public ResultMessageBO WriteLog(string strTitle, string strContent, string strEvent, string strModuleName, string strUsername = "system", int locationId = 0)
        {
            //WriteLog(strTitle, strContent, strEvent, strUsername, intStoreId, strModuleName);
            SAB.Library.Data.FileLogger.LogAction(strTitle, strContent, strEvent, strUsername, locationId);
            return MethodHelper.Instance.FillResultMessage(true, ErrorTypes.Others, strTitle, strContent);
        }
    }
}

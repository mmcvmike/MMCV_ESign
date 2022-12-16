using SAB.Library.Core.FileService;
using System.Reflection;

namespace MMCV_BLL
{
    public class BaseBLL
    {
        #region Fields
        protected ResultMessageBO objResultMessageBO = new ResultMessageBO();
        #endregion

        #region Properties

        public ResultMessageBO ResultMessageBO
        {
            get { return objResultMessageBO; }
            set { objResultMessageBO = value; }
        }

        public string ErrorMsg { get; protected set; }

        public string NameSpace
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Namespace; }
        }
        #endregion

        #region Method

        #endregion
    }
}

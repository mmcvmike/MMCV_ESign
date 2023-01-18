namespace MMCV_Model.Common
{
    public class MenuBO
    {
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public int MenuParent { get; set; }
        public int MenuIndex { get; set; }
        public int Active { get; set; }
    }
}

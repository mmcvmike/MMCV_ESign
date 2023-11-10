using MMCV_Common;
using MMCV_Model.DB68;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MMCV_ESign.Controllers
{
    public class TagController : BaseController
    {
        // GET: Tag
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetTags()
        {
            try
            {
                using (var entity = new MMCV_ESignEntities())
                {
                    var listTags = entity.Tags.Where(x => x.EmployeeID == currentUser.EmployeeID).ToList();
                    return Json(new { rs = true, msg = "", data = listTags }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "TagController");
                return Json(new { rs = false, msg = "Error get tags" });
            }
        }

        public ActionResult GetTagById(long id)
        {
            try
            {
                using (var entity = new MMCV_ESignEntities())
                {
                    var tag = entity.Tags.Where(x => x.TagID == id).FirstOrDefault();
                    return Json(new { rs = true, msg = "", data = tag }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "TagController");
                return Json(new { rs = false, msg = "Error get tag by id" });
            }
        }

        public ActionResult GetTagByEmployeeId()
        {
            try
            {
                using (var entity = new MMCV_ESignEntities())
                {
                    var listTag = entity.Tags.Where(x => (x.EmployeeID == currentUser.EmployeeID && x.Active == 1)).ToList();
                    return Json(new { rs = true, msg = "", data = listTag }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "TagController");
                return Json(new { rs = false, msg = "Error get list tag by emp id" });
            }
        }

        public async Task<ActionResult> AddTag(Tag tag)
        {
            try
            {
                using (var entity = new MMCV_ESignEntities())
                {
                    if (ModelState.IsValid)
                    {
                        // check exist
                        var tagExist = entity.Tags.Where(x => (x.EmployeeID == currentUser.EmployeeID && x.TagName.Trim().ToLower() == tag.TagName.Trim().ToLower())).ToList();
                        if (tagExist.Count > 0)
                        {
                            return Json(new { rs = false, msg = $"There is already exist tag name: {tag.TagName}" });
                        }
                        tag.EmployeeID = currentUser.EmployeeID;
                        entity.Tags.Add(tag);
                        await entity.SaveChangesAsync();

                        return Json(new { rs = true, msg = "Add tag succssfully", data = tag }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var errors = ModelState.Select(x => (x.Value.Errors))
                                     .Where(y => y.Count > 0)
                                     .ToList();
                        var returnData = errors.Where(x => x.Count > 0).Select(y => y.FirstOrDefault()).Select(x => x.ErrorMessage);

                        return Json(new { rs = false, msg = string.Join("<br/>", returnData) }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "TagController");
                return Json(new { rs = false, msg = "Error add tag" });
            }
        }

        public async Task<ActionResult> UpdateTag(Tag tag)
        {
            try
            {
                using (var entity = new MMCV_ESignEntities())
                {
                    // check exist
                    var tagExist = entity.Tags.Where(x => x.TagID == tag.TagID).ToList();
                    if (tagExist.Count <= 0)
                    {
                        return Json(new { rs = false, msg = $"There is no exist tag name: {tag.TagName}" });
                    }
                    var updatedObj = tagExist.FirstOrDefault();

                    if (ModelState.IsValid)
                    {
                        // check exist
                        if (tag.TagName != updatedObj.TagName)
                        {
                            var tagExistName = entity.Tags.Where(x => (x.EmployeeID == currentUser.EmployeeID && x.TagName.Trim().ToLower() == tag.TagName.Trim().ToLower())).ToList();
                            if (tagExistName.Count > 0)
                            {
                                return Json(new { rs = false, msg = $"There is already exist tag name: {tag.TagName}" });
                            }
                        }

                        updatedObj.TagName = tag.TagName;
                        updatedObj.Order = tag.Order;
                        updatedObj.Active = tag.Active;

                        entity.Entry(updatedObj).State = System.Data.Entity.EntityState.Modified;
                        await entity.SaveChangesAsync();

                        return Json(new { rs = true, msg = "Update doc type succssfully", data = tag }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var errors = ModelState.Select(x => (x.Value.Errors))
                                     .Where(y => y.Count > 0)
                                     .ToList();
                        var returnData = errors.Where(x => x.Count > 0).Select(y => y.FirstOrDefault()).Select(x => x.ErrorMessage);

                        return Json(new { rs = false, msg = string.Join("<br/>", returnData) }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.Message, ex, MethodHelper.Instance.MergeEventStr(MethodBase.GetCurrentMethod()), "TagController");
                return Json(new { rs = false, msg = "Error update tag" });
            }
        }
    }
}
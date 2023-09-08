using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ComputerManager.Models;

namespace ComputerManager.Controllers
{
    public class OrdercompuersController : Controller
    {
        private ComputerManagerDBEntities1 db = new ComputerManagerDBEntities1();

        // 订单列表
        public ActionResult Index()
        {
            return View(db.ordercompuer.ToList());
        }

        // 编辑订单
        public ActionResult Edit(string stu)
        {
            return View(db.ordercompuer.Find(stu));
        }
        //保存管理员
        public ActionResult Save(ordercompuer oc)
        {
            var dborder = db.ordercompuer.Find(oc.ocode);
            if(dborder!=null)
            {
                dborder.sid = oc.sid;
                dborder.comcode = oc.comcode;
                dborder.Start_Date = oc.Start_Date;
                dborder.End_Date = oc.End_Date;
                try
                {
                    db.SaveChanges();
                    return Content("<script>alert('修改成功！');window.history.back(-1);</script>");
                }
                catch(DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                    string errorMessage = string.Join("; ", errorMessages);
                    return Content($"<script>alert('保存失败！错误信息：{errorMessage}');window.history.back(-1);</script>");

                }
            }
            else
            {
                db.ordercompuer.Add(oc);
                try
                {
                    db.SaveChanges();
                    return Content("<script>alert('新建成功！');window.history.back(-1);</script>");
                }
                catch(DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                    string errorMessage = string.Join("; ", errorMessages);
                    return Content($"<script>alert('保存失败！错误信息：{errorMessage}');window.history.back(-1);</script>");

                }
            }
        }

        // 删除订单
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var rub = db.ordercompuer.FirstOrDefault(m => m.ocode == id);
            if(rub!=null)
            {
                db.ordercompuer.Remove(rub);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

       
    }
}

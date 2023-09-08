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
    public class ManagersController : Controller
    {
        private ComputerManagerDBEntities1 db = new ComputerManagerDBEntities1();

        // 管理员列表
        public ActionResult Index()
        {
            return View(db.manager.ToList());
        }

        //编辑管理员
        public ActionResult Edit(string id)
        {
            return View(db.manager.Find(id));
        }

        //保存管理员
        public ActionResult Save(manager man)
        {
            var dbmanager = db.manager.Find(man.mid);
            if (dbmanager != null)
            {
                dbmanager.mname = man.mname;
                dbmanager.mpass = man.mpass;
                dbmanager.power = man.power;
                dbmanager.mstate = man.mstate;
                dbmanager.sex = man.sex;
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
                //添加新的管理员信息
                db.manager.Add(man);

                //创建对应的Login记录
                login login = new login();
                login.username = man.mid;  // 使用Student的主键作为Login的外键
                login.password = man.mpass;
                login.power = man.power;
                login.state = man.mstate;

                try
                {
                    //添加Login记录到数据库
                    db.login.Add(login);
                    db.SaveChanges();
                    return Content("<script>alert('新建成功！');window.history.back(-1);</script>");
                }
                catch(DbEntityValidationException ex)
                {
                    //处理验证错误
                    var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                    string errorMessage = string.Join("; ", errorMessages);
                    return Content($"<script>alert('保存失败！错误信息：{errorMessage}');window.history.back(-1);</script>");

                }
            }
        }

        // 删除管理员
        [HttpPost]
        public ActionResult Delete(string id)
        {
            var rub = db.manager.FirstOrDefault(m => m.mid == id);
            if(rub!=null)
            {
                db.manager.Remove(rub);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        //修改状态
        [HttpPost]
        public ActionResult ChangeStatus(string id)
        {
            manager rub = db.manager.FirstOrDefault(m => m.mid == id);
            if (rub != null)
            {
                if (rub.mstate.Trim() == "0")
                {
                    rub.mstate = "1";
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else if (rub.mstate.Trim() == "1")
                {
                    rub.mstate = "0";
                    db.SaveChanges();
                    return Json(new { success = true });
                }

            }
            return Json(new { success = false });
        }

    }
}

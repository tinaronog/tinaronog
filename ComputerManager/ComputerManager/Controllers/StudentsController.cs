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
    public class StudentsController : Controller
    {
        private ComputerManagerDBEntities1 db = new ComputerManagerDBEntities1();

        // 学生列表
        public ActionResult Index()
        {
            return View(db.student.ToList());
        }

        // 编辑计算机
       
      
        public ActionResult Edit(string id)
        {
            return View(db.student.Find(id));
        }

        [HttpPost]
        public ActionResult Save(student stu)
        {
            var dbstudent = db.student.Find(stu.sid);
            if (dbstudent != null)
            {
                // 更新已存在的Student记录
                dbstudent.spass = stu.spass;
                dbstudent.sstate = stu.sstate;
                dbstudent.sname = stu.sname;
                dbstudent.sclass = stu.sclass;
                dbstudent.sex = stu.sex;
                dbstudent.stele = stu.stele;
                dbstudent.power = stu.power;


                try
                {
                    db.SaveChanges();
                    return Content("<script>alert('修改成功！');window.history.back(-1);</script>");
                }
                catch (DbEntityValidationException ex)
                {
                    // 处理验证错误
                    var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                    string errorMessage = string.Join("; ", errorMessages);
                    return Content($"<script>alert('保存失败！错误信息：{errorMessage}');window.history.back(-1);</script>");
                }
            }
            else
            {
                // 添加新的Student记录
                db.student.Add(stu);

                // 创建对应的Login记录
                login login = new login();
                login.username = stu.sid;  // 使用Student的主键作为Login的外键
                login.password = stu.spass;
                login.power = stu.power;
                login.state = stu.sstate;

                try
                {
                    // 添加Login记录到数据库
                    db.login.Add(login);
                    db.SaveChanges();
                    return Content("<script>alert('新建成功！');window.history.back(-1);</script>");
                }
                catch (DbEntityValidationException ex)
                {
                    // 处理验证错误
                    var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                    string errorMessage = string.Join("; ", errorMessages);
                    return Content($"<script>alert('保存失败！错误信息：{errorMessage}');window.history.back(-1);</script>");
                }
            }
        }


        [HttpPost]
        public ActionResult Delete(string id)
        {
            var rub = db.student.FirstOrDefault(m => m.sid == id);
            if(rub!=null)
            {
                db.student.Remove(rub);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult ChangeStatus(string id)
        {
            student rub = db.student.FirstOrDefault(m => m.sid == id);
            if (rub != null)
            {
                if (rub.sstate.Trim() == "0")
                {
                    rub.sstate = "1";
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else if (rub.sstate.Trim() == "1")
                {
                    rub.sstate = "0";
                    db.SaveChanges();
                    return Json(new { success = true });
                }

            }
            return Json(new { success = false });
        }
    }
}

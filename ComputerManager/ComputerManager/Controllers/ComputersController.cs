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
    //计算机控制器
    public class ComputersController : Controller
    {
        private ComputerManagerDBEntities1 db = new ComputerManagerDBEntities1();

        // 计算机列表

        public ActionResult Index()
        {
            return View(db.computer.ToList());
        }


        // 编辑计算机
        public ActionResult Edit(string id)
        {
            return View(db.computer.Find(id));
        }

        // 保存编辑
        [HttpPost]
        public ActionResult Save(computer com)
        {
            var dbcomputer = db.computer.Find(com.comcode);
            if (dbcomputer != null)
            {
                dbcomputer.room = com.room;
                dbcomputer.cstate = com.cstate;
                db.SaveChanges();
                return Content("<script>alert('修改成功！');window.history.back(-1);</script>");
            }
            else
            {
                db.computer.Add(com);
                db.SaveChanges();
                return Content("<script>alert('新建成功！');window.history.back(-1);</script>");
            }
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            var rub = db.computer.FirstOrDefault(m => m.comcode == id);
            if (rub != null)
            {
                db.computer.Remove(rub);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult ChangeStatus(string id)
        {
            computer rub = db.computer.FirstOrDefault(m => m.comcode == id);
            if (rub != null)
            {
                if (rub.cstate.Trim() == "0")
                {
                    rub.cstate = "1";
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else if(rub.cstate.Trim()=="1")
                {
                    rub.cstate = "0";
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                                
            }
            return Json(new { success = false });
        }


    }
}

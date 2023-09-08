using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ComputerManager.Models;


namespace ComputerManager.Controllers
{
    //Login控制器名
    public class LoginController : Controller
    {
        //使用EF对象
        private ComputerManagerDBEntities1 db = new ComputerManagerDBEntities1();
        // Index方法名
        //他的访问路径是：/Login/Index
        public ActionResult Index()
        {
            return View();
        }
        //执行登录方法
        [HttpPost]
        public ActionResult DoLogin(string username,string password,int power)
        {
            //1、查询用户回来
            login info = db.login.Where(p => p.username == username && p.password == password && p.power == power).FirstOrDefault();
            if (info != null)
            {
                //2、判断是否可以使用这个账号
                if (info.state.Trim() == "1")
                {
                    return Content("<script>alert('该用户已被禁用！');window.history.back(-1);</script>");
                }
                //3、需要存储的用户的信息=》用到Session
                //需要补充代码
                Session["info"] = info;//这里存储的是用户登录信息
                if (power == 0)
                {
                    //获取学生信息
                    student stuInfo = db.student.FirstOrDefault(p => p.sid == info.username);
                    Session["stuInfo"] = stuInfo.sid;
                    Session["role"] = "User";//设置用户的角色为User
                    
                }
                else
                {
                    //获取管理员信息
                    manager manInfo = db.manager.FirstOrDefault(p => p.mid == info.username);
                    Session["manInfo"] = manInfo;
                    Session["role"] = "Admin";//设置用户角色为Admin
                    
                }
                //到管理员系统的首页
                return Redirect("/Home/Index");
            }
            else
            {
                //提示用户不存在
                return Content("<script>alert('用户不存在！');window.history.back(-1);</script>");
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session["info"] = null;
            Session["stuInfo"] = null;
            Session["manInfo"] = null;
            Session["role"] = null;
            return Redirect("/Login/Index");
        }
    }
}
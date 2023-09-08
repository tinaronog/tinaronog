using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Timers;
using ComputerManager.Models;

namespace ComputerManager.Controllers
{
    public class SelectComputers1Controller : Controller
    {
        private ComputerManagerDBEntities1 db = new ComputerManagerDBEntities1();
        private Timer orderTimer;

        // 计算机列表
        public ActionResult Index()
        {
            return View(db.computer.ToList());
        }

        //初始化定时器
        public SelectComputers1Controller()
        {
            orderTimer = new Timer();
            orderTimer.Interval = 3600000; // 1小时
            orderTimer.Elapsed += OrderTimer_Elapsed;
            orderTimer.AutoReset = true;
            orderTimer.Start();
        }

        //定时任务，设置状态
        private void OrderTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //定时任务，处理订单超时逻辑
            var orders = db.ordercompuer.ToList();
            var currentTime = DateTime.Now;
            foreach(var ord in orders)
            {
                //检查订单是否超时
                if(ord.End_Date<=currentTime)
                {
                    //更新计算机状态为0
                    var com = db.computer.FirstOrDefault(c => c.comcode == ord.comcode);
                    if(com!=null)
                    {
                        com.cstate = "0";
                    }
                    //订单表中删除该订单
                    db.ordercompuer.Remove(ord);
                }
            }
            db.SaveChanges();
        }

        // 编辑计算机
        public ActionResult Edit(string sid, string cid)
        {
            return View(db.ordercompuer.FirstOrDefault(o => o.sid == sid && o.comcode == cid));
        }


        // 保存编辑
        [HttpPost]
        public ActionResult Save(ordercompuer ocm)
        {
            var com = db.computer.Find(ocm.comcode);
            if (com != null && com.cstate.Trim() == "0")
            {
                // 新建订单
                var order = new ordercompuer
                {
                    sid = ocm.sid,
                    comcode = ocm.comcode,
                    Start_Date = DateTime.Now,
                    End_Date = DateTime.Now.AddHours(1)
                };
                db.ordercompuer.Add(order);
                db.SaveChanges();

                // 设置计算机状态为1
                com.cstate = "1";
                db.SaveChanges();

                // 启动定时器
                orderTimer.Start();

                return Content("<script>alert('新建成功！');window.history.back(-1);</script>");
            }
            else if (com != null && com.cstate.Trim() == "1")
            {
                // 修改订单时间为1小时
                var order = db.ordercompuer.FirstOrDefault(o => o.sid == ocm.sid && o.comcode == ocm.comcode);
                if (order != null)
                {
                    order.Start_Date = DateTime.Now;
                    order.End_Date = DateTime.Now.AddHours(1);
                }

                db.SaveChanges();

                // 启动定时器
                orderTimer.Start();

                return Content("<script>alert('修改成功！');window.history.back(-1);</script>");
            }
            else
            {
                return Content("<script>alert('无法进行新建订单！');window.history.back(-1);</script>");
            }
        }

    }
}

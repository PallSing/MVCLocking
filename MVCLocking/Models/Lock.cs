using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCLocking.Models
{
    public class Lock
    {
        public string startDate;
        public string startTime;
        public string endDate;
        public string endTime;
        public string invhCode;
        public string lockPositionCode;
        public string salesArea;

        public List<Lock> locks;
    }

    //public class ListLock
    //{
    //   public  List<Lock> locks;

    //    public void Add(Lock l)
    //    {
    //        locks.Add(l);
    //    }

    //}
}
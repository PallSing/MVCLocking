using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ServiceModel;
using MVCLocking.LockingReference;
using System.IO;
using System.Net;
using System.ServiceModel.Channels;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using MVCLocking.Models;

namespace MVCLocking.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public List<int> GetInventoryLocks()
        {
            List<int> locks = new List<int>();

            return locks;
        }
        public int DeleteInventoryLock()
        {
            return 1;
        }

        public int AmendInventoryLock()
        {
            return 1;
        }

        public int CreateInventoryLock()
        {
            return 1;
        }

        public ActionResult GetLocks()
        {
            LandmarkSystemInventoryLockClient client = new LandmarkSystemInventoryLockClient();

            ValidationStandardParameters parameters = new ValidationStandardParameters();

            parameters.CurrencyCode = "GBP";
            parameters.OrganisationCode = "UP";
            parameters.PositionCode = "SDMAS";
            parameters.SystemDate = DateTime.Now;

            int[] salesAreaNumber = { 1 };
            string inventoryLockCode = "LU";
            DateTime startDate = Convert.ToDateTime("1/11/2017");
            DateTime endDate = Convert.ToDateTime("30/11/2017");
            //DateTime startDate = DateTime.Now;
            //DateTime endDate = (DateTime.Now).AddHours(1);
           // int startTime = 12;

            //int endTime = 13;
           // Error[] errors;

            //string lockPositionCode = "SDMAS";
           // int sessionID = 0;

            using (new OperationContextScope(client.InnerChannel))
            {
                HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
                string lmkPassword = "PASSWORD";//ConfigurationManager.AppSettings["lmkPassword"];
                string username = "JO FOSTER";//ConfigurationManager.AppSettings["username"];
                string password = GetHashSha256(lmkPassword);
                requestMessage.Headers["LMK-Environment"] = "1:itvmig";//ConfigurationManager.AppSettings["LMK-Environment"];
                requestMessage.Headers["Authorization"] = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(username + ":" + password));
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
                try
                {
                     List<Lock> locks =new List<Lock>();

                   // ListLock locksNew = new ListLock();

                    InventoryLock[] res2= client.ListInventoryLock(salesAreaNumber, inventoryLockCode, startDate, endDate);

                    for(int i=0;i<res2.Length;i++)
                    {
                        
                        Lock l1 = new Lock();
                        l1.startDate = res2[i].StartDate.ToShortDateString();
                        l1.startTime = FormatTime(Convert.ToString(res2[i].StartTime));
                        l1.endDate = res2[i].EndDate.ToShortDateString();
                        l1.endTime = FormatTime(Convert.ToString(res2[i].EndTime));
                        l1.invhCode = Convert.ToString(res2[i].InvhCode);
                        l1.lockPositionCode = Convert.ToString(res2[i].LockPositionCode);
                        l1.salesArea = Convert.ToString(res2[i].SalesArea);

                        locks.Add(l1);

                        
                        
                       // locks.Add(l1) ;

                    }
                   // locksNew = locks;
                    ViewBag.Locks = locks;
                    return View("Index",locks);
                    //return;
                    // Create Inventory Lock call 
                    // bool res = client.CreateInventoryLock(salesAreaNumber, inventoryLockCode, startDate, endDate, startTime, endTime, lockPositionCode, sessionID, parameters, out errors);
                    //Console.WriteLine("JOB STATUS: " + res.ToString() + " . PRESS ENTER TO CLOSE");

                    // Delete Inventory Lock Call 
                    // bool res1 = client.DeleteInventoryLock(salesAreaNumber, inventoryLockCode, startDate, startTime, parameters, out errors);
                }
                catch (FaultException faultEx)
                {
                    //Console.WriteLine("Fault Exception: " + faultEx.Message + faultEx.StackTrace);
                    //Console.ReadLine();
                    ViewBag.Message = faultEx;
                    return View("Index");
                }
                catch (TimeoutException timeProblem)
                {
                    //Console.WriteLine("Timeout Exception: " + timeProblem.Message);
                    //Console.ReadLine();
                    ViewBag.Message = timeProblem;
                    return View("Index");
                }
                catch (CommunicationException exp)
                {
                    if (exp.InnerException is WebException)
                    {
                        WebException webex = (WebException)exp.InnerException;
                        HttpWebResponse response = (HttpWebResponse)webex.Response;
                        string responseData = "";
                        using (Stream respStream = response.GetResponseStream())
                        {
                            if (respStream != null)
                            {
                                StreamReader streamReader = new StreamReader(respStream, Encoding.ASCII);
                                responseData = streamReader.ReadToEnd();
                                Console.WriteLine("INVENTORY LOCK STATUS: " + responseData.ToString() + " . PRESS ENTER TO CLOSE");
                                Console.ReadLine();
                            }
                        }
                        ViewBag.Message = "Error getting Locks";
                        return View("Index");
                    }
                    ViewBag.Message = "Error getting Locks";
                    return View("Index");
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("General Exception " + ex.Message);
                    //Console.ReadLine();
                    ViewBag.Message = ex.Message;
                    return View("Index");
                }
                //return View();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newFormat"></param>
        /// <returns></returns>
        public string ReFormatTime(string newFormat)
        {
            StringBuilder oldFormat = new StringBuilder();
            var parts = new string[3];

            parts[0] = newFormat.Substring(0, 2);
            parts[1] = newFormat.Substring(3, 2);
            parts[2] = newFormat.Substring(6, 2);

            oldFormat.Append(Convert.ToString(Convert.ToInt32(parts[0]) + 6));
            oldFormat.Append(parts[1]);
            oldFormat.Append(parts[2]);

            return oldFormat.ToString();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldFormat"></param>
        /// <returns></returns>
        public string FormatTime(string oldFormat)
        {
            StringBuilder newFormat = new StringBuilder();

            var parts = new string[3];
          
            parts[0] = oldFormat.Substring(0, 2);
            parts[1] = oldFormat.Substring(2, 2);
            parts[2] = oldFormat.Substring(4, 2);

            newFormat.Append(Convert.ToString(Convert.ToInt32(parts[0]) -6));
            newFormat.Append(":");
            newFormat.Append(parts[1]);
            newFormat.Append(":");
            newFormat.Append(parts[2]);
            return newFormat.ToString();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetHashSha256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }
    }
}
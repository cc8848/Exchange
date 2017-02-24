using System.IO;
using CommonClass.DBOperator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tide.Models;

namespace Tide.Controllers
{
    public class MainController : Controller
    {
        //
        // GET: /Main/
        private static AcessDBHelper heper;

        private static DataTable currentDt;

        private static DataTable currentUnitDt;

        public ActionResult Start()
        {
            return View();
        }

        public ActionResult Index()
        {
            var useragent = Request.UserAgent;

            if (!IsMobile(useragent))
                return View("Indexm");
            return View();
        }

        public ActionResult Indexm()
        {
            //var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\aibaopen.accdb");

            //try
            //{
            //    heper = new AcessDBHelper();
            //    currentDt = heper.QueryData(heper.SelectJYSINfo("Exchange"));
            //}
            //catch(Exception ex)
            //{
            //    path = ex.Message;
            //}
           
            //ViewBag.Title = path;

            return View();
        }

        public ActionResult GetData()
        {
            heper = new AcessDBHelper();
            currentDt = heper.QueryData(heper.SelectJYSINfo("Exchange"));

            var item = new List<MapItem>();

            for (int i = 0; i < currentDt.Rows.Count; i++)
            {
                var mapinfo = GetMapItemInfo(currentDt.Rows[i], currentDt.Rows.Count - i);
                if (!item.Select(_ => _.City).Contains(mapinfo.City) && !item.Select(_ => _.Loc).Contains(mapinfo.Loc))
                    item.Add(mapinfo);
            }

            return Json(item);
        }

        /// <summary>
        /// 根据选中ID获取当前省份所有的交易所
        /// </summary>
        /// <param name="exchangeID"></param>
        /// <returns></returns>
        public ActionResult GetAreaData(string exchangeID,string provice)
        {
            var item = new List<DetailItem>();
            string proviceName = "";

            if(string.IsNullOrEmpty(exchangeID))
            {
                proviceName = provice;
            }
            else
            {
                var info = SelectExchangeInfo(exchangeID);

                proviceName = GetProvinceName(info.Address);
            }

            var array = currentDt.Select("Address  like '%" + proviceName + "%'");

            item = GetMapItemByExchangeInfo(array);

            return PartialView(item);
        }

        public ActionResult SubUnits(string id,string exchangeName)
        {
            var item = new List<SubUnit>();

            var result = heper.QueryData(heper.QuerySubUnit("Unit1113",id));
            for (int i = 0; i < result.Rows.Count; i++)
            {
                var unitmodel = GetUnitInfo(result.Rows[i]);

                if (!item.Select(_ => _.UnitName).Contains(unitmodel.UnitName))
                    item.Add(unitmodel);
            }

            ViewBag.Title = exchangeName;
            ViewBag.Info = SelectExchangeInfo(id);
            
            return View(item);
        }

        public ActionResult SearchList()
        {
            GetExchangeInfo();

            var array = currentDt.Rows.OfType<DataRow>().ToArray();

            var detailItem = GetMapItemByExchangeInfo(array);

            //detailItem.Sort(new Comparison<DetailItem>((x, y) => { return x.Name > y.Name; }));System.Collections.IComparer
            //var result = detailItem.GroupBy(_ => _.Name.Substring(0, 1)).Select(item=>item as IEnumerable<DetailItem>).ToArray();
            var result = detailItem.OrderBy(_ => _.Name) as IEnumerable<DetailItem>;

            return View(result);
        }

        public ActionResult SearchUnit()
        {
            return View();
        }

        /// <summary>
        /// 模糊搜索子单位
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult GetSubUnitListView(string name)
        {
            //获取单位表
            GetUnitTable();

            var item = new List<SubUnit>();

            if(!string.IsNullOrEmpty(name))
            {
                var result = currentUnitDt.Select("UnitName like '%" + name + "%'");

                for (int i = 0; i < result.Length; i++)
                {
                    var unitmodel = GetUnitInfo(result[i]);

                    if (!item.Select(_ => _.UnitName).Contains(unitmodel.UnitName))
                        item.Add(unitmodel);
                }

            }

            return PartialView(item);
        }

        /// <summary>
        /// 根据单位ID获取所属交易所信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult InfoPanel(string id)
        {
            SearchInfo info = new SearchInfo();

            var result = currentUnitDt.Select("ID=" + id + "");

            if (result.Length > 0)
            {
                var unitmodel = GetUnitInfo(result[0]);
                info.UnitName = unitmodel.UnitName;
                info.UnitCity = unitmodel.City;
                info.UnitAddress = unitmodel.Address;

                var parentId = result[0]["ParentID"].ToString();

                var exchangeModel = SelectExchangeInfo(parentId);

                info.ExchangeID = exchangeModel.ID;
                info.ExchangeName = exchangeModel.Name;
                info.ExchangeCity = exchangeModel.Address;

            }
            return PartialView(info);
        }

        private double GetLocation(string loc,int i)
        {
            return ConvertToDouble(loc.Split(new[] { ',', '，', ' ' }, StringSplitOptions.RemoveEmptyEntries)[i].Trim());
        }

        private double ConvertToDouble(string str)
        {
            try
            {
                return Convert.ToDouble(str.Trim());
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private string GetProvinceName(string address)
        {
            var directCity = new[] { "北京", "上海", "天津", "重庆" };
            if (directCity.Any(address.Contains))
            {
                return directCity.First(address.Contains);
            }
            else
            {
                var province = address.Split('省')[0];

                return province;
            }
        }

        private List<DetailItem> GetMapItemByExchangeInfo(DataRow[] rows)
        {
            var item = new List<DetailItem>();

            for (int i = 0; i < rows.Length; i++)
            {
                var name = rows[i]["Name"].ToString();
                var address = rows[i]["Address"].ToString();

                if (!item.Select(_ => _.Name).Contains(name))
                    item.Add(new DetailItem()
                    {
                        ID = rows[i]["ExchangeID"].ToString(),
                        Name = name.Trim(),
                        Address = address.Trim(),
                        MajorTradings = rows[i]["MajorTradings"].ToString(),
                        FoundingTime = rows[i]["FoundingTime"].ToString()
                    });
            }

            return item;
        }

        private AcessDBHelper GetDBHelper()
        {
            if (null == heper)
                heper = new AcessDBHelper();
            return heper;
        }

        private void GetExchangeInfo()
        {
            if (null == currentDt)
                currentDt = GetDBHelper().QueryData(heper.SelectJYSINfo("Exchange"));
        }

        private void GetUnitTable()
        {
            if (null == currentUnitDt)
            {
                currentUnitDt = GetDBHelper().QueryData(heper.SelectJYSINfo("Unit1113"));
            }
        }

        private SubUnit GetUnitInfo(DataRow row)
        {
            return new SubUnit() 
            {
                UnitID = row["ID"].ToString(),
                UnitName = row["UnitName"].ToString(),
                City = row["MatchAddress"].ToString(),
                Address = row["UnitAddress"].ToString(),
            };
        }

        private DetailItem GetExchangeInfo(DataRow row)
        {
            return new DetailItem() 
            {
                ID = row["ExchangeID"].ToString(),
                Name = row["Name"].ToString(),
                Address = row["Address"].ToString(),
                MajorTradings = row["MajorTradings"].ToString(),
                FoundingTime = row["FoundingTime"].ToString()
            };
        }

        private MapItem GetMapItemInfo(DataRow row,int value)
        {
            var id = row["ID"].ToString();
            var name = row["Name"].ToString();
            var loc = row["Loc"].ToString();

            return new MapItem() 
            {
                ID = id,
                City = name,
                Value = value,
                Loc = new Location()
                {
                    E = GetLocation(loc, 0),
                    W = GetLocation(loc, 1),
                }
            };
        }

        private DetailItem SelectExchangeInfo(string ID)
        {
            DetailItem item = new DetailItem();

            GetExchangeInfo();

            var rows = currentDt.Select("ID=" + ID + "");
            if (rows.Any())
            {
                item = GetExchangeInfo(rows.First());
            }

            return item;
        }


        /// <summary> 
        /// 根据 Agent 判断是否是智能手机 
        /// </summary> 
        /// <returns></returns> 
        public static bool IsMobile(string agent)
        {
            bool flag = false;
            string[] keywords = { "Android", "iPhone", "iPod", "iPad", "Windows Phone", "MQQBrowser" };
            //排除Window 桌面系统 和 苹果桌面系统 
            if (!agent.Contains("Windows NT") && !agent.Contains("Macintosh"))
            {
                foreach (string item in keywords)
                {
                    if (agent.Contains(item))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            return flag;
        } 

    }

}

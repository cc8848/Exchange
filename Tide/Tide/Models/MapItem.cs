using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tide.Models
{
    /// <summary>
    /// 每个地图点信息
    /// @author:zhangerzhao
    /// </summary>
    public class MapItem
    {
        public string ID;
        public string City;
        public int Value;
        public Location Loc;
    }

    /// <summary>
    /// 经纬度位置
    /// </summary>
    public struct Location
    {
        public double E;
        public double W;
    }

    public class DetailItem
    {
        public string ID;

        public string Name;

        public string MajorTradings;

        public string FoundingTime;

        public string Address;
    }

    public class SubUnit
    {
        public string UnitID { get; set; }

        public string UnitName { get; set; }

        public string City { get; set; }

        public string Address { get; set; }
    }
}
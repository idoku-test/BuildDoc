using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Baidu.Bmap
{
    /// <summary>
    /// Class to generate a static map using the baidu StaticMaps API
    /// http://developer.baidu.com/map/staticimg.htm
    /// Desigin from <see cref=" http://www.codeproject.com/Articles/28492/A-C-Wrapper-for-Google-s-Static-Map-API "/>
    /// </summary>
    public class StaticMapHelper
    {
        private const string BaiduStaticMapUrl = "http://api.map.baidu.com/staticimage";

        

        public string RenderV2()
        {
            string qs = BaiduStaticMapUrl + "?center={0},{1}&zoom={2}&width={3}&height={4}";
            string mkqs = "";
            string mkstyles = "";
            qs = string.Format(qs, Longitude, Latitude, Zoom, Width, Height);

            if (!string.IsNullOrEmpty(MarkParms))
            {
                qs += MarkParms;
            }

            if (!string.IsNullOrEmpty(LabelParms))
            {
                qs += LabelParms;
            }

            return qs;
        }

        /// <summary>
        /// Renders an image for display
        /// </summary>
        /// <returns>Primarily this just creates an imageUrl string</returns>
        public string Render()
        {
            string qs = BaiduStaticMapUrl + "?center={0},{1}&zoom={2}&width={3}&height={4}";
            string mkqs = "";
            string mkstyles = "";


            qs = string.Format(qs, Longitude, Latitude,Math.Ceiling(Zoom), Width, Height);
            //add markers

            foreach (var marker in _markers)
            {
                if (marker.MarkStyle == mMarkStyle.icon)
                {
                    //zoom caculate 
                    //zoom max is 18 and mapping x 65 y 50
                    //zoom decrease 1 coordinate add 50
                    //double offsetZoom = (18 - Math.Ceiling(Zoom)) * 50;
                    //double offsetZoom = 256 * (Math.Pow(2, Math.Ceiling(Zoom)));
                    //double offsetX =  (marker.MarkIcon.OffSetX+offsetZoom) / 100000d;
                    //double offsetY = (marker.MarkIcon.OffSetY+offsetZoom) / 100000d;

                    int x, y;
                    MapPoint.LatLongToPixelXY(marker.Latitude, marker.Longitude, (int)Math.Ceiling(Zoom), out x, out y);
                    int offsetX = x + marker.MarkIcon.OffSetX / 2;
                    int offsetY = y + marker.MarkIcon.OffSetY;
                    double lat, lng;
                    MapPoint.PixelXYToLatLong(offsetX, offsetY, (int)Math.Ceiling(Zoom), out lat, out lng);

                    //double lat, lng;
                    //lat = ((marker.Latitude * Math.Pow(2, Zoom))
                    //    + marker.MarkIcon.OffSetX / 2) / Math.Pow(2, Zoom);
                    //lng = ((marker.Longitude * Math.Pow(2, Zoom))
                    //    + marker.MarkIcon.OffSetY / 2) / Math.Pow(2, Zoom);

                    mkqs += string.Format("{0},{1}|", lng, lat);
                    mkstyles += string.Format("{0}|",
                        GetMarkerIconParms(marker.MarkIcon.IconUrl));
                }
                else 
                {
                    mkqs += string.Format("{0},{1}|", marker.Longitude, marker.Latitude);
                    mkstyles += string.Format("{0}|",
                        GetMarkerParams(marker.Size, marker.Color, marker.Label));
                }

            }

            if (mkqs.Length > 0)
            {
                MarkParms = "&markers=" + mkqs.TrimEnd('|') + "&markerStyles=" + mkstyles.TrimEnd('|');
                qs += MarkParms;
            }

            string mlqs = "";
            string mlstyles = "";
            //add labels
            foreach (var label in _labels)
            {
                mlqs += string.Format("{0},{1}|", label.Longitude, label.Latitude);
                mlstyles += string.Format("{0}|", GetLablesParms(label.Content, label.Border, label.FontSize,
                    label.FontColor, label.BgColor, label.FontWeight));
            }
            if (mlqs.Length > 0)
            {
                LabelParms = "&labels=" + mlqs.TrimEnd('|') + "&labelStyles=" + mlstyles.TrimEnd('|');
                qs += LabelParms;
            }
            return qs;
        }

        private string GetMarkerIconParms(string iconUrl)
        {
            string marker;
            marker = string.Format("-1,{0},-1", iconUrl);
            return marker;
        }

        /// <summary>
        /// Build the correct string for marker parameters
        /// </summary>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        private string GetMarkerParams(mSize size, mColor color, string label)
        {
            string marker;

            marker = size.ToString().ToLower() + "," + color.ToString().ToLower() + "," + label;
            return marker;
        }
        //http://api.map.baidu.com/staticimage?
        //width=400&height=200&center=116.403874,39.914888&
        //markers=百度大厦|116.403874,39.914888&zoom=15&markerStyles=l,A,0xff0000

        /// <summary>
        /// Build the correct string for lable parameters
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fSize"></param>
        /// <param name="fColor"></param>
        /// <param name="bgColor"></param>
        /// <returns></returns>
        private string GetLablesParms(string content, int border, int fSize,
            System.Drawing.Color fColor, System.Drawing.Color bgColor, int fontWeight)
        {
            string lables;
            lables = string.Format("{0},{1},{2},{3:x6},{4:x6},{5}", content, border, fSize,
                     fColor.ToArgb(), bgColor.ToArgb(), fontWeight);
            return lables;


        }

        //http://api.map.baidu.com/staticimage?
        //width=400&height=200&center=北京&labels=海淀|116.487812,40.017524&
        //labelStyles=小吃,1,14,0xffffff,0x000fff,1
        public StaticMapHelper()
        { }

    

        /// <summary>
        /// Defines a icon to display to a map
        /// </summary>
        public class MapMarkIcon
        {
            public int OffSetX { get; set; }
            public int OffSetY { get; set; }
            public string IconUrl { get; set; }
        }

        /// <summary>
        /// Defines a single map point to be added to a map
        /// </summary>
        public class MapMarker
        {
            private string label = "";

            public string Label
            {
                get { return label; }
                set { label = value; }
            }

            #region Auto-Properties
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public mSize Size { get; set; }
            public mColor Color { get; set; }
            public mMarkStyle MarkStyle { get; set; }
            //public string IconUrl { get; set; }
            public MapMarkIcon MarkIcon { get; set; }
            #endregion
        }

        /// <summary>
        /// Defines a single map label to be added to a map
        /// </summary>
        public class MapLabel
        {
            public string Content { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public int Border { get; set; }
            public int FontSize { get; set; }
            public System.Drawing.Color FontColor { get; set; }
            public System.Drawing.Color BgColor { get; set; }
            public int FontWeight { get; set; }            
        }


       

        /// <summary>
        /// All Map rendering properties as enums
        /// </summary>
        #region Marker enums
        public enum mMarkStyle
        {
            text = 1,
            icon = 2
        }

        public enum mFormat
        {
            gif = 0,
            jpg = 1,
            png = 2
        }

        public enum mSize
        {
            s = 1,
            m = 2,
            l = 3
        }

        public enum mColor
        {
            Black = 0,
            Brown = 1,
            Green = 2,
            Purple = 3,
            Yellow = 4,
            Blue = 5,
            Gray = 6,
            Orange = 7,
            Red = 8,
            White = 9
        }



        #endregion


        #region Properties
        private List<MapMarker> _markers = new List<MapMarker>();
        private List<MapLabel> _labels = new List<MapLabel>();

        public List<MapMarker> Markers
        {
            get { return _markers; }
            set { _markers = value; }
        }

        public List<MapLabel> Labels
        {
            get { return _labels; }
            set { _labels = value; }
        }


        #endregion


        #region Auto-Properties
        public double Width { get; set; }
        public double Height { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Zoom { get; set; }
        public int Scale { get; set; }
        public string MarkParms { get; set; }
        public string LabelParms { get; set; }        
        #endregion
    }
}

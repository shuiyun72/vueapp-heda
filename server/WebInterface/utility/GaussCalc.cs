using System;
using System.Collections.Generic;
using System.Text;


    /*----------------------------------------------------------------
    // Copyright (C) 安河清源　版权所有 
    //
    // 文件名：GaussCalc.cs
    // 文件功能描述：高斯投影坐标转换
    // 
    // 创建标识：
    //
    // 修改标识：
    // 修改描述：
    //----------------------------------------------------------------*/

    /// <summary>
    /// 高斯投影坐标正反算
    /// </summary>
    public static class GaussCalc//:Interface.IAHGaussCalc
    {
        // a为长半轴, 单位:米, e2为第一偏心率平方
        private static double _a, _e, _e2;
        private static double _ao, _bo, _co, _do, _eo;
        private const double _pi = 3.141592653589793;
        private const double _po = 0.017453292519943; //(_pi/180)
        /// <summary>构造
        /// </summary>
        //public GaussCalc()
        //{            
        //    GetModulus();
        //}

        /// <summary>
        /// 计算出转换参数
        /// </summary>
        public static void GetModulus()
        {
            _a = SemiMajorAxis;
            double fai = 1 / Flattening;
            _e2 = 2 * fai - fai * fai;
            _e = _e2 / (1.0 - _e2);
            double d = _a * (1.0 - _e2);

            _ao = d * (1.0 + 0.75 * _e2 + 0.703125 * _e2 * _e2 + 0.68359375 * _e2 * _e2 * _e2 + 0.67291259765625 * _e2 * _e2 * _e2 * _e2);
            _bo = d * (0.75 * _e2 + 0.703125 * _e2 * _e2 + 0.68359375 * _e2 * _e2 * _e2 + 0.67291259765625 * _e2 * _e2 * _e2 * _e2);
            _co = d * (0.46875 * _e2 * _e2 + 175 * _e2 * _e2 * _e2 / 384 + 0.4486083984375 * _e2 * _e2 * _e2 * _e2);
            _do = d * (35.0 * _e2 * _e2 * _e2 / 96 + 0.35888671875 * _e2 * _e2 * _e2 * _e2);
            _eo = d * (0.3076171875 * _e2 * _e2 * _e2 * _e2);
        }

        /// <summary>
        /// 椭球体长半轴,默认为6378245.0(BeiJing1954)
        /// </summary>
        public static double SemiMajorAxis
        {
            get
            {
                return _semiMajorAxis;
            }
            set
            {
                _semiMajorAxis = value;
                GetModulus();
            }
        }
        //private static double _semiMajorAxis = 6378245.0;
        private static double _semiMajorAxis = 6378137;

        /// <summary>
        /// 扁率,默认为298.3(BeiJing1954)
        /// </summary>
        public static double Flattening
        {
            get
            {
                return _flattening;
            }
            set
            {
                _flattening = value;
                GetModulus();
            }
        }
        private static double _flattening = 298.257223563;

        /// <summary>
        /// 经纬度转换为大地坐标
        /// </summary>
        /// <param name="B">纬度，单位：度</param>
        /// <param name="L">经度差(经度减去中央经度)，单位：度</param>
        /// <param name="N">北方向坐标，单位：米</param>
        /// <param name="E">东方向坐标，单位：米</param>
        public static void GaussBL2NE(double B, double L, out double N, out double E)
        {
            double x, n, v, m, brad, sinb, cosb, tan1, tan2, tan4;

            brad = B * _po;
            sinb = Math.Sin(brad);
            cosb = Math.Cos(brad);
            tan1 = Math.Tan(brad);
            tan2 = Math.Pow(tan1, 2);
            tan4 = Math.Pow(tan1, 4);

            m = L * cosb * _po;
            n = _e * cosb * cosb;
            v = _a / Math.Sqrt(1.0 - _e2 * sinb * sinb);
            x = _ao * brad - (_bo * sinb + _co * Math.Pow(sinb, 3) + _do * Math.Pow(sinb, 5) + _eo * Math.Pow(sinb, 7)) * cosb;

            N = x + v * tan1 * (0.5 * m * m + (5.0 - tan2 + 9.0 * n + 4.0 * n * n) * Math.Pow(m, 4) / 24.0 + (61.0 - 58.0 * tan2 + tan4) * Math.Pow(m, 6) / 720.0);
            E = 500000 + v * (m + (1.0 - tan2 + n) * Math.Pow(m, 3) / 6.0 + (5.0 - 18.0 * tan2 + tan4 + 14.0 * n - 58.0 * n * tan2) * Math.Pow(m, 5) / 120.0);
        }

        /// <summary>
        /// 大地坐标转换为经纬度
        /// </summary>
        /// <param name="N">北方向坐标，单位：米</param>
        /// <param name="E">东方向坐标，单位：米</param>
        /// <param name="B">纬度，单位：度</param>
        /// <param name="L">经度差，单位：度</param>
        public static void GaussNE2BL(double N, double E, out double B, out double L)
        {
            double n, v, no, bo, brad, sinb, cosb, tan1, tan2, tan4;

            bo = N / _ao / _po;
            brad = bo * _po;
            sinb = Math.Sin(brad);
            cosb = Math.Cos(brad);
            while (true)
            {
                bo = (N + (_bo * sinb + _co * Math.Pow(sinb, 3) + _do * Math.Pow(sinb, 5) + _eo * Math.Pow(sinb, 7)) * cosb) / _ao / _po;

                brad = bo * _po;
                sinb = Math.Sin(brad);
                cosb = Math.Cos(brad);

                no = _ao * brad - (_bo * sinb + _co * Math.Pow(sinb, 3) + _do * Math.Pow(sinb, 5) + _eo * Math.Pow(sinb, 7)) * cosb;

                if (Math.Abs(no - N) <= 0.0001)
                    break;
            }

            brad = bo * _po;
            sinb = Math.Sin(brad);
            cosb = Math.Cos(brad);
            tan1 = Math.Tan(brad);
            tan2 = Math.Pow(tan1, 2);
            tan4 = Math.Pow(tan1, 4);

            n = _e * cosb * cosb;
            v = E / _a * Math.Sqrt(1.0 - _e2 * sinb * sinb);

            L = 1.0 / _pi / cosb * (180 - 30 * (1.0 + 2 * tan2 + n) * Math.Pow(v, 2) + 1.5 * (5 + 28 * tan2 + 24 * tan4) * Math.Pow(v, 4)) * v;
            B = bo - (1 + n) / _pi * tan1 * (90 * Math.Pow(v, 2) - 7.5 * (5 + 3 * tan2 + n - 9 * n * tan2) * Math.Pow(v, 4) + 0.25 * (61 + 90 * tan2 + 45 * tan4) * Math.Pow(v, 6));
        }

        /// <summary>
        /// 度数转换为度分秒
        /// </summary>
        /// <param name="deg">输入的度数</param>
        /// <param name="degree">度</param>
        /// <param name="minute">分</param>
        /// <param name="second">秒</param>
        public static void DEG2DMS(double deg, out int degree, out double minute, out double second)
        {
            degree = (int)deg;
            minute = (deg - degree) * 60;
            second = (minute - (int)minute) * 60;
            minute = (int)minute;
        }

        /// <summary>
        /// 度分秒转换为度数
        /// </summary>
        /// <param name="degree">度</param>
        /// <param name="minute">分</param>
        /// <param name="second">秒</param>
        /// <returns></returns>
        public static double DMS2DEG(int degree, double minute, double second)
        {
            return (degree + minute / 60 + second / 3600);
        }
    }


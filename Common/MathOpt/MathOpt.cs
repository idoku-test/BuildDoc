using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class MathOpt
    {///<summary>
        ///用最小二乘法拟合二元多次曲线
        ///例如y=ax+b
        ///其中MultiLine将返回a，b两个参数。
        ///a对应MultiLine[1]
        ///b对应MultiLine[0]
        ///</summary>
        ///<param name="arrX">已知点的x坐标集合</param>
        ///<param name="arrY">已知点的y坐标集合</param>
        ///<param name="length">已知点的个数</param>
        ///<param name="dimension">方程的最高次数</param>
        public  double[] MultiLine(double[] arrX, double[] arrY, int length, int dimension)//二元多次线性方程拟合曲线
        {
            int n = dimension + 1;                  //dimension次方程需要求 dimension+1个 系数
            double[,] Guass = new double[n, n + 1];      //高斯矩阵 例如：y=a0+a1*x+a2*x*x
            for (int i = 0; i < n; i++)
            {
                int j;
                for (j = 0; j < n; j++)
                {
                    Guass[i, j] = SumArr(arrX, j + i, length);
                }
                Guass[i, j] = SumArr(arrX, i, arrY, 1, length);
            }

            return ComputGauss(Guass, n);

        }
        private  double SumArr(double[] arr, int n, int length) //求数组的元素的n次方的和
        {
            double s = 0;
            for (int i = 0; i < length; i++)
            {
                if (arr[i] != 0 || n != 0)
                    s = s + Math.Pow(arr[i], n);
                else
                    s = s + 1;
            }
            return s;
        }
        private  double SumArr(double[] arr1, int n1, double[] arr2, int n2, int length)
        {
            double s = 0;
            for (int i = 0; i < length; i++)
            {
                if ((arr1[i] != 0 || n1 != 0) && (arr2[i] != 0 || n2 != 0))
                    s = s + Math.Pow(arr1[i], n1) * Math.Pow(arr2[i], n2);
                else
                    s = s + 1;
            }
            return s;

        }
        private double[] ComputGauss(double[,] Guass, int n)
        {
            int i, j;
            int k, m;
            double temp;
            double max;
            double s;
            double[] x = new double[n];

            for (i = 0; i < n; i++) x[i] = 0.0;//初始化


            for (j = 0; j < n; j++)
            {
                max = 0;

                k = j;
                for (i = j; i < n; i++)
                {
                    if (Math.Abs(Guass[i, j]) > max)
                    {
                        max = Guass[i, j];
                        k = i;
                    }
                }



                if (k != j)
                {
                    for (m = j; m < n + 1; m++)
                    {
                        temp = Guass[j, m];
                        Guass[j, m] = Guass[k, m];
                        Guass[k, m] = temp;

                    }
                }

                if (0 == max)
                {
                    // "此线性方程为奇异线性方程" 

                    return x;
                }


                for (i = j + 1; i < n; i++)
                {

                    s = Guass[i, j];
                    for (m = j; m < n + 1; m++)
                    {
                        Guass[i, m] = Guass[i, m] - Guass[j, m] * s / (Guass[j, j]);

                    }
                }


            }//结束for (j=0;j<n;j++)


            for (i = n - 1; i >= 0; i--)
            {
                s = 0;
                for (j = i + 1; j < n; j++)
                {
                    s = s + Guass[i, j] * x[j];
                }

                x[i] = (Guass[i, n] - s) / Guass[i, i];

            }

            

            return x;
        }

        public double GetSum(double[] dAvas) {

            double dSum = 0;
            for (int i = 0; i < dAvas.Length; i++) {
                dSum = dSum + dAvas[i];
            }
            return dSum;
        }

        public double GetAvarage(double[] dAvas) {
            double dAva = 0;
            if(dAvas.Length > 0)
            {
                dAva = GetSum(dAvas)/dAvas.Length;
            };
            return dAva;
        }

        public double GetSEE(double[] dXs, double[] dYs, double[] dParams) { 
            double dY1 = 0; //误差和
            double dYA  = 0;//拟合值

            //误差的平方和  SSE(和方差) 该统计参数计算的是拟合数据和原始数据对应点的误差的平方和 越接近于0，说明模型选择和拟合更好，数据预测也越成功
            
            for (int i = 0; i < dXs.Length; i++) {
                dYA = 0;
                for (int j = 0; j < dParams.Length; j++) {
                    dYA = dYA + dParams[j] * Math.Pow(dXs[i], j);
                }
                dY1 = dY1 + Math.Pow(dYs[i] - dYA, 2);
            }

            return dY1;
        }

        public double GetSST(double[] dXs, double[] dYs, double[] dParams)
        {
            double dY1 = 0; //平均误差和 

            //SST 原始数据和均值之差的平方和
            double dSumAva = GetAvarage(dYs); ;
            for (int i = 0; i < dXs.Length; i++)
            {
                dY1 = dY1 + Math.Pow(dYs[i] - dSumAva, 2); 
            }

            return dY1;
        }

        public double GetR_Square(double[] dXs, double[] dYs, double[] dParams)
        { 
            //R方 ： 其实“确定系数           ”是通过数据的变化来表征一个拟合的好坏。
            //由上面的表达式可以知道“确定系数”的正常取值范围为
            //[0 1]，越接近1，表明方程的变量对y的解释能力越强，这个模型对数据拟合的也较好    
            //R_Square = 1- SSE/SST
            double dR_Square =0;
            double dSST = GetSST(dXs, dYs, dParams);
            if (dSST != 0) {
                dR_Square = 1 - GetSEE(dXs, dYs, dParams) / GetSST(dXs, dYs, dParams);
            }
            return dR_Square;
        }


        /// <summary>
        ///  一元线性回归分析
        /// </summary>
        /// <param name="x"> 存放自变量x的n个取值</param>
        /// <param name="y">存放与自变量x的n个取值相对应的随机变量y的观察值</param>
        /// <param name="n"> 观察点数</param>
        /// <param name="a"> a(0) 返回回归系数b ,a(1)返回回归系数a</param>
        /// <param name="dt"> dt(0) 返回偏差平方和q ,dt(1)返回平均标准偏差s ,dt(2)返回回归平方和p,dt(3)返回最大偏差umax,dt(4)返回最小偏差umin,dt(5)返回偏差平均值u</param>
        public void SPT1(double[] x, double[] y, int n, double[] a,
                double[] dt)
        // double x[],y[],a[2],dt[6];
        {
            int i;
            double xx, yy, e, f, q, u, p, umax, umin, s;
            xx = 0.0;
            yy = 0.0;
            for (i = 0; i <= n - 1; i++)
            {
                xx = xx + x[i] / n;
                yy = yy + y[i] / n;
            }
            e = 0.0;
            f = 0.0;
            for (i = 0; i <= n - 1; i++)
            {
                q = x[i] - xx;
                e = e + q * q;
                f = f + q * (y[i] - yy);
            }
            a[1] = f / e;
            a[0] = yy - a[1] * xx;
            q = 0.0;
            u = 0.0;
            p = 0.0;
            umax = 0.0;
            umin = 1.0e+30;
            for (i = 0; i <= n - 1; i++)
            {
                s = a[1] * x[i] + a[0];
                q = q + (y[i] - s) * (y[i] - s);
                p = p + (s - yy) * (s - yy);
                e = Math.Abs(y[i] - s);
                if (e > umax)
                    umax = e;
                if (e < umin)
                    umin = e;
                u = u + e / n;
            }
            dt[1] = Math.Sqrt(q / n);
            dt[0] = q;
            dt[2] = p;
            dt[3] = umax;
            dt[4] = umin;
            dt[5] = u;
        }

        /// <summary>
        /// 多元线性回归分析
        /// </summary>
        /// <param name="x">每一列存放m个自变量的观察值</param>
        /// <param name="y">存放随即变量y的n个观察值</param>
        /// <param name="m">自变量的个数</param>
        /// <param name="n"> 观察数据的组数</param>
        /// <param name="a">返回回归系数a0,...,am</param>
        /// <param name="dt"> dt[0]偏差平方和q,dt[1] 平均标准偏差s dt[2]返回复相关系数r dt[3]返回回归平方和u</param>
        /// <param name="v">返回m个自变量的偏相关系数</param>
        public void sqt2(double[][] x, double[] y, int m, int n, double[] a,
                double[] dt, double[] v)
        {
            int i, j, k, mm;
            double q, e, u, p, yy, s, r, pp;
            double[] b = new double[m*m];
            mm = m-1;
            b[mm * mm - 1] = n;
            for (j = 0; j <= m - 1; j++)
            {
                p = 0.0;
                for (i = 0; i <= n - 1; i++)
                    p = p + x[j][i];
                b[m * mm + j] = p;
                b[j * mm + m] = p;
            }
            for (i = 0; i <= m - 1; i++)
                for (j = i; j <= m - 1; j++)
                {
                    p = 0.0;
                    for (k = 0; k <= n - 1; k++)
                        p = p + x[i][k] * x[j][k];
                    b[j * mm + i] = p;
                    b[i * mm + j] = p;
                }
            a[m-1] = 0.0;
            for (i = 0; i <= n - 1; i++)
                a[m-1] = a[m-1] + y[i];
            for (i = 0; i <= m - 1; i++)
            {
                a[i] = 0.0;
                for (j = 0; j <= n - 1; j++)
                    a[i] = a[i] + x[i][j] * y[j];
            }
            chlk(b, mm, 1, a);
            yy = 0.0;
            for (i = 0; i <= n - 1; i++)
                yy = yy + y[i] / n;
            q = 0.0;
            e = 0.0;
            u = 0.0;
            for (i = 0; i <= n - 1; i++)
            {
                p = a[m-1];
                for (j = 0; j <= m - 1; j++)
                    p = p + a[j] * x[j][i];
                q = q + (y[i] - p) * (y[i] - p);
                e = e + (y[i] - yy) * (y[i] - yy);
                u = u + (yy - p) * (yy - p);
            }
            s = Math.Sqrt(q / n);
            r = Math.Sqrt(1.0 - q / e);
            for (j = 0; j <= m - 1; j++)
            {
                p = 0.0;
                for (i = 0; i <= n - 1; i++)
                {
                    pp = a[m-1];
                    for (k = 0; k <= m - 1; k++)
                        if (k != j)
                            pp = pp + a[k] * x[k][i];
                    p = p + (y[i] - pp) * (y[i] - pp);
                }
                v[j] = Math.Sqrt(1.0 - q / p);
            }
            dt[0] = q;
            dt[1] = s;
            dt[2] = r;
            dt[3] = u;
        }

        private int chlk(double[] a, int n, int m, double[] d)
        {
            int i, j, k, u, v;
            if ((a[0] + 1.0 == 1.0) || (a[0] < 0.0))
            { 
                return (-2);
            }
            a[0] = Math.Sqrt(a[0]);
            for (j = 1; j <= n - 1; j++)
                a[j] = a[j] / a[0];
            for (i = 1; i <= n - 1; i++)
            {
                u = i * n + i;
                for (j = 1; j <= i; j++)
                {
                    v = (j - 1) * n + i;
                    a[u] = a[u] - a[v] * a[v];
                }
                if ((a[u] + 1.0 == 1.0) || (a[u] < 0.0))
                {
                    //MessageBox.Show("fail2");
                    //System.out.println("fail\n");
                    return (-2);
                }
                a[u] = Math.Sqrt(a[u]);
                if (i != (n - 1))
                {
                    for (j = i + 1; j <= n - 1; j++)
                    {
                        v = i * n + j;
                        for (k = 1; k <= i; k++)
                            a[v] = a[v] - a[(k - 1) * n + i] * a[(k - 1) * n + j];
                        a[v] = a[v] / a[u];
                    }
                }
            }
            for (j = 0; j <= m - 1; j++)
            {
                d[j] = d[j] / a[0];
                for (i = 1; i <= n - 1; i++)
                {
                    u = i * n + i;
                    v = i * m + j;
                    for (k = 1; k <= i; k++)
                        d[v-1] = d[v-1] - a[(k - 1) * n + i] * d[(k - 1) * m + j];
                    d[v-1] = d[v-1] / a[u];
                }
            }
            for (j = 0; j <= m - 1; j++)
            {
                u = (n - 1) * m + j;
                d[u-1] = d[u-1] / a[n * n - 1];
                for (k = n - 1; k >= 1; k--)
                {
                    u = (k - 1) * m + j;
                    for (i = k; i <= n - 1; i++)
                    {
                        v = (k - 1) * n + i;
                        d[u-1] = d[u-1] - a[v] * d[i * m + j];
                    }
                    v = (k - 1) * n + k - 1;
                    d[u-1] = d[u-1] / a[v];
                }
            }
            return (2);
        }
    }

    
}

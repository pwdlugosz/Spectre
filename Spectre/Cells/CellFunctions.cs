using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Structures;

namespace Spectre.Cells
{

    /// <summary>
    /// Contains functions for cells
    /// </summary>
    public static class CellFunctions
    {

        /// <summary>
        /// Performs the log base AWValue; the resulting AWValue will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <returns>Cell AWValue</returns>
        public static Cell Log(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Log(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the log base 2; the resulting AWValue will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <returns>Cell AWValue</returns>
        public static Cell Log2(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Log(C.valueDOUBLE) / Math.Log(2D);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the log base 10; the resulting AWValue will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <returns>Cell AWValue</returns>
        public static Cell Log10(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Log10(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the exponential base AWValue; the resulting AWValue will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <returns>Cell AWValue</returns>
        public static Cell Exp(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Exp(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the exponential base 2; the resulting AWValue will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <returns>Cell AWValue</returns>
        public static Cell Exp2(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Pow(2, C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the exponential base 10; the resulting AWValue will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <returns>Cell AWValue</returns>
        public static Cell Exp10(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Pow(10, C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the square root; the resulting AWValue will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <returns>Cell AWValue</returns>
        public static Cell Sqrt(Cell C)
        {

            if (C.NULL == 1)
                return C;

            //if (C.AFFINITY == CellAffinity.LONG)
            //{

            //    if (C.LONG <= 0)
            //    {
            //        C.NULL = 1;
            //        return C;
            //    }

            //    C.LONG = Cell.IntRoot(C.LONG);
            //    return C;

            //}

            double d = Math.Sqrt(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the power; the resulting AWValue will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C1">The base</param>
        /// <param name="C2">The exponent</param>
        /// <returns>Cell AWValue</returns>
        public static Cell Power(Cell C1, Cell C2)
        {

            if (C1.NULL == 1)
                return C1;
            else if (C2.NULL == 1)
                return C2;

            //if (C1.AFFINITY == CellAffinity.LONG && C2.AFFINITY == CellAffinity.LONG)
            //{
            //    C1.LONG = Cell.IntPower(C1.LONG, C2.LONG);
            //    return C1;
            //}

            double d = Math.Pow(C1.valueDOUBLE, C2.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C1.NULL = 1;
                return C1;
            }

            if (C1.AFFINITY == CellAffinity.DOUBLE) C1.DOUBLE = d;
            else if (C1.AFFINITY == CellAffinity.SINGLE) C1.SINGLE = (float)d;
            else if (C1.AFFINITY == CellAffinity.LONG) C1.LONG = (long)d;
            else if (C1.AFFINITY == CellAffinity.INT) C1.INT = (int)d;
            else if (C1.AFFINITY == CellAffinity.SHORT) C1.SHORT = (short)d;
            else if (C1.AFFINITY == CellAffinity.BYTE) C1.BYTE = (byte)d;
            else C1.NULL = 1;

            return C1;

        }

        // Geographic Functions //
        public static Cell ATan2(Cell C1, Cell C2)
        {

            CellAffinity t = CellAffinityHelper.Highest(C1.AFFINITY, C2.AFFINITY);

            if (C1.NULL == 1 || C2.NULL == 1)
                return CellValues.Null(t);

            double d = Math.Atan2(C1.valueDOUBLE, C2.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C1.NULL = 1;
                return C1;
            }

            if (t == CellAffinity.DOUBLE) C1.DOUBLE = d;
            else if (t == CellAffinity.SINGLE) C1.SINGLE = (float)d;
            else if (t == CellAffinity.LONG) C1.LONG = (long)d;
            else if (t == CellAffinity.INT) C1.INT = (int)d;
            else if (t == CellAffinity.SHORT) C1.SHORT = (short)d;
            else if (t == CellAffinity.BYTE) C1.BYTE = (byte)d;
            else C1.NULL = 1;

            return C1;

        }

        // Basic Trig Functions //
        /// <summary>
        /// Performs the trigonomic sine; the resulting AWValue will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <returns>Cell AWValue</returns>
        public static Cell Sin(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Sin(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the trigonomic cosine; the resulting AWValue will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <returns>Cell AWValue</returns>
        public static Cell Cos(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Cos(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the trigonomic tangent; the resulting AWValue will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <returns>Cell AWValue</returns>
        public static Cell Tan(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Tan(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell Csc(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / Math.Sin(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell Sec(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / Math.Cos(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell Cot(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / Math.Tan(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }
        
        // Inverse trig functions //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcSin(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Asin(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcCos(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Acos(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcTan(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Atan(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcCsc(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Asin(1D / C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcSec(Cell C)
        {

            if (C.NULL == 1)
                return C;


            double d = Math.Acos(1D / C.valueDOUBLE); 
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcCot(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Atan(1D / C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        // Hyperbolic functions //
        /// <summary>
        /// Performs the hyperbolic sine; the resulting AWValue will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <returns>Cell AWValue</returns>
        public static Cell Sinh(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Sinh(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the hyperbolic cosine; the resulting AWValue will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <returns>Cell AWValue</returns>
        public static Cell Cosh(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Cosh(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the hyperbolic tangent; the resulting AWValue will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <returns>Cell AWValue</returns>
        public static Cell Tanh(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Tanh(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell Csch(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / Math.Sinh(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell Sech(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / Math.Cosh(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell Coth(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / Math.Tanh(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        // Inverse hyperbolic
        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcSinh(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = C.valueDOUBLE;
            d = Math.Log(d + Math.Sqrt(d * d + 1));
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcCosh(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = C.valueDOUBLE;
            d = Math.Log(d + Math.Sqrt(d * d - 1));
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcTanh(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = C.valueDOUBLE;
            d = Math.Log((1+d)/(1-d)) * 0.5;
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcCsch(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / C.valueDOUBLE;
            d = Math.Log(d + Math.Sqrt(d * d + 1));
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcSech(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / C.valueDOUBLE; 
            d = Math.Log(d + Math.Sqrt(d * d + 1));
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell ArcCoth(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / C.valueDOUBLE; 
            d = Math.Log(d + Math.Sqrt(d * d + 1));
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        public static Cell Logit(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = 1D / (1 + Math.Exp(-C.valueDOUBLE));
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Base"></param>
        /// <param name="Exp"></param>
        /// <param name="Mod"></param>
        /// <returns></returns>
        public static Cell ModPow(Cell Base, Cell Exp, Cell Mod)
        {

            Cell u = LongModPow(Base.valueLONG, Exp.valueLONG, Mod.valueLONG);
            return CellConverter.Cast(u, Base.Affinity);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Base"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell DLog(Cell Base, Cell Value)
        {
            long rem = 0;
            long exp = DiscreteLog(Base.valueLONG, Value.valueLONG, out rem);
            CellArray v = new CellArray();
            v.Append(new Cell(exp));
            v.Append(new Cell(rem));
            return new Cell(v);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Base"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell DLogExp(Cell Base, Cell Value)
        {
            long rem = 0;
            return DiscreteLog(Base.valueLONG, Value.valueLONG, out rem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Base"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell DLogRem(Cell Base, Cell Value)
        {
            long rem = 0;
            long exp = DiscreteLog(Base.valueLONG, Value.valueLONG, out rem);
            return rem;
        }
        
        // Other //
        /// <summary>
        /// Returns the absolute AWValue of a cell's numeric AWValue; the resulting AWValue will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <returns>Cell AWValue</returns>
        public static Cell Abs(Cell C)
        {

            if (C.NULL == 1)
                return C;

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = Math.Abs(C.DOUBLE);
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = Math.Abs(C.SINGLE);
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = Math.Abs(C.LONG);
            else if (C.AFFINITY == CellAffinity.INT) C.INT = Math.Abs(C.INT);
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = Math.Abs(C.SHORT);
            else if (C.AFFINITY == CellAffinity.BYTE) C.LONG = Math.Abs(C.BYTE);
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Returns the sign of a cell's numeric AWValue; the resulting AWValue will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <returns>Cell AWValue, NULL, +1, -1, or 0</returns>
        public static Cell Sign(Cell C)
        {

            if (C.NULL == 1)
                return C;

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = Math.Sign(C.DOUBLE);
            else if (C.AFFINITY == CellAffinity.SINGLE) C.SINGLE = Math.Sign(C.SINGLE);
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = Math.Sign(C.LONG);
            else if (C.AFFINITY == CellAffinity.INT) C.INT = Math.Sign(C.INT);
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)Math.Sign(C.SHORT);
            else if (C.AFFINITY == CellAffinity.BYTE) C.LONG = Math.Sign(C.BYTE);
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="C"></param>
        /// <param name="Percision"></param>
        /// <returns></returns>
        public static Cell Round(Cell C, int Percision)
        {

            if (C.NULL == 1)
                return C;

            if (C.AFFINITY == CellAffinity.DOUBLE) 
                C.DOUBLE = Math.Round(C.DOUBLE, Percision);
            else if (C.AFFINITY == CellAffinity.SINGLE) 
                C.SINGLE = (Single)Math.Round((double)C.SINGLE, Percision);
            else if (C.AFFINITY != CellAffinity.BYTE && C.AFFINITY != CellAffinity.SHORT && C.AFFINITY != CellAffinity.INT && C.AFFINITY != CellAffinity.LONG) 
                C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the logic 'IF'
        /// </summary>
        /// <param name="A">Predicate: uses A.BOOL to perform the logical if</param>
        /// <param name="B">The AWValue returned if A is true</param>
        /// <param name="C">The AWValue returned if A is false</param>
        /// <returns>Aither B or C</returns>
        public static Cell If(Cell A, Cell B, Cell C)
        {
            if (A.BOOL)
                return B;
            if (B.AFFINITY != C.AFFINITY)
                return CellConverter.Cast(C, B.Affinity);
            return C;
        }

        // Dates //

        /// <summary>
        /// Extracts the year AWValue of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell AWValue</param>
        /// <returns>An integer cell</returns>
        public static Cell Year(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE_TIME)
                return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE_TIME.Year);
        }

        /// <summary>
        /// Extracts the month AWValue of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell AWValue</param>
        /// <returns>An integer cell</returns>
        public static Cell Month(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE_TIME) return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE_TIME.Month);
        }

        /// <summary>
        /// Extracts the day AWValue of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell AWValue</param>
        /// <returns>An integer cell</returns>
        public static Cell Day(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE_TIME) return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE_TIME.Day);
        }

        /// <summary>
        /// Extracts the hour AWValue of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell AWValue</param>
        /// <returns>An integer cell</returns>
        public static Cell Hour(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE_TIME) return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE_TIME.Hour);
        }

        /// <summary>
        /// Extracts the minute AWValue of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell AWValue</param>
        /// <returns>An integer cell</returns>
        public static Cell Minute(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE_TIME) return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE_TIME.Minute);
        }

        /// <summary>
        /// Extracts the second AWValue of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell AWValue</param>
        /// <returns>An integer cell</returns>
        public static Cell Second(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE_TIME) return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE_TIME.Second);
        }

        /// <summary>
        /// Extracts the millisecond AWValue of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell AWValue</param>
        /// <returns>An integer cell</returns>
        public static Cell Millisecond(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE_TIME) return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE_TIME.Millisecond);
        }

        /// <summary>
        /// Manipulates the ticks AWValue
        /// </summary>
        /// <param name="C"></param>
        /// <param name="Ticks"></param>
        /// <returns></returns>
        public static Cell AddTicks(Cell C, Cell Ticks)
        {

            if (C.AFFINITY != CellAffinity.DATE_TIME)
                return CellValues.NullDATE;

            C.LONG += Ticks.LONG;

            return C;

        }

        /// <summary>
        /// Trims a given string AWValue
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <returns>Cell with a stirng affinity</returns>
        public static Cell Trim(Cell C)
        {
            if (C.IsNull) return C;
            return (C.Affinity == CellAffinity.BSTRING ? new Cell(C.valueBSTRING.Trim()) : new Cell(C.valueCSTRING.Trim()));
        }

        /// <summary>
        /// Converts a given string to uppercase
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <returns>Cell with a stirng affinity</returns>
        public static Cell ToUpper(Cell C)
        {
            if (C.IsNull) return C;
            return (C.Affinity == CellAffinity.BSTRING ? new Cell(C.valueBSTRING.ToUpper()) : new Cell(C.valueCSTRING.ToUpper()));
        }

        /// <summary>
        /// Converts a given string to lowercase
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <returns>Cell with a stirng affinity</returns>
        public static Cell ToLower(Cell C)
        {
            if (C.IsNull) return C;
            return (C.Affinity == CellAffinity.BSTRING ? new Cell(C.valueBSTRING.ToLower()) : new Cell(C.valueCSTRING.ToLower()));
        }

        /// <summary>
        /// Returns all characters/bytes left of given point
        /// </summary>
        /// <param name="C">The string or BINARY AWValue</param>
        /// <param name="Length">The maximum number of chars/bytes</param>
        /// <returns>A string or blob cell</returns>
        public static Cell Left(Cell C, long Length)
        {
            int len = Math.Min(C.AFFINITY == CellAffinity.BINARY ? C.BINARY.Length : C.valueCSTRING.Length, (int)Length);
            return CellFunctions.Substring(C, 0, len);
        }

        /// <summary>
        /// Returns all characters/bytes right of given point
        /// </summary>
        /// <param name="C">The string or BINARY AWValue</param>
        /// <param name="Length">The maximum number of chars/bytes</param>
        /// <returns>A string or blob cell</returns>
        public static Cell Right(Cell C, long Length)
        {
            int l = C.AFFINITY == CellAffinity.BINARY ? C.BINARY.Length : C.valueCSTRING.Length;
            int begin = Math.Max(l - (int)Length, 0);
            int len = (int)Length;
            if (begin + Length > l) len = l - begin;
            return CellFunctions.Substring(C, begin, len);
        }

        // Array Applicable //

        /// <summary>
        /// Returns the smallest AWValue of an array of cells
        /// </summary>
        /// <param name="Elements">A collection of cells</param>
        /// <returns>A cell</returns>
        public static Cell Min(params Cell[] Data)
        {

            // Empty //
            if (Data == null) new Cell(CellAffinity.LONG);
            if (Data.Length == 0) new Cell(CellAffinity.LONG);

            // One AWValue //
            if (Data.Length == 1) return Data[0];

            // Two values //
            if (Data.Length == 2 && Data[0] < Data[1]) return Data[0];
            if (Data.Length == 2) return Data[1];

            // Three or more //
            Cell c = Data[0];
            for (int i = 1; i < Data.Length; i++)
            {
                if (Data[i] < c)
                    c = Data[i];
            }
            return c;

        }

        /// <summary>
        /// Returns the largest AWValue of an array of cells
        /// </summary>
        /// <param name="Elements">A collection of cells</param>
        /// <returns>A cell</returns>
        public static Cell Max(params Cell[] Data)
        {

            // Empty //
            if (Data == null) new Cell(CellAffinity.LONG);
            if (Data.Length == 0) new Cell(CellAffinity.LONG);

            // One AWValue //
            if (Data.Length == 1) return Data[0];

            // Two values //
            if (Data.Length == 2 && Data[0] > Data[1]) return Data[0];
            if (Data.Length == 2) return Data[1];

            // Three or more //
            Cell c = Data[0];
            for (int i = 1; i < Data.Length; i++)
            {
                if (Data[i] > c) c = Data[i];
            }
            return c;

        }

        /// <summary>
        /// Returns the most extreme AWValue in a sequence
        /// </summary>
        /// <param name="Elements">The sequence to evaluate; the first AWValue is the radix, the next N values are compared to the radix</param>
        /// <returns>The AWValue with the greatest distance from the radix</returns>
        public static Cell Extreme(params Cell[] Data)
        {

            // Handle invalid argument structures //
            if (Data.Length == 0)
                return CellValues.NullINT;

            CellAffinity t = Data[0].AFFINITY;

            // Handle invalid types //
            if (t == CellAffinity.BINARY || t == CellAffinity.BOOL || t == CellAffinity.CSTRING)
                return new Cell(t);

            // Handle arrays too small //
            if (Data.Length < 2)
                return new Cell(t);

            // Get the radix //
            Cell radix = Data[0];
            Cell MostExtreme = CellValues.Zero(t);
            Cell GreatestDistance = CellValues.Zero(t);

            // Cycle through looking for the most extreme AWValue //
            for (int i = 1; i < Data.Length; i++)
            {

                Cell distance = CellFunctions.Abs(Data[i] - radix);

                if (distance > GreatestDistance)
                {
                    GreatestDistance = distance;
                    MostExtreme = Data[i];
                }

            }
            
            return MostExtreme;

        }

        /// <summary>
        /// Returns the cumulative AND AWValue of an array of cells
        /// </summary>
        /// <param name="Elements">A collection of cells</param>
        /// <returns>A cell</returns>
        public static Cell And(params Cell[] Data)
        {

            // Empty //
            if (Data == null) new Cell(false);
            if (Data.Length == 0) new Cell(false);

            // Three or more //
            Cell b = CellValues.True;
            for (int i = 0; i < Data.Length; i++)
            {
                b = b && Data[i];
                if (!b) return b;
            }
            return b;

        }

        /// <summary>
        /// Returns the cumulative OR AWValue of an array of cells
        /// </summary>
        /// <param name="Elements">A collection of cells</param>
        /// <returns>A cell</returns>
        public static Cell Or(params Cell[] Data)
        {

            // Empty //
            if (Data == null) new Cell(false);
            if (Data.Length == 0) new Cell(false);

            // Three or more //
            Cell b = CellValues.False;
            for (int i = 0; i < Data.Length; i++)
            {
                b = b || Data[i].valueBOOL;
                if (b) return b;
            }
            return b;

        }

        /// <summary>
        /// Returns the sum of an array of cells
        /// </summary>
        /// <param name="Elements">A collection of cells</param>
        /// <returns>A cell</returns>
        public static Cell Sum(params Cell[] Data)
        {

            // Empty //
            if (Data == null) new Cell(CellAffinity.LONG);
            if (Data.Length == 0) new Cell(CellAffinity.LONG);

            // One AWValue //
            if (Data.Length == 1) return Data[0];

            // Two values //
            if (Data.Length == 2) return Data[0] + Data[1];

            // Three or more //
            Cell c = Data[0];
            for (int i = 1; i < Data.Length; i++)
            {
                c += Data[i];
            }
            return c;

        }

        /// <summary>
        /// Returns the first non-null cell in a collection
        /// </summary>
        /// <param name="Elements">A collection of cells</param>
        /// <returns>A cell</returns>
        public static Cell Coalesce(params Cell[] Data)
        {

            // Empty //
            if (Data == null) new Cell(CellAffinity.LONG);
            if (Data.Length == 0) new Cell(CellAffinity.LONG);
            CellAffinity x = CellAffinity.BOOL;
            for (int i = 0; i < Data.Length; i++)
            {
                if (!Data[i].IsNull) return Data[i];
                x = CellAffinityHelper.Highest(x, Data[i].Affinity);
            }
            return new Cell(x);

        }

        /// <summary>
        /// Checks if a given string contains another string
        /// </summary>
        /// <param name="Source">The string to be checked</param>
        /// <param name="Check">The string being check for</param>
        /// <returns>Cell with boolean type</returns>
        public static Cell Contains(Cell Source, Cell Pattern)
        {
            return Position(Source, Pattern, 0).IsNull ? CellValues.False : CellValues.True;
        }

        /// <summary>
        /// Returns either the sub stirng or sub blob
        /// </summary>
        /// <param name="C">Cell AWValue</param>
        /// <param name="Position">The starting point</param>
        /// <param name="Length">The maximum length of the new string</param>
        /// <returns>Either a string or blob AWValue</returns>
        public static Cell Substring(Cell C, int Position, int Length)
        {
            return C[Position, Length];
        }

        /// <summary>
        /// Replaces all occurances of a string/BINARY AWValue with another string/BINARY AWValue
        /// </summary>
        /// <param name="Source">The string to be searched</param>
        /// <param name="LookFor">The string being searched for</param>
        /// <param name="ReplaceWith">The string that serves as the replacement</param>
        /// <returns>Cell string AWValue</returns>
        public static Cell Replace(Cell Source, Cell LookFor, Cell ReplaceWith)
        {

            // Null or not variable //
            if (!CellAffinityHelper.IsVariableLength(Source.Affinity) || Source.IsNull) 
                return new Cell(Source.AFFINITY);

            // Binary //
            if (Source.AFFINITY == CellAffinity.BINARY)
            {
                if (LookFor.IsNull || ReplaceWith.IsNull) return CellValues.NullBLOB;
                BString x = new BString(Source.BINARY);
                x = x.Replace(LookFor.valueBSTRING, ReplaceWith.valueBSTRING);
                return new Cell(x.ToByteArray);
            }

            // BString //
            if (Source.AFFINITY == CellAffinity.BSTRING)
            {
                if (LookFor.IsNull || ReplaceWith.IsNull) return CellValues.NullBSTRING;
                BString x = Source.valueBSTRING;
                x = x.Replace(LookFor.valueBSTRING, ReplaceWith.valueBSTRING);
                return new Cell(new BString(x.ToByteArray));
            }

            // CString //
            if (Source.AFFINITY == CellAffinity.CSTRING)
            {
                if (LookFor.IsNull || ReplaceWith.IsNull) return CellValues.NullCSTRING;
                string x = Source.valueCSTRING;
                x = x.Replace(LookFor.valueCSTRING, ReplaceWith.valueCSTRING);
                return new Cell(x);
            }

            // Array //
            if (Source.AFFINITY == CellAffinity.ARRAY)
            {
                CellArray x = new CellArray();
                foreach (Cell c in Source.ARRAY)
                {
                    if (c == LookFor)
                        x.Append(ReplaceWith);
                    else
                        x.Append(c);
                }
                return new Cell(x);
            }

            throw new Exception();

        }

        /// <summary>
        /// Given a pattern and a string or blob, this function will try to seek out the starting position of a patern
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Pattern"></param>
        /// <param name="StartAt"></param>
        /// <returns>Either an integer indicating the position of the pattern or NULL if the pattern wasn't found</returns>
        public static Cell Position(Cell Source, Cell Pattern, int StartAt)
        {

            if (StartAt < 0) StartAt = 0;

            if (StartAt > Source.Length || Source.IsNull || Pattern.IsNull)
                return CellValues.NullINT;

            if (Source.AFFINITY == CellAffinity.ARRAY)
            {
                for (int i = 0; i < Source.ARRAY.Count; i++)
                {
                    if (Source[i] == Pattern)
                        return new Cell(i);
                }
            }

            if (Source.AFFINITY == CellAffinity.CSTRING)
            {
                int idx = Source.CSTRING.IndexOf(Pattern.valueCSTRING, StartAt);
                if (idx == -1) return CellValues.NullINT;
                return new Cell(Source.CSTRING.IndexOf(Pattern.valueCSTRING, StartAt));
            }

            if (Source.AFFINITY == CellAffinity.BSTRING)
            {
                int idx = Source.BSTRING.Find(Pattern.valueBSTRING, StartAt);
                if (idx == -1) return CellValues.NullINT;
                return new Cell(Source.BSTRING.Find(Pattern.valueBSTRING, StartAt));
            }

            if (Source.AFFINITY == CellAffinity.BINARY)
            {

                byte[] data = Source.BINARY;
                byte[] pattern = Pattern.valueBINARY;
                bool match = false;
                for (int i = StartAt; i < data.Length; i++)
                {

                    match = true;
                    for (int j = 0; j < pattern.Length; j++)
                    {
                        if (i + j >= data.Length)
                        {
                            match = false;
                            break;
                        }
                        if (data[i + j] != pattern[j])
                        {
                            match = false;
                            break;
                        }

                    }

                    if (match)
                    {
                        return new Cell(i);
                    }

                }

            }

            return CellValues.NullINT;

        }

        // Internal support //

        /// <summary>
        /// Converts a byte array to a string using UTF16 encoding
        /// </summary>
        /// <param name="Hash"></param>
        /// <returns></returns>
        internal static string ByteArrayToUTF16String(byte[] Hash)
        {

            byte[] to_convert = Hash;
            if (Hash.Length % 2 != 0)
            {
                to_convert = new byte[Hash.Length + 1];
                Array.Copy(Hash, to_convert, Hash.Length);
            }

            return ASCIIEncoding.BigEndianUnicode.GetString(to_convert);

        }

        /// <summary>
        /// Performs an optimized integer power
        /// </summary>
        /// <param name="Base">The base AWValue</param>
        /// <param name="Exp">The exponent</param>
        /// <returns>Another integer: Base ^ Exp</returns>
        internal static long IntPower(long Base, long Exp)
        {

            if (Exp == 0)
                return 1;
            else if (Exp == 1)
                return Base;

            if ((Exp % 2) == 1)
                return IntPower(Base * Base, Exp / 2) * Base;
            else
                return IntPower(Base * Base, Exp / 2);

        }

        /// <summary>
        /// Calculates the integer root
        /// </summary>
        /// <param name="AWValue"></param>
        /// <returns></returns>
        internal static long IntRoot(long Value)
        {

            if (Value <= 0)
                return 0;

            long x = (Value / 2) + 1;
            long y = (x + Value / x) / 2;
            while (y < x)
            {
                x = y;
                y = (x + Value / x) / 2;
            }

            return x;

        }

        /// <summary>
        /// Gets the index of the highest bit //
        /// </summary>
        /// <param name="AWValue"></param>
        /// <returns></returns>
        internal static int HighestBit(long Value)
        {

            long mask = 1;

            for (int i = 0; i < 63; i++)
            {

                if ((mask & Value) == mask)
                    return i;

                mask = mask << 1;

            }

            return 0;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Base"></param>
        /// <param name="Exp"></param>
        /// <param name="Mod"></param>
        /// <returns></returns>
        internal static long LongModPow(long Base, long Exp, long Mod)
        {

            long x = 1;

            while (Exp != 0)
            {
                if ((Exp & 1) == 1)
                {
                    x = (x * Base) % Mod;
                }
                Base = (Base * Base) % Mod;
                Exp >>= 1;
            }

            return x;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Base"></param>
        /// <param name="Value"></param>
        /// <param name="Remainder"></param>
        /// <returns></returns>
        internal static long DiscreteLog(long Base, long Value, out long Remainder)
        {

            long x = 0, y = 1, z = Value;

            if (Value == Base)
            {
                Remainder = 0;
                return 1;
            }

            if (Value < Base)
            {
                Remainder = Value;
                return 0;
            }

            while(Base >= z)
            {
                z /= Base;
                x++;
                y *= Base;
            }

            Remainder = Value - y;
            return x;

        }

    }

}

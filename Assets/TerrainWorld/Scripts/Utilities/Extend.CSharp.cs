﻿using System;
using System.Collections.Generic;
namespace TWorld.Utility
{
    public static partial class Extend
    {
        #region Collection

        /// <summary>
        /// 获取一个集合从start开始的length个
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="value">目标</param>
        /// <param name="start">开始索引</param>
        /// <param name="length">目标长度</param>
        /// <returns></returns>
        public static T[] Take<T>(this IList<T> value, int start, int length)
        {
            if (start + length > value.Count)
            {
                throw new Exception("[IList]超出索引范围");
            }
            T[] temp = new T[length];
            for (int i = 0; i < length; i++)
            {
                temp[i] = value[start + i];
            }
            return temp;
        }

        /// <summary>
        /// 对枚举器的所以数据进行某种操作
        /// </summary>
        /// <typeparam name="T">目标对象</typeparam>
        /// <param name="value">目标</param>
        /// <param name="action">操作事件</param>
        public static void ForEach<T>(this IEnumerable<T> value, Action<T> action)
        {
            foreach (T obj in value)
            {
                action(obj);
            }
        }

        /// <summary>
        /// 转变数组类型
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="array">原数组</param>
        /// <returns></returns>
        public static T[] Convert<T>(this Array array) where T : class
        {
            T[] tArray = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                tArray[i] = array.GetValue(i) as T;
            }
            return tArray;
        }



        /// <summary>
        /// 拼接二维数组的第一维
        /// </summary>
        public static T[,] Concat0<T>(this T[,] array_0, T[,] array_1)
        {
            if (array_0.GetLength(1) != array_1.GetLength(1))
            {
                throw new System.Exception("两个数组第二维不一致");
            }
            T[,] ret = new T[array_0.GetLength(0) + array_1.GetLength(0), array_0.GetLength(1)];
            for (int i = 0; i < array_0.GetLength(0); i++)
            {
                for (int j = 0; j < array_0.GetLength(1); j++)
                {
                    ret[i, j] = array_0[i, j];
                }
            }
            for (int i = 0; i < array_1.GetLength(0); i++)
            {
                for (int j = 0; j < array_1.GetLength(1); j++)
                {
                    ret[i + array_0.GetLength(0), j] = array_1[i, j];
                }
            }
            return ret;
        }

        /// <summary>
        /// 拼接二维数组的第二维
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static T[,] Concat1<T>(this T[,] array_0, T[,] array_1)
        {
            if (array_0.GetLength(0) != array_1.GetLength(0))
            {
                throw new System.Exception("两个数组第一维不一致");
            }
            T[,] ret = new T[array_0.GetLength(0), array_0.GetLength(1) + array_1.GetLength(1)];
            for (int i = 0; i < array_0.GetLength(0); i++)
            {
                for (int j = 0; j < array_0.GetLength(1); j++)
                {
                    ret[i, j] = array_0[i, j];
                }
            }
            for (int i = 0; i < array_1.GetLength(0); i++)
            {
                for (int j = 0; j < array_1.GetLength(1); j++)
                {
                    ret[i, j + array_0.GetLength(1)] = array_1[i, j];
                }
            }
            return ret;
        }

        /// <summary>
        /// 获取一个二维数组的某一部分并返回
        /// </summary>
        /// <param name="array">目标数组</param>
        /// <param name="base_0">第一维的起始索引</param>
        /// <param name="base_1">第二维的起始索引</param>
        /// <param name="length_0">第一维要获取的数据长度</param>
        /// <param name="length_1">第二维要获取的数据长度</param>
        /// <returns></returns>
        public static T[,] GetPart<T>(this T[,] array, int base_0, int base_1, int length_0, int length_1)
        {
            if (base_0 + length_0 > array.GetLength(0) || base_1 + length_1 > array.GetLength(1))
            {
                throw new System.Exception("索引超出范围");
            }
            T[,] ret = new T[length_0, length_1];
            for (int i = 0; i < length_0; i++)
            {
                for (int j = 0; j < length_1; j++)
                {
                    ret[i, j] = array[i + base_0, j + base_1];
                }
            }
            return ret;
        }

        /// <summary>
        /// 拼接三维数组
        /// </summary>
        public static T[,,] Concat0<T>(this T[,,] array_0, T[,,] array_1)
        {
            if (array_0.GetLength(1) != array_1.GetLength(1) || array_0.GetLength(2) != array_1.GetLength(2))
            {
                throw new System.Exception("两个数组二三维的长度要相等");
            }

            T[,,] ret = new T[array_0.GetLength(0) + array_1.GetLength(0), array_0.GetLength(1), array_1.GetLength(2)];
            for (int i = 0; i < array_0.GetLength(0); i++)
            {
                for (int j = 0; j < array_0.GetLength(1); j++)
                {
                    for (int k = 0; k < array_0.GetLength(2); k++)
                    {
                        ret[i, j, k] = array_0[i, j, k];
                    }
                }
            }
            for (int i = 0; i < array_1.GetLength(0); i++)
            {
                for (int j = 0; j < array_1.GetLength(1); j++)
                {
                    for (int k = 0; k < array_0.GetLength(2); k++)
                    {
                        ret[i + array_0.GetLength(0), j, k] = array_1[i, j, k];
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 拼接三维数组
        /// </summary>
        public static T[,,] Concat1<T>(this T[,,] array_0, T[,,] array_1)
        {
            if (array_0.GetLength(0) != array_1.GetLength(0) || array_0.GetLength(2) != array_1.GetLength(2))
            {
                throw new System.Exception("两个数组一三维的长度要相等");
            }

            T[,,] ret = new T[array_0.GetLength(0), array_0.GetLength(1) + array_1.GetLength(1), array_1.GetLength(2)];
            for (int i = 0; i < array_0.GetLength(0); i++)
            {
                for (int j = 0; j < array_0.GetLength(1); j++)
                {
                    for (int k = 0; k < array_0.GetLength(2); k++)
                    {
                        ret[i, j, k] = array_0[i, j, k];
                    }
                }
            }
            for (int i = 0; i < array_1.GetLength(0); i++)
            {
                for (int j = 0; j < array_1.GetLength(1); j++)
                {
                    for (int k = 0; k < array_0.GetLength(2); k++)
                    {
                        ret[i, j + array_0.GetLength(1), k] = array_1[i, j, k];
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 拼接三维数组
        /// </summary>
        public static T[,,] Concat2<T>(this T[,,] array_0, T[,,] array_1)
        {
            if (array_0.GetLength(0) != array_1.GetLength(0) || array_0.GetLength(1) != array_1.GetLength(1))
            {
                throw new System.Exception("两个数组一二维的长度要相等");
            }

            T[,,] ret = new T[array_0.GetLength(0), array_0.GetLength(1), array_1.GetLength(1) + array_1.GetLength(2)];
            for (int i = 0; i < array_0.GetLength(0); i++)
            {
                for (int j = 0; j < array_0.GetLength(1); j++)
                {
                    for (int k = 0; k < array_0.GetLength(2); k++)
                    {
                        ret[i, j, k] = array_0[i, j, k];
                    }
                }
            }
            for (int i = 0; i < array_1.GetLength(0); i++)
            {
                for (int j = 0; j < array_1.GetLength(1); j++)
                {
                    for (int k = 0; k < array_0.GetLength(2); k++)
                    {
                        ret[i, j, k + array_0.GetLength(2)] = array_1[i, j, k];
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 获取一个三维数组的某一部分并返回
        /// </summary>
        /// <param name="array">目标数组</param>
        /// <param name="base_0">第一维的起始索引</param>
        /// <param name="base_1">第二维的起始索引</param>
        /// <param name="base_2">第三维的起始索引</param>
        /// <param name="length_0">第一维要获取的数据长度</param>
        /// <param name="length_1">第二维要获取的数据长度</param>
        /// <param name="length_2">第三维要获取的数据长度</param>
        /// <returns></returns>
        public static T[,,] GetPart<T>(this T[,,] array, int base_0, int base_1, int base_2, int length_0, int length_1, int length_2)
        {
            if (base_0 + length_0 > array.GetLength(0) || base_1 + length_1 > array.GetLength(1) || base_2 + length_2 > array.GetLength(2))
            {
                throw new System.Exception("索引超出范围");
            }
            T[,,] ret = new T[length_0, length_1, length_2];
            for (int i = 0; i < length_0; i++)
            {
                for (int j = 0; j < length_1; j++)
                {
                    for (int k = 0; k < length_2; k++)
                    {
                        ret[i, j, k] = array[i + base_0, j + base_1, k + base_2];
                    }
                }
            }
            return ret;
        }
        #endregion

    }
}
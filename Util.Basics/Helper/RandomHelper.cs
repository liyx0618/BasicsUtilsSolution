﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util.Basics.Helper
{
    public static class RandomHelper
    {
        /// <summary>
        /// 随机数
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static decimal RandomNext(int maxValue)
        {
            var rand = new Random(Guid.NewGuid().GetHashCode());
            var result = rand.Next(maxValue);
            return result;
        }
    }
}

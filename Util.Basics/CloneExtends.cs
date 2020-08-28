using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Util.Basics
{
    //class ObjectClone
    //{
    //}
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
    //public static class TransExpV2<TIn, TOut>
    public class CloneExtends<T>
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly Func<T, T> cache = GetFunc();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static Func<T, T> GetFunc()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            foreach (var item in typeof(T).GetProperties())
            {
                if (!item.CanWrite)
                    continue;

                MemberExpression property = Expression.Property(parameterExpression, typeof(T).GetProperty(item.Name));
                MemberBinding memberBinding = Expression.Bind(item, property);
                memberBindingList.Add(memberBinding);
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(T)), memberBindingList.ToArray());
            Expression<Func<T, T>> lambda = Expression.Lambda<Func<T, T>>(memberInitExpression, new ParameterExpression[] { parameterExpression });
            var ret = lambda.Compile();
            return ret;
        }

        public static T Trans(T t)
        {
            return cache(t);
        }

        public static void Main()
        {
            Student stu = new Student();
            stu.Age = 10;
            stu.Name = "小明";
            stu.Id = 2;
            Student ss = CloneExtends<Student>.Trans(stu);
        }
    }
}

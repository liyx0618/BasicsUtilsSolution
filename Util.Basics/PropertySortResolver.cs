using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Util.Basics
{
    /// <summary>
    /// 字段排序解析器
    /// </summary>
    public class PropertySortResolver : DefaultContractResolver
    {
        /// <summary>
        /// 排序方式  0;升序  1:降序
        /// </summary>
        int sort = 0;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sort">排序方式  0;升序  1:降序</param>
        public PropertySortResolver(int sort = 0)
        {
            this.sort = sort;
        }
        /// <summary>
        ///  属性名称按照字典顺序排序输出
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> list = base.CreateProperties(type, memberSerialization);
            if (sort == 0)
                return list.OrderBy(a => a.PropertyName).ToList();
            else
                return list.OrderByDescending(a => a.PropertyName).ToList();
        }

        /// <summary>
        /// 使用案例
        /// </summary>
        /// <param name="args"></param>
        //static 
        public    void Main()
        {
            var entity = new { name = "小明", age = 12 };
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.NullValueHandling = NullValueHandling.Ignore; //忽略null值; 
            setting.ContractResolver = new PropertySortResolver();
            //setting.ContractResolver = new CamelCasePropertyNamesContractResolver();  //驼峰命名
            Console.WriteLine(JsonConvert.SerializeObject(entity, setting));
        }
    }
}

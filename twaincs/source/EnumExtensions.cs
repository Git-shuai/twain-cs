using System;
using System.ComponentModel;
namespace TWAINWorkingGroup
{
    public static class EnumExtensions
    {
        // 为所有枚举类型扩展TryParse方法
        public static bool TryParse<TEnum>(string value, out TEnum result) where TEnum : struct, IConvertible
        {
            // 检查TEnum是否为枚举类型
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an enumerated type");
            }

            // 使用Enum.Parse尝试解析字符串，捕获可能的异常
            try
            {
                result = (TEnum)Enum.Parse(typeof(TEnum), value, true);
                return true;
            }
            catch (ArgumentException)
            {
                // 如果解析失败，设置result为默认值并返回false
                result = default(TEnum);
                return false;
            }
        }
    }
}
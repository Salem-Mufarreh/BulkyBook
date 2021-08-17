using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
    public class SD
    {
        public const string Proc_CoverType_Create = "usp_CreateCoverType";
        public const string Proc_CoverType_Get = "usp_GetCoverType";
        public const string Proc_CoverType_GetAll = "usp_GetCoverTypes";
        public const string Proc_CoverType_update = "usp_UpdateCoverType";
        public const string Proc_CoverType_Delete = "usp_DeleteCoverType";

        public const string Role_User_Indi = "Individual Customer";
        public const string Role_User_Comp = "Company Customer";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";
        public const string ssShopingCart = "Shoping Cart Session";

        public static double GetPriceBasedOnQuantity(int quantity, double price, double price50, double price100)
        {
            if(quantity < 50)
            {
                return price;
            }
            if(quantity > 50 && quantity < 100)
            {
                return price50;
            }
            else
            {
                return price100;
            }
        }
        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
    }
}

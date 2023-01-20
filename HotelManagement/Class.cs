using System.Text.RegularExpressions;

namespace HotelManagement
{
    public class Class
    {

        public bool IdentityIDCheck(string id)
        {
            string pattern = @"^\d{17}(?:\d|X)$";
            string birth = id.Substring(6, 8).Insert(6, "-").Insert(4, "-");


            int[] arr_weight = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            string[] id_last = { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += arr_weight[i] * int.Parse(id[i].ToString());
            }
            int result = sum % 11;  // 实际校验位的值

            if (Regex.IsMatch(id, pattern))
            {
                if (DateTime.TryParse(birth, out _))
                {
                    if (id_last[result] == id[17].ToString())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World");
            Console.ReadKey();

        }

    }
}

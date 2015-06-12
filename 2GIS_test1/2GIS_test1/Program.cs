using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2GIS_test1
{
    class Program
    {
        
        static void Main(string[] args)
        {
            ExtendedDictionary<int, UserType, string> a = new ExtendedDictionary<int, UserType, string>();

            a[37, new UserType("Nikita", 23)] = "Programmer";
            a[666, new UserType("Nikita", 23)] = "Clone";
            a[38, new UserType("Andrey", 33)] = "1st Programmer";
            a[39, new UserType("Nikolay", 13)] = "Bad Programmer";
            a[375, new UserType("Alex", 93)] = "Ex Programmer";
            a[371, new UserType("Marina", 43)] = "Seller";
            a[370, new UserType("Daniil", 20)] = "Dealer";
            a[3722, new UserType("Dima", 25)] = "Worker";
            a[3711, new UserType("Vasya", 53)] = "Slave";
            a[377, new UserType("Yulia", 22)] = "Runer";
            a[3766, new UserType("Masha", 0)] = "Nothing";
            a[37333, new UserType("Misha", 13)] = "Human";

            Console.WriteLine("Who is Yulia with id 377? : " + a[377,new UserType("Yulia", 22)]);

            foreach (var item in a.GetById(375))
            {
                Console.WriteLine("Who have id 375, is: " + item);
            }
            foreach (var item in a.GetByName(new UserType("Nikita", 23)))
            {
                Console.WriteLine("Who named Nikita, is: " + item);
            }


            foreach (var item in a)
            {
                Console.WriteLine(item.Key + " " + item.Value);
            }


            Console.ReadLine();

        }

        struct UserType
        {
            int age;
            string name;
            public UserType(string name, int age)
            {
                this.age = age;
                this.name = name;
            }
            public override bool Equals(object ob)
            {
                if (ob is UserType)
                {
                    UserType c = (UserType)ob;
                    return age.Equals(c.age) && name.Equals(c.name);
                }
                else
                {
                    return false;
                }
            }

            public override int GetHashCode()
            {
                return age.GetHashCode() ^ name.GetHashCode();
            }

            public override string ToString()   //Для отладки
            {
                return age.ToString() + " " + name.ToString();
            }
        }


    }
}

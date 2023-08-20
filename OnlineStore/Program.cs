using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Metadata;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OnlineStore
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var array = new Courier[] //Список работающих курьеров
            {
                new Courier
                {
                    Name = "Дмитрий Иванов",
                    Status = true,
                    Address = "Moscow"
                },
                new Courier
                {
                    Name = "Иван Дмитров",
                    Status = false,
                    Address = "Sankt-Petersburg"
                },
            };
            CourierGroup courierGroup = new CourierGroup(array);
            Courier courier = courierGroup[0]; // Поиск курьера по индексу
            Console.WriteLine(courier.Name);
            Console.WriteLine(courier.Status);
            Console.WriteLine(courier.Address);
            courier = courierGroup[true]; // Проверяем наличие или отсутствие курьеров для доставки

            PickPointDelivery pickPointDelivery = new PickPointDelivery();
            pickPointDelivery.GetAddress(); // Адрес постамата
            Random rnd = new Random();
            pickPointDelivery.GetPickPointCode = rnd.Next(); // Генерация случайного кода для открытия постамата
            Console.WriteLine($"Код для открытия постамата:{pickPointDelivery.GetPickPointCode}"); 

            Order<HomeDelivery, string> order = new Order<HomeDelivery, string>(); // Играюсь с обобщениями
            Order<HomeDelivery, int> orderfornumber = new Order<HomeDelivery, int>(); // Играюсь с обобщениями
            HomeDelivery homeDelivery = new HomeDelivery();
            homeDelivery.GetAddress(); // Проверка на возможность доставки на дом (если есть курьер)
            courier.DeliveryPossible(homeDelivery.Address);
            orderfornumber.Number = 3421;
            if (courier.DeliveryPossibilityStatus == true) // Если доставка возможна то отобразим информацию о заказе
            {
                order.Description = "Описание товара";
                Console.WriteLine(order.Description); // Описание заказа
                Console.WriteLine($"Заказ номер:{orderfornumber.Number}"); // Номер заказа
            }
            order.Price = 100; // Цена заказа
            PremiumOrder<Delivery, int> premiumOrder = new PremiumOrder<Delivery, int>();
            premiumOrder.Coeff = 3; // увеличиваем стоимость заказа при премиум доставке в X раз (стоимость не может увеличиться больше, чем в 2 раза. Логику для расчета коэффициента пока не реализовал, поэтому вписываем пока вручную))
            premiumOrder.GiveCoeff = order.Price;
            double b = premiumOrder.GiveCoeff;
            Console.WriteLine(b); // Стоимость заказа при премиум доставке
            Console.ReadKey();
        }
    }


    abstract class Delivery
    {
        public string Address;
        public virtual void GetAddress()
        {
            Console.WriteLine(Address);
        }
    }

    class HomeDelivery : Delivery
    {
        public bool DeliveryPossibilityStatus = default;
        public override void GetAddress()
        {
            Console.WriteLine("Введите город");
            Address = Console.ReadLine();
            base.GetAddress();
            
        }
        
    }

    class PickPointDelivery : Delivery
    {
        private int PickPointCode;
        
        public PickPointDelivery()
        {
            Address = Console.ReadLine();
        }
        public override void GetAddress()
        {
            Console.WriteLine("Ул. Центральная, дом 2");
        }
        public int GetPickPointCode
        {
            get { return PickPointCode; }
            set
            { 
                    PickPointCode = value;
            }
        }
        
    }

    class ShopDelivery : Delivery
    {
        public ShopDelivery()
        {
            Address = "Ул. Ленина, дом 21";
        }
        public override void GetAddress()
        {
            Console.WriteLine(Address);
        }
    }

    class Order<TDelivery,
    TStruct> where TDelivery : Delivery
    {
        public TDelivery Delivery;

        public TStruct Number;

        public TStruct Description;

        public double Price;

        public void DisplayAddress()
        {
            Console.WriteLine(Delivery.Address);
        }

        // ... Другие поля
    }

    class PremiumOrder<TDelivery, TStruct> : Order<TDelivery, TStruct> where TDelivery : Delivery
    {
        public double Coeff;
        public double GiveCoeff
        {
            get { return Price; }
            set
            {
                if (value * Coeff > value * 2)
                {
                    Price = value * 2;
                    return;
                }
                Price = value;
            }
        }
    }


    class Manager
    {
        public string ManagerFirstName;
        public string ManagerLastName;
        public string ManagerPhoneNumber;
        public Manager() 
        {
            ManagerFirstName = "Алексей";
            ManagerLastName = "Cмирнов";
            ManagerPhoneNumber = "89202818900";
        }

    }

    class Courier : HomeDelivery
    {
        public string Name = default;
        public bool Status = default;
        public void DeliveryPossible(string CAddress)
        {
            if (Address != CAddress)
            {
                Console.WriteLine("Доставка не работает в вашем городе");
            }
            else
            {
                DeliveryPossibilityStatus = true; Console.WriteLine("Курьер найден!");
            }
        }
    }

    class CourierGroup
    {
        private Courier[] Group;
        public CourierGroup(Courier[] Group) 
        {
            this.Group = Group;
        }

        public Courier this[int index]
        {
            get
            {
                if (index >= 0 && index < Group.Length)
                {
                    return Group[index];
                }
                else
                {
                    return null;
                }
            }
            private set
            {
                if (index >= 0 && index < Group.Length)
                {
                    Group[index] = value;
                }
            }
        }

        public Courier this[bool status]
        {
            get
            {
                for(int i = 0; i < Group.Length; i++)
                {
                    if (Group[i].Status == status)
                    {
                        return Group[i];
                    }
                }
                return null;
            }
        }
    }
}
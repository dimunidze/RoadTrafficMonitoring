using System;
using System.Collections.Generic;
using System.Text;

namespace RoadTrafficMonitoring
{
    public class Person
    {
        public string Name; //имя
        public int Number; //номер машины

        public Person(string n, int num)
        {
            Name = n;
            Number = num;
        }
    }
}

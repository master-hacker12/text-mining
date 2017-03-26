using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace text_mining
{
    [Serializable]
    public class Person
    {
        public string name;
        public string surname;
        public string secname;
        public string birthday;
        public string phone;
        public string gender;
        public string status;
        public string addres;
        public bool crytical;

        public Person()
        {
            name = "Нет данных";
            surname = "Нет данных";
            secname = "Нет данных";
            birthday = "Нет данных";
            phone = "Нет данных";
            gender = "Нет данных";
            status = "Нет данных";
            addres = "Нет данных";
            crytical = false;

        }

        /// <summary>
        /// Заполнение персональных данных
        /// </summary>
        /// <param name="name">Имя, если неизвестно, то в качестве параметра указывать null</param>
        /// <param name="surname">Фамилия, если неизвестно, то в качестве параметра указывать null</param>
        /// <param name="secname">Отчество, если неизвестно, то в качестве параметра указывать null</param>
        /// <param name="birthday">Дата рождения, то если неизвестно, в качестве параметра указывать null</param>
        /// <param name="phone">Номер телефона, то если неизвестно, в качестве параметра указывать null</param>
        /// <param name="gender">Пол, если неизвестно, в качестве параметра указывать null</param>
        /// <param name="status">Должность, если неизвестно, в качестве параметра указывать null</param>
        /// <param name="addres">Адрес, если неизвестно, в качестве параметра указывать null</param>
        public void AddPerson(string name, string surname, string secname, string birthday, string phone, string gender, string status,string addres)
        {
            if (name==null)
            {
                this.name = "Нет данных";
            }
            else
            {
                this.name = name;
            }
            if (surname == null)
            {
                this.surname = "Нет данных";
            }
            else
            {
                this.surname = surname;
            }

            if (secname == null)
            {
                this.secname = "Нет данных";
            }
            else
            {
                this.secname = secname;
            }

            if (birthday == null)
            {
                this.birthday = "Нет данных";
            }
            else
            {
                this.birthday = birthday;
            }
            if (phone == null)
            {
                this.phone = "Нет данных";
            }
            else
            {
                this.phone = phone;
            }
            if (gender == null)
            {
                this.gender = "Нет данных";
            }
            else
            {
                this.gender = gender;
            }
            if (status==null)
            {
                this.status = "Нет данных"; 
            }
            else
            {
                this.status = status;
            }
            if (addres == null)
            {
                this.addres = "Нет данных";
            }
            else
            {
                this.addres = addres;
            }
        }

        /// <summary>
        /// Дополнение персональных данных
        /// </summary>
        /// <param name="name">Имя, если неизвестно, то в качестве параметра указывать null</param>
        /// <param name="surname">Фамилия, если неизвестно, то в качестве параметра указывать null</param>
        /// <param name="secname">Отчество, если неизвестно, то в качестве параметра указывать null</param>
        /// <param name="birthday">Дата рождения, то если неизвестно, в качестве параметра указывать null</param>
        /// <param name="phone">Номер телефона, то если неизвестно, в качестве параметра указывать null</param>
        /// <param name="gender">Пол, если неизвестно, в качестве параметра указывать null</param>
        /// <param name="status">Должность, если неизвестно, в качестве параметра указывать null</param>
        /// <param name="addres">Адрес, если неизвестно, в качестве параметра указывать null</param>
        /// <param name="crytical">Критичность, если неизвестно, в качестве параметра указывать false</param>
        public void Append(string name, string surname, string secname, string birthday, string phone, string gender, string status, string addres,bool crytical)
        {
            if (name != null)
                this.name = name;
            if (surname != null)
                this.surname = surname;
            if (secname != null)
                this.secname = secname;
            if (birthday != null)
                this.birthday = birthday;
            if (phone != null)
                this.phone = phone;
            if (gender != null)
                this.gender = gender;
            if (status != null)
                this.status = status;
            if (name != null)
                this.addres = addres;
            this.crytical = crytical;
        }

        ///<summary>
        ///Выгрузка персональных данных
        ///</sumamry
        public string[] Get()
        {
            string[] result = new string[9];
            result[0] = this.surname;
            result[1] = this.name;
            result[2] = this.secname;
            result[3] = this.gender;
            result[4] = this.birthday;
            result[5] = this.phone;
            result[6] = this.addres;
            result[7] = this.status;
            result[8] = this.crytical.ToString();
            return result;
        }
        ///<summary>
        ///Объединение нескольких объектов
        ///</sumamry
        public static Person[] Summa (Person[] p1, Person[] p2)
        {
            Person[] result = null;
            if (p1 == null)
            {
                result = new Person[0 + p2.Length];
                for (int i = 0; i < p2.Length; i++)
                {
                    result[i] = new Person();
                    result[i] = p2[i];
                }
            }
            if (p2 == null)
            {
                result = new Person[p1.Length + 0];
                for (int i = 0; i < p1.Length; i++)
                {
                    result[i] = new Person();
                    result[i] = p1[i];
                }
            }
            if ((p1 != null) && (p2 != null))
            {
                result = new Person[p1.Length + p2.Length];

                for (int i = 0; i < p1.Length; i++)
                {
                    result[i] = new Person();
                    result[i] = p1[i];
                }

                for (int i = 0; i < p2.Length; i++)
                {
                    result[i + p1.Length] = new Person();
                    result[i + p1.Length] = p2[i];
                }
            }

            return result;
        }




    }

}

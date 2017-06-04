using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

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
        public string link;
        public string document;

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
            link = "Нет данных";
            document = "Нет данных";

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
            if (addres != null)
                this.addres = addres;
            this.crytical = crytical;
        }

        ///<summary>
        ///Выгрузка персональных данных
        ///</sumamry>
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
        ///</sumamry>
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

        ///<summary>
        ///Является ли персональные данные критичными
        ///</sumamry>
        public static bool IsCryticalPerson(ref Person data)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(Person[]));
            if (!File.Exists("people.cry"))
                return false;
            Person[] import = Form2.DeserializeBase();
            try
            {
                for (int i = 0; i < import.Length; i++)
                {
                    if ((import[i].surname == data.surname) && ((data.name.StartsWith(import[i].name, StringComparison.CurrentCultureIgnoreCase)) || (data.secname.StartsWith(import[i].secname))))
                    {
                        if ((data.birthday == "Нет данных") && (import[i]).birthday != "Нет данных")
                        {
                            data.birthday = import[i].birthday;
                        }
                        if ((data.gender == "Нет данных") && (import[i]).gender != "Нет данных")
                        {
                            data.gender = import[i].gender;
                        }
                        if ((data.phone == "Нет данных") && (import[i]).phone != "Нет данных")
                        {
                            data.phone = import[i].phone;
                        }
                        if ((data.status == "Нет данных") && (import[i]).status != "Нет данных")
                        {
                            data.status = import[i].status;
                        }
                        if ((data.addres == "Нет данных") && (import[i]).addres != "Нет данных")
                        {
                            data.addres = import[i].addres;
                        }
                        if ((import[i].crytical) || (data.document != "Нет данных"))
                            return true;
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                File.Delete("people.xml");
                File.Delete("people.cry");
            }
            return false;
        }

        ///<summary>
        ///Удаление повторяющихся персон
        ///</sumamry>
        public static Person[] CheckDublicate(Person[] p1)
        {
            Person[] dublicate = new Person[p1.Length];
            for (int i=0;i<dublicate.Length;i++)
            {
                dublicate[i] = new Person();
                dublicate[i].surname = p1[i].surname;
                dublicate[i].name = p1[i].name;
                dublicate[i].secname = p1[i].secname;
                dublicate[i].birthday = p1[i].birthday;
                dublicate[i].phone = p1[i].phone;
                dublicate[i].addres = p1[i].addres;
                dublicate[i].status = p1[i].status;
                dublicate[i].gender = p1[i].gender;
                dublicate[i].link = p1[i].link;
                dublicate[i].crytical = p1[i].crytical;
                dublicate[i].document = p1[i].document;
            }

            for (int i = 0; i < dublicate.Length; i++)
            {
                for (int j = 0; j < p1.Length; j++)
                {
                    if (p1[j] == null)
                        continue;
                    if (i == j)
                        continue;
                    if ((dublicate[i].surname == p1[j].surname) && ((p1[j].name.StartsWith(dublicate[i].name, StringComparison.CurrentCultureIgnoreCase)) || (p1[j].secname.StartsWith(dublicate[i].secname))) && (p1[i]!=null))
                    {
                        int col1 = countComplitedData(dublicate[i]);
                        int col2 = countComplitedData(p1[j]);
                        if (col1 >= col2)
                        {
                            if (p1[j].name.Length > dublicate[i].name.Length)
                            {
                                dublicate[i].name = p1[j].name;
                            }
                            if (p1[j].secname.Length > dublicate[i].secname.Length)
                            {
                                dublicate[i].secname = p1[j].secname;
                            }

                            if ((dublicate[i].birthday == "Нет данных") && (p1[j].birthday != "Нет данных"))
                            {
                                dublicate[i].birthday = p1[j].birthday;
                            }
                            if ((dublicate[i].phone == "Нет данных") && (p1[j].phone != "Нет данных"))
                            {
                                dublicate[i].phone = p1[j].phone;
                            }
                            if ((dublicate[i].addres == "Нет данных") && (p1[j].addres != "Нет данных"))
                            {
                                dublicate[i].addres = p1[j].addres;
                            }
                            if ((dublicate[i].status == "Нет данных") && (p1[j].status != "Нет данных"))
                            {
                                dublicate[i].status = p1[j].status;
                            }
                            if ((dublicate[i].gender == "Нет данных") && (p1[j].gender != "Нет данных"))
                            {
                                dublicate[i].gender = p1[j].gender;
                            }
                            if ((dublicate[i].link == "Нет данных") && (p1[j].link != "Нет данных"))
                            {
                                dublicate[i].link = p1[j].link;
                            }
                            if ((dublicate[i].document == "Нет данных") && (p1[j].document != "Нет данных"))
                            {
                                dublicate[i].document = p1[j].document;
                            }
                            p1[j] = null;
                        }
                        else
                        {
                            if (p1[i].name.Length > dublicate[j].name.Length)
                            {
                                dublicate[j].name = p1[i].name;
                            }
                            if (p1[i].secname.Length > dublicate[j].secname.Length)
                            {
                                dublicate[j].secname = p1[i].secname;
                            }

                            if ((dublicate[j].birthday == "Нет данных") && (p1[i].birthday != "Нет данных"))
                            {
                                dublicate[j].birthday = p1[i].birthday;
                            }
                            if ((dublicate[j].phone == "Нет данных") && (p1[i].phone != "Нет данных"))
                            {
                                dublicate[j].phone = p1[i].phone;
                            }
                            if ((dublicate[j].addres == "Нет данных") && (p1[i].addres != "Нет данных"))
                            {
                                dublicate[j].addres = p1[i].addres;
                            }
                            if ((dublicate[j].status == "Нет данных") && (p1[i].status != "Нет данных"))
                            {
                                dublicate[j].status = p1[i].status;
                            }
                            if ((dublicate[j].gender == "Нет данных") && (p1[i].gender != "Нет данных"))
                            {
                                dublicate[j].gender = p1[i].gender;
                            }
                            if ((dublicate[j].link == "Нет данных") && (p1[i].link != "Нет данных"))
                            {
                                dublicate[j].link = p1[i].link;
                            }
                            if ((dublicate[j].document == "Нет данных") && (p1[i].document != "Нет данных"))
                            {
                                dublicate[j].document = p1[i].document;
                            }
                            p1[i] = null;
                        }
                    }
                }
            }
            int countNull = 0;
            for (int i = 0; i < p1.Length; i++)
            {
                if (p1[i] == null)
                    countNull++;
                else
                {
                    p1[i] = dublicate[i];
                }
            }

            Person[] result = new Person[p1.Length-countNull];
            int k = 0;
            for (int i=0;i<p1.Length;i++)
            {
                if (p1[i] != null)
                {
                    result[k] = p1[i];
                    k++;
                }
            }


            return result;


        }

        ///<summary>
        ///Количество заполненных данных о человеке
        ///</sumamry>
        public static int countComplitedData(Person p1)
        {
            int result = 0;
            if (p1.surname != "Нет данных")
                result++;
            if (p1.name != "Нет данных")
                result++;
            if (p1.secname != "Нет данных")
                result++;
            if (p1.gender != "Нет данных")
                result++;
            if (p1.birthday != "Нет данных")
                result++;
            if (p1.phone != "Нет данных")
                result++;
            if (p1.addres != "Нет данных")
                result++;
            if (p1.surname != "Нет данных")
                result++;
            if (p1.link != "Нет данных")
                result++;
            if (p1.document != "Нет данных")
                result++;

            return result;
        }
    }

}

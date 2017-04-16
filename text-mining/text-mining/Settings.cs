using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace text_mining
{
    class Settings
    {
        public string type;
        public string title;
        public string statusup;
        public string fioup;
        public string statusdown;
        public string fiodown;
        public static string yearnow = DateTime.Now.Year.ToString();

        public Settings()
        {
            fioup = "И.И. Иванов";
            title = "Рога и Копыта";
            type = "Открытое Акционерное общество";
            statusup = "Руководитель комании";
            statusdown = "Старший инженер";
            fiodown = "П.П. Петров";
        }
    }
}

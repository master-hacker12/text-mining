using EP.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace text_mining
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            EP.ProcessorService.Initialize(false, MorphLang.RU);
            (new EP.Semantix.MiscInitializer()).Initialize();
            (new EP.Semantix.DateInitializer()).Initialize();
            (new EP.Semantix.LocationInitializer()).Initialize();
            (new EP.Semantix.OrgInitializer()).Initialize();
            (new EP.Semantix.PersonInitializer()).Initialize();
            (new EP.Semantix.TechnicalInitializer()).Initialize();
            (new EP.Semantix.DecreeInitializer()).Initialize();
            (new EP.Semantix.BiblioInitializer()).Initialize();
            (new EP.Semantix.BusinessInitializer()).Initialize();
            (new EP.Semantix.SemanticInitializer()).Initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

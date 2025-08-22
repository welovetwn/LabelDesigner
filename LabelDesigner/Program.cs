using LabelDesigner.Services;
using System;
using System.Windows.Forms;

namespace LabelDesigner
{
    internal static class Program
    {
        public static FieldResolver Resolver { get; } = new FieldResolver();
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}

using System.Windows;

namespace CourseWorkCompiller.View;

public partial class AboutProgramWindow : Window
{
    public AboutProgramWindow()
    {
        InitializeComponent();
        Text.Text =
            "Курсовая работ по дисциплине 'Теория формальных языков и компиляторов'\n" +
            "Тема: Объявление прототипа функции на языке C/C++\n"+
            "Выполнил:\n" +
            "Студент:\n" +
            "Гордиенко Дмитрий Андреевич\n" +
            "Группа: АВТ-213\n";
    }
}
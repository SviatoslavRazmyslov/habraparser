using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using HabrParserLib;

namespace OutputInfo
{
    class OutFile
    {
        //Поля для вывода "шапки" в csv-файл
        private static readonly string _nameIo = "Название";
        private static readonly string _linkIo = "Ссылка";
        private static readonly string _raitingIo = "Рейтинг";
        private static readonly string _bootmarksIo = "Закладки";
        private static readonly string _viewsIo = "Просмотры";
        private static readonly string _numbOfCommentsIo = "Коментарии";
        private static readonly string _dateOfPublicationIo = "Дата";
        private static readonly string _timeOfPublicationIo = "Время";
        private static readonly string _labelIo = "Метки";
        private static readonly string _hubsIo = "Хабы";
        private static readonly string _symIo = ";";

        static public void Output(List<InfoMoreBlogsWithHabr> myInfoBlog, string namePathOutput)
        {
            FileStream File = new FileStream(namePathOutput, FileMode.Create, FileAccess.Write);
            StreamWriter Writer = new StreamWriter(File, Encoding.Unicode);

            Writer.WriteLine(_nameIo + _symIo
                             + _linkIo + _symIo
                             + _raitingIo + _symIo
                             + _bootmarksIo + _symIo
                             + _viewsIo + _symIo
                             + _numbOfCommentsIo + _symIo
                             + _dateOfPublicationIo + _symIo
                             + _timeOfPublicationIo + _symIo
                             + _labelIo + _symIo
                             + _hubsIo);

            for (int i = 0; i < myInfoBlog.Count; i++)
            {
                for (int j = 0; j < myInfoBlog[i].InfoSingeBlogs.Count; j++)
                {
                    string bufLabels = string.Join(", ", myInfoBlog[i].InfoSingeBlogs[j].labels);
                    string bufHubs = string.Join(", ", myInfoBlog[i].InfoSingeBlogs[j].hubs);

                    Writer.WriteLine(myInfoBlog[i].InfoSingeBlogs[j].name + _symIo
                                     + myInfoBlog[i].InfoSingeBlogs[j].link + _symIo
                                     + myInfoBlog[i].InfoSingeBlogs[j].rating + _symIo
                                     + myInfoBlog[i].InfoSingeBlogs[j].bootmarks + _symIo
                                     + myInfoBlog[i].InfoSingeBlogs[j].views + _symIo
                                     + myInfoBlog[i].InfoSingeBlogs[j].numbOfComments + _symIo
                                     + myInfoBlog[i].InfoSingeBlogs[j].dateOfPublication + _symIo
                                     + myInfoBlog[i].InfoSingeBlogs[j].timeOfPublication + _symIo
                                     + bufLabels + _symIo
                                     + bufHubs);
                }
            }
            Writer.Close();
        }
    }
}

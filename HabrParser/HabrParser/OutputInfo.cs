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

        static public void Output(List<BlogInfo> myInfoBlog, string namePathOutput)
        {
            FileStream File = new FileStream(namePathOutput, FileMode.Create, FileAccess.Write);
            StreamWriter Writer = new StreamWriter(File, Encoding.Unicode);
            string buff = string.Join(_symIo, _nameIo, _linkIo, _raitingIo, _bootmarksIo, _viewsIo, 
                _numbOfCommentsIo, _dateOfPublicationIo, _timeOfPublicationIo, _labelIo, _hubsIo);

            Writer.WriteLine(buff);

            //TODO переделать на foreach
            foreach (BlogInfo singleBlog in myInfoBlog)
            {
                foreach (ArticleInfo field in singleBlog.InfoSingeBlogs)
                {
                    string bufLabels = string.Join(", ", field.labels);
                    string bufHubs = string.Join(", ", field.hubs);
                    //TODO сделать через String.Join
                    buff = string.Join(_symIo,
                        field.name,
                        field.link,
                        field.rating,
                        field.bookmarks,
                        field.views,
                        field.numbOfComments,
                        field.dateOfPublication,
                        field.timeOfPublication,
                        bufLabels, bufHubs);

                    Writer.WriteLine(buff);
                }
            }
            Writer.Close();
        }
    }
}

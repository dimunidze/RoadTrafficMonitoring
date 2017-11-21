using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RoadTrafficMonitoring
{
    public class Camera
    {
        int number; //номер камеры
        public List<string> offences; //нарушения
        List<string> database; //база данных
        List<Person> persons; // люди

        RichTextBox tbProc;
        RichTextBox tbCam;
        Form1 form;

        public AutoResetEvent catchevent; //событие нарушения
        public AutoResetEvent releaseevent; //событие отключения камеры
        AutoResetEvent dbevent; //событие добавления инфы в бд

        Thread procThread; //поток обработки данных
        Thread dbThread; //поток отправки данных в базу ГИБДД
        Thread releaseThread; //поток ожидания отключения камеры

        public bool on = false; //работает ли камера

        Random randtime = new Random();

        public Camera(Form1 f, List<Person> p, int n, RichTextBox tbp, RichTextBox tbcm)
        {
            form = f;
            number = n;
            database = new List<string>();
            persons = p;
            offences = new List<string>();
            tbCam = tbcm;
            tbProc = tbp;
            procThread = new Thread(new ThreadStart(process));
            dbThread = new Thread(new ThreadStart(dbWork));
            releaseThread = new Thread(new ThreadStart(camRelease));
        }

        /// <summary>
        /// Включает камеру, начинает все потоки
        /// </summary>
        public void startThreads()
        {
            on = true;
            dbevent = new AutoResetEvent(false);
            catchevent = new AutoResetEvent(false);
            releaseevent = new AutoResetEvent(false);
            procThread = new Thread(new ThreadStart(process));
            dbThread = new Thread(new ThreadStart(dbWork));
            releaseThread = new Thread(new ThreadStart(camRelease));
            procThread.Start();
            dbThread.Start();
            releaseThread.Start();
        }

        /// <summary>
        /// Пока камера включена, ждет нарушения, идентифицирует нарушителя, добавляет 
        /// информацию в базу данных
        /// </summary>
        private void process()
        {
            while (on)
            {
                catchevent.WaitOne();
                for (int i = 0; i < offences.Count; i++)
                {
                    Person iden = getIdentity(offences[i]);
                    Thread.Sleep(randtime.Next(2000, 5000));
                    addTextProc("Нарушитель " + iden.Name.ToString() + " идентифицирован камерой " + number.ToString() + "\n");
                    string data = "Нарушитель " + iden.Name.ToString() + ", номер транспортного средства " + iden.Number.ToString();
                    offences.RemoveAt(i);
                    database.Add(data);
                    dbevent.Set();
                }
            }
        }

        /// <summary>
        /// Идентифицирует нарушителя
        /// </summary>
        /// <param name="numberoff">номер машины</param>
        /// <returns>Возвращает "нарушителя"</returns>
        private Person getIdentity(string numberoff)
        {
            for (int i = 0; i< persons.Count; i++)
            {
                if (persons[i].Number == Convert.ToInt32(numberoff))
                {
                    return persons[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Пока камера включена, ждет появления информации в бд
        /// </summary>
        private void dbWork()
        {
            while (on)
            {
                dbevent.WaitOne();
                for (int i = 0; i < database.Count; i++)
                {
                    Thread.Sleep(randtime.Next(2000, 5000));
                    addTextProc("В базу ГИБДД отправлены данные: " + database[i] + "\n");
                    database.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Ждет отключения камеры, останавливает все потоки
        /// </summary>
        public void camRelease()
        {
            releaseevent.WaitOne();
            try
            {
                on = false;
                //procThread.Abort();
                //dbThread.Abort();
                addTextCam("Камера " + number.ToString() + " переведена в автономный режим\n");
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// Функция добавления текста из другого потока
        /// </summary>
        /// <param name="text"></param>
        public void addTextProc(string text)
        {
            try
            {
                if (form.InvokeRequired)
                {
                    form.Invoke(new Action<string>(addTextProc), new object[] { text });
                    return;
                }
                tbProc.Text += text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Функция добавления текста из другого потока
        /// </summary>
        /// <param name="text"></param>
        public void addTextCam(string text)
        {
            try
            {
                if (form.InvokeRequired)
                {
                    form.Invoke(new Action<string>(addTextCam), new object[] { text });
                    return;
                }
                tbCam.Text += text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}

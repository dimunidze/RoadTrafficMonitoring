using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace RoadTrafficMonitoring
{
    public class MonitoringSystem
    {
        List<Camera> cameras; //список камер
        List<Person> persons; //список людей

        RichTextBox tbCatch; //текст фиксирования нарушений
        RichTextBox tbProc; //текст обработки нарушений
        RichTextBox tbCam; //текст состояния камер
        Form1 form; //окно программы

        Thread monitoringThread; // поток создания нарушений

        Random rand = new Random();
        Random numr = new Random();
        Random randtime = new Random();

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="num">количество камер</param>
        /// <param name="tbc"></param>
        /// <param name="tbp"></param>
        /// <param name="tbcm"></param>
        /// <param name="f"></param>
        public MonitoringSystem(int num, RichTextBox tbc, RichTextBox tbp, RichTextBox tbcm, Form1 f)
        {
            form = f;
            persons = new List<Person>();
            //заполняется список людей с "именами" и номерами машин
            for (int i = 0; i < 20; i++)
            {
                int j = rand.Next(1, 10) * 100 + rand.Next(0, 10) * 10 + rand.Next(0, 10);
                persons.Add(new Person("гражданин "+i.ToString(), j));
            }
            tbCatch = tbc;
            tbCam = tbcm;
            tbProc = tbp;
            cameras = new List<Camera>();
            //заполняется список камер
            for (int i =0; i < num; i++)
            {
                cameras.Add(new Camera(form, persons, i+1, tbProc, tbCam));
            }
        }

        /// <summary>
        /// Запускает камеры и начинает генерацию нарушений
        /// </summary>
        public void StartMonitoring()
        {
            for (int i = 0; i < cameras.Count; i++)
            {
                cameras[i].startThreads();
            }
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < cameras.Count; j++)
                {
                    int wait = randtime.Next(0, 2000);
                    //если рандом и камера включена
                    if (rand.Next(0,1) == 0 && cameras[j].on)
                    {
                        string n = persons[numr.Next(0, 20)].Number.ToString();
                        addTextCatch("Нарушитель " + n + " зафиксирован камерой " + (j+1).ToString() + "\n");
                        cameras[j].offences.Add(n);
                        cameras[j].catchevent.Set();
                        Thread.Sleep(wait);
                    }
                }
            }
        }

        /// <summary>
        /// Начинает главный поток
        /// </summary>
        public void StartAll()
        {
            monitoringThread = new Thread(new ThreadStart(StartMonitoring));
            monitoringThread.Start();
        }

        /// <summary>
        /// Останавливает генерацию нарушений и выключает камеры
        /// </summary>
        public void StopAll()
        {
            try
            {
                monitoringThread.Abort();
                for (int i = 0; i < cameras.Count; i++)
                {
                    cameras[i].releaseevent.Set();
                }
            }
            catch(Exception ex) { }
        }

        /// <summary>
        /// Функция добавления текста в поле из другого потока
        /// </summary>
        /// <param name="text"></param>
        public void addTextCatch(string text)
        {
            try
            {
                if (form.InvokeRequired)
                {
                    form.Invoke(new Action<string>(addTextCatch), new object[] { text });
                    return;
                }
                tbCatch.Text += text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

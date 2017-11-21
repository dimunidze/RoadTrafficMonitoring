using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RoadTrafficMonitoring
{
    public partial class Form1 : Form
    {
        int cameras; //количество камер
        MonitoringSystem system; //система мониторинга

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// По клику на старт проверяет введенное число камер
        /// и запускает систему
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                cameras = Int32.Parse(textBox1.Text);
                if (cameras <= 0)
                {
                    throw new Exception("Неверное количество камер");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            buttonStart.Enabled = false;
            system = new MonitoringSystem(cameras, tbCatch, tbProcessing, tbCam, this);
            system.StartAll();
        }

        /// <summary>
        /// По клику останавливает работу системы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStop_Click(object sender, EventArgs e)
        {
            system.StopAll();
            buttonStart.Enabled = true;
        }

        /// <summary>
        /// Удаляет текст состояния системы из всех полей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClear_Click(object sender, EventArgs e)
        {
            tbCam.Clear();
            tbCatch.Clear();
            tbProcessing.Clear();
        }
    }
}

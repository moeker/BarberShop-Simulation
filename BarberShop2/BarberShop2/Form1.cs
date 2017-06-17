using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;


namespace BarberShop2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }
        ThreadStart TH;
        Thread []customers;
        Thread []barbers;
        Thread []cashiers;
        Semaphore max_capacity;
        Semaphore sofa;
        Semaphore barberChair;
        Semaphore coordinator;
        Semaphore custReady;
        Semaphore leaveChair;
        Semaphore payment;
        Semaphore recipt;
        Semaphore[] finished;
        Semaphore mutex1;
        Semaphore mutex2;
        Queue Q1;
        Queue Q2;
        int count;
        int nCustomers;
        int nCap;
        int nSofa;
        int nBarbers = 3;
        int nCashier = 1;
       
        public void customer()
        {
            int cust_nr;
            cust_nr = count++;
            max_capacity.Wait();
            standingArea.Items.Add("Customer " + cust_nr.ToString());
            mutex1.Wait();
            Thread.Sleep(500);
            standingArea.Items.Remove("Customer " + cust_nr.ToString());    
            Thread.Sleep(500);
            mutex1.Signal();
            sofa.Wait();
            sofaList.Items.Add("Customer " + cust_nr.ToString());
            Thread.Sleep(500);
            barberChair.Wait();
            Thread.Sleep(500);
            sofaList.Items.Remove("Customer " + cust_nr.ToString());
            sofa.Signal();
            eventLog.Items.Add("Customer " + cust_nr.ToString() + " has started hair cutting");
            Thread.Sleep(500);
            mutex2.Wait();
            //Thread.Sleep(500);
            Q1.Enqueue((int)cust_nr);
            mutex2.Signal();
            Thread.Sleep(500);
            custReady.Signal();
            Thread.Sleep(500);
            finished[cust_nr - 1].Wait();
            Thread.Sleep(500);
            eventLog.Items.Add("Customer " + cust_nr.ToString() + "has finished hair cutting");
            Thread.Sleep(500);
            leaveChair.Signal();
            Thread.Sleep(500);
            cashierList.Items.Add("Customer " + cust_nr.ToString());
            payment.Signal();
            recipt.Wait();
            cashierList.Items.Remove("Customer " + cust_nr.ToString());
            outside.Items.Add("Customer " + cust_nr.ToString());
            max_capacity.Signal();

        }
        public void barber()
        {
            
            while (true)
            {
                int b_cust = 0;
                Thread.Sleep(500);
                custReady.Wait();
                Thread.Sleep(500);
                mutex2.Wait();
                Thread.Sleep(500);
                b_cust= (int)Q1.Dequeue();
                Q2.Enqueue((int) b_cust);
                mutex2.Signal();
                Thread.Sleep(500);
                coordinator.Wait();
                //lst_BA.Items.Add("Barber is cutting hair for customer"+b_cust.ToString());
                Thread.Sleep(500);  
                coordinator.Signal();
                Thread.Sleep(500);
                finished[b_cust-1].Signal();
                Thread.Sleep(500);
                leaveChair.Wait();
                Thread.Sleep(500);
                barberChair.Signal();
            }
        }
        public void cashier()
        {
            int count = 0;

            while (true)
            {
                
                Thread.Sleep(500);
                payment.Wait();
                Thread.Sleep(500);
                coordinator.Wait();
                count = (int)Q2.Dequeue();
                Thread.Sleep(500);
                eventLog.Items.Add("Payment of cust ("+count.ToString()+ ")is accepted");
                Thread.Sleep(500);
                coordinator.Signal();
                Thread.Sleep(500);
                recipt.Signal();
            }
        }
        //Start
        private void button2_Click(object sender, EventArgs e)
        {
            //standingArea.Items.Clear();
            //eventLog.Items.Clear();
            //sofaList.Items.Clear();
            //cashierList.Items.Clear();
            //outside.Items.Clear();
            nCustomers = Int32.Parse(textBox1.Text);
            nCap = Int32.Parse(textBox2.Text);
            nSofa = Int32.Parse(textBox3.Text);

            max_capacity = new Semaphore(nCap);
            sofa = new Semaphore(nSofa);
            barberChair = new Semaphore(3);
            coordinator = new Semaphore(3);
            custReady = new Semaphore(0);
            leaveChair = new Semaphore(0);
            payment = new Semaphore(0);
            recipt = new Semaphore(0);
            finished = new Semaphore[nCustomers];
            mutex1 = new Semaphore(1);
            mutex2 = new Semaphore(1);
            customers = new Thread[nCustomers];
            barbers = new Thread[nBarbers];
            cashiers = new Thread[nCashier];
            Q1 = new Queue();
            Q2 = new Queue();
            count = 1;
            for (int i = 0; i < nCustomers; i++)
            {
                finished[i] = new Semaphore(nCustomers);
            }

            for (int i = 0; i < nCustomers; i++)
            {
                TH = new ThreadStart(customer);
                customers[i] = new Thread(TH);
                customers[i].Start();
            }
            for (int i = 0; i < nBarbers; i++)
            {
                TH = new ThreadStart(barber);
                barbers[i] = new Thread(TH);
                barbers[i].Start();
            }
            for (int i = 0; i < nCashier; i++)
            {
                TH = new ThreadStart(cashier);
                cashiers[i] = new Thread(TH);
                cashiers[i].Start();
            }
            


        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < nCustomers; i++)
            {
                customers[i].Abort();
            }
            for (int i = 0; i < nBarbers; i++)
            {
                barbers[i].Abort();
            }
            for (int i = 0; i < nCashier; i++)
            {
                cashiers[i].Abort();
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void lst_OD_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, System.EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, System.EventArgs e)
        {
        
        }

        private void textBox2_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, System.EventArgs e)
        {

        }
    }
}
